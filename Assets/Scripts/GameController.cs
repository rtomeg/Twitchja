using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    private Dictionary<string, string> whispers = new Dictionary<string, string>();
    private static string command = "!ouija ";

    private string[] yesWords = {"YES", "YEP", "YEAH", "YUP", "SI", "SIP", "SIS", "CHI"};
    private string[] noWords = {"NO", "NOPE", "NOS", "NAH"};

    private string yesText = "YES";
    private string noText = "NO";
    private string startText = "START";

    [SerializeField] private GameObject planchette;
    [SerializeField] private GameObject ouijaBoard;
    [SerializeField] private Texture2D skeletonHand;

    private string startReadingMessageEsp = "/me ha abierto un portal entre vuestro mundo y el suyo. Podéis contestar a su llamada usando el comando !ouija";
    private string startReadingMessageEng = "/me has opened a portal between their world and ours. You can answer their call using the command !ouija";

    
    private string stopReadingMessageEsp = "/me ha cortado la conexión entre los mundos. Vuestras plegarias han sido escuchadas.";
    private string stopReadingMessageEng = "/me has cut off the link between both worlds. Your prayers have been heard.";


    private Random rnd = new Random();

    [SerializeField] private TwitchController _twitchController;

    public static bool inRitual { get; private set; }

    private void Start()
    {
        TwitchController.messageRecievedEvent.AddListener(OnChatMsgReceived);

        Cursor.SetCursor(skeletonHand, Vector2.zero, CursorMode.Auto);

        EventsManager.onStartReadingTwitchResponses += StartReadingTwitchResponses;
        EventsManager.onEndReadingTwitchResponses += EndReadingTwitchResponses;
    }

    private void OnDestroy()
    {
        EventsManager.onStartReadingTwitchResponses -= StartReadingTwitchResponses;
        EventsManager.onEndReadingTwitchResponses -= EndReadingTwitchResponses;
    }

    public void StartReadingTwitchResponses()
    {
        _twitchController.SendMsg(startReadingMessageEsp);
        _twitchController.SendMsg(startReadingMessageEng);

        inRitual = true;
    }

    public void EndReadingTwitchResponses()
    {
        _twitchController.SendMsg(stopReadingMessageEsp);
        _twitchController.SendMsg(stopReadingMessageEng);

        inRitual = false;

        string response = "";
        if (whispers.Count > 0)
        {
            whispers = whispers.OrderBy(x => rnd.Next()).ToDictionary(item => item.Key, item => item.Value);

            response = whispers.GroupBy(v => v.Value)
                .OrderByDescending(g => g.Count())
                .First().Key;

            //Debug.Log("THE SPIRITS HAVE TALKED " + response + " is the answer.");

        }
        
        StartOuijaResponse(response);
        
        whispers.Clear();
    }

    public void StartOuijaResponse(string response)
    {
        EventsManager.onOuijaResponseStarted();
        MovePlanchette(response);
    }

    public void EndOuijaResponse(string response)
    {
        EventsManager.onOuijaResponseEnded(response);
    }

    void OnChatMsgReceived(string msg)
    {
        if (msg.Contains(command))
        {
            if (inRitual)
            {
                //TODO: Check if starts with the command

                int msgIndex = msg.IndexOf("PRIVMSG #");
                string msgString = msg.Substring(msgIndex + TwitchData.Instance.channelName.Length + 11);

                if (msgString.StartsWith(command))
                {
                    msgString = msgString.Substring(command.Length);
                }
                else
                {
                    return;
                }
                
                string user = msg.Substring(1, msg.IndexOf('!') - 1);

                msgString = RemoveDiacritics(msgString);
                msgString = Regex.Replace(msgString, @"[^A-Za-z0-9 ]+", "");
                msgString = msgString.ToUpper();


                msgString = yesWords.Contains(msgString) ? yesText : msgString;
                msgString = noWords.Contains(msgString) ? noText : msgString;

                if (msgString.Length > 0)
                {
                    msgString = msgString.Substring(0, Mathf.Min(msgString.Length, 25));
                }

                if (string.IsNullOrEmpty(msgString) || string.IsNullOrWhiteSpace(msgString)) return;

                if (!whispers.ContainsKey(user))
                {
                    whispers.Add(user, msgString);
                }
                else
                {
                    whispers[user] = msgString;
                }

                EventsManager.onCommandReceived(msgString);

                Debug.Log(String.Format("Received {0} from {1}", msgString, user));
            }
        }
        else
        {
            //Es un mensaje de texto normal (o un comando) 
        }
    }

    void MovePlanchette(string word)
    {
        Sequence seq = DOTween.Sequence();

        if (word.Equals(yesText))
        {
            planchette.transform.DOMove(ouijaBoard.transform.Find(yesText).position, rnd.Next(1, 2))
                .OnComplete(() =>
                {
                    
                    planchette.transform.DOMove(ouijaBoard.transform.Find(startText).position,
                        rnd.Next(1, 2)).SetDelay(0.5f, false).OnComplete(() =>
                    {
                        EventsManager.onLetterReached(yesText);
                        EndOuijaResponse(yesText);
                    });
                });
            return;
        }

        if (word.Equals(noText))
        {
            planchette.transform.DOMove(ouijaBoard.transform.Find(noText).position, rnd.Next(1, 2))
                .OnComplete(() =>
                {
                    planchette.transform.DOMove(ouijaBoard.transform.Find(startText).position,
                        rnd.Next(1, 2)).SetDelay(0.5f, false).OnComplete(() =>
                    {
                        EventsManager.onLetterReached(noText);
                        EndOuijaResponse(noText);
                    });
                });
            return;
        }

        foreach (var c in word)
        {
            if (c == ' ')
            {
                seq.Append(planchette.transform.DOMove(ouijaBoard.transform.Find(startText).position,
                        rnd.Next(1, 2)).SetDelay(0.5f, false)
                    .OnComplete(() => EventsManager.onLetterReached(c.ToString())));
            }
            else
            {
                seq.Append(planchette.transform.DOMove(ouijaBoard.transform.Find(c.ToString()).position,
                        rnd.Next(1, 2)).SetDelay(0.5f, false)
                    .OnComplete(() => EventsManager.onLetterReached(c.ToString())));
            }
        }
        
        seq.Append(planchette.transform.DOMove(ouijaBoard.transform.Find(startText).position,
                rnd.Next(1, 2)).SetDelay(0.5f, false)
            .OnComplete(() => EventsManager.onLetterReached(" ")));
        
        seq.OnComplete(() => EndOuijaResponse(word));
    }

    static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
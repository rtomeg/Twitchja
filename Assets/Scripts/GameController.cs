using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    private Dictionary<string, string> whispers = new Dictionary<string, string>();
    private static string command = "!ouija ";

    private string[] yesWords = {"YES", "YEP", "YEAH", "YUP", "SI", "SIP", "SIS", "CHI"};
    private string[] noWords = {"NO", "NOPE", "NOS", "NAH"};

    [SerializeField] private GameObject planchette;
    [SerializeField] private GameObject ouijaBoard;

    private int currentResponseIndex;

    private Random rnd = new Random();

    public static bool inRitual { get; private set; }

    private void Start()
    {
        TwitchController.messageRecievedEvent.AddListener(OnChatMsgReceived);
    }

    public void StartRitual()
    {
        inRitual = true;
    }

    public void EndRitual()
    {
        inRitual = false;

        whispers.OrderBy(x => rnd.Next()).ToDictionary(item => item.Key, item => item.Value);

        string response = whispers.GroupBy(v => v.Value)
            .OrderByDescending(g => g.Count())
            .First().Key;


        /*
        foreach (KeyValuePair<string, string> val in whispers)
        {
            Debug.Log((string.Format("Submitted {0} by {1}", val.Value, val.Key)));
        }
        */

        Debug.Log("THE SPIRITS HAVE TALKED " + response + " is the answer.");
        MovePlanchette(response);

        whispers.Clear();
    }

    void OnChatMsgReceived(string msg)
    {
        if (msg.Contains(command))
        {
            if (inRitual)
            {
                int msgIndex = msg.IndexOf("PRIVMSG #");
                string msgString = msg.Substring(msgIndex + Secrets.CHANNEL_NAME.Length + 11 + command.Length);
                string user = msg.Substring(1, msg.IndexOf('!') - 1);
                msgString = RemoveDiacritics(msgString);
                msgString = Regex.Replace(msgString, @"[^A-Za-z0-9 ]+", "");
                msgString = msgString.ToUpper();

                Debug.Log(String.Format("Received {0} from {1}", msgString, user));


                msgString = yesWords.Contains(msgString) ? "SI" : msgString;
                msgString = noWords.Contains(msgString) ? "NO" : msgString;

                if (!whispers.ContainsKey(user))
                {
                    whispers.Add(user, msgString);
                }
                else
                {
                    whispers[user] = msgString;
                }
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

        if (word.Equals("SI"))
        {
            planchette.transform.DOMove(ouijaBoard.transform.Find("SI").position, rnd.Next(1, 2))
                .OnComplete(ReachedLetter);
            return;
        }

        if (word.Equals("NO"))
        {
            planchette.transform.DOMove(ouijaBoard.transform.Find("NO").position, rnd.Next(1, 2))
                .OnComplete(ReachedLetter);
            return;
        }

        foreach (var c in word)
        {
            //SI: es espacio, volver al centro
            //SI: es el mismo char que el anterior, volver al centro y volver a la letra

            seq.Append(planchette.transform.DOMove(ouijaBoard.transform.Find(c.ToString()).position,
                rnd.Next(1, 2)).SetDelay(0.5f, false).OnComplete(ReachedLetter));
        }
    }

    private void ReachedLetter()
    {
        //DO SMTHNG
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
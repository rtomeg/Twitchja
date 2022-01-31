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
    public static string command = "!ouija ";

    private string[] yesWords = {"YES", "YEP", "YEAH", "YUP", "SI", "SIP", "SIS", "CHI"};
    private string[] noWords = {"NO", "NOPE", "NOS", "NAH"};

    private string yesText = "YES";
    private string noText = "NO";
    private string startText = "START";

    [SerializeField] private GameObject planchette;
    [SerializeField] private GameObject ouijaBoard;
    [SerializeField] private Texture2D skeletonHand;
    private Random rnd = new Random();

    [SerializeField] private TwitchController _twitchController;

    public static bool inRitual { get; private set; }

    private void Start()
    {
        EventsManager.onCommandReceived += OnChatMsgReceived;
        EventsManager.onStartReadingTwitchResponses += StartReadingTwitchResponses;
        EventsManager.onEndReadingTwitchResponses += EndReadingTwitchResponses;

        Debug.Log(System.Environment.GetEnvironmentVariable("USERNAME"));
    }

    private void OnDestroy()
    {
        EventsManager.onStartReadingTwitchResponses -= StartReadingTwitchResponses;
        EventsManager.onEndReadingTwitchResponses -= EndReadingTwitchResponses;
    }

    public void StartReadingTwitchResponses()
    {
        inRitual = true;
    }

    public void EndReadingTwitchResponses()
    {

        inRitual = false;

        string response = "";
        if (whispers.Count > 0)
        {
            whispers = whispers.OrderBy(x => rnd.Next()).ToDictionary(item => item.Key, item => item.Value);

            response = whispers.GroupBy(v => v.Value)
                .OrderByDescending(g => g.Count())
                .First().Key;
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

    void OnChatMsgReceived(string user, string message)
    {
        if (inRitual)
        {
            message = RemoveDiacritics(message);
            message = Regex.Replace(message, @"[^A-Za-z0-9 ]+", "");
            message = message.ToUpper();


            message = yesWords.Contains(message) ? yesText : message;
            message = noWords.Contains(message) ? noText : message;

            if (message.Length > 0)
            {
                message = message.Substring(0, Mathf.Min(message.Length, 25));
                Debug.Log(message);
            }


            
            if (!whispers.ContainsKey(user))
            {
                whispers.Add(user, message);
            }
            else
            {
                whispers[user] = message;
            }
                
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
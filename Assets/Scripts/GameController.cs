using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Dictionary<string, string> whispers = new Dictionary<string, string>();
    private static string command = "!ouija ";

    private void Start()
    {
        TwitchController.messageRecievedEvent.AddListener(OnChatMsgReceived);
    }

    void OnChatMsgReceived(string msg)
    {
        if (msg.Contains(command))
        {
            int msgIndex = msg.IndexOf("PRIVMSG #" + command);
            string msgString = msg.Substring(msgIndex + Secrets.CHANNEL_NAME.Length + 11);
            string user = msg.Substring(1, msg.IndexOf('!') - 1);
            msgString = RemoveDiacritics(msgString);
            msgString = Regex.Replace(msgString,  @"\\W + ", "");

            Debug.Log(String.Format("Received {0} from {1}", msgString, user));
        }
        else
        {
            //Es un mensaje de texto normal (o un comando) 
        }
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
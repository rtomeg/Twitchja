using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebug : MonoBehaviour
{
#if UNITY_EDITOR
    public void StartReadingTwitch()
    {
        EventsManager.onStartReadingTwitchResponses();
    }

    public void EndReadingTwitch()
    {
        EventsManager.onEndReadingTwitchResponses();
    }

    public void StartOuijaResponse()
    {
        EventsManager.onOuijaResponseStarted();
    }
    public void EndOuijaResponse(string word)
    {
        EventsManager.onOuijaResponseEnded(word);
    }

    public void LetterReached(string letter)
    {
        EventsManager.onLetterReached(letter);
    }
    
#endif
}
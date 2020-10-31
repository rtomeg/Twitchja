public class EventsManager
{
    public delegate void OnStartReadingTwitchResponses();
    public static OnStartReadingTwitchResponses onStartReadingTwitchResponses;
    
    public delegate void OnEndReadingTwitchResponses();
    public static OnEndReadingTwitchResponses onEndReadingTwitchResponses;
    
    public delegate void OnOuijaResponseStarted();
    public static OnOuijaResponseStarted onOuijaResponseStarted;
    
    public delegate void OnOuijaResponseEnded(string word);
    public static OnOuijaResponseEnded onOuijaResponseEnded;

    public delegate void OnLetterReached(string letter);
    public static OnLetterReached onLetterReached;

    public delegate void OnCommandReceived(string message);
    public static OnCommandReceived onCommandReceived;



}

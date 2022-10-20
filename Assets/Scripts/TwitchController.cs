using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;

[AddComponentMenu("")]
public class TwitchController : MonoBehaviour
{
    private static readonly int PORT = 6667;
    private static readonly string SERVER = "irc.chat.twitch.tv";
    private static readonly string PASSWORD = "kappa";
    private static readonly string USERNAME = "justinfan12245";

    #region SINGLETON
    private static TwitchController instance;
    public static TwitchController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("TwitchController");
                instance = go.AddComponent<TwitchController>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }
    #endregion

    public bool isConnected { get; private set; }

    private float timer;
    private string channelName = "";

    private StreamReader input;
    private StreamWriter output;
    private NetworkStream networkStream;
    private List<string> recievedMsgs = new List<string>();
    private Queue<string> commandQueue = new Queue<string>();

    private void Update()
    {
        if (networkStream == null) return;
        IRCInputProcedure();
        IRCOutputProcedure();

        if (recievedMsgs.Count > 0)
        {
            for (int i = 0; i < recievedMsgs.Count; i++)
            {
                ParseChatMessage(recievedMsgs[i]);
            }

            recievedMsgs.Clear();
        }
    }

    public void Start()
    {
        Login("RothioTome");
    }

    public void Login(string channel)
    {
        int startIndex = channel.IndexOf(".tv/") == -1 ? 0 : channel.IndexOf(".tv/") + 4;
        channel = channel.TrimEnd('/');
        string trimmedChannelName = channel.Substring(startIndex, channel.Length - startIndex);
        channelName = trimmedChannelName;
        StartIRC();
    }

    private void StartIRC()
    {
        TcpClient sock = new TcpClient();
        sock.Connect(SERVER, PORT);
        if (!sock.Connected)
        {
            Debug.Log("Failed to connect!");
        }
        else
        {
            Debug.Log("Connected successfully");
        }

        networkStream = sock.GetStream();
        input = new StreamReader(networkStream);
        output = new StreamWriter(networkStream);

        output.WriteLine("PASS " + PASSWORD);
        output.WriteLine("NICK " + USERNAME.ToLower());
        output.Flush();
    }

    private void IRCInputProcedure()
    {
        if (!networkStream.DataAvailable)
            return;

        string buffer = input.ReadLine();
        
        Debug.Log(buffer);

        //was message?
        if (buffer.Contains("PRIVMSG #"))
        {
            recievedMsgs.Add(buffer);
        }

        //Send pong reply to any ping messages
        if (buffer.StartsWith("PING "))
        {
            SendCommand(buffer.Replace("PING", "PONG"));
        }

        //After server sends 001 command, we can join a channel
        if (buffer.Split(' ')[1] == "001")
        {
            SendCommand("JOIN #" + channelName.ToLower());
            isConnected = true;
        }
    }

    private void IRCOutputProcedure()
    {
        timer += Time.deltaTime;

        if (commandQueue.Count > 0) //do we have any commands to send?
        {
            //have enough time passed since we last sent a message/command?
            if (timer > 1.750f)
            {
                //send msg.
                output.WriteLine(commandQueue.Peek());
                output.Flush();
                //remove msg from queue.
                commandQueue.Dequeue();
                //restart stopwatch.
                timer = 0;
            }
        }
    }

    public void SendCommand(string cmd)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue(cmd);
        }
    }

    private void ParseChatMessage(string msg)
    {
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);
        if (msgString.Length > 0)
        {
            if (msgString[0].Equals('!'))
            {
                if (msgString.StartsWith(GameController.command))
                {
                    msgString = msgString.Remove(0, GameController.command.Length);
                    if (!string.IsNullOrEmpty(msgString) && !string.IsNullOrWhiteSpace(msgString))
                    {
                        EventsManager.onCommandReceived.Invoke(user, msgString);
                    }
                }
            }
        }
    }

}
﻿using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class TwitchController : MonoBehaviour
{
    private string server = "irc.chat.twitch.tv";
    private int port = 6667;

    public class MsgEvent : UnityEngine.Events.UnityEvent<string>
    {
    }

    public static MsgEvent messageRecievedEvent = new MsgEvent();

    private string buffer = string.Empty;
    private bool stopThreads = false;
    private Queue<string> commandQueue = new Queue<string>();
    private List<string> recievedMsgs = new List<string>();
    private Thread inProc, outProc;

    private void StartIRC()
    {
        System.Net.Sockets.TcpClient sock = new System.Net.Sockets.TcpClient();
        sock.Connect(server, port);
        if (!sock.Connected)
        {
            Debug.Log("Failed to connect!");
            return;
        }
        else
        {
            Debug.Log("Connected successfully");
        }

        var networkStream = sock.GetStream();
        var input = new System.IO.StreamReader(networkStream);
        var output = new System.IO.StreamWriter(networkStream);

        //Send PASS & NICK.
        output.WriteLine("PASS " + TwitchData.Instance.password);
        output.WriteLine("NICK " + TwitchData.Instance.username.ToLower());
        output.Flush();

        //output proc
        outProc = new System.Threading.Thread(() => IRCOutputProcedure(output));
        outProc.Start();
        //input proc
        inProc = new System.Threading.Thread(() => IRCInputProcedure(input, networkStream));
        inProc.Start();
    }

    private void IRCInputProcedure(System.IO.TextReader input, System.Net.Sockets.NetworkStream networkStream)
    {
        while (!stopThreads)
        {
            if (!networkStream.DataAvailable)
            {
                Thread.Sleep(1);
                continue;
            }

            buffer = input.ReadLine();
            //Debug.Log(buffer);

            //was message?
            if (buffer.Contains("PRIVMSG #"))
            {
                lock (recievedMsgs)
                {
                    recievedMsgs.Add(buffer);
                }
            }

            //Send pong reply to any ping messages
            if (buffer.StartsWith("PING "))
            {
                SendCommand(buffer.Replace("PING", "PONG"));
            }

            //After server sends 001 command, we can join a channel
            if (buffer.Split(' ')[1] == "001")
            {
                SendCommand("JOIN #" + TwitchData.Instance.channelName.ToLower());
            }

            Thread.Sleep(1);
        }
    }

    private void IRCOutputProcedure(System.IO.TextWriter output)
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        while (!stopThreads)
        {
            lock (commandQueue)
            {
                if (commandQueue.Count > 0) //do we have any commands to send?
                {
                    // https://github.com/justintv/Twitch-API/blob/master/IRC.md#command--message-limit 
                    //have enough time passed since we last sent a message/command?
                    if (stopWatch.ElapsedMilliseconds > 1750)
                    {
                        //send msg.
                        output.WriteLine(commandQueue.Peek());
                        output.Flush();
                        //remove msg from queue.
                        commandQueue.Dequeue();
                        //restart stopwatch.
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    public void SendCommand(string cmd)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue(cmd);
        }
    }

    public void SendMsg(string msg)
    {
        lock (commandQueue)
        {
            commandQueue.Enqueue("PRIVMSG #" + TwitchData.Instance.channelName.ToLower() + " :" + msg+"\r\n");
        }
    }

    void OnEnable()
    {
        stopThreads = false;
        StartIRC();
    }

    void OnDisable()
    {
        stopThreads = true;
    }

    void OnDestroy()
    {
        stopThreads = true;
    }

    void Update()
    {
        lock (recievedMsgs)
        {
            if (recievedMsgs.Count > 0)
            {
                for (int i = 0; i < recievedMsgs.Count; i++)
                {
                    messageRecievedEvent.Invoke(recievedMsgs[i]);
                }

                recievedMsgs.Clear();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientUDP : MonoBehaviour
{
    Socket newSocket;
    byte[] data = new byte[1024];
    byte[] message = new byte[1024];
    int recv;
    string stringData;

    IPEndPoint ipep;
    

    string strMessage = "";
    int port = 9050;

    void Start()
    {
        Debug.Log("Start");

        Thread listenThread;

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();

    }

    private void ReceiveData()
    {
        string welcome = "Hello, are you there?";
        data = Encoding.ASCII.GetBytes(welcome);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);
        while (true)
        {          
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        }       
        
    }

    private void OnDestroy()
    {
        newSocket.Close();
    }
    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 380, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -lu 127.0.0.1  " + port + " \n"
                , style);

        strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
        {
            SendString(strMessage + "\n");
        }
    }

    private void SendString(string v)
    {
        try
        {
            message = Encoding.ASCII.GetBytes(v);
            newSocket.SendTo(message, message.Length, SocketFlags.None, ipep);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}
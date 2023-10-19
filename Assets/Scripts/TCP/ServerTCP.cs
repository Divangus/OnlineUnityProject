using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerTCP : MonoBehaviour
{
    Socket newsock;
    int port = 9050;
    byte[] data = new byte[1024];
    int recv;
    Thread listenThread;

    void Start()
    {
        Debug.Log("Start");
        

        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);

        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        newsock.Bind(ipep);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();

    }

    private void ReceiveData()
    {
        Debug.Log("Recieve Data");
        newsock.Listen(10);

        Socket client = newsock.Accept();
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("Connected with " + clientep.Address + " at port " + clientep.Port);

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);

        client.Send(data, data.Length, SocketFlags.None);
        recv = client.Receive(data);
    }

    void OnApplicationQuit()
    {
        try
        {
            newsock.Close();
            listenThread.Abort();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

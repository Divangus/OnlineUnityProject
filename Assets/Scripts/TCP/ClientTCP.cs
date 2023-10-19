using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientTCP : MonoBehaviour
{
    Socket server;
    byte[] data = new byte[1024];
    int recv;
    string input;
    string stringData;
    IPEndPoint ipep;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");

        Thread listenThread;

        ipep = new IPEndPoint(IPAddress.Parse("10.0.203.33"), 9050);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Debug.Log("Unable to connect to server.");
            Debug.Log(e.ToString());
            return;
        }

        listenThread = new Thread(ReceiveData);
        listenThread.Start();

    }

    private void ReceiveData()
    {
        data = new byte[1024];
        int recv = server.Receive(data);

        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Debug.Log(stringData);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerUDP : MonoBehaviour
{

    Socket newSocket;
    // Start is called before the first frame update
    int port = 9050;
    byte[] data = new byte[1024];
    int recv;

    void Start()
    {
        Debug.Log("Start");
        Thread listenThread;

        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();
        // newSocket.SendTo(data, recv, SocketFlags.None, Remote);

    }

    private void ReceiveData()
    {
        Debug.Log("Receive");
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
        EndPoint Remote = (EndPoint)(sender);
        recv = newSocket.ReceiveFrom(data, ref Remote);

        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        newSocket.SendTo(data, data.Length, SocketFlags.None, Remote);
        //newSocket.Close();
    }
}

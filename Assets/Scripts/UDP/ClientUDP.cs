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
    int recv;
    string stringData;
    IPEndPoint ipep;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");

        Thread listenThread;

        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();

    }

    private void ReceiveData()
    {
        string welcome = "Hello, are you there?";
        data = Encoding.ASCII.GetBytes(welcome);
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;

        data = new byte[1024];
        int recv = newSocket.ReceiveFrom(data, ref Remote);

        Debug.Log("Message received from:"+ Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        newSocket.Close();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Threading;

public class ServerUDP : MonoBehaviour
{
    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];

    // Start is called before the first frame update
    void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

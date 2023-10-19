
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

    public string lastReceivedUdpPacket = "";
    public string allReceivedUdpPacket = "";

    Socket newSocket;
    // Start is called before the first frame update
    int port = 9050;
    byte[] data = new byte[1024];
    int recv;
    EndPoint Remote;

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
        

        while (true)
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
            Remote = (EndPoint)(sender);
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote);
            //newSocket.Close();
            Debug.Log("Receive");          
            
            recv = newSocket.ReceiveFrom(data, ref Remote);

            string text = Encoding.UTF8.GetString(data);

            print(">> " + text);

            lastReceivedUdpPacket = text;

            allReceivedUdpPacket = allReceivedUdpPacket + text;
        }            
    }

    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -u 127.0.0.1 : " + port + " \n"
                    + "\nLast Packet: \n" + lastReceivedUdpPacket
                    + "\n\nAll Messages: \n" + allReceivedUdpPacket
                , style);
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using UnityEditor.Experimental.GraphView;

public class UDPReceive : MonoBehaviour
{
    Thread listenThread;

    UdpClient udpClient;

    public string IP = "10.0.103.15";
    public int serverPort;

    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];
    IPAddress serverIP;

    public string lastReceivedUdpPacket = "";
    public string allReceivedUdpPacket = "";

    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.init();
    }

    // Start is called before the first frame update
    void Start()
    {
        init();      

    }

    private void init()
    {
        print("UDPSend.init()");

        serverPort = 29000;

        listenThread = new Thread(new ThreadStart(ReceiveData));
        listenThread.IsBackground = true;
        listenThread.Start();
    }
    //private void ReceiveCallback(IAsyncResult ar)
    //{
    //    try
    //    {
    //        // Finish receiving data
    //        int bytesRead = clientSocket.EndReceive(ar);

    //        if (bytesRead > 0)
    //        {
    //            // Handle received data here
    //            string receivedData = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
    //            Debug.Log("Received data: " + receivedData);
    //        }

    //        // Continue listening for more data
    //        clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error receiving data: " + e.Message);
    //    }
    //}

    // You can send data using clientSocket.Send() when needed.

    private void OnDestroy()
    {
        // Close the socket when the GameObject is destroyed
        clientSocket.Close();
    }

    void ReceiveData()
    {
        // Connect to the server
        udpClient = new UdpClient(serverPort);
        while(true)
        {
            try
            {
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref serverEndPoint);

                string text = Encoding.UTF8.GetString(data);

                print(">> " + text);

                lastReceivedUdpPacket = text;

                allReceivedUdpPacket = allReceivedUdpPacket + text;  
            }
            catch (Exception err)
            {
                print(err.ToString()); 
            }
        }
    }

    public string getLastUDPPacket()
    {
        allReceivedUdpPacket = " ";
        return lastReceivedUdpPacket;
    }
}


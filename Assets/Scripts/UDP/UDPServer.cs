using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPServer : MonoBehaviour
{
    private UdpClient server;
    private IPEndPoint clientEndPoint;
    private Thread receiveThread;

    void Start()
    {
        Debug.Log("UDP Server started.");

        // Create a new UDP server and bind it to a specific port
        server = new UdpClient(9050);
        clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        // Start a thread to receive data
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                byte[] receivedData = server.Receive(ref clientEndPoint);
                string receivedMessage = Encoding.ASCII.GetString(receivedData);
                Debug.Log("Received: " + receivedMessage);

                // Send a response back to the client
                string response = "Hello from the server!";
                byte[] responseData = Encoding.ASCII.GetBytes(response);
                server.Send(responseData, responseData.Length, clientEndPoint);
            }
            catch (Exception e)
            {
                Debug.LogError("Error in ReceiveData: " + e.Message);
            }
        }
    }

    void OnDestroy()
    {
        if (server != null)
        {
            server.Close();
        }

        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}

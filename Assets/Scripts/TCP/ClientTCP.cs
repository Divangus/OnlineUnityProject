using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class ClientTCP : MonoBehaviour
{
    private Socket clientSocket;
    private byte[] receiveBuffer = new byte[1024];

    private void Start()
    {
        // Create a new TCP socket
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Define the IP address and port of the server you want to connect to
        IPAddress serverIP = IPAddress.Parse("10.0.103.15"); // Replace with your server's IP
        int serverPort = 29000; // Replace with your server's port

        // Create an endpoint that represents the server
        IPEndPoint serverEndPoint = new IPEndPoint(serverIP, serverPort);

        // Connect to the server
        clientSocket.BeginConnect(serverEndPoint, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Finish connecting to the server
            clientSocket.EndConnect(ar);

            Debug.Log("Connected to server.");

            // Start receiving data asynchronously
            clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Finish receiving data
            int bytesRead = clientSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                // Handle received data here
                string receivedData = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                Debug.Log("Received data: " + receivedData);
            }

            // Continue listening for more data
            clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    // You can send data using clientSocket.Send() when needed.

    private void OnDestroy()
    {
        // Close the socket when the GameObject is destroyed
        clientSocket.Close();
    }
}

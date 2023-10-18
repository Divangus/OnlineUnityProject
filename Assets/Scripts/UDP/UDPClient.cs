using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPClient : MonoBehaviour
{
    private UdpClient client;
    private IPEndPoint serverEndPoint;
    private Thread receiveThread;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("UDP Client started.");

        // Create a new UDP client and set the server endpoint
        client = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);

        // Start a thread to receive data
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();

        // Send a message to the server
        string message = "Hello from the client!";
        byte[] data = Encoding.ASCII.GetBytes(message);
        client.Send(data, data.Length, serverEndPoint);
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                byte[] receivedData = client.Receive(ref serverEndPoint);
                string receivedMessage = Encoding.ASCII.GetString(receivedData);
                Debug.Log("Received: " + receivedMessage);
            }
            catch (SocketException socketException)
            {
                Debug.LogError("SocketException: " + socketException.SocketErrorCode.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError("Error in ReceiveData: " + e.Message);
            }
        }
    }

    // OnDestroy is called when the script is destroyed (e.g., when stopping the game)
    void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }

        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}

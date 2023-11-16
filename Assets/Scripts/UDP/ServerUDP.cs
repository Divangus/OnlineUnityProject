using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerUDP : MonoBehaviour
{
    Socket newSocket;
    int port = 9050;
    byte[] data = new byte[1024];
    int recv;
    Thread listenThread;

    public TMP_Text ipAddressText;

    // Reference to the UI button in the Unity editor
    public Button sendMessageButton;

    void Start()
    {
        Debug.Log("Start");

        string localIP = GetLocalIPAddress();
        ipAddressText.text = "Local IP: " + localIP;
        Debug.Log("Local IP Address: " + localIP);

        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(localIP), port);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();

        // Add a listener to the button click event
        sendMessageButton.onClick.AddListener(SendMessageToClient);
    }

    private string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1";

        try
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                {
                    localIP = address.ToString();
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error getting local IP address: " + e.Message);
        }

        return localIP;
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

        string startMessage = "Start";
        data = Encoding.ASCII.GetBytes(startMessage);
        newSocket.SendTo(data, data.Length, SocketFlags.None, Remote);
    }

    private void SendMessageToClient()
    {
        try
        {
            // Message to be sent
            string message = "Game";

            // Convert the message to bytes
            byte[] messageData = Encoding.ASCII.GetBytes(message);

            // Broadcast the message to all connected clients
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
            newSocket.SendTo(messageData, messageData.Length, SocketFlags.None, clientEndPoint);

            Debug.Log("Message sent to all connected clients: " + message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error sending message to clients: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            newSocket.Close();
            listenThread.Abort();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}

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

        Thread.Sleep(1000);

        startMessage = "Game";
        data = Encoding.ASCII.GetBytes(startMessage);
        newSocket.SendTo(data, data.Length, SocketFlags.None, Remote);


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

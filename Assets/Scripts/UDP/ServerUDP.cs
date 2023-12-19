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
    Thread playGame;
    int players = 0;

    public TMP_Text ipAddressText;

    public Button sendMessageButton;

    IPEndPoint ipep;
    IPEndPoint sender;
    EndPoint[] Remote = new EndPoint[2];

    private bool playGameThreadRunning = false;

    void Start()
    {
        Debug.Log("Start");

        string localIP = GetLocalIPAddress();
        ipAddressText.text = "Local IP: " + localIP;
        Debug.Log("Local IP Address: " + localIP);

        ipep = new IPEndPoint(IPAddress.Parse(localIP), port);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        newSocket.Bind(ipep);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();
        playGame = new Thread(playCounter);
    }

    private void Update()
    {
        if (playGameThreadRunning)
        {
            SaveData savedData = FindObjectOfType<SaveData>();
            savedData.socket = newSocket;
            savedData.Remote = Remote;
            savedData.client = true;
            playGame.Start();
            playGameThreadRunning = false;
        }
    }

    void playCounter()
    {
        for(int i = 0; i < Remote.Length; i++)
        {
            Debug.Log("aqui");
            string startMessage = "Game";
            data = Encoding.ASCII.GetBytes(startMessage);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[i]);
        }
        
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
        while(!playGameThreadRunning)
        {
            Debug.Log("Receive");
            sender = new IPEndPoint(IPAddress.Any, players);
            Remote[players] = (EndPoint)(sender);
            recv = newSocket.ReceiveFrom(data, ref Remote[players]);

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            string startMessage = "Start";
            data = Encoding.ASCII.GetBytes(startMessage);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            string PlayerNum = "No";
            if (players == 0)
            {
                PlayerNum = "Player 1";
            }
            if (players == 1)
            {
                PlayerNum = "Player 2";
            }
            Debug.Log(PlayerNum);
            data = Encoding.ASCII.GetBytes(PlayerNum);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            players++;
        }        

    }

    public void OnButtonClick()
    {
        // Toggle the playGameThreadRunning flag
        playGameThreadRunning = !playGameThreadRunning;
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

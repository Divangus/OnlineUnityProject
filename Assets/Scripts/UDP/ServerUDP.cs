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
    
    Thread listenThread;
    Thread playGame;
    int players = 0;

    public TMP_Text ipAddressText;

    public Button sendMessageButton;

    EndPoint[] Remote = new EndPoint[2];

    private bool playGameThreadRunning = false;

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
        playGame = new Thread(playCounter);
    }

    private void Update()
    {
        if (playGameThreadRunning)
        {
            SaveData savedData = FindObjectOfType<SaveData>();
            savedData.socket = newSocket;
            savedData.Remote = Remote;
            savedData.server = true;
            playGame.Start();
            playGameThreadRunning = false;
        }
    }

    void playCounter()
    {
        for(int i = 0; i < Remote.Length; i++)
        {
            byte[] data = new byte[1024];
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
        while (!playGameThreadRunning)
        {
            byte[] data = new byte[1024];
            int recv = 0;
            //Debug.Log("Receive");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, players);
            Remote[players] = (EndPoint)(sender);

            try
            {
                recv = newSocket.ReceiveFrom(data, ref Remote[players]);
                Debug.Log("Receive Player" + (players + 1).ToString());
            }
            catch
            {
                Debug.Log("Stop waiting for clients!");
                //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
                return;
            }

            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            string startMessage = "Start";
            data = Encoding.ASCII.GetBytes(startMessage);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            string PlayerNum = "Player " + (players + 1).ToString();
            data = Encoding.ASCII.GetBytes(PlayerNum);
            newSocket.SendTo(data, data.Length, SocketFlags.None, Remote[players]);

            players++;

            if(players == 2)
            {
                listenThread.Abort();
            }
        }        

    }

    public void OnButtonClick()
    {
        // Toggle the playGameThreadRunning flag
        playGameThreadRunning = !playGameThreadRunning;
    }

    void OnApplicationQuit()
    {
         newSocket.Close();
         listenThread.Abort();        
    }
}

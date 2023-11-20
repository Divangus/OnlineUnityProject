using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine.SceneManagement;

public class ClientUDP : MonoBehaviour
{
    Socket newSocket;
    string stringData;
    IPEndPoint ipep;
    Thread listenThread;
    IPEndPoint sender;
    EndPoint Remote;

    public TMP_InputField ipAddressText;
    public TextMeshProUGUI connectionStatusText;
    public GameObject player1;
    public GameObject player2;
    private GameObject instantiatedPlayer1;
    private GameObject instantiatedPlayer2;

    private string input;

    bool connected;
    bool startGame;
    bool showPlayers;

    // Start is called before the first frame update
    void Start()
    {
        connected = false;
        showPlayers = false;
        connectionStatusText.text = "Not Connected";
        listenThread = new Thread(NetworkThread);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(ipAddressText.text))
        {
            input = ipAddressText.text;
            Debug.Log("Input: " + input);
            Join();
        }

        if (connected)
        {
            // Update UI on the main thread
            connectionStatusText.text = "Connected";
            connected = false;
        }

        if (startGame)
        {
            SaveData savedData = FindObjectOfType<SaveData>();
            savedData.socket = newSocket;
            savedData.Remote = Remote;
            savedData.player2 = true;
            SceneManager.LoadScene("MainScene");
            startGame = false;
        }

        if (showPlayers)
        {
            instantiatedPlayer1 = Instantiate(player1, new Vector3(-10, 0, 11), Quaternion.identity);
            instantiatedPlayer2 = Instantiate(player2, new Vector3(10, 0, 11), Quaternion.identity);
            showPlayers = false;
        }
    }

    private void Join()
    {
        ipep = new IPEndPoint(IPAddress.Parse(input), 9050);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        listenThread.Start();
    }

    //THREAD
    private void NetworkThread()
    {
        byte[] data = Encoding.ASCII.GetBytes("Hello, are you there?");
        newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

        sender = new IPEndPoint(IPAddress.Any, 0);
        Remote = (EndPoint)sender;


        try
        {
            data = new byte[1024];
            int recv = newSocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            connected = true;
        }
        catch
        {
            Debug.Log("(1) Client stopped waiting.");
            return;
        }

        string startMessage = "";
        try
        {
            data = new byte[1024];
            int recv = newSocket.ReceiveFrom(data, ref Remote);
            startMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + startMessage);
        }
        catch
        {
            Debug.Log("(2) Client stopped waiting.");
            return;
        }

        if (startMessage == "Start")
        {
            showPlayers = true;
        }
        else
        {
            return;
        }

        //Wait for server to start the game
        string gameMessage = "";
        try
        {
            data = new byte[1024];
            int recv = newSocket.ReceiveFrom(data, ref Remote);
            gameMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + gameMessage);
        }
        catch
        {
            Debug.Log("(3) Client stopped waiting.");
            return;
        }

        // Update UI and instantiate players on the main thread
        if (gameMessage == "Game")
        {
            startGame = true;
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            newSocket.Close();
            listenThread.Abort();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void ReadStringInputstring(string s)
    {
        input = s;
        Debug.Log(input);
    }
}


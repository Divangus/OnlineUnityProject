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
    byte[] data = new byte[1024];
    int recv;
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

    // Start is called before the first frame update
    void Start()
    {
        connectionStatusText.text = "Not Connected";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(ipAddressText.text))
        {
            input = ipAddressText.text;
            Debug.Log("Input: " + input);
            StartCoroutine(Join());
        }
    }

    private IEnumerator Join()
    {
        ipep = new IPEndPoint(IPAddress.Parse(input), 9050);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        yield return StartCoroutine(NetworkCoroutine());

        // Start a thread for network operations
    }

    private IEnumerator NetworkCoroutine()
    {
        try
        {
            string welcome = "Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

            sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            // Update UI on the main thread
            connectionStatusText.text = "Connected";

            recv = newSocket.ReceiveFrom(data, ref Remote);
            string startMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + startMessage);

            if (startMessage == "Start")
            {
                instantiatedPlayer1 = Instantiate(player1, new Vector3(-10, 0, 11), Quaternion.identity);
                instantiatedPlayer2 = Instantiate(player2, new Vector3(10, 0, 11), Quaternion.identity);
            }

            recv = newSocket.ReceiveFrom(data, ref Remote);
            string gameMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + gameMessage);

            // Update UI and instantiate players on the main thread
            if (gameMessage == "Game")
            {
                SceneManager.LoadScene("MainScene");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        yield return null;
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

//Build Version

/*using System.Collections;
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
    byte[] data = new byte[1024];
    int recv;
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

    // Start is called before the first frame update
    void Start()
    {
        connectionStatusText.text = "Not Connected";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(ipAddressText.text))
        {
            input = ipAddressText.text;
            Debug.Log("Input: " + input);
            StartCoroutine(Join());
        }
    }

    private IEnumerator Join()
    {
        ipep = new IPEndPoint(IPAddress.Parse(input), 9050);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        yield return null;

        // Start a thread for network operations
        listenThread = new Thread(NetworkThreadFunction);
        listenThread.IsBackground = true;
        listenThread.Start();
    }

    private void NetworkThreadFunction()
    {
        try
        {
            string welcome = "Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

            sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            // Update UI on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                connectionStatusText.text = "Connected";
            });

            recv = newSocket.ReceiveFrom(data, ref Remote);
            string startMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + startMessage);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (startMessage == "Start")
                {
                    instantiatedPlayer1 = Instantiate(player1, new Vector3(-10, 0, 11), Quaternion.identity);
                    instantiatedPlayer2 = Instantiate(player2, new Vector3(10, 0, 11), Quaternion.identity);
                }
            });

            recv = newSocket.ReceiveFrom(data, ref Remote);
            string gameMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + gameMessage);

            // Update UI and instantiate players on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (gameMessage == "Game")
                {
                    SceneManager.LoadScene("MainScene");
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
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
}*/
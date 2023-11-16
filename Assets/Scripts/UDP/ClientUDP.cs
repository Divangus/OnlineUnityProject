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
    public TMP_InputField ipAddressText;
    public TextMeshProUGUI connectionStatusText; // Reference to the TextMeshPro object
    private string input;

    // Start is called before the first frame update
    void Start()
    {
        connectionStatusText.text = "Not Connected"; // Initial status
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(ipAddressText.text))
        {
            input = ipAddressText.text;
            Debug.Log("Input: " + input);
            Join();
        }
    }

    private void Join()
    {
        Debug.Log("Start");

        ipep = new IPEndPoint(IPAddress.Parse(input), 9050);

        newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        listenThread = new Thread(ReceiveData);
        listenThread.Start();
    }

    private void ReceiveData()
    {
        try
        {
            string welcome = "Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            newSocket.SendTo(data, data.Length, SocketFlags.None, ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            data = new byte[1024];
            recv = newSocket.ReceiveFrom(data, ref Remote);

            Debug.Log("Message received from:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            // Update the connection status text
            connectionStatusText.text = "Connected";

            recv = newSocket.ReceiveFrom(data, ref Remote);
            string startMessage = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log("Start message from server: " + startMessage);

            if (startMessage == "Start")
            {
                // Load the next scene (you can replace "YourSceneName" with the actual scene name)
                SceneManager.LoadScene("MainScene");
            }

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
}

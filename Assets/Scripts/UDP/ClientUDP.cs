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
    public TextMeshProUGUI connectionStatusText;
    public GameObject spherePrefab; // Reference to the prefab of the sphere
    private GameObject instantiatedSphere;


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

        yield return null; // Wait for a frame to ensure socket creation is done

        StartCoroutine(ReceiveData());
    }

    private IEnumerator ReceiveData()
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
                instantiatedSphere = Instantiate(spherePrefab, new Vector3(0, 1, 0), Quaternion.identity);
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
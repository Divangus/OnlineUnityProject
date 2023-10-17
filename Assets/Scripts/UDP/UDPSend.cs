using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    public string IP;

    public int port;

    IPEndPoint remoteEndPoint;
    UdpClient udpClient;

    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.init();

        sendObj.sendEndless("endles info \n ");
    }

    public void Start()
    {
        init();
    }

    public void init()
    {
        print("UDPSend.init()");

        IP = "10.0.103.15";
        port = 29000;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        udpClient = new UdpClient();
    }

    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();
                if (text != "")
                {
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    udpClient.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    private void SendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            udpClient.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    private void sendEndless(string textStr) 
    {
        do
        {
            SendString(textStr);
        } while (true);
    }
}

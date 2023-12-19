using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public Socket socket;
    public EndPoint[] Remote = new EndPoint[2];
    public EndPoint ServerRemote;
    public bool player2 = false;
    public bool player1 = false;
    public bool client = false;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

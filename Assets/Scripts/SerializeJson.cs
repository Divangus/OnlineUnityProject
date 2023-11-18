using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using UnityEngine.tvOS;
using KartGame.KartSystems;

public class PlayerData
{
    public Vector3 PlayerPos;
}

public class SerializeJson : MonoBehaviour
{
    GameObject PlayerDefault;
    GameObject PlayerEnemy;
    //string json;
    float time = 1.0f;
    SaveData saveData;
    Thread reciveEnemy, sendPlayer;
    bool playing = true;

    private void Start()
    {
        saveData = FindObjectOfType<SaveData>();

        if (saveData.player1 == true)
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player1");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player2");
        }
        else
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player2");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player1");
        }

        PlayerEnemy.GetComponent<ArcadeKart>().enabled = false;

        reciveEnemy = new Thread(LoadPlayer);
        reciveEnemy.Start();

        sendPlayer = new Thread(SavePlayer);
        sendPlayer.Start();
    }

    void SavePlayer()
    {
        while(playing)
        {
            if(time <= 0.0f)
            {
                PlayerData playerData = new PlayerData
                {
                    PlayerPos = PlayerDefault.transform.position
                };

                string json = JsonUtility.ToJson(playerData);
                byte[] data = Encoding.ASCII.GetBytes(json);

                saveData.socket.SendTo(data, data.Length, SocketFlags.None, saveData.Remote);

                time = 1.0f;
            }
            else
            {
                time -= Time.deltaTime;
            }
        }

    }

    void LoadPlayer()
    {
        while(playing)
        {
            byte[] data = new byte[1024];

            int recv = saveData.socket.ReceiveFrom(data, ref saveData.Remote);

            string json = Encoding.ASCII.GetString(data, 0, recv);

            // Decerialize JSON back to player date
            PlayerData deserializedPlayer = JsonUtility.FromJson<PlayerData>(json);
            //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
            PlayerEnemy.transform.position = deserializedPlayer.PlayerPos;
        }
    }
}

//public GameObject Player;

    //static MemoryStream stream;
    //bool a = true;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (a)
    //    {
    //        serializeJson();
    //        deserializeJson();
    //        a = false;
    //    }
    //}

    //public class testClass
    //{
    //    public Vector3 PlayerPos;
    //    public string PlayerName;
    //    //public int hp = 12;
    //    //public List<int> pos = new List<int> { 3, 3, 3 };
    //}

    //void serializeJson()
    //{
    //    var t = new testClass();
    //    t.PlayerName = "caca";
    //    t.PlayerPos = Player.transform.position;
    //    //t.hp = 40;
    //    //t.pos = new List<int> { 10, 3, 12 };
    //    string json = JsonUtility.ToJson(t);
    //    stream = new MemoryStream();
    //    BinaryWriter writer = new BinaryWriter(stream);
    //    writer.Write(json);
    //}
    //void deserializeJson()
    //{
    //    var t = new testClass();
    //    BinaryReader reader = new BinaryReader(stream);
    //    stream.Seek(0, SeekOrigin.Begin);

    //    string json = reader.ReadString();
    //    Debug.Log(json);
    //    t = JsonUtility.FromJson<testClass>(json);
    //    Debug.Log(t.PlayerName.ToString() + " " + t.PlayerPos.ToString());
    //}   

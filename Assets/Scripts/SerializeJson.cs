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
using KartGame.KartSystems;

public class PlayerData
{
    public Vector3 PlayerPos;
}

public class SerializeJson : MonoBehaviour
{
    GameObject PlayerDefault;
    GameObject PlayerEnemy;

    SaveData saveData;
    Thread reciveEnemy, sendPlayer, reciveClients, sendClients;
    bool playing = true;
    PlayerData playerData;
    bool playerTransform = false;
    bool updateEnemy = false;
    PlayerData deserializedPlayer;
    PlayerData player1;
    PlayerData player2;

    private void Start()
    {
        saveData = FindObjectOfType<SaveData>();
        playerData = new PlayerData();

        if (saveData.player1 == true)
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player2");

            GameObject.FindGameObjectWithTag("Camera1").SetActive(false);
        }
        else
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player2");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player");

            GameObject.FindGameObjectWithTag("Camera2").SetActive(false);
        }
            
        if(saveData.client)
        {
            PlayerDefault.GetComponent<ArcadeKart>().enabled = false;
            PlayerEnemy.GetComponent<ArcadeKart>().enabled = false;

            reciveClients = new Thread(LoadClient);
            reciveEnemy.Start();
            sendClients = new Thread(SaveClient);
            sendPlayer.Start();
        }
        else
        {
            PlayerEnemy.GetComponent<ArcadeKart>().enabled = false; //client

            reciveEnemy = new Thread(LoadPlayer);
            reciveEnemy.Start();

            sendPlayer = new Thread(SavePlayer);
            sendPlayer.Start();
        }        

    }

    private void Update()
    {
        if (playerTransform)
        {
            playerData.PlayerPos = PlayerDefault.transform.position;
            playerTransform = false;
        }
        if (updateEnemy)
        {
            PlayerEnemy.transform.position = deserializedPlayer.PlayerPos;
            updateEnemy = false;
        }
    }

    void SavePlayer()
    {
        while(playing)
        {
            
                playerTransform = true;
               
                string json = JsonUtility.ToJson(playerData);
                byte[] data = Encoding.ASCII.GetBytes(json);

                //Debug.Log(saveData.socket);
                //Debug.Log(saveData.ServerRemote);
                saveData.socket.SendTo(data, data.Length, SocketFlags.None, saveData.ServerRemote);
                
        }

    }

    void LoadPlayer()
    {
        while(playing)
        {
            byte[] data = new byte[1024];

            //Debug.Log(saveData.socket);
            //Debug.Log(saveData.ServerRemote);
            int recv = saveData.socket.ReceiveFrom(data, ref saveData.ServerRemote);//server

            string json = Encoding.ASCII.GetString(data, 0, recv);

            // Decerialize JSON back to player date
            deserializedPlayer = JsonUtility.FromJson<PlayerData>(json);
            //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
            updateEnemy = true;
        }
    }

    void SaveClient()
    {
        while (playing)
        {

            //playerTransform = true;

            string json = JsonUtility.ToJson(player1);
            byte[] data = Encoding.ASCII.GetBytes(json);

            //Debug.Log(saveData.socket);
            //Debug.Log(saveData.Remote);
            saveData.socket.SendTo(data, data.Length, SocketFlags.None, saveData.Remote[1]);

            string json_ = JsonUtility.ToJson(player2);
            byte[] data_ = Encoding.ASCII.GetBytes(json_);

            //Debug.Log(saveData.socket);
            //Debug.Log(saveData.Remote);
            saveData.socket.SendTo(data_, data_.Length, SocketFlags.None, saveData.Remote[0]);

        }

    }
    void LoadClient()
    {
        while (playing)
        {
            byte[] data = new byte[1024];

            //Debug.Log(saveData.socket);
            //Debug.Log(saveData.Remote);
            int recv = saveData.socket.ReceiveFrom(data, ref saveData.Remote[0]);

            string json = Encoding.ASCII.GetString(data, 0, recv);

            // Decerialize JSON back to player date
            player1 = JsonUtility.FromJson<PlayerData>(json);
            //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos

            byte[] data_ = new byte[1024];

            //Debug.Log(saveData.socket);
            //Debug.Log(saveData.Remote);
            int recv_ = saveData.socket.ReceiveFrom(data_, ref saveData.Remote[1]);

            string json_ = Encoding.ASCII.GetString(data_, 0, recv_);

            // Decerialize JSON back to player date
            player2 = JsonUtility.FromJson<PlayerData>(json_);
            //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
        }
    }
}


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
    public GameObject Camera1;
    public GameObject Camera2;

    SaveData saveData;
    Thread reciveEnemy, sendPlayer;
    bool playing = true;
    PlayerData playerData;
    bool playerTransform = false;
    bool updateEnemy = false;
    PlayerData deserializedPlayer;

    private void Start()
    {
        saveData = FindObjectOfType<SaveData>();
        playerData = new PlayerData();

        if (saveData.player1 == true)
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player2");

            Camera1.SetActive(true);
        }
        else
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player2");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player");

            Camera2.SetActive(true);
        }
             

        PlayerEnemy.GetComponent<ArcadeKart>().enabled = false; //client

        reciveEnemy = new Thread(LoadPlayer);
        reciveEnemy.Start();

        sendPlayer = new Thread(SavePlayer);
        sendPlayer.Start();

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

                Debug.Log(saveData.socket);
                Debug.Log(saveData.Remote);
                saveData.socket.SendTo(data, data.Length, SocketFlags.None, saveData.Remote);
                
        }

    }

    void LoadPlayer()
    {
        while(playing)
        {
            byte[] data = new byte[1024];

            Debug.Log(saveData.socket);
            Debug.Log(saveData.Remote);
            int recv = saveData.socket.ReceiveFrom(data, ref saveData.Remote);//server

            string json = Encoding.ASCII.GetString(data, 0, recv);

            // Decerialize JSON back to player date
            deserializedPlayer = JsonUtility.FromJson<PlayerData>(json);
            //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
            updateEnemy = true;
        }
    }   
}


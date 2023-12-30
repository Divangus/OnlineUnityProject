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
    public Quaternion PlayerRot;
    public int playerNum;
}

public class SerializeJson : MonoBehaviour
{
    GameObject PlayerDefault;
    GameObject PlayerEnemy;

    SaveData saveData;
    Thread reciveEnemy, sendPlayer;
    bool playing = true;
    PlayerData playerData;
    bool playerTransform = false;
    bool updateEnemy = false;
    PlayerData deserializedPlayer;
    PlayerData player1;
    PlayerData player2;

    EndPoint remote;

    private void Start()
    {
        saveData = FindObjectOfType<SaveData>();
        playerData = new PlayerData();

        if (saveData.player1 == true)
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player2");

            playerData.playerNum = 1;

            GameObject.FindGameObjectWithTag("Camera1").SetActive(false);
        }
        else
        {
            PlayerDefault = GameObject.FindGameObjectWithTag("Player2");

            PlayerEnemy = GameObject.FindGameObjectWithTag("Player");

            playerData.playerNum = 2;

            GameObject.FindGameObjectWithTag("Camera2").SetActive(false);
        }
            
        if(saveData.server)
        {
            Destroy(PlayerDefault.GetComponent<KeyboardInput>());
            Destroy(PlayerDefault.GetComponent<KartAnimation>());
            Destroy(PlayerDefault.GetComponent<KartPlayerAnimator>());
            Destroy(PlayerEnemy.GetComponent<KeyboardInput>());
            Destroy(PlayerEnemy.GetComponent<KartAnimation>());
            Destroy(PlayerEnemy.GetComponent<KartPlayerAnimator>());
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag("ServerCanvas"));
            Destroy(PlayerEnemy.GetComponent<KeyboardInput>()); //client
            Destroy(PlayerEnemy.GetComponent<KartAnimation>());
            Destroy(PlayerEnemy.GetComponent<KartPlayerAnimator>());
            Destroy(PlayerEnemy.GetComponent<ArcadeKart>());
        }

        remote = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

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
            playerData.PlayerRot = PlayerDefault.transform.rotation;
            playerTransform = false;
        }
        if (updateEnemy)
        {
            PlayerEnemy.transform.position = deserializedPlayer.PlayerPos;
            PlayerEnemy.transform.rotation = deserializedPlayer.PlayerRot;
            updateEnemy = false;
        }
    }

    void SavePlayer()
    {
        while(playing)
        {
            
            if(saveData.server)
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
            else
            {
                playerTransform = true;

                string json = JsonUtility.ToJson(playerData);
                byte[] data = Encoding.ASCII.GetBytes(json);

                //Debug.Log(saveData.socket);
                //Debug.Log(saveData.ServerRemote);
                saveData.socket.SendTo(data, data.Length, SocketFlags.None, saveData.ServerRemote);
            }
                
                
        }

    }

    void LoadPlayer()
    {
        while(playing)
        {
            if(saveData.server)
            {
                byte[] data = new byte[1024];

                //Debug.Log(saveData.socket);
                //Debug.Log(saveData.Remote);
                int recv = saveData.socket.ReceiveFrom(data, ref remote);

                string json = Encoding.ASCII.GetString(data, 0, recv);

                // Decerialize JSON back to player date
                playerData = JsonUtility.FromJson<PlayerData>(json);

                if (playerData.playerNum == 1)
                {
                    player1 = playerData;
                }
                else
                {
                    player2 = playerData;
                }
                
                
                //Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
            }
            else
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
    }
   
}


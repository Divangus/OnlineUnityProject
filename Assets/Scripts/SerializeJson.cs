using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class PlayerData
{
    public Vector3 PlayerPos;
    public string PlayerName;
}

public class SerializeJson : MonoBehaviour
{
    public GameObject PlayerDefault;

    public void ExampleUsage()
    {
        PlayerData playerData = new PlayerData
        {
            PlayerName = "TestPlayer",
            PlayerPos = PlayerDefault.transform.position
        };

        string json = JsonUtility.ToJson(playerData);
        Debug.Log(json);  // Output: {"playerName":"TestPlayer","score":5000}

        // Decerialize JSON back to player date
        PlayerData deserializedPlayer = JsonUtility.FromJson<PlayerData>(json);
        Debug.Log(deserializedPlayer.PlayerName);  // Output: TestPlayer
        Debug.Log(deserializedPlayer.PlayerPos);  // Output: Pos
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

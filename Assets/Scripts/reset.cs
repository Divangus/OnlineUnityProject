using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reset : MonoBehaviour
{
    SaveData saveData;
    // Start is called before the first frame update
    void Start()
    {
        saveData = FindObjectOfType<SaveData>();

        if (saveData != null )
        {
            saveData.socket.Close();
            Destroy( saveData.gameObject );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

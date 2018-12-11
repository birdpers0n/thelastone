using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataPersistence : MonoBehaviour {

    public static DataPersistence data;
    public int exp, coins;

    // Use this for initialization
    void Awake() {
        if (data == null) {
            DontDestroyOnLoad(this);
            data = this;
        } else {
            Destroy(this);
        }

    }

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerData.dat");

        PlayerData data = new PlayerData();
        data.exp = exp;
        data.coins = coins;

        bf.Serialize(file, data);
        file.Close();

    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/playerData.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            exp = data.exp;
            coins = data.coins;
        }
    }
}

[Serializable]
class PlayerData {
    public int exp, coins;
}



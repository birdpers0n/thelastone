using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Disconnect : NetworkBehaviour {

    public NetworkManager networkManager;
    public GameObject spawner;
    GameObject[] players;
    [HideInInspector]
    public bool startChecking = false;
    public int disconnectedCount;

    void Update()
    {
        if (startChecking)
        {
            Debug.Log("Started Checking");
            disconnectedCount = 0;
            players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<Player>().disconnected == 1)
                    disconnectedCount++;
            }

            if (disconnectedCount == 2)
            {
                startChecking = false;
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].GetComponent<Player>().EndForReal();
                }
            }
        }
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void EndConnection()
    {
        spawner.GetComponent<ButtonSpawner>().DisconnectedFeedback();

        // bez ovoga na pocetku se javlja onaj problem (connection still exists)
        //if (networkManager == null) {
        //    networkManager = NetworkManager.singleton;
        //}

        //networkManager.matchMaker.DropConnection(networkManager.matchInfo.networkId, networkManager.matchInfo.nodeId,
        //    0, networkManager.OnDropConnection);
        //networkManager.StopHost();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            players[i].GetComponent<Player>().TargetOpponent();
            players[i].GetComponent<Player>().disconnectMark = true;
        }
        startChecking = true;
    }

    public void ReallyEnd()
    {
        if (networkManager == null) {
            networkManager = FindObjectOfType<NetworkManager>();
        }

        if (spawner == null) {
            spawner = FindObjectOfType<ButtonSpawner>().gameObject;
        }


        networkManager.matchMaker.DropConnection(networkManager.matchInfo.networkId, networkManager.matchInfo.nodeId,
        0, networkManager.OnDropConnection);
        networkManager.StopHost();

        spawner.GetComponent<ButtonSpawner>().checkedAlready = false;
    }

  

}

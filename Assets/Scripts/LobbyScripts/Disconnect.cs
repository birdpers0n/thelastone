﻿using System.Collections;
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
            spawner.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void EndConnection()
    {
        networkManager = NetworkManager.singleton;

        //networkManager.matchMaker.DropConnection(networkManager.matchInfo.networkId, networkManager.matchInfo.nodeId,
        //0, networkManager.OnDropConnection);

        spawner.SetActive(true);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Player>().TargetOpponent();
            players[i].GetComponent<Player>().doStuff = true;
        }
        startChecking = true;
    }

    public void ReallyEnd()
    {
        if (!networkManager)
            networkManager = NetworkManager.singleton;
        
        networkManager.matchMaker.DropConnection(networkManager.matchInfo.networkId, networkManager.matchInfo.nodeId,
        0, networkManager.OnDropConnection);
        networkManager.StopHost();
        //spawner.GetComponent<ButtonSpawner>().Deactivate();
        //GameObject.Find("Button Manager").GetComponent<ButtonManager>().enabled = true;
        //GameObject.Find("Button Manager").GetComponent<ButtonManager>().ActivateMenus();
    }

}

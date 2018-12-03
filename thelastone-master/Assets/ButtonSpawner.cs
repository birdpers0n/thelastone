using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpawner : MonoBehaviour {

    public GameObject buttonHolder;
    public Text WaitingText;
    public GameObject disconnectButton;
    public GameObject backButton;
    public GameObject lobbyDisconnectButton;
    public GameObject authenticationMenu;

    int playerNumber;

    void Update () {
		if(GameObject.FindGameObjectsWithTag("Player").Length == 2) {
            if (!CheckForDisconnectionRequests())
            {
                buttonHolder.SetActive(true);
                WaitingText.text = "";
                if(backButton.activeSelf)
                    backButton.SetActive(false);
                lobbyDisconnectButton.SetActive(false);
                disconnectButton.SetActive(true);
                disconnectButton.GetComponent<Disconnect>().spawner = gameObject;
                if(GameObject.Find("AuthenticationMenu"))
                    GameObject.Find("AuthenticationMenu").SetActive(false);

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].GetComponent<Player>().spawner = gameObject;
                }

                playerNumber = 2;
            }
        }
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0 && buttonHolder.activeSelf)
        {
            buttonHolder.SetActive(false);
            lobbyDisconnectButton.SetActive(false);
            WaitingText.text = "Player Disconnected";
            backButton.SetActive(true);
        }
        if(GameObject.FindGameObjectsWithTag("Player").Length==1 && playerNumber == 2)
        {
            GameObject.Find("Disconnect Button").GetComponent<Disconnect>().ReallyEnd();
        }
	}

    public void Deactivate()
    {
        buttonHolder.SetActive(false);
    }

    bool CheckForDisconnectionRequests()
    {
        int disconnectCounter = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if (player.GetComponent<Player>().disconnected == 1)
                disconnectCounter++;
        }
        return disconnectCounter == 1;
    }

    public void ActivateAuthenticationUI()
    {
        GameObject.Find("Menus").SetActive(false);
        authenticationMenu.SetActive(true);
    }

    public void DeactivateAuthenticationUI()
    {
        authenticationMenu.SetActive(false);
    }
}

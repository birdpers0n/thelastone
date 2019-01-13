using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private GameWorld gw;
    public GameObject spawner;
    public bool disconnectMark = false;
    [HideInInspector]
    public bool changedText = false;
    public Button disconnectBtn;

    [SyncVar]
    public int disconnected = 0;

    public GameObject opponentPlayer;
    public bool requestedRematch = false;
    public bool acceptedRematch = false;

    public static int numOfPlayers = 0;

    void Start() {
        gw = FindObjectOfType<GameWorld>().GetComponent<GameWorld>();
        numOfPlayers++;

        if ((numOfPlayers >= 2) && (numOfPlayers%2 == 0)) {
            gw.canTimerStart = true;
        }

  
        if (!isServer) {
            gw.client = true;
            return;
        }
        if (!isLocalPlayer) return;
        for (int i = 0; i < 25; i++) gw.syncList.Add(-1);
        CmdRand();
  
    }


    void Update()
    {
        if (disconnectMark)
        {
               if (isLocalPlayer) {
            CmdMarkAsDisconnected();
            disconnectMark = false;

            }
        }

        //if (isServer)
        //{
        if (opponentPlayer)
        {
            if (disconnected == 0 && opponentPlayer.GetComponent<Player>().disconnected == 1)
            {
                disconnected = 1;
                //GameObject.Find("Disconnect Button").GetComponent<Disconnect>().startChecking = true;
                //GameObject.Find("Disconnect Button").GetComponent<Disconnect>().networkManager = NetworkManager.singleton;
                disconnectBtn.GetComponent<Disconnect>().startChecking = true;
                disconnectBtn.GetComponent<Disconnect>().networkManager = NetworkManager.singleton;
            }
        }
        else
        {
            TargetOpponent();
        }
        //}
        if (opponentPlayer != null)
        {
            if (opponentPlayer.GetComponent<Player>().requestedRematch && !changedText)
            {
                changedText = true;
                string endText = GameObject.FindGameObjectWithTag("EndText").GetComponent<Text>().text;
                if (endText != "WAITING")
                    GameObject.FindGameObjectWithTag("EndText").GetComponent<Text>().text = "ACCEPT";
            }

            if (requestedRematch && opponentPlayer.GetComponent<Player>().acceptedRematch)
            {
                gw.RestartButtons();
                if (isServer) {
                    for (int i = 0; i < 25; i++) gw.syncList.Insert(i, -1);
                    CmdRand();
                }
                requestedRematch = false;
                gw.GetComponent<GameWorld>().Restart();
            }
            else if (acceptedRematch)
            {
                gw.RestartButtons();
                if (isServer) {
                    for (int i = 0; i < 25; i++) gw.syncList.Insert(i, -1);
                    CmdRand();
                }
                acceptedRematch = false;
                gw.GetComponent<GameWorld>().Restart();
            }
        }
    }

    public bool CheckIfServer()
    {
        return isServer;
    }

    public void EndForReal()
    {
        // if (isLocalPlayer) // Ako je ovako onda samo client moze fino izaci 
        //GameObject.Find("Disconnect Button").GetComponent<Disconnect>().ReallyEnd();
        if (isLocalPlayer) {
            disconnectBtn.GetComponent<Disconnect>().ReallyEnd();
        }
    }

    public void DoRematch()
    {
        if (!isLocalPlayer)
            return;

        if (opponentPlayer == null)
            TargetOpponent();

        if (opponentPlayer.GetComponent<Player>().requestedRematch)
        {
            CmdAcceptRematch();
        }
        else
        {
            GameObject.FindGameObjectWithTag("EndText").GetComponent<Text>().text = "WAITING";
            CmdRequestRematch();
        }
    }

    [Command]
    public void CmdRequestRematch()
    {
        requestedRematch = true;
        RpcRequestRematch();
    }

    [ClientRpc]
    public void RpcRequestRematch()
    {
        requestedRematch = true;
    }

    [Command]
    public void CmdAcceptRematch()
    {
        acceptedRematch = true;
        RpcAcceptRematch();
    }

    [ClientRpc]
    public void RpcAcceptRematch()
    {
        acceptedRematch = true;
    }

    [Command]
    public void CmdRand() {
        int rand;
        for (int i = 0; i < Random.Range(3, 6); i++) {
            rand = Random.Range(0, 25);
            while (!gw.buttonList[rand].interactable) rand = Random.Range(0, 25);
            gw.syncList[rand] = rand;
        }
    }

    [Command]
    public void CmdCheckSquares(string tag) {
        RpcCheckSquares(tag);
    }

    [ClientRpc]
    void RpcCheckSquares(string tag) {
        gw.CheckSquares(tag);
    }

    [Command]
    public void CmdEndTurn() {
        RpcEndTurn();
    }

    [ClientRpc]
    public void RpcEndTurn() {
        gw.EndTurn();
    }

    [Command]
    public void CmdDeleteInMove(string tag, float thisPos, float lastPos) {
        RpcDeleteInMove(tag, thisPos, lastPos);
    }

    [ClientRpc]
    public void RpcDeleteInMove(string tag, float thisPos, float lastPos) {
        gw.buttonList[int.Parse(tag)].GetComponent<Square>().DeleteInMove(thisPos, lastPos);
    }

    [Command]
    public void CmdCancelMove() {
        RpcCancelMove();
    }

    [ClientRpc]
    public void RpcCancelMove() {
        gw.CancelMove();
    }

    [Command]
    public void CmdUnSelect() {
        RpcUnSelect();
    }

    [ClientRpc]
    public void RpcUnSelect() {
        gw.UnSelect();
    }

    [Command]
    void CmdMarkAsDisconnected()
    {
        disconnected = 1;
        RpcMarkAsDisconnected();
    }

    [ClientRpc]
    void RpcMarkAsDisconnected()
    {
        numOfPlayers = 0;
        disconnected = 1;
    }

    public override void OnStartLocalPlayer()
    {
        gameObject.name="localPlayer";
    }

    public void TargetOpponent()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 2)
        {
            opponentPlayer = GameObject.Find("Player(Clone)");
            opponentPlayer.GetComponent<Player>().opponentPlayer = GameObject.Find("localPlayer");
        }
    }
}
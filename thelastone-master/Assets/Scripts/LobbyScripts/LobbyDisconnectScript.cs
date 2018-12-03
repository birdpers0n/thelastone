using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyDisconnectScript : NetworkBehaviour {

    NetworkManager networkManager;

    public void LobbyDisconnect()
    {
        networkManager = NetworkManager.singleton;
        networkManager.matchMaker.DropConnection(networkManager.matchInfo.networkId, networkManager.matchInfo.nodeId,
            0, networkManager.OnDropConnection);
        networkManager.StopHost();
        GameObject.Find("HostControl").GetComponent<HostGame>().ReactivateMenu();
    }
}

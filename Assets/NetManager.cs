using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager {

    public override void OnClientDisconnect(NetworkConnection nc) {
        StopClient();
    }

    public override void OnServerDisconnect(NetworkConnection nc) {

        NetworkServer.DestroyPlayersForConnection(nc);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OurNetworkManager : NetworkManager {

    public override void OnServerConnect(NetworkConnection nc) {
        base.OnServerConnect(nc);

        int cid = nc.connectionId;
        int hid = nc.hostId;
    }

    public override void OnClientSceneChanged(NetworkConnection conn) {
        // base.OnClientSceneChanged(conn);
    }

    public override void OnClientConnect(NetworkConnection nc) {
        base.OnClientConnect(nc);
        // ** critical ** you must stop your client discovery RIGHT HERE,
        // directly after the call to the base.  you actually can NOT do it
        // in your discovery client
        int cid = nc.connectionId;
        int hid = nc.hostId;

    }

    public override void OnClientDisconnect(NetworkConnection nc) {
        // 1. You certainly want to do this:
        StopClient();

        // 2. Almost certainly, you will seek a new server ...
        // using your own code, you will basically again launch your DiscoveryClient
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        NetworkServer.DestroyPlayersForConnection(conn);
        StartCoroutine(StopNextFrame());
    }

    private IEnumerator StopNextFrame() {
        yield return null;
        StopHost();
    }

}
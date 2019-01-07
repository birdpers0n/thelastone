using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartController : MonoBehaviour {

    public void RequestPlayerRestart()
    {
        GameObject.Find("localPlayer").GetComponent<Player>().DoRematch();
    }

}

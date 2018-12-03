using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchProcedures : MonoBehaviour {

	void Start () {
        Text tt = GameObject.Find("Turn").GetComponent<Text>();

        if (GameObject.Find("localPlayer").GetComponent<Player>().CheckIfServer())
        {
            tt.text = "YOUR TURN";
        }
        else
        {
            tt.text = "OPPONENT'S TURN";
        }
	}
	
}

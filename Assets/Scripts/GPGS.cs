//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;

//public class GPGS : MonoBehaviour {

//    // Use this for initialization
//    void Start() {

//        if (PlayGamesPlatform.Instance.localUser.authenticated) return;

//        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
//        PlayGamesPlatform.InitializeInstance(config);
//        PlayGamesPlatform.Activate();

//        Social.localUser.Authenticate((bool success) => {});
//}

//    public void ShowLeaderBoard() {

//        if (PlayGamesPlatform.Instance.localUser.authenticated) {
//            PlayGamesPlatform.Instance.ShowLeaderboardUI();
//        } else {
//            Social.localUser.Authenticate((bool success) => { });
//        }
//    }

//	// Update is called once per frame
//	void Update () {

//	}


//}

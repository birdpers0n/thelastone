using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour {

    private static AdManager instance { get; set; }

    private InterstitialAd inter;
    private BannerView bannerView;

    private string appId = "ca-app-pub-9762569011858340~8738566469";

    private string bannerId = "ca-app-pub-3940256099942544/6300978111";
    private string interId = "ca-app-pub-3940256099942544/1033173712";

    // Use this for initialization
    void Start() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
        }
        MobileAds.Initialize(appId);
        RequestBanner();
        RequestInter();
    }

    public void RequestInter() {
        inter = new InterstitialAd(interId);
        AdRequest request = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
        inter.LoadAd(request);
    }

    public void ShowInter() {
        if (inter.IsLoaded()) inter.Show();
    }

    private void RequestBanner() {
        bannerView = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
        bannerView.LoadAd(request);
    }


}

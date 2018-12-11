using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManagerScript : MonoBehaviour {

    public BannerView bannerView;
    private string AdMobID = "ca-app-pub-9762569011858340/1672332680";
    private string appId = "ca-app-pub-9762569011858340~8738566469";

    void Start () {
        MobileAds.Initialize(appId);
        RequestBanner();
    }

    private void RequestBanner() {
        bannerView = new BannerView(AdMobID, AdSize.SmartBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }





}

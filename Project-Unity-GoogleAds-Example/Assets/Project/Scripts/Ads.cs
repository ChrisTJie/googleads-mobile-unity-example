using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour
{
    // Ad units.
#if UNITY_ANDROID
    private const string _BannerID = "ca-app-pub-3940256099942544/6300978111";
    private const string _InterstitialID = "ca-app-pub-3940256099942544/1033173712";
    private const string _RewardedID = "ca-app-pub-3940256099942544/5224354917";
    private const string _RewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
    private const string _NativeID = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IOS
    private const string _BannerID = "ca-app-pub-3940256099942544/2934735716";
    private const string _InterstitialID = "ca-app-pub-3940256099942544/4411468910";
    private const string _RewardedID = "ca-app-pub-3940256099942544/1712485313";
    private const string _RewardedInterstitialID = "ca-app-pub-3940256099942544/6978759866";
    private const string _NativeID = "ca-app-pub-3940256099942544/3986624511";
#else
    private const string _BannerID = "unexpected_platform";
    private const string _InterstitialID = "unexpected_platform";
    private const string _RewardedID = "unexpected_platform";
    private const string _RewardedInterstitialID = "unexpected_platform";
    private const string _NativeID = "unexpected_platform";
#endif

    // Test device ID.
    private List<string> _DeviceIDs = new List<string>();

    private BannerView _BannerView;

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(_init_status => { });

        RequestBanner();
    }

    private void SetTestDeviceIds()
    {
        // Add your device IDs.
        _DeviceIDs.Add("Your device ID.");

        RequestConfiguration _request_configuration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(_DeviceIDs)
            .build();

        // Set requestConfiguration globally to MobileAds.
        MobileAds.SetRequestConfiguration(_request_configuration);
    }

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        _BannerView = new BannerView(_BannerID, AdSize.Banner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        _BannerView.OnAdLoaded += HandleOnAdLoaded;

        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        _BannerView.LoadAd(_ad_request);
    }
    private void HandleOnAdLoaded(object _sender, EventArgs _args)
    {
        Debug.Log("HandleAdLoaded event received.");
    }
}

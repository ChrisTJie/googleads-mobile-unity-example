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
    private InterstitialAd _InterstitialAd;
    private RewardedAd _RewardedAd;
    private RewardedInterstitialAd _RewardedInterstitialAd;

    private void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(_init_status => { });

        RequestBanner();
        RequestInterstitial();
        RequestRewarded();
        RequestRewardedInterstitial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {

        }
        if (Input.GetKeyDown(KeyCode.S))
        {

        }
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
        _BannerView = new BannerView(_BannerID, AdSize.Banner, AdPosition.Bottom);

        // Called when an ad request has successfully loaded.
        _BannerView.OnAdLoaded += BannerOnAdLoaded;
        // Called when an ad request failed to load.
        _BannerView.OnAdFailedToLoad += BannerOnAdFailedToLoad;
        // Called when an ad is clicked.
        _BannerView.OnAdOpening += BannerOnAdOpening;
        // Called when the user returned from the app after an ad click.
        _BannerView.OnAdClosed += BannerOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        _BannerView.OnAdLeavingApplication += BannerOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        _BannerView.LoadAd(_ad_request);
    }
    private void BannerOnAdLoaded(object _sender, EventArgs _args)
    {
        Debug.Log("BannerOnAdLoaded event received.");
    }
    private void BannerOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
    {
        Debug.LogWarningFormat("BannerOnAdFailedToLoad event received with message: {0}.", _args.Message);
    }
    private void BannerOnAdOpening(object _sender, EventArgs _args)
    {
        Debug.Log("BannerOnAdOpening event received.");
    }
    private void BannerOnAdClosed(object _sender, EventArgs _args)
    {
        Debug.Log("BannerOnAdClosed event received.");
    }
    private void BannerOnAdLeavingApplication(object _sender, EventArgs _args)
    {
        Debug.Log("BannerOnAdLeavingApplication event received.");
    }
    private void BannerViewDestroy() => _BannerView.Destroy();

    private void RequestInterstitial()
    {
        // Initialize an InterstitialAd.
        _InterstitialAd = new InterstitialAd(_InterstitialID);

        // Called when an ad request has successfully loaded.
        _InterstitialAd.OnAdLoaded += InterstitialOnAdLoaded;
        // Called when an ad request failed to load.
        _InterstitialAd.OnAdFailedToLoad += InterstitialOnAdFailedToLoad;
        // Called when an ad is shown.
        _InterstitialAd.OnAdOpening += InterstitialOnAdOpening;
        // Called when the ad is closed.
        _InterstitialAd.OnAdClosed += InterstitialOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        _InterstitialAd.OnAdLeavingApplication += InterstitialOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        _InterstitialAd.LoadAd(_ad_request);
    }
    public void ShowInterstitialAd()
    {
        if (_InterstitialAd.IsLoaded())
        {
            _InterstitialAd.Show();
            Debug.Log("Displays the InterstitialAd.");
        }
        else
        {
            Debug.LogWarning("InterstitialAd has not loaded yet.");
        }
    }
    private void InterstitialOnAdLoaded(object _sender, EventArgs _args)
    {
        Debug.Log("InterstitialOnAdLoaded event received.");
    }
    private void InterstitialOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
    {
        Debug.LogWarningFormat("InterstitialOnAdFailedToLoad event received with message: {0}.", _args.Message);
    }
    private void InterstitialOnAdOpening(object _sender, EventArgs _args)
    {
        Debug.Log("InterstitialOnAdOpening event received.");
    }
    private void InterstitialOnAdClosed(object _sender, EventArgs _args)
    {
        Debug.Log("InterstitialOnAdClosed event received.");
        RequestInterstitial();
    }
    private void InterstitialOnAdLeavingApplication(object _sender, EventArgs _args)
    {
        Debug.Log("InterstitialOnAdLeavingApplication event received.");
    }
    private void InterstitialAdDestroy() => _InterstitialAd.Destroy();

    private void RequestRewarded()
    {
        _RewardedAd = new RewardedAd(_RewardedID);

        // Called when an ad request has successfully loaded.
        _RewardedAd.OnAdLoaded += RewardedOnAdLoaded;
        // Called when an ad request failed to load.
        _RewardedAd.OnAdFailedToLoad += RewardedOnAdFailedToLoad;
        // Called when an ad is shown.
        _RewardedAd.OnAdOpening += RewardedOnAdOpening;
        // Called when an ad request failed to show.
        _RewardedAd.OnAdFailedToShow += RewardedOnAdFailedToShow;
        // Called when the ad is closed.
        _RewardedAd.OnAdClosed += RewardedOnAdClosed;
        // Called when the user should be rewarded for interacting with the ad.
        _RewardedAd.OnUserEarnedReward += RewardedOnUserEarnedReward;

        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

        // Load the rewarded ad with the request.
        _RewardedAd.LoadAd(_ad_request);
    }
    public void ShowRewardedAd()
    {
        if (_RewardedAd.IsLoaded())
        {
            _RewardedAd.Show();
            Debug.Log("Displays the RewardedAd.");
        }
        else
        {
            Debug.LogWarning("RewardedAd has not loaded yet.");
        }
    }
    private void RewardedOnAdLoaded(object _sender, EventArgs _args)
    {
        Debug.Log("RewardedOnAdLoaded event received.");
    }
    private void RewardedOnAdFailedToLoad(object _sender, AdErrorEventArgs _args)
    {
        Debug.LogWarningFormat("RewardedOnAdFailedToLoad event received with message: {0}.", _args.Message);
    }
    private void RewardedOnAdOpening(object _sender, EventArgs _args)
    {
        Debug.Log("RewardedOnAdOpening event received.");
    }
    private void RewardedOnAdFailedToShow(object _sender, AdErrorEventArgs _args)
    {
        Debug.LogWarningFormat("RewardedOnAdFailedToShow event received with message: {0}.", _args.Message);
    }
    private void RewardedOnAdClosed(object _sender, EventArgs _args)
    {
        Debug.Log("RewardedOnAdClosed event received.");
        RequestRewarded();
    }
    private void RewardedOnUserEarnedReward(object _sender, Reward _args)
    {
        string _type = _args.Type;
        double _amount = _args.Amount;
        Debug.LogFormat("RewardedOnUserEarnedReward event received for {0} {1}.", _amount.ToString(), _type);
    }

    private void RequestRewardedInterstitial()
    {
        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

        RewardedInterstitialAd.LoadAd(_RewardedInterstitialID, _ad_request, AdLoadCallBack);
    }
    public void ShowRewardedInterstitialAd()
    {
        if (_RewardedInterstitialAd != null)
        {
            _RewardedInterstitialAd.Show(UserEarnedRewardCallBack);
        }
        else
        {
            Debug.LogWarningFormat("{0} is Null.", nameof(_RewardedInterstitialAd));
        }
    }
    private void AdLoadCallBack(RewardedInterstitialAd _rewarded_interstitial_ad, string _error)
    {
        if (_error == null)
        {
            _RewardedInterstitialAd = _rewarded_interstitial_ad;

            _RewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += RewardedInterstitialOnAdFailedToPresentFullScreenContent;
            _RewardedInterstitialAd.OnAdDidPresentFullScreenContent += RewardedInterstitialOnAdDidPresentFullScreenContent;
            _RewardedInterstitialAd.OnAdDidDismissFullScreenContent += RewardedInterstitialOnAdDidDismissFullScreenContent;
            _RewardedInterstitialAd.OnPaidEvent += RewardedInterstitialOnPaidEvent;
        }
    }
    private void UserEarnedRewardCallBack(Reward _reward)
    {
        Debug.Log("TODO: Reward the user.");
    }
    private void RewardedInterstitialOnAdFailedToPresentFullScreenContent(object _sender, AdErrorEventArgs _args)
    {
        Debug.LogWarning("RewardedInterstitialOnAdFailedToPresentFullScreenContent has failed to present.");
    }
    private void RewardedInterstitialOnAdDidPresentFullScreenContent(object _sender, EventArgs _args)
    {
        Debug.Log("RewardedInterstitialOnAdDidPresentFullScreenContent has presented.");
    }
    private void RewardedInterstitialOnAdDidDismissFullScreenContent(object _sender, EventArgs _args)
    {
        Debug.Log("RewardedInterstitialOnAdDidDismissFullScreenContent has dismissed presentation.");
        RequestRewardedInterstitial();
    }
    private void RewardedInterstitialOnPaidEvent(object _sender, AdValueEventArgs _args)
    {
        Debug.Log("RewardedInterstitialOnPaidEvent has received a paid event.");
    }
}

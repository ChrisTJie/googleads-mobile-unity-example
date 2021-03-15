using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
    public enum Mode
    {
        Enable,
        Disable
    }
    public Mode _Mode;

    // Ad units.
    public string _Android_BannerID = "ca-app-pub-6569373569137925/2194855636";
    public string _Android_InterstitialID = "ca-app-pub-6569373569137925/9139537939";
    public string _Android_RewardedID = "ca-app-pub-6569373569137925/8947966245";
    public string _Android_RewardedInterstitialID = "ca-app-pub-6569373569137925/5200292928";
    public string _Android_NativeID = "ca-app-pub-6569373569137925/3151702326";
    public string _IOS_BannerID = "ca-app-pub-6569373569137925/2190986208";
    public string _IOS_InterstitialID = "ca-app-pub-6569373569137925/9877904538";
    public string _IOS_RewardedID = "ca-app-pub-6569373569137925/1999414514";
    public string _IOS_RewardedInterstitialID = "ca-app-pub-6569373569137925/5938659526";
    public string _IOS_NativeID = "ca-app-pub-6569373569137925/7060169505";
    private const string _BannerID = "unexpected_platform";
    private const string _InterstitialID = "unexpected_platform";
    private const string _RewardedID = "unexpected_platform";
    private const string _RewardedInterstitialID = "unexpected_platform";
    private const string _NativeID = "unexpected_platform";

    public bool _AutoInitialize;
    public bool _TestMode;
    public bool _AutoAdRequest;
    public float _AdRequestTime;
    // Test device ID.
    private List<string> _DeviceIDs = new List<string>();
    public bool _EnableBanner;
    public bool _EnableInterstitial;
    public bool _EnableRewarded;
    public bool _EnableRewardedInterstitial;
    public bool _EnableNative;

    private BannerView _BannerView;
    public enum BannerAdSize
    {
        Banner,
        IABBanner,
        Leaderboard,
        MediumRectangle,
        SmartBanner
    }
    public BannerAdSize _BannerAdSize;
    private AdSize _AdSize
    {
        get
        {
            switch (_BannerAdSize)
            {
                case BannerAdSize.Banner:
                    return AdSize.Banner;
                case BannerAdSize.IABBanner:
                    return AdSize.IABBanner;
                case BannerAdSize.Leaderboard:
                    return AdSize.Leaderboard;
                case BannerAdSize.MediumRectangle:
                    return AdSize.MediumRectangle;
                case BannerAdSize.SmartBanner:
                    return AdSize.SmartBanner;
                default:
                    return AdSize.Banner;
            }
        }
    }
    public AdPosition _AdPosition;
    private InterstitialAd _InterstitialAd;
    private RewardedAd _RewardedAd;
    private RewardedInterstitialAd _RewardedInterstitialAd;
    private UnifiedNativeAd _UnifiedNativeAd;

    private bool _BannerActivated;
    private bool _InterstitialActivated;
    private bool _RewardedActivated;
    private bool _RewardedInterstitialActivated;
    private bool _NativeActivated;

    public RawImage _Native_Icon;
    public RawImage _Native_ChoicesIcon;
    public Text _Native_Headline;
    public Text _Native_Body;
    public Text _Native_CallToAction;
    public Text _Native_Advertiser;


    private void Start()
    {
        if (!_AutoInitialize) return;

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(_init_status => { });
        SetTestDeviceIds();
        StartCoroutine(AutoAdRequest(_AdRequestTime));
        if (!_AutoAdRequest) Invoke("Request", 5.0f);
    }

    private void Update()
    {
        ShowNativeAd();
    }

    private void SetTestDeviceIds()
    {
        if (!_TestMode) return;

        // Add test device ID.
        _DeviceIDs.Add(AdRequest.TestDeviceSimulator);
#if UNITY_ANDROID
        _DeviceIDs.Add(SystemInfo.deviceUniqueIdentifier.ToUpper().Trim());
#elif UNITY_IOS
        string _ios_device_id;
        _ios_device_id = UnityEngine.iOS.Device.advertisingIdentifier;
        _ios_device_id = CreateMD5(_ios_device_id);
        _ios_device_id = _ios_device_id.ToLower();
        _DeviceIDs.Add(_ios_device_id);
#endif

        foreach (string _device_ids in _DeviceIDs)
        {
            Debug.LogFormat("Added device ID is: {0}.", _device_ids);
        }

        RequestConfiguration _request_configuration = new RequestConfiguration.Builder().SetTestDeviceIds(_DeviceIDs).build();

        // Set requestConfiguration globally to MobileAds.
        MobileAds.SetRequestConfiguration(_request_configuration);
    }
    private static string CreateMD5(string _input)
    {
        if (string.IsNullOrEmpty(_input)) return string.Empty;

        using (System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] _input_bytes = Encoding.ASCII.GetBytes(_input);
            byte[] _hash_bytes = _md5.ComputeHash(_input_bytes);

            StringBuilder _s_b = new StringBuilder();
            for (int i = 0; i < _hash_bytes.Length; i++)
            {
                _s_b.Append(_hash_bytes[i].ToString("X2"));
            }
            return _s_b.ToString();
        }
    }

    private IEnumerator AutoAdRequest(float _delay)
    {
        if (!_AutoAdRequest) yield break;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(_delay);

            Debug.Log("Ad active checking.");
            Request();
        }
    }

#if UNITY_ANDROID
    // Detect Google Play pre-launch report.
    private static bool IsTestLab()
    {
        try
        {
            using (var _act_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var _context = _act_class.GetStatic<AndroidJavaObject>("currentActivity");
                var _system_global = new AndroidJavaClass("android.provider.Settings$System");
                var _test_lab = _system_global.CallStatic<string>("getString", _context.Call<AndroidJavaObject>("getContentResolver"), "firebase.test.lab");
                Debug.LogWarningFormat("{0}: {1}.", nameof(IsTestLab), _test_lab);
                return _test_lab == "true";
            }
        }
        catch (Exception _exception)
        {
            Debug.LogWarning(_exception);
            return false;
        }
    }
#endif

    private void Request()
    {
        if (IsTestLab()) { Debug.LogWarningFormat("{0}: {1}.", nameof(IsTestLab), IsTestLab()); return; };

        RequestBanner();
        RequestInterstitial();
        RequestRewarded();
        RequestRewardedInterstitial();
        RequestNative();
    }

    private void RequestBanner()
    {
        if (!_EnableBanner) return;

        if (_BannerView != null) { if (_BannerActivated) return; }

        // Create a 320x50 banner at the top of the screen.
#if UNITY_ANDROID
        _BannerView = new BannerView(_Android_BannerID, _AdSize, _AdPosition);
#elif UNITY_IOS
        _BannerView = new BannerView(_IOS_BannerID, _AdSize, _AdPosition);
#else
        _BannerView = new BannerView(_BannerID, _AdSize, _AdPosition);
#endif

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

        _BannerActivated = true;
    }
    private void BannerOnAdLoaded(object _sender, EventArgs _args)
    {
        Debug.Log("BannerOnAdLoaded event received.");
        _BannerActivated = true;
    }
    private void BannerOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
    {
        Debug.LogWarningFormat("BannerOnAdFailedToLoad event received with message: {0}.", _args.Message);
        _BannerActivated = false;
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
    private void BannerViewHide() => _BannerView.Hide();
    private void BannerViewDestroy()
    {
        _BannerView.Destroy();
        Debug.LogWarning("Banner Destroyed.");
        _BannerActivated = false;
    }

    private void RequestInterstitial()
    {
        if (!_EnableInterstitial) return;

        // Initialize an InterstitialAd.
#if UNITY_ANDROID
        _InterstitialAd = new InterstitialAd(_Android_InterstitialID);
#elif UNITY_IOS
        _InterstitialAd = new InterstitialAd(_IOS_InterstitialID);
#else
        _InterstitialAd = new InterstitialAd(_InterstitialID);
#endif

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
        if (!_EnableInterstitial) return;

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
    private void InterstitialAdDestroy()
    {
        _InterstitialAd.Destroy();
        Debug.LogWarning("Interstitial Destroyed.");
    }

    private void RequestRewarded()
    {
        if (!_EnableRewarded) return;

#if UNITY_ANDROID
        _RewardedAd = new RewardedAd(_Android_RewardedID);
#elif UNITY_IOS
        _RewardedAd = new RewardedAd(_IOS_RewardedID);
#else
        _RewardedAd = new RewardedAd(_RewardedID);
#endif

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
        if (!_EnableRewarded) return;

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
        if (!_EnableRewardedInterstitial) return;

        // Create an empty ad request.
        AdRequest _ad_request = new AdRequest.Builder().Build();

#if UNITY_ANDROID
        RewardedInterstitialAd.LoadAd(_Android_RewardedInterstitialID, _ad_request, AdLoadCallBack);
#elif UNITY_IOS
        RewardedInterstitialAd.LoadAd(_IOS_RewardedInterstitialID, _ad_request, AdLoadCallBack);
#else
        RewardedInterstitialAd.LoadAd(_RewardedInterstitialID, _ad_request, AdLoadCallBack);
#endif
    }
    public void ShowRewardedInterstitialAd()
    {
        if (!_EnableRewardedInterstitial) return;

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

    private void RequestNative()
    {
        if (!_EnableNative) return;

#if UNITY_ANDROID
        AdLoader _ad_loader = new AdLoader.Builder(_Android_NativeID).ForUnifiedNativeAd().Build();
#elif UNITY_IOS
        AdLoader _ad_loader = new AdLoader.Builder(_IOS_NativeID).ForUnifiedNativeAd().Build();
#else
        AdLoader _ad_loader = new AdLoader.Builder(_NativeID).ForUnifiedNativeAd().Build();
#endif

        _ad_loader.OnUnifiedNativeAdLoaded += NativeOnUnifiedNativeAdLoaded;
        _ad_loader.OnAdFailedToLoad += NativeOnAdFailedToLoad;

        _ad_loader.LoadAd(new AdRequest.Builder().Build());
    }
    private bool _UnifiedNativeAdLoaded = false;
    private void ShowNativeAd()
    {
        if (!_EnableNative) return;

        if (_UnifiedNativeAdLoaded)
        {
            // Get Texture2D for icon asset of native ad.
            Texture2D _icon = _UnifiedNativeAd.GetIconTexture();
            Texture2D _choices_icon = _UnifiedNativeAd.GetAdChoicesLogoTexture();
            // Get string for headline asset of native ad.
            string _headline = _UnifiedNativeAd.GetHeadlineText();
            string _body = _UnifiedNativeAd.GetBodyText();
            string _call_to_action = _UnifiedNativeAd.GetCallToActionText();
            string _advertiser = _UnifiedNativeAd.GetAdvertiserText();

            _Native_Icon.texture = _icon;
            _Native_ChoicesIcon.texture = _choices_icon;
            _Native_Headline.text = _headline;
            _Native_Body.text = _body;
            _Native_CallToAction.text = _call_to_action;
            _Native_Advertiser.text = _advertiser;

            // Register gameobjects.
            _UnifiedNativeAd.RegisterIconImageGameObject(_Native_Icon.gameObject);
            _UnifiedNativeAd.RegisterAdChoicesLogoGameObject(_Native_ChoicesIcon.gameObject);
            _UnifiedNativeAd.RegisterHeadlineTextGameObject(_Native_Headline.gameObject);
            _UnifiedNativeAd.RegisterBodyTextGameObject(_Native_Body.gameObject);
            _UnifiedNativeAd.RegisterCallToActionGameObject(_Native_CallToAction.gameObject);
            _UnifiedNativeAd.RegisterAdvertiserTextGameObject(_Native_Advertiser.gameObject);

            _UnifiedNativeAdLoaded = false;
        }
    }
    private void NativeOnUnifiedNativeAdLoaded(object _sender, UnifiedNativeAdEventArgs _args)
    {
        Debug.Log("NativeOnUnifiedNativeAdLoaded loaded.");
        _UnifiedNativeAd = _args.nativeAd;
        _UnifiedNativeAdLoaded = true;
    }
    private void NativeOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
    {
        Debug.LogWarningFormat("NativeOnAdFailedToLoad failed to load: {0}.", _args.Message);
    }
}

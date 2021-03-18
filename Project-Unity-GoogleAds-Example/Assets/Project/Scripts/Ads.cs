using GoogleMobileAds.Api;
using GoogleMobileAdsMediationTestSuite.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CTJ
{
    public class Ads : MonoBehaviour
    {
        public enum Mode
        {
            Enable,
            Disable
        }
        public Mode _Mode;

        #region Ad Units
        public string _Android_BannerID;
        public string _Android_InterstitialID;
        public string _Android_RewardedID;
        public string _Android_RewardedInterstitialID;
        public string _Android_NativeID;
        public string _IOS_BannerID;
        public string _IOS_InterstitialID;
        public string _IOS_RewardedID;
        public string _IOS_RewardedInterstitialID;
        public string _IOS_NativeID;
        private const string _BannerID = "unexpected_platform";
        private const string _InterstitialID = "unexpected_platform";
        private const string _RewardedID = "unexpected_platform";
        private const string _RewardedInterstitialID = "unexpected_platform";
        private const string _NativeID = "unexpected_platform";
        // Test Ad units.
        private const string _Test_Android_BannerID = "ca-app-pub-3940256099942544/6300978111";
        private const string _Test_Android_InterstitialID = "ca-app-pub-3940256099942544/1033173712";
        private const string _Test_Android_RewardedID = "ca-app-pub-3940256099942544/5224354917";
        private const string _Test_Android_RewardedInterstitialID = "ca-app-pub-3940256099942544/5354046379";
        private const string _Test_Android_NativeID = "ca-app-pub-3940256099942544/2247696110";
        private const string _Test_IOS_BannerID = "ca-app-pub-3940256099942544/2934735716";
        private const string _Test_IOS_InterstitialID = "ca-app-pub-3940256099942544/4411468910";
        private const string _Test_IOS_RewardedID = "ca-app-pub-3940256099942544/1712485313";
        private const string _Test_IOS_RewardedInterstitialID = "ca-app-pub-3940256099942544/6978759866";
        private const string _Test_IOS_NativeID = "ca-app-pub-3940256099942544/3986624511";
        #endregion

        public bool _AutoInitialize;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            if (!_AutoInitialize) return;

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(_init_status =>
            {
                Dictionary<string, AdapterStatus> _map = _init_status.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> _key_value_pair in _map)
                {
                    string _class_name = _key_value_pair.Key;
                    AdapterStatus _status = _key_value_pair.Value;
                    switch (_status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Logger.LogWarningFormat("Adapter: {0} not ready.", _class_name);
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Logger.LogFormat("Adapter: {0} is initialized.", _class_name);
                            break;
                    }
                }
            });
            SetTestDeviceIds();
            TestLab();
            if (!_AutoAdRequest) Invoke("Request", 5.0f);
            else StartCoroutine(AutoAdRequest(_AdRequestTime));
            if (_MediationTestSuiteMode) MediationTestSuite.OnMediationTestSuiteDismissed += HandleMediationTestSuiteDismissed;
        }

        private void Update()
        {
            ShowNativeAd();
        }

        #region Test Mode
        public bool _TestDeviceMode;
        public bool _MediationTestSuiteMode;
        // Test device ID.
        private List<string> _DeviceIDs = new List<string>();
        private void SetTestDeviceIds()
        {
            if (!_TestDeviceMode) return;

            // Add test device ID.
            _DeviceIDs.Add(AdRequest.TestDeviceSimulator);
#if UNITY_ANDROID
            string _andriod_device_id;
            _andriod_device_id = SystemInfo.deviceUniqueIdentifier.ToUpper().Trim();
            _DeviceIDs.Add(_andriod_device_id);
#elif UNITY_IOS
            string _ios_device_id;
            _ios_device_id = UnityEngine.iOS.Device.advertisingIdentifier;
            _ios_device_id = CreateMD5(_ios_device_id);
            _ios_device_id = _ios_device_id.ToLower();
            _DeviceIDs.Add(_ios_device_id);
#endif

            foreach (string _device_ids in _DeviceIDs)
            {
                Logger.LogFormat("Added test device ID is: {0}.", _device_ids);
            }

            RequestConfiguration _request_configuration = new RequestConfiguration.Builder().SetTestDeviceIds(_DeviceIDs).build();

            // Set requestConfiguration globally to MobileAds.
            MobileAds.SetRequestConfiguration(_request_configuration);

            // Mediation Test Suite test device.
            if (!_MediationTestSuiteMode) return;
#if UNITY_ANDROID
            MediationTestSuite.AdRequest = new AdRequest.Builder().AddTestDevice(_andriod_device_id).Build();
#elif UNITY_IOS
            MediationTestSuite.AdRequest = new AdRequest.Builder().AddTestDevice(_ios_device_id).Build();
#endif
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
        public bool _EnableTestBanner;
        public bool _EnableTestInterstitial;
        public bool _EnableTestRewarded;
        public bool _EnableTestRewardedInterstitial;
        public bool _EnableTestNative;
#if UNITY_ANDROID
        private static bool IsTestLab = false;
        // Detect Google Play pre-launch report.
        private static void TestLab()
        {
            try
            {
                using (var _act_class = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var _context = _act_class.GetStatic<AndroidJavaObject>("currentActivity");
                    var _system_global = new AndroidJavaClass("android.provider.Settings$System");
                    var _test_lab = _system_global.CallStatic<string>("getString", _context.Call<AndroidJavaObject>("getContentResolver"), "firebase.test.lab");
                    IsTestLab = _test_lab == "true";
                }
            }
            catch (Exception _exception)
            {
                Logger.LogWarningFormat("{0}: {1}.", nameof(TestLab), _exception);
                IsTestLab = false;
            }
            finally
            {
                Logger.LogWarningFormat("{0}: {1}.", nameof(TestLab), IsTestLab);
            }
        }
#endif
        #endregion

        #region Ad Request
        public bool _AutoAdRequest;
        public float _AdRequestTime;
        private IEnumerator AutoAdRequest(float _delay)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(_delay);

                Logger.Log("Ad active checking.");
                Request();
            }
        }
        private void Request()
        {
#if UNITY_ANDROID
            if (IsTestLab) { Logger.LogWarningFormat("{0}: {1}.", nameof(TestLab), IsTestLab); return; };
#endif

            RequestBanner();
            RequestInterstitial();
            RequestRewarded();
            RequestRewardedInterstitial();
            RequestNative();
        }
        #endregion

        #region Banner Ads
        public bool _EnableBanner;
        private bool _BannerActivated;
        private BannerView _BannerView;
        public enum BannerAdSize
        {
            Banner,
            IABBanner,
            Leaderboard,
            MediumRectangle,
            SmartBanner,
            AdaptiveBanner,
            Custom
        }
        public BannerAdSize _BannerAdSize;
        public Vector2Int _WH;
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
                    case BannerAdSize.AdaptiveBanner:
                        return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                    case BannerAdSize.Custom:
                        return new AdSize(_WH.x, _WH.y);
                    default:
                        return AdSize.Banner;
                }
            }
        }
        public enum BannerAdPosition
        {
            Bottom,
            BottomLeft,
            BottomRight,
            Center,
            Custom,
            Top,
            TopLeft,
            TopRight
        }
        public BannerAdPosition _BannerAdPosition;
        public Vector2Int _Pos;
        private AdPosition _AdPosition
        {
            get
            {
                switch (_BannerAdPosition)
                {
                    case BannerAdPosition.Bottom:
                        return AdPosition.Bottom;
                    case BannerAdPosition.BottomLeft:
                        return AdPosition.BottomLeft;
                    case BannerAdPosition.BottomRight:
                        return AdPosition.BottomRight;
                    case BannerAdPosition.Center:
                        return AdPosition.Center;
                    case BannerAdPosition.Custom:
                        return AdPosition.Custom;
                    case BannerAdPosition.Top:
                        return AdPosition.Top;
                    case BannerAdPosition.TopLeft:
                        return AdPosition.TopLeft;
                    case BannerAdPosition.TopRight:
                        return AdPosition.TopRight;
                    default:
                        return AdPosition.Bottom;
                }
            }
        }
        private void RequestBanner()
        {
            if (!_EnableBanner) return;

            if (_BannerView != null) { if (_BannerActivated) return; }

            // Create a X banner at the X of the screen.
#if UNITY_ANDROID
            switch (_EnableTestBanner)
            {
                case false:
                    if (_BannerAdPosition == BannerAdPosition.Custom) _BannerView = new BannerView(_Android_BannerID, _AdSize, _Pos.x, _Pos.y);
                    else _BannerView = new BannerView(_Android_BannerID, _AdSize, _AdPosition);
                    break;
                case true:
                    if (_BannerAdPosition == BannerAdPosition.Custom) _BannerView = new BannerView(_Test_Android_BannerID, _AdSize, _Pos.x, _Pos.y);
                    else _BannerView = new BannerView(_Test_Android_BannerID, _AdSize, _AdPosition);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestBanner)
            {
                case false:
                    if (_BannerAdPosition == BannerAdPosition.Custom) _BannerView = new BannerView(_IOS_BannerID, _AdSize, _Pos.x, _Pos.y);
                    else _BannerView = new BannerView(_IOS_BannerID, _AdSize, _AdPosition);
                    break;
                case true:
                    if (_BannerAdPosition == BannerAdPosition.Custom) _BannerView = new BannerView(_Test_IOS_BannerID, _AdSize, _Pos.x, _Pos.y);
                    else _BannerView = new BannerView(_Test_IOS_BannerID, _AdSize, _AdPosition);
                    break;
            }
#else
            if (_BannerAdPosition == BannerAdPosition.Custom) _BannerView = new BannerView(_BannerID, _AdSize, _Pos.x, _Pos.y);
            else _BannerView = new BannerView(_BannerID, _AdSize, _AdPosition);
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
            Logger.Log("BannerOnAdLoaded event received.");
            Logger.LogFormat("Banner Ad Height: {0}. Width: {1}.", _BannerView.GetHeightInPixels(), _BannerView.GetWidthInPixels());
            _BannerActivated = true;
        }
        private void BannerOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("BannerOnAdFailedToLoad event received with message: {0}.", _args.Message);
            _BannerActivated = false;
        }
        private void BannerOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.Log("BannerOnAdOpening event received.");
        }
        private void BannerOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.Log("BannerOnAdClosed event received.");
        }
        private void BannerOnAdLeavingApplication(object _sender, EventArgs _args)
        {
            Logger.Log("BannerOnAdLeavingApplication event received.");
        }
        private void BannerViewHide() => _BannerView.Hide();
        private void BannerViewDestroy()
        {
            _BannerView.Destroy();
            Logger.LogWarning("Banner Destroyed.");
            _BannerActivated = false;
        }
        #endregion

        #region Interstitial Ads
        public bool _EnableInterstitial;
        private bool _InterstitialActivated;
        private InterstitialAd _InterstitialAd;
        private void RequestInterstitial()
        {
            if (!_EnableInterstitial) return;

            if (_InterstitialAd != null) { if (_InterstitialActivated) return; }

            // Initialize an InterstitialAd.
#if UNITY_ANDROID
            switch (_EnableTestInterstitial)
            {
                case false:
                    _InterstitialAd = new InterstitialAd(_Android_InterstitialID);
                    break;
                case true:
                    _InterstitialAd = new InterstitialAd(_Test_Android_InterstitialID);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestInterstitial)
            {
                case false:
                    _InterstitialAd = new InterstitialAd(_IOS_InterstitialID);
                    break;
                case true:
                    _InterstitialAd = new InterstitialAd(_Test_IOS_InterstitialID);
                    break;
            }
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

            _InterstitialActivated = true;
        }
        public void ShowInterstitialAd()
        {
            if (!_EnableInterstitial) return;

            if (_InterstitialAd == null) { Logger.LogWarningFormat("{0}: {1}.", nameof(_InterstitialAd), _InterstitialAd); return; }

            if (_InterstitialAd.IsLoaded())
            {
                _InterstitialAd.Show();
                Logger.Log("Displays the Interstitial Ad.");
                return;
            }
            else
            {
                Logger.LogWarning("InterstitialAd has not loaded yet.");
            }
        }
        private void InterstitialOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.Log("InterstitialOnAdLoaded event received.");
            _InterstitialActivated = true;
        }
        private void InterstitialOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("InterstitialOnAdFailedToLoad event received with message: {0}.", _args.Message);
            _InterstitialActivated = false;
        }
        private void InterstitialOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.Log("InterstitialOnAdOpening event received.");
        }
        private void InterstitialOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.Log("InterstitialOnAdClosed event received.");
            _InterstitialActivated = false;
            RequestInterstitial();
        }
        private void InterstitialOnAdLeavingApplication(object _sender, EventArgs _args)
        {
            Logger.Log("InterstitialOnAdLeavingApplication event received.");
        }
        private void InterstitialAdDestroy()
        {
            _InterstitialAd.Destroy();
            Logger.LogWarning("Interstitial Destroyed.");
            _InterstitialActivated = false;
        }
        #endregion

        #region Rewarded Ads
        public bool _EnableRewarded;
        private bool _RewardedActivated;
        private RewardedAd _RewardedAd;
        private void RequestRewarded()
        {
            if (!_EnableRewarded) return;

            if (_RewardedAd != null) { if (_RewardedActivated) return; }

#if UNITY_ANDROID
            switch (_EnableTestRewarded)
            {
                case false:
                    _RewardedAd = new RewardedAd(_Android_RewardedID);
                    break;
                case true:
                    _RewardedAd = new RewardedAd(_Test_Android_RewardedID);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestRewarded)
            {
                case false:
                    _RewardedAd = new RewardedAd(_IOS_RewardedID);
                    break;
                case true:
                    _RewardedAd = new RewardedAd(_Test_IOS_RewardedID);
                    break;
            }
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

            _RewardedActivated = true;
        }
        public void ShowRewardedAd()
        {
            if (!_EnableRewarded) return;

            if (_RewardedAd == null) { Logger.LogWarningFormat("{0}: {1}.", nameof(_RewardedAd), _RewardedAd); return; }

            if (_RewardedAd.IsLoaded())
            {
                _RewardedAd.Show();
                Logger.Log("Displays the Rewarded Ad.");
                return;
            }
            else
            {
                Logger.LogWarning("RewardedAd has not loaded yet.");
            }
        }
        private void RewardedOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.Log("RewardedOnAdLoaded event received.");
            _RewardedActivated = true;
        }
        private void RewardedOnAdFailedToLoad(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("RewardedOnAdFailedToLoad event received with message: {0}.", _args.Message);
            _RewardedActivated = false;
        }
        private void RewardedOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.Log("RewardedOnAdOpening event received.");
        }
        private void RewardedOnAdFailedToShow(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("RewardedOnAdFailedToShow event received with message: {0}.", _args.Message);
        }
        private void RewardedOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.Log("RewardedOnAdClosed event received.");
            _RewardedActivated = false;
            RequestRewarded();
        }
        private void RewardedOnUserEarnedReward(object _sender, Reward _args)
        {
            string _type = _args.Type;
            double _amount = _args.Amount;
            Logger.LogFormat("RewardedOnUserEarnedReward event received for {0} {1}.", _amount.ToString(), _type);
        }
        #endregion

        #region Rewarded Interstitial Ads
        public bool _EnableRewardedInterstitial;
        private bool _RewardedInterstitialActivated;
        private RewardedInterstitialAd _RewardedInterstitialAd;
        private void RequestRewardedInterstitial()
        {
            if (!_EnableRewardedInterstitial) return;

            if (_RewardedInterstitialAd != null) { if (_RewardedInterstitialActivated) return; }

            // Create an empty ad request.
            AdRequest _ad_request = new AdRequest.Builder().Build();

#if UNITY_ANDROID
            switch (_EnableTestRewardedInterstitial)
            {
                case false:
                    RewardedInterstitialAd.LoadAd(_Android_RewardedInterstitialID, _ad_request, AdLoadCallBack);
                    break;
                case true:
                    RewardedInterstitialAd.LoadAd(_Test_Android_RewardedInterstitialID, _ad_request, AdLoadCallBack);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestRewardedInterstitial)
            {
                case false:
                    RewardedInterstitialAd.LoadAd(_IOS_RewardedInterstitialID, _ad_request, AdLoadCallBack);
                    break;
                case true:
                    RewardedInterstitialAd.LoadAd(_Test_IOS_RewardedInterstitialID, _ad_request, AdLoadCallBack);
                    break;
            }
#else
            RewardedInterstitialAd.LoadAd(_RewardedInterstitialID, _ad_request, AdLoadCallBack);
#endif

            _RewardedInterstitialActivated = true;
        }
        public void ShowRewardedInterstitialAd()
        {
            if (!_EnableRewardedInterstitial) return;

            if (_RewardedInterstitialAd != null)
            {
                _RewardedInterstitialAd.Show(UserEarnedRewardCallBack);
                Logger.Log("Displays the Rewarded Interstitial Ad.");
                return;
            }
            else
            {
                Logger.LogWarningFormat("{0}: {1}.", nameof(_RewardedInterstitialAd), _RewardedInterstitialAd);
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
            Logger.Log("TODO: Reward the user.");
        }
        private void RewardedInterstitialOnAdFailedToPresentFullScreenContent(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarning("RewardedInterstitialOnAdFailedToPresentFullScreenContent has failed to present.");
            _RewardedInterstitialActivated = false;
        }
        private void RewardedInterstitialOnAdDidPresentFullScreenContent(object _sender, EventArgs _args)
        {
            Logger.Log("RewardedInterstitialOnAdDidPresentFullScreenContent has presented.");
        }
        private void RewardedInterstitialOnAdDidDismissFullScreenContent(object _sender, EventArgs _args)
        {
            Logger.Log("RewardedInterstitialOnAdDidDismissFullScreenContent has dismissed presentation.");
            RequestRewardedInterstitial();
        }
        private void RewardedInterstitialOnPaidEvent(object _sender, AdValueEventArgs _args)
        {
            Logger.Log("RewardedInterstitialOnPaidEvent has received a paid event.");
        }
        #endregion

        #region Native Ads Advanced (Unified)
        public bool _EnableNative;
        private bool _NativeActivated;
        private UnifiedNativeAd _UnifiedNativeAd;
        public RawImage _Native_AdChoicesLogo;
        public Text _Native_Advertiser;
        public Text _Native_Body;
        public Text _Native_CallToAction;
        public Text _Native_Headline;
        public RawImage _Native_Icon;
        private void RequestNative()
        {
            if (!_EnableNative) return;

            if (_UnifiedNativeAd != null) { if (_NativeActivated) return; }

            AdLoader _ad_loader;
#if UNITY_ANDROID
            switch (_EnableTestNative)
            {
                case false:
                    _ad_loader = new AdLoader.Builder(_Android_NativeID).ForUnifiedNativeAd().Build();
                    break;
                case true:
                    _ad_loader = new AdLoader.Builder(_Test_Android_NativeID).ForUnifiedNativeAd().Build();
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestNative)
            {
                case false:
                    _ad_loader = new AdLoader.Builder(_IOS_NativeID).ForUnifiedNativeAd().Build();
                    break;
                case true:
                    _ad_loader = new AdLoader.Builder(_Test_IOS_NativeID).ForUnifiedNativeAd().Build();
                    break;
            }
#else
            _ad_loader = new AdLoader.Builder(_NativeID).ForUnifiedNativeAd().Build();
#endif

            _ad_loader.OnUnifiedNativeAdLoaded += NativeOnUnifiedNativeAdLoaded;
            _ad_loader.OnAdFailedToLoad += NativeOnAdFailedToLoad;

            _ad_loader.LoadAd(new AdRequest.Builder().Build());

            _NativeActivated = true;
        }
        private Texture2D _AdChoicesLogo;
        private string _Advertiser;
        private string _Body;
        private string _CallToAction;
        private int _HashCode;
        private string _Headline;
        private Texture2D _Icon;
        private List<Texture2D> _Image;
        private string _Price;
        private ResponseInfo _ResponseInfo;
        private double _StarRating;
        private string _Store;
        private Type _Type;
        private bool _UnifiedNativeAdLoaded = false;
        private void ShowNativeAd()
        {
            if (!_EnableNative) return;

            if (_UnifiedNativeAdLoaded)
            {
                // Get asset of native ad.
                _AdChoicesLogo = _UnifiedNativeAd.GetAdChoicesLogoTexture();
                _Advertiser = _UnifiedNativeAd.GetAdvertiserText();
                _Body = _UnifiedNativeAd.GetBodyText();
                _CallToAction = _UnifiedNativeAd.GetCallToActionText();
                _HashCode = _UnifiedNativeAd.GetHashCode();
                _Headline = _UnifiedNativeAd.GetHeadlineText();
                _Icon = _UnifiedNativeAd.GetIconTexture();
                _Image = _UnifiedNativeAd.GetImageTextures();
                _Price = _UnifiedNativeAd.GetPrice();
                _ResponseInfo = _UnifiedNativeAd.GetResponseInfo();
                _StarRating = _UnifiedNativeAd.GetStarRating();
                _Store = _UnifiedNativeAd.GetStore();
                _Type = _UnifiedNativeAd.GetType();

                _Native_AdChoicesLogo.texture = _AdChoicesLogo;
                _Native_Advertiser.text = _Advertiser;
                _Native_Body.text = _Body;
                _Native_CallToAction.text = _CallToAction;
                _Native_Headline.text = _Headline;
                _Native_Icon.texture = _Icon;

                // Register gameobjects.
                _UnifiedNativeAd.RegisterAdChoicesLogoGameObject(_Native_AdChoicesLogo.gameObject);
                _UnifiedNativeAd.RegisterAdvertiserTextGameObject(_Native_Advertiser.gameObject);
                _UnifiedNativeAd.RegisterBodyTextGameObject(_Native_Body.gameObject);
                _UnifiedNativeAd.RegisterCallToActionGameObject(_Native_CallToAction.gameObject);
                _UnifiedNativeAd.RegisterHeadlineTextGameObject(_Native_Headline.gameObject);
                _UnifiedNativeAd.RegisterIconImageGameObject(_Native_Icon.gameObject);

                _UnifiedNativeAdLoaded = false;
            }
        }
        private void NativeOnUnifiedNativeAdLoaded(object _sender, UnifiedNativeAdEventArgs _args)
        {
            Logger.Log("NativeOnUnifiedNativeAdLoaded loaded.");
            _UnifiedNativeAd = _args.nativeAd;
            _UnifiedNativeAdLoaded = true;
            _NativeActivated = true;
        }
        private void NativeOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("NativeOnAdFailedToLoad failed to load: {0}.", _args.Message);
            _UnifiedNativeAdLoaded = false;
            _NativeActivated = false;
        }
        #endregion

        #region Mediation Test Suite
        public void ShowMediationTestSuite()
        {
            if (_MediationTestSuiteMode) MediationTestSuite.Show();
            else Logger.LogWarningFormat("{0}: {1}.", nameof(_MediationTestSuiteMode), _MediationTestSuiteMode);
        }
        private void HandleMediationTestSuiteDismissed(object _sender, EventArgs _args)
        {
            Logger.Log("HandleMediationTestSuiteDismissed event received.");
        }
        #endregion
    }
}
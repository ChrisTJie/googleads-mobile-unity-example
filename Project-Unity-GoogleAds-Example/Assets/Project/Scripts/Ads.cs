using GoogleMobileAds.Api;
using GoogleMobileAdsMediationTestSuite.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace CTJ
{
    public class Ads : MonoBehaviour
    {
        internal static Ads Instance;

        private enum Mode { Enable, Disable }
        [SerializeField] private Mode _Mode;

        [SerializeField] private bool _AutoInitialize;

        #region Units
        [SerializeField] private string _Android_BannerID;
        [SerializeField] private string _Android_InterstitialID;
        [SerializeField] private string _Android_RewardedID;
        [SerializeField] private string _Android_RewardedInterstitialID;
        [SerializeField] private string _Android_NativeID;
        [SerializeField] private string _IOS_BannerID;
        [SerializeField] private string _IOS_InterstitialID;
        [SerializeField] private string _IOS_RewardedID;
        [SerializeField] private string _IOS_RewardedInterstitialID;
        [SerializeField] private string _IOS_NativeID;
        private const string _BannerID = "unexpected_platform";
        private const string _InterstitialID = "unexpected_platform";
        private const string _RewardedID = "unexpected_platform";
        private const string _RewardedInterstitialID = "unexpected_platform";
        private const string _NativeID = "unexpected_platform";
        // Test units.
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

        #region MonoBehaviour
        private void Awake()
        {
            if (Instance == null) { DontDestroyOnLoad(gameObject); Instance = this; }
            else { Destroy(gameObject); }
        }

        private Dictionary<string, AdapterStatus> _Map;
        private string _ClassName;
        private AdapterStatus _Status;
        private void Start()
        {
            if (!_AutoInitialize) { return; }

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(_init_status =>
            {
                _Map = _init_status.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> _key_value_pair in _Map)
                {
                    _ClassName = _key_value_pair.Key;
                    _Status = _key_value_pair.Value;
                    switch (_Status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Logger.LogWarningFormat("Adapter: {0} not ready.", _ClassName);
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Logger.LogFormat("Adapter: {0} is initialized.", _ClassName);
                            break;
                    }
                }
            });
            SetTestDeviceIds();
            TestLab();
            if (!_AutoAdRequest) { Invoke("Request", 10.0f); }
            else { StartCoroutine(AutoAdRequest(_AdRequestTime)); }
            if (_MediationTestSuiteMode) { MediationTestSuite.OnMediationTestSuiteDismissed += HandleMediationTestSuiteDismissed; }
        }

        private void Update() => ShowNativeAd();
        #endregion

        #region Test Mode
        [SerializeField] private bool _TestDeviceMode;
        [SerializeField] private bool _MediationTestSuiteMode;
        // Test device ID.
        private List<string> _DeviceIDs = new List<string>();
        private string _AndroidDeviceID;
        private string _IOSDeviceID;
        private RequestConfiguration _RequestConfiguration;
        private void SetTestDeviceIds()
        {
            if (!_TestDeviceMode) { return; }

            // Add test device ID.
            _DeviceIDs.Add(AdRequest.TestDeviceSimulator);
#if UNITY_ANDROID
            _AndroidDeviceID = SystemInfo.deviceUniqueIdentifier.ToUpper().Trim();
            _DeviceIDs.Add(_AndroidDeviceID);
#elif UNITY_IOS
            _IOSDeviceID = UnityEngine.iOS.Device.advertisingIdentifier;
            _IOSDeviceID = CreateMD5(_IOSDeviceID);
            _IOSDeviceID = _IOSDeviceID.ToLower();
            _DeviceIDs.Add(_IOSDeviceID);
#endif

            foreach (string _device_ids in _DeviceIDs) { Logger.LogFormat("Added test device ID is: {0}", _device_ids); }

            _RequestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(_DeviceIDs).build();

            // Set requestConfiguration globally to MobileAds.
            MobileAds.SetRequestConfiguration(_RequestConfiguration);

            // Mediation Test Suite test device.
            if (!_MediationTestSuiteMode) { return; }
#if UNITY_ANDROID
            MediationTestSuite.AdRequest = new AdRequest.Builder().AddTestDevice(_AndroidDeviceID).Build();
#elif UNITY_IOS
            MediationTestSuite.AdRequest = new AdRequest.Builder().AddTestDevice(_IOSDeviceID).Build();
#endif
        }
        private string CreateMD5(string _input)
        {
            if (string.IsNullOrEmpty(_input)) { return string.Empty; }

            using (System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] _input_bytes = Encoding.ASCII.GetBytes(_input);
                byte[] _hash_bytes = _md5.ComputeHash(_input_bytes);

                StringBuilder _s_b = new StringBuilder();
                for (int i = 0; i < _hash_bytes.Length; i++) { _s_b.Append(_hash_bytes[i].ToString("X2")); }
                return _s_b.ToString();
            }
        }
        [SerializeField] private bool _EnableTestBanner;
        [SerializeField] private bool _EnableTestInterstitial;
        [SerializeField] private bool _EnableTestRewarded;
        [SerializeField] private bool _EnableTestRewardedInterstitial;
        [SerializeField] private bool _EnableTestNative;
#if UNITY_ANDROID
        private bool IsTestLab = false;
        // Detect Google Play pre-launch report.
        private void TestLab()
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
            catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(TestLab), _exception.Message); IsTestLab = false; }
            finally
            {
                switch (IsTestLab)
                {
                    case false:
                        Logger.LogFormat("{0}: {1}", nameof(TestLab), IsTestLab);
                        break;
                    case true:
                        Logger.LogWarningFormat("{0}: {1}", nameof(TestLab), IsTestLab);
                        break;
                }
            }
        }
#endif
        #endregion

        #region Ads Request
        [SerializeField] private bool _AutoAdRequest;
        [SerializeField] [Range(5.0f, 100.0f)] private float _AdRequestTime;
        private IEnumerator AutoAdRequest(float _delay)
        {
            while (true)
            {
                yield return Coroutine.EndOfFrame;
                yield return Coroutine.GetWaitForSeconds(_delay);

                Logger.Log("Automatically requesting ads...");
                Request();
            }
        }
        private AdRequest _AdRequest;
        private void Request()
        {
#if UNITY_ANDROID
            if (IsTestLab) { Logger.LogWarningFormat("{0}: {1}", nameof(TestLab), IsTestLab); return; };
#endif

            RequestBanner();
            RequestInterstitial();
            RequestRewarded();
            RequestRewardedInterstitial();
            if (_EnableAwake) { RequestNative(); }
        }
        #endregion

        #region Banner Ads
        [SerializeField] private bool _EnableBanner;
        private bool _BannerActivated = false;
        private BannerView _BannerView;
        private enum BannerAdSize
        {
            Banner,
            IABBanner,
            Leaderboard,
            MediumRectangle,
            SmartBanner,
            AdaptiveBanner,
            Custom
        }
        [SerializeField] private BannerAdSize _BannerAdSize;
        [SerializeField] private Vector2Int _WH;
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
        private enum BannerAdPosition
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
        [SerializeField] private BannerAdPosition _BannerAdPosition;
        [SerializeField] private Vector2Int _Pos;
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
        public void RequestBanner()
        {
            if (!_EnableBanner) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableBanner), _EnableBanner); return; }

            if (_BannerActivated) { return; }

            // Create a X banner at the X of the screen.
#if UNITY_ANDROID
            switch (_EnableTestBanner)
            {
                case false:
                    if (_BannerAdPosition == BannerAdPosition.Custom) { _BannerView = new BannerView(_Android_BannerID, _AdSize, _Pos.x, _Pos.y); }
                    else { _BannerView = new BannerView(_Android_BannerID, _AdSize, _AdPosition); }
                    break;
                case true:
                    if (_BannerAdPosition == BannerAdPosition.Custom) { _BannerView = new BannerView(_Test_Android_BannerID, _AdSize, _Pos.x, _Pos.y); }
                    else { _BannerView = new BannerView(_Test_Android_BannerID, _AdSize, _AdPosition); }
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestBanner)
            {
                case false:
                    if (_BannerAdPosition == BannerAdPosition.Custom) { _BannerView = new BannerView(_IOS_BannerID, _AdSize, _Pos.x, _Pos.y); }
                    else { _BannerView = new BannerView(_IOS_BannerID, _AdSize, _AdPosition); }
                    break;
                case true:
                    if (_BannerAdPosition == BannerAdPosition.Custom) { _BannerView = new BannerView(_Test_IOS_BannerID, _AdSize, _Pos.x, _Pos.y); }
                    else { _BannerView = new BannerView(_Test_IOS_BannerID, _AdSize, _AdPosition); }
                    break;
            }
#else
            if (_BannerAdPosition == BannerAdPosition.Custom) { _BannerView = new BannerView(_BannerID, _AdSize, _Pos.x, _Pos.y); }
            else { _BannerView = new BannerView(_BannerID, _AdSize, _AdPosition); }
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
            _BannerView.OnPaidEvent += BannerOnPaidEvent;

            _AdRequest = null;
            // Create an empty ad request.
            _AdRequest = new AdRequest.Builder().Build();

            // Load the banner with the request.
            _BannerView.LoadAd(_AdRequest);
            HideBannerAd();

            _BannerActivated = true;
        }
        [SerializeField] private bool _BannerCallbacks;
        [SerializeField] private UnityEvent _BannerOnAdLoaded;
        [SerializeField] private UnityEvent _BannerOnAdFailedToLoad;
        [SerializeField] private UnityEvent _BannerOnAdOpening;
        [SerializeField] private UnityEvent _BannerOnAdClosed;
        [SerializeField] private UnityEvent _BannerOnAdLeavingApplication;
        [SerializeField] private UnityEvent _BannerOnPaidEvent;
        private void BannerOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnAdLoaded));
            Logger.LogFormat("Banner Ad Height: {0} Width: {1}", _BannerView.GetHeightInPixels(), _BannerView.GetWidthInPixels());
            _BannerOnAdLoaded.Invoke();
            _BannerActivated = true;
        }
        private void BannerOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("{0} event received with message: {1}", nameof(BannerOnAdFailedToLoad), _args.Message);
            _BannerOnAdFailedToLoad.Invoke();
            _BannerActivated = false;
        }
        private void BannerOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnAdOpening));
            _BannerOnAdOpening.Invoke();
        }
        private void BannerOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnAdClosed));
            _BannerOnAdClosed.Invoke();
        }
        private void BannerOnAdLeavingApplication(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnAdLeavingApplication));
            _BannerOnAdLeavingApplication.Invoke();
        }
        private void BannerOnPaidEvent(object _sender, AdValueEventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnPaidEvent));
            _BannerOnPaidEvent.Invoke();
        }
        public void ShowBannerAd()
        {
            if (!_EnableBanner) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableBanner), _EnableBanner); return; }
            if (_BannerView == null) { Logger.LogWarningFormat("{0} is null.", nameof(_BannerView)); return; }
            _BannerView.Show();
        }
        public void HideBannerAd()
        {
            if (!_EnableBanner) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableBanner), _EnableBanner); return; }
            if (_BannerView == null) { Logger.LogWarningFormat("{0} is null.", nameof(_BannerView)); return; }
            _BannerView.Hide();
        }
        public void DestroyBannerAd()
        {
            _BannerView.Destroy();
            Logger.LogWarningFormat("{0} Destroyed.", nameof(_BannerView));
            _BannerActivated = false;
        }
        #endregion

        #region Interstitial Ads
        [SerializeField] private bool _EnableInterstitial;
        private bool _InterstitialActivated = false;
        private InterstitialAd _InterstitialAd;
        internal bool GetInterstitialAdIsLoaded
        {
            get
            {
                if (_InterstitialAd == null) { Logger.LogWarningFormat("{0} is null.", nameof(_InterstitialAd)); return false; }
                else if (_InterstitialAd.IsLoaded()) { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_InterstitialAd), _InterstitialAd.IsLoaded()); return true; }
                else { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_InterstitialAd), _InterstitialAd.IsLoaded()); return false; }
            }
        }
        public void RequestInterstitial()
        {
            if (!_EnableInterstitial) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableInterstitial), _EnableInterstitial); return; }

            if (_InterstitialActivated) { return; }

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
            _InterstitialAd.OnPaidEvent += InterstitialOnPaidEvent;

            _AdRequest = null;
            // Create an empty ad request.
            _AdRequest = new AdRequest.Builder().Build();

            // Load the interstitial with the request.
            _InterstitialAd.LoadAd(_AdRequest);

            _InterstitialActivated = true;
        }
        public void ShowInterstitialAd()
        {
            if (!_EnableInterstitial) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableInterstitial), _EnableInterstitial); return; }

            if (_InterstitialAd == null) { Logger.LogWarningFormat("{0} is null.", nameof(_InterstitialAd)); return; }

            if (_InterstitialAd.IsLoaded())
            {
                Logger.LogFormat("Show {0}", nameof(_InterstitialAd));
                _InterstitialAd.Show();
                return;
            }
            else { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_InterstitialAd), _InterstitialAd.IsLoaded()); }
        }
        [SerializeField] private bool _InterstitialCallbacks;
        [SerializeField] private UnityEvent _InterstitialOnAdLoaded;
        [SerializeField] private UnityEvent _InterstitialOnAdFailedToLoad;
        [SerializeField] private UnityEvent _InterstitialOnAdOpening;
        [SerializeField] private UnityEvent _InterstitialOnAdClosed;
        [SerializeField] private UnityEvent _InterstitialOnAdLeavingApplication;
        [SerializeField] private UnityEvent _InterstitialOnPaidEvent;
        private void InterstitialOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdLoaded));
            _InterstitialOnAdLoaded.Invoke();
            _InterstitialActivated = true;
        }
        private void InterstitialOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("{0} event received with message: {1}", nameof(InterstitialOnAdFailedToLoad), _args.Message);
            _InterstitialOnAdFailedToLoad.Invoke();
            _InterstitialActivated = false;
        }
        private void InterstitialOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdOpening));
            _InterstitialOnAdOpening.Invoke();
        }
        private void InterstitialOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdClosed));
            _InterstitialOnAdClosed.Invoke();
            _InterstitialActivated = false;
            RequestInterstitial();
        }
        private void InterstitialOnAdLeavingApplication(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdLeavingApplication));
            _InterstitialOnAdLeavingApplication.Invoke();
        }
        private void InterstitialOnPaidEvent(object _sender, AdValueEventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnPaidEvent));
            _InterstitialOnPaidEvent.Invoke();
        }
        public void DestroyInterstitialAd()
        {
            _InterstitialAd.Destroy();
            Logger.LogWarningFormat("{0} Destroyed.", nameof(_InterstitialAd));
            _InterstitialActivated = false;
        }
        #endregion

        #region Rewarded Ads
        [SerializeField] private bool _EnableRewarded;
        private bool _RewardedActivated = false;
        private RewardedAd _RewardedAd;
        internal bool GetRewardedAdIsLoaded
        {
            get
            {
                if (_RewardedAd == null) { Logger.LogWarningFormat("{0} is null.", nameof(_RewardedAd)); return false; }
                else if (_RewardedAd.IsLoaded()) { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_RewardedAd), _RewardedAd.IsLoaded()); return true; }
                else { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_RewardedAd), _RewardedAd.IsLoaded()); return false; }
            }
        }
        public void RequestRewarded()
        {
            if (!_EnableRewarded) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableRewarded), _EnableRewarded); return; }

            if (_RewardedActivated) { return; }

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
            // Called when an ad request failed to show.
            _RewardedAd.OnAdFailedToShow += RewardedOnAdFailedToShow;
            // Called when an ad is shown.
            _RewardedAd.OnAdOpening += RewardedOnAdOpening;
            // Called when the user should be rewarded for interacting with the ad.
            _RewardedAd.OnUserEarnedReward += RewardedOnUserEarnedReward;
            // Called when the ad is closed.
            _RewardedAd.OnAdClosed += RewardedOnAdClosed;
            _RewardedAd.OnPaidEvent += RewardedOnPaidEvent;

            _AdRequest = null;
            // Create an empty ad request.
            _AdRequest = new AdRequest.Builder().Build();

            // Load the rewarded ad with the request.
            _RewardedAd.LoadAd(_AdRequest);

            _RewardedActivated = true;
        }
        public void ShowRewardedAd()
        {
            if (!_EnableRewarded) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableRewarded), _EnableRewarded); return; }

            if (_RewardedAd == null) { Logger.LogWarningFormat("{0} is null.", nameof(_RewardedAd)); return; }

            if (_RewardedAd.IsLoaded())
            {
                Logger.LogFormat("Show {0}", nameof(_RewardedAd));
                _RewardedAd.Show();
                return;
            }
            else { Logger.LogWarningFormat("{0} IsLoaded(): {1}", nameof(_RewardedAd), _RewardedAd.IsLoaded()); }
        }
        [SerializeField] private bool _RewardedCallbacks;
        [SerializeField] private UnityEvent _RewardedOnAdLoaded;
        [SerializeField] private UnityEvent _RewardedOnAdFailedToLoad;
        [SerializeField] private UnityEvent _RewardedOnAdFailedToShow;
        [SerializeField] private UnityEvent _RewardedOnAdOpening;
        [SerializeField] private UnityEvent _RewardedOnUserEarnedReward;
        [SerializeField] private UnityEvent _RewardedOnAdClosed;
        [SerializeField] private UnityEvent _RewardedOnPaidEvent;
        private void RewardedOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(RewardedOnAdLoaded));
            _RewardedOnAdLoaded.Invoke();
            _RewardedActivated = true;
        }
        private void RewardedOnAdFailedToLoad(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("{0} event received with message: {1}", nameof(RewardedOnAdFailedToLoad), _args.Message);
            _RewardedOnAdFailedToLoad.Invoke();
            _RewardedActivated = false;
        }
        private void RewardedOnAdFailedToShow(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("{0} event received with message: {1}", nameof(RewardedOnAdFailedToShow), _args.Message);
            _RewardedOnAdFailedToShow.Invoke();
        }
        private void RewardedOnAdOpening(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(RewardedOnAdOpening));
            _RewardedOnAdOpening.Invoke();
        }
        private void RewardedOnUserEarnedReward(object _sender, Reward _args)
        {
            string _type = _args.Type;
            double _amount = _args.Amount;
            Logger.LogFormat("{0} event received for {1} {2}", nameof(RewardedOnUserEarnedReward), _amount.ToString(), _type);
            _RewardedOnUserEarnedReward.Invoke();
        }
        private void RewardedOnAdClosed(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(RewardedOnAdClosed));
            _RewardedOnAdClosed.Invoke();
            _RewardedActivated = false;
            RequestRewarded();
        }
        private void RewardedOnPaidEvent(object _sender, AdValueEventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(RewardedOnPaidEvent));
            _RewardedOnPaidEvent.Invoke();
        }
        #endregion

        #region Rewarded Interstitial Ads
        [SerializeField] private bool _EnableRewardedInterstitial;
        private bool _RewardedInterstitialActivated = false;
        private RewardedInterstitialAd _RewardedInterstitialAd;
        public void RequestRewardedInterstitial()
        {
            if (!_EnableRewardedInterstitial) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableRewardedInterstitial), _EnableRewardedInterstitial); return; }

            if (_RewardedInterstitialActivated) { return; }

            _AdRequest = null;
            // Create an empty ad request.
            _AdRequest = new AdRequest.Builder().Build();

#if UNITY_ANDROID
            switch (_EnableTestRewardedInterstitial)
            {
                case false:
                    RewardedInterstitialAd.LoadAd(_Android_RewardedInterstitialID, _AdRequest, AdLoadCallBack);
                    break;
                case true:
                    RewardedInterstitialAd.LoadAd(_Test_Android_RewardedInterstitialID, _AdRequest, AdLoadCallBack);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestRewardedInterstitial)
            {
                case false:
                    RewardedInterstitialAd.LoadAd(_IOS_RewardedInterstitialID, _AdRequest, AdLoadCallBack);
                    break;
                case true:
                    RewardedInterstitialAd.LoadAd(_Test_IOS_RewardedInterstitialID, _AdRequest, AdLoadCallBack);
                    break;
            }
#else
            RewardedInterstitialAd.LoadAd(_RewardedInterstitialID, _AdRequest, AdLoadCallBack);
#endif

            _RewardedInterstitialActivated = true;
        }
        public void ShowRewardedInterstitialAd()
        {
            if (!_EnableRewardedInterstitial) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableRewardedInterstitial), _EnableRewardedInterstitial); return; }

            if (_RewardedInterstitialAd != null)
            {
                Logger.LogFormat("Show {0}", nameof(_RewardedInterstitialAd));
                _RewardedInterstitialAd.Show(RewardedInterstitialOnUserEarnedReward);
                return;
            }
            else { Logger.LogWarningFormat("{0} is null.", nameof(_RewardedInterstitialAd)); }
        }
        private void AdLoadCallBack(RewardedInterstitialAd _rewarded_interstitial_ad, string _error)
        {
            if (_error == null)
            {
                _RewardedInterstitialAd = _rewarded_interstitial_ad;

                _RewardedInterstitialAd.OnAdDidPresentFullScreenContent += RewardedInterstitialOnAdDidPresentFullScreenContent;
                _RewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += RewardedInterstitialOnAdFailedToPresentFullScreenContent;
                _RewardedInterstitialAd.OnAdDidDismissFullScreenContent += RewardedInterstitialOnAdDidDismissFullScreenContent;
                _RewardedInterstitialAd.OnPaidEvent += RewardedInterstitialOnPaidEvent;

                Logger.LogFormat("{0} successfully loaded.", nameof(_RewardedInterstitialAd));
            }
            else { _RewardedInterstitialActivated = false; Logger.LogWarningFormat("{0}: {1}", nameof(_RewardedInterstitialAd), _error); }
        }
        [SerializeField] private bool _RewardedInterstitialCallbacks;
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdDidPresentFullScreenContent;
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdFailedToPresentFullScreenContent;
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdDidDismissFullScreenContent;
        [SerializeField] private UnityEvent _RewardedInterstitialOnUserEarnedReward;
        [SerializeField] private UnityEvent _RewardedInterstitialOnPaidEvent;
        private void RewardedInterstitialOnAdDidPresentFullScreenContent(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} has presented.", nameof(RewardedInterstitialOnAdDidPresentFullScreenContent));
            _RewardedInterstitialOnAdDidPresentFullScreenContent.Invoke();
        }
        private void RewardedInterstitialOnAdFailedToPresentFullScreenContent(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("{0} has failed to present.", nameof(RewardedInterstitialOnAdFailedToPresentFullScreenContent));
            _RewardedInterstitialOnAdFailedToPresentFullScreenContent.Invoke();
            _RewardedInterstitialActivated = false;
        }
        private void RewardedInterstitialOnAdDidDismissFullScreenContent(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} has dismissed presentation.", nameof(RewardedInterstitialOnAdDidDismissFullScreenContent));
            _RewardedInterstitialOnAdDidDismissFullScreenContent.Invoke();
            _RewardedInterstitialActivated = false;
            RequestRewardedInterstitial();
        }
        private void RewardedInterstitialOnUserEarnedReward(Reward _reward)
        {
            Logger.LogFormat("{0}", nameof(RewardedInterstitialOnUserEarnedReward));
            _RewardedInterstitialOnUserEarnedReward.Invoke();
        }
        private void RewardedInterstitialOnPaidEvent(object _sender, AdValueEventArgs _args)
        {
            Logger.LogFormat("{0} has received a paid event.", nameof(RewardedInterstitialOnPaidEvent));
            _RewardedInterstitialOnPaidEvent.Invoke();
        }
        #endregion

        #region Native Ads Advanced (Unified)
        [SerializeField] private bool _EnableNative;
        [SerializeField] private bool _EnableAwake;
        private bool _NativeActivated = false;
        private UnifiedNativeAd _UnifiedNativeAd;
        private AdLoader _AdLoader;
        public void RequestNative()
        {
            if (!_EnableNative) { Logger.LogWarningFormat("{0}: {1}", nameof(_EnableNative), _EnableNative); return; }

            if (_NativeActivated) { return; }

#if UNITY_ANDROID
            switch (_EnableTestNative)
            {
                case false:
                    _AdLoader = new AdLoader.Builder(_Android_NativeID).ForUnifiedNativeAd().Build();
                    break;
                case true:
                    _AdLoader = new AdLoader.Builder(_Test_Android_NativeID).ForUnifiedNativeAd().Build();
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestNative)
            {
                case false:
                    _AdLoader = new AdLoader.Builder(_IOS_NativeID).ForUnifiedNativeAd().Build();
                    break;
                case true:
                    _AdLoader = new AdLoader.Builder(_Test_IOS_NativeID).ForUnifiedNativeAd().Build();
                    break;
            }
#else
            _AdLoader = new AdLoader.Builder(_NativeID).ForUnifiedNativeAd().Build();
#endif

            _AdLoader.OnUnifiedNativeAdLoaded += NativeOnUnifiedNativeAdLoaded;
            _AdLoader.OnCustomNativeTemplateAdLoaded += NativeOnCustomNativeTemplateAdLoaded;
            _AdLoader.OnAdFailedToLoad += NativeOnAdFailedToLoad;
            _AdLoader.OnNativeAdClicked += NativeOnNativeAdClicked;
            _AdLoader.OnNativeAdOpening += NativeOnNativeAdOpening;
            _AdLoader.OnNativeAdClosed += NativeOnNativeAdClosed;
            _AdLoader.OnNativeAdImpression += NativeOnNativeAdImpression;
            _AdLoader.OnNativeAdLeavingApplication += NativeOnNativeAdLeavingApplication;

            _AdLoader.LoadAd(new AdRequest.Builder().Build());

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
        // External call get assets of native advanced ads.
        internal Texture2D GetAdChoicesLogo { get { return _AdChoicesLogo; } }
        internal string GetAdvertiser { get { return _Advertiser; } }
        internal string GetBody { get { return _Body; } }
        internal string GetCallToAction { get { return _CallToAction; } }
        internal new int GetHashCode { get { return _HashCode; } }
        internal string GetHeadline { get { return _Headline; } }
        internal Texture2D GetIcon { get { return _Icon; } }
        internal List<Texture2D> GetImage { get { return _Image; } }
        internal string GetPrice { get { return _Price; } }
        internal ResponseInfo GetResponseInfo { get { return _ResponseInfo; } }
        internal double GetStarRating { get { return _StarRating; } }
        internal string GetStore { get { return _Store; } }
        internal new Type GetType { get { return _Type; } }
        private GameObject _RegisterAdChoicesLogo;
        private GameObject _RegisterAdvertiser;
        private GameObject _RegisterBody;
        private GameObject _RegisterCallToAction;
        private GameObject _RegisterHeadline;
        private GameObject _RegisterIcon;
        private List<GameObject> _RegisterImage;
        private GameObject _RegisterPrice;
        private GameObject _RegisterStore;
        // External call register gameobject function.
        internal void RegisterAdChoicesLogo(GameObject _go) => _RegisterAdChoicesLogo = _go;
        internal void RegisterAdvertiser(GameObject _go) => _RegisterAdvertiser = _go;
        internal void RegisterBody(GameObject _go) => _RegisterBody = _go;
        internal void RegisterCallToAction(GameObject _go) => _RegisterCallToAction = _go;
        internal void RegisterHeadline(GameObject _go) => _RegisterHeadline = _go;
        internal void RegisterIcon(GameObject _go) => _RegisterIcon = _go;
        internal void RegisterImage(List<GameObject> _go) => _RegisterImage = _go;
        internal void RegisterPrice(GameObject _go) => _RegisterPrice = _go;
        internal void RegisterStore(GameObject _go) => _RegisterStore = _go;
        [SerializeField] private UnityEvent _NativeInitialize;
        internal void EventNativeInitialize(UnityAction _unity_action) => _NativeInitialize.AddListener(_unity_action);
        private bool _UnifiedNativeAdLoaded = false;
        private void ShowNativeAd()
        {
            if (!_EnableNative) { return; }

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

                // Invoke all registered callbacks (runtime and persistent).
                _NativeInitialize.Invoke();

                // Register gameobjects.
                try
                {
                    _UnifiedNativeAd.RegisterAdChoicesLogoGameObject(_RegisterAdChoicesLogo);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterAdChoicesLogo), _UnifiedNativeAd.RegisterAdChoicesLogoGameObject(_RegisterAdChoicesLogo));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterAdChoicesLogo), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterAdvertiserTextGameObject(_RegisterAdvertiser);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterAdvertiser), _UnifiedNativeAd.RegisterAdvertiserTextGameObject(_RegisterAdvertiser));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterAdvertiser), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterBodyTextGameObject(_RegisterBody);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterBody), _UnifiedNativeAd.RegisterBodyTextGameObject(_RegisterBody));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterBody), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterCallToActionGameObject(_RegisterCallToAction);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterCallToAction), _UnifiedNativeAd.RegisterCallToActionGameObject(_RegisterCallToAction));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterCallToAction), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterHeadlineTextGameObject(_RegisterHeadline);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterHeadline), _UnifiedNativeAd.RegisterHeadlineTextGameObject(_RegisterHeadline));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterHeadline), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterIconImageGameObject(_RegisterIcon);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterIcon), _UnifiedNativeAd.RegisterIconImageGameObject(_RegisterIcon));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterIcon), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterImageGameObjects(_RegisterImage);
                    Logger.LogFormat("{0} total length: {1}", nameof(_RegisterImage), _RegisterImage.Count);
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterImage), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterPriceGameObject(_RegisterPrice);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterPrice), _UnifiedNativeAd.RegisterPriceGameObject(_RegisterPrice));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterPrice), _exception.Message); }
                try
                {
                    _UnifiedNativeAd.RegisterStoreGameObject(_RegisterStore);
                    Logger.LogFormat("{0} registration status: {1}", nameof(_RegisterStore), _UnifiedNativeAd.RegisterStoreGameObject(_RegisterStore));
                }
                catch (Exception _exception) { Logger.LogWarningFormat("{0}: {1}", nameof(_RegisterStore), _exception.Message); }

                _UnifiedNativeAdLoaded = false;
            }
        }
        [SerializeField] private bool _NativeCallbacks;
        [SerializeField] private UnityEvent _NativeOnUnifiedNativeAdLoaded;
        [SerializeField] private UnityEvent _NativeOnCustomNativeTemplateAdLoaded;
        [SerializeField] private UnityEvent _NativeOnAdFailedToLoad;
        [SerializeField] private UnityEvent _NativeOnNativeAdClicked;
        [SerializeField] private UnityEvent _NativeOnNativeAdOpening;
        [SerializeField] private UnityEvent _NativeOnNativeAdClosed;
        [SerializeField] private UnityEvent _NativeOnNativeAdImpression;
        [SerializeField] private UnityEvent _NativeOnNativeAdLeavingApplication;
        private void NativeOnUnifiedNativeAdLoaded(object _sender, UnifiedNativeAdEventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnUnifiedNativeAdLoaded));
            _UnifiedNativeAd = _args.nativeAd;
            _UnifiedNativeAdLoaded = true;
            _NativeOnUnifiedNativeAdLoaded.Invoke();
            _NativeActivated = true;
        }
        private void NativeOnCustomNativeTemplateAdLoaded(object _sender, CustomNativeEventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnCustomNativeTemplateAdLoaded));
            _NativeOnCustomNativeTemplateAdLoaded.Invoke();
        }
        private void NativeOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            Logger.LogWarningFormat("{0} failed to load: {1}", nameof(NativeOnAdFailedToLoad), _args.Message);
            _UnifiedNativeAdLoaded = false;
            _NativeOnAdFailedToLoad.Invoke();
            _NativeActivated = false;
        }
        private void NativeOnNativeAdClicked(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnNativeAdClicked));
            _NativeOnNativeAdClicked.Invoke();
        }
        private void NativeOnNativeAdOpening(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnNativeAdOpening));
            _NativeOnNativeAdOpening.Invoke();
        }
        private void NativeOnNativeAdClosed(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnNativeAdClosed));
            _NativeOnNativeAdClosed.Invoke();
        }
        private void NativeOnNativeAdImpression(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnNativeAdImpression));
            _NativeOnNativeAdImpression.Invoke();
        }
        private void NativeOnNativeAdLeavingApplication(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(NativeOnNativeAdLeavingApplication));
            _NativeOnNativeAdLeavingApplication.Invoke();
        }
        public void DestroyUnifiedNativeAd()
        {
            _UnifiedNativeAd.Destroy();
            Logger.LogWarningFormat("{0} Destroyed.", nameof(_UnifiedNativeAd));
            _NativeActivated = false;
        }
        #endregion

        #region Mediation Test Suite
        public void ShowMediationTestSuite()
        {
            if (_MediationTestSuiteMode) { MediationTestSuite.Show(); return; }
            else { Logger.LogWarningFormat("{0}: {1}", nameof(_MediationTestSuiteMode), _MediationTestSuiteMode); }
        }
        private void HandleMediationTestSuiteDismissed(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(HandleMediationTestSuiteDismissed));
        }
        #endregion
    }
}
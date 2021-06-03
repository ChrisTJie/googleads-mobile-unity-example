using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
        }
        #endregion

        #region Test Mode
        [SerializeField] private bool _TestDeviceMode;
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

        #region GDPR
        private class GDPRJson
        {
            [SerializeField] internal bool is_request_in_eea_or_unknown;
        }
        //private bool? _GPPRApplicable = null;
        private string _Response;
        private const string EU_QUERY_URL = "https://adservice.google.com/getconfig/pubvendors";
        //private int _Index;
        internal bool GetGDPRApplicable
        {
            get
            {
                GDPRJson _GDPRJson = new GDPRJson();
                try
                {
                    using (WebClient _web_client = new WebClient())
                    {
                        _Response = _web_client.DownloadString(EU_QUERY_URL);
                        _GDPRJson = JsonUtility.FromJson<GDPRJson>(_Response);
                    }
                }
                catch (Exception _exception)
                {
                    _GDPRJson.is_request_in_eea_or_unknown = true;
                    Logger.LogWarningFormat("{0}: {1}", nameof(GetGDPRApplicable), _exception.Message);
                }
                finally { Logger.LogFormat("Is user possibly located in the EEA: {0}", _GDPRJson.is_request_in_eea_or_unknown); }
                return _GDPRJson.is_request_in_eea_or_unknown;
            }
            /*
            get
            {
                if (!_GPPRApplicable.HasValue)
                {
                    try
                    {
                        using (WebClient _web_client = new WebClient())
                        {
                            _Response = _web_client.DownloadString(EU_QUERY_URL);
                            _Index = _Response.IndexOf("is_request_in_eea_or_unknown\":");
                            if (_Index < 0)
                                _GPPRApplicable = true;
                            else
                            {
                                _Index += 30;
                                _GPPRApplicable = _Index >= _Response.Length || !_Response.Substring(_Index).TrimStart().StartsWith("false");
                            }
                        }
                    }
                    catch (Exception _exception)
                    {
                        Logger.LogWarningFormat("{0}: {1}", nameof(_GPPRApplicable), _exception.Message);
                        _GPPRApplicable = true;
                    }
                }
                return _GPPRApplicable.Value;
            }
            */
        }
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
        }
        #endregion

        #region Banner Ads
        [SerializeField] private bool _EnableBanner;
        [SerializeField] private bool _AutoShowBanner;
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
            Top,
            TopLeft,
            TopRight
        }
        [SerializeField] private BannerAdPosition _BannerAdPosition;
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
                    _BannerView = new BannerView(_Android_BannerID, _AdSize, _AdPosition);
                    break;
                case true:
                    _BannerView = new BannerView(_Test_Android_BannerID, _AdSize, _AdPosition);
                    break;
            }
#elif UNITY_IOS
            switch (_EnableTestBanner)
            {
                case false:
                    _BannerView = new BannerView(_IOS_BannerID, _AdSize, _AdPosition);
                    break;
                case true:
                    _BannerView = new BannerView(_Test_IOS_BannerID, _AdSize, _AdPosition);
                    break;
            }
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
            _BannerView.OnPaidEvent += BannerOnPaidEvent;

            _AdRequest = null;
            // Create an empty ad request.
            _AdRequest = new AdRequest.Builder().Build();

            // Load the banner with the request.
            _BannerView.LoadAd(_AdRequest);

            if (_AutoShowBanner) { ShowBannerAd(); }
            else if (!_AutoShowBanner) { HideBannerAd(); }

            _BannerActivated = true;
        }
        [SerializeField] private bool _BannerCallbacks;
        [SerializeField] private UnityEvent _BannerOnAdLoaded;
        internal void EventAddListener_BannerOnAdLoaded(UnityAction _unity_action) => _BannerOnAdLoaded.AddListener(_unity_action);
        public void EventRemoveAllListeners_BannerOnAdLoaded() => _BannerOnAdLoaded.RemoveAllListeners();
        [SerializeField] private UnityEvent _BannerOnAdFailedToLoad;
        internal void EventAddListener_BannerOnAdFailedToLoad(UnityAction _unity_action) => _BannerOnAdFailedToLoad.AddListener(_unity_action);
        public void EventRemoveAllListeners_BannerOnAdFailedToLoad() => _BannerOnAdFailedToLoad.RemoveAllListeners();
        [SerializeField] private UnityEvent _BannerOnAdOpening;
        internal void EventAddListener_BannerOnAdOpening(UnityAction _unity_action) => _BannerOnAdOpening.AddListener(_unity_action);
        public void EventRemoveAllListeners_BannerOnAdOpening() => _BannerOnAdOpening.RemoveAllListeners();
        [SerializeField] private UnityEvent _BannerOnAdClosed;
        internal void EventAddListener_BannerOnAdClosed(UnityAction _unity_action) => _BannerOnAdClosed.AddListener(_unity_action);
        public void EventRemoveAllListeners_BannerOnAdClosed() => _BannerOnAdClosed.RemoveAllListeners();
        [SerializeField] private UnityEvent _BannerOnPaidEvent;
        internal void EventAddListener_BannerOnPaidEvent(UnityAction _unity_action) => _BannerOnPaidEvent.AddListener(_unity_action);
        public void EventRemoveAllListeners_BannerOnPaidEvent() => _BannerOnPaidEvent.RemoveAllListeners();
        private void BannerOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(BannerOnAdLoaded));
            Logger.LogFormat("Banner Ad Height: {0} Width: {1}", _BannerView.GetHeightInPixels(), _BannerView.GetWidthInPixels());
            _BannerOnAdLoaded.Invoke();
            _BannerActivated = true;
        }
        private void BannerOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            LoadAdError _load_ad_error = _args.LoadAdError;

            // Gets the domain from which the error came.
            string _domain = _load_ad_error.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            int _code = _load_ad_error.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            string _message = _load_ad_error.GetMessage();

            // Gets the cause of the error, if available.
            AdError _ad_error = _load_ad_error.GetCause();

            // All of this information is available via the error's toString() method.
            Logger.LogWarningFormat("{0} load error string: {1}", nameof(BannerOnAdFailedToLoad), _load_ad_error.ToString());

            // Get response information, which may include results of mediation requests.
            ResponseInfo _response_info = _load_ad_error.GetResponseInfo();
            Logger.LogWarningFormat("{0} response info: {1}", nameof(BannerOnAdFailedToLoad), _response_info.ToString());

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
            _InterstitialAd.OnAdFailedToShow += InterstitialOnAdFailedToShow;
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
        internal void EventAddListener_InterstitialOnAdLoaded(UnityAction _unity_action) => _InterstitialOnAdLoaded.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdLoaded() => _InterstitialOnAdLoaded.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnAdFailedToLoad;
        internal void EventAddListener_InterstitialOnAdFailedToLoad(UnityAction _unity_action) => _InterstitialOnAdFailedToLoad.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdFailedToLoad() => _InterstitialOnAdFailedToLoad.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnAdOpening;
        internal void EventAddListener_InterstitialOnAdOpening(UnityAction _unity_action) => _InterstitialOnAdOpening.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdOpening() => _InterstitialOnAdOpening.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnAdClosed;
        internal void EventAddListener_InterstitialOnAdClosed(UnityAction _unity_action) => _InterstitialOnAdClosed.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdClosed() => _InterstitialOnAdClosed.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnAdFailedToShow;
        internal void EventAddListener_InterstitialOnAdFailedToShow(UnityAction _unity_action) => _InterstitialOnAdFailedToShow.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdFailedToShow() => _InterstitialOnAdFailedToShow.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnAdDidRecordImpression;
        internal void EventAddListener_InterstitialOnAdDidRecordImpression(UnityAction _unity_action) => _InterstitialOnAdDidRecordImpression.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnAdDidRecordImpression() => _InterstitialOnAdDidRecordImpression.RemoveAllListeners();
        [SerializeField] private UnityEvent _InterstitialOnPaidEvent;
        internal void EventAddListener_InterstitialOnPaidEvent(UnityAction _unity_action) => _InterstitialOnPaidEvent.AddListener(_unity_action);
        public void EventRemoveAllListeners_InterstitialOnPaidEvent() => _InterstitialOnPaidEvent.RemoveAllListeners();
        private void InterstitialOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdLoaded));
            _InterstitialOnAdLoaded.Invoke();
            _InterstitialActivated = true;
        }
        private void InterstitialOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            LoadAdError _load_ad_error = _args.LoadAdError;

            // Gets the domain from which the error came.
            string _domain = _load_ad_error.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            int _code = _load_ad_error.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            string _message = _load_ad_error.GetMessage();

            // Gets the cause of the error, if available.
            AdError _ad_error = _load_ad_error.GetCause();

            // All of this information is available via the error's toString() method.
            Logger.LogWarningFormat("{0} load error string: {1}", nameof(InterstitialOnAdFailedToLoad), _load_ad_error.ToString());

            // Get response information, which may include results of mediation requests.
            ResponseInfo _response_info = _load_ad_error.GetResponseInfo();
            Logger.LogWarningFormat("{0} response info: {1}", nameof(InterstitialOnAdFailedToLoad), _response_info.ToString());

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
        private void InterstitialOnAdFailedToShow(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogFormat("{0} event received with message: {1}", nameof(InterstitialOnAdFailedToShow), _args.AdError.GetMessage());
            _InterstitialOnAdFailedToShow.Invoke();
        }
        private void InterstitialOnAdDidRecordImpression(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(InterstitialOnAdDidRecordImpression));
            _InterstitialOnAdDidRecordImpression.Invoke();
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
        internal void EventAddListener_RewardedOnAdLoaded(UnityAction _unity_action) => _RewardedOnAdLoaded.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnAdLoaded() => _RewardedOnAdLoaded.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnAdFailedToLoad;
        internal void EventAddListener_RewardedOnAdFailedToLoad(UnityAction _unity_action) => _RewardedOnAdFailedToLoad.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnAdFailedToLoad() => _RewardedOnAdFailedToLoad.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnAdFailedToShow;
        internal void EventAddListener_RewardedOnAdFailedToShow(UnityAction _unity_action) => _RewardedOnAdFailedToShow.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnAdFailedToShow() => _RewardedOnAdFailedToShow.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnAdOpening;
        internal void EventAddListener_RewardedOnAdOpening(UnityAction _unity_action) => _RewardedOnAdOpening.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnAdOpening() => _RewardedOnAdOpening.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnUserEarnedReward;
        internal void EventAddListener_RewardedOnUserEarnedReward(UnityAction _unity_action) => _RewardedOnUserEarnedReward.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnUserEarnedReward() => _RewardedOnUserEarnedReward.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnAdClosed;
        internal void EventAddListener_RewardedOnAdClosed(UnityAction _unity_action) => _RewardedOnAdClosed.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnAdClosed() => _RewardedOnAdClosed.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedOnPaidEvent;
        internal void EventAddListener_RewardedOnPaidEvent(UnityAction _unity_action) => _RewardedOnPaidEvent.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedOnPaidEvent() => _RewardedOnPaidEvent.RemoveAllListeners();
        private void RewardedOnAdLoaded(object _sender, EventArgs _args)
        {
            Logger.LogFormat("{0} event received.", nameof(RewardedOnAdLoaded));
            _RewardedOnAdLoaded.Invoke();
            _RewardedActivated = true;
        }
        private void RewardedOnAdFailedToLoad(object _sender, AdFailedToLoadEventArgs _args)
        {
            LoadAdError _load_ad_error = _args.LoadAdError;

            // Gets the domain from which the error came.
            string _domain = _load_ad_error.GetDomain();

            // Gets the error code. See
            // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
            // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
            // for a list of possible codes.
            int _code = _load_ad_error.GetCode();

            // Gets an error message.
            // For example "Account not approved yet". See
            // https://support.google.com/admob/answer/9905175 for explanations of
            // common errors.
            string _message = _load_ad_error.GetMessage();

            // Gets the cause of the error, if available.
            AdError _ad_error = _load_ad_error.GetCause();

            // All of this information is available via the error's toString() method.
            Logger.LogWarningFormat("{0} load error string: {1}", nameof(RewardedOnAdFailedToLoad), _load_ad_error.ToString());

            // Get response information, which may include results of mediation requests.
            ResponseInfo _response_info = _load_ad_error.GetResponseInfo();
            Logger.LogWarningFormat("{0} response info: {1}", nameof(RewardedOnAdFailedToLoad), _response_info.ToString());

            _RewardedOnAdFailedToLoad.Invoke();
            _RewardedActivated = false;
        }
        private void RewardedOnAdFailedToShow(object _sender, AdErrorEventArgs _args)
        {
            Logger.LogWarningFormat("{0} event received with message: {1}", nameof(RewardedOnAdFailedToShow), _args.AdError.GetMessage());
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
        private void AdLoadCallBack(RewardedInterstitialAd _rewarded_interstitial_ad, AdFailedToLoadEventArgs _args)
        {
            if (_args == null)
            {
                _RewardedInterstitialAd = _rewarded_interstitial_ad;

                _RewardedInterstitialAd.OnAdDidPresentFullScreenContent += RewardedInterstitialOnAdDidPresentFullScreenContent;
                _RewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += RewardedInterstitialOnAdFailedToPresentFullScreenContent;
                _RewardedInterstitialAd.OnAdDidDismissFullScreenContent += RewardedInterstitialOnAdDidDismissFullScreenContent;
                _RewardedInterstitialAd.OnPaidEvent += RewardedInterstitialOnPaidEvent;

                Logger.LogFormat("{0} successfully loaded.", nameof(_RewardedInterstitialAd));
            }
            else
            {
                LoadAdError _load_ad_error = _args.LoadAdError;

                // Gets the domain from which the error came.
                string _domain = _load_ad_error.GetDomain();

                // Gets the error code. See
                // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
                // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
                // for a list of possible codes.
                int _code = _load_ad_error.GetCode();

                // Gets an error message.
                // For example "Account not approved yet". See
                // https://support.google.com/admob/answer/9905175 for explanations of
                // common errors.
                string _message = _load_ad_error.GetMessage();

                // Gets the cause of the error, if available.
                AdError _ad_error = _load_ad_error.GetCause();

                // All of this information is available via the error's toString() method.
                Logger.LogWarningFormat("{0} load error string: {1}", nameof(_RewardedInterstitialAd), _load_ad_error.ToString());

                // Get response information, which may include results of mediation requests.
                ResponseInfo _response_info = _load_ad_error.GetResponseInfo();
                Logger.LogWarningFormat("{0} response info: {1}", nameof(_RewardedInterstitialAd), _response_info.ToString());

                _RewardedInterstitialActivated = false;
            }
        }
        [SerializeField] private bool _RewardedInterstitialCallbacks;
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdDidPresentFullScreenContent;
        internal void EventAddListener_RewardedInterstitialOnAdDidPresentFullScreenContent(UnityAction _unity_action) => _RewardedInterstitialOnAdDidPresentFullScreenContent.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedInterstitialOnAdDidPresentFullScreenContent() => _RewardedInterstitialOnAdDidPresentFullScreenContent.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdFailedToPresentFullScreenContent;
        internal void EventAddListener_RewardedInterstitialOnAdFailedToPresentFullScreenContent(UnityAction _unity_action) => _RewardedInterstitialOnAdFailedToPresentFullScreenContent.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedInterstitialOnAdFailedToPresentFullScreenContent() => _RewardedInterstitialOnAdFailedToPresentFullScreenContent.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedInterstitialOnAdDidDismissFullScreenContent;
        internal void EventAddListener_RewardedInterstitialOnAdDidDismissFullScreenContent(UnityAction _unity_action) => _RewardedInterstitialOnAdDidDismissFullScreenContent.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedInterstitialOnAdDidDismissFullScreenContent() => _RewardedInterstitialOnAdDidDismissFullScreenContent.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedInterstitialOnUserEarnedReward;
        internal void EventAddListener_RewardedInterstitialOnUserEarnedReward(UnityAction _unity_action) => _RewardedInterstitialOnUserEarnedReward.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedInterstitialOnUserEarnedReward() => _RewardedInterstitialOnUserEarnedReward.RemoveAllListeners();
        [SerializeField] private UnityEvent _RewardedInterstitialOnPaidEvent;
        internal void EventAddListener_RewardedInterstitialOnPaidEvent(UnityAction _unity_action) => _RewardedInterstitialOnPaidEvent.AddListener(_unity_action);
        public void EventRemoveAllListeners_RewardedInterstitialOnPaidEvent() => _RewardedInterstitialOnPaidEvent.RemoveAllListeners();
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
    }
}
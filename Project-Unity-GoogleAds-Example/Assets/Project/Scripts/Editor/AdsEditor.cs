using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CTJ
{
    [CustomEditor(typeof(Ads))]
    public class AdsEditor : Editor
    {
        private Ads _Ads;

        public override void OnInspectorGUI()
        {
            _Ads = target as Ads;
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Editor in play mode.", MessageType.Info);
                return;
            }
            if (_Ads._Mode == Ads.Mode.Enable)
            {
                if (GUILayout.Button("Collapse", GUILayout.Height(25)))
                {
                    _Ads._Mode = Ads.Mode.Disable;
                    return;
                }
                EditorUtility.SetDirty(_Ads);
                EditorGUILayout.Space(25);
                _Ads._AutoInitialize = EditorGUILayout.Toggle("Auto Initialize", _Ads._AutoInitialize);
                if (_Ads._AutoInitialize)
                {
                    EditorGUILayout.Space(25);
                    _Ads._TestDeviceMode = EditorGUILayout.Toggle("Test Device Mode", _Ads._TestDeviceMode);
                    if (_Ads._TestDeviceMode)
                    {
                        EditorGUILayout.HelpBox("Test device mode enabled.", MessageType.Warning);
                    }
                    _Ads._MediationTestSuiteMode = EditorGUILayout.Toggle("Mediation Test Suite Mode", _Ads._MediationTestSuiteMode);
                    if (_Ads._MediationTestSuiteMode)
                    {
                        EditorGUILayout.HelpBox("Mediation Test Suite mode enabled.", MessageType.Warning);
                    }
                    EditorGUILayout.Space(25);
                    _Ads._AutoAdRequest = EditorGUILayout.Toggle("Auto Ad Request", _Ads._AutoAdRequest);
                    if (_Ads._AutoAdRequest)
                    {
                        _Ads._AdRequestTime = EditorGUILayout.Slider("Ad Request Time", _Ads._AdRequestTime, 5.0f, 100.0f);
                        EditorGUILayout.HelpBox("Ad request interval per second.", MessageType.Info);
                    }
                    EditorGUILayout.Space(25);
                    _Ads._EnableBanner = EditorGUILayout.Toggle("Enable Banner", _Ads._EnableBanner);
                    if (_Ads._EnableBanner)
                    {
                        _Ads._EnableTestBanner = EditorGUILayout.Toggle("Test Ad Unit ID", _Ads._EnableTestBanner);
                        if (_Ads._EnableTestBanner)
                        {
                            EditorGUILayout.HelpBox("Banner ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            _Ads._Android_BannerID = EditorGUILayout.TextField("Android Banner ID", _Ads._Android_BannerID);
                            _Ads._IOS_BannerID = EditorGUILayout.TextField("iOS Banner ID", _Ads._IOS_BannerID);
                        }
                        _Ads._BannerAdSize = (Ads.BannerAdSize)EditorGUILayout.EnumPopup("Banner Ad Size", _Ads._BannerAdSize);
                        if (_Ads._BannerAdSize == Ads.BannerAdSize.Custom)
                        {
                            _Ads._WH = EditorGUILayout.Vector2IntField("Banner Width & Height", new Vector2Int(Mathf.Abs(_Ads._WH.x), Mathf.Abs(_Ads._WH.y)));
                            EditorGUILayout.HelpBox("Size in dp (W x H).", MessageType.Info);
                        }
                        _Ads._BannerAdPosition = (Ads.BannerAdPosition)EditorGUILayout.EnumPopup("Banner Ad Position", _Ads._BannerAdPosition);
                        if (_Ads._BannerAdPosition == Ads.BannerAdPosition.Custom)
                        {
                            _Ads._Pos = EditorGUILayout.Vector2IntField("Banner Position X & Y", _Ads._Pos);
                            EditorGUILayout.HelpBox("The top-left corner of the BannerView will be positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.", MessageType.Info);
                        }
                    }
                    EditorGUILayout.Space(25);
                    _Ads._EnableInterstitial = EditorGUILayout.Toggle("Enable Interstitial", _Ads._EnableInterstitial);
                    if (_Ads._EnableInterstitial)
                    {
                        _Ads._EnableTestInterstitial = EditorGUILayout.Toggle("Test Ad Unit ID", _Ads._EnableTestInterstitial);
                        if (_Ads._EnableTestInterstitial)
                        {
                            EditorGUILayout.HelpBox("Interstitial ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            _Ads._Android_InterstitialID = EditorGUILayout.TextField("Android Interstitial ID", _Ads._Android_InterstitialID);
                            _Ads._IOS_InterstitialID = EditorGUILayout.TextField("iOS Interstitial ID", _Ads._IOS_InterstitialID);
                        }
                    }
                    EditorGUILayout.Space(25);
                    _Ads._EnableRewarded = EditorGUILayout.Toggle("Enable Rewarded", _Ads._EnableRewarded);
                    if (_Ads._EnableRewarded)
                    {
                        _Ads._EnableTestRewarded = EditorGUILayout.Toggle("Test Ad Unit ID", _Ads._EnableTestRewarded);
                        if (_Ads._EnableTestRewarded)
                        {
                            EditorGUILayout.HelpBox("Rewarded ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            _Ads._Android_RewardedID = EditorGUILayout.TextField("Android Rewarded ID", _Ads._Android_RewardedID);
                            _Ads._IOS_RewardedID = EditorGUILayout.TextField("iOS Rewarded ID", _Ads._IOS_RewardedID);
                        }
                    }
                    EditorGUILayout.Space(25);
                    _Ads._EnableRewardedInterstitial = EditorGUILayout.Toggle("Enable Rewarded Interstitial", _Ads._EnableRewardedInterstitial);
                    if (_Ads._EnableRewardedInterstitial)
                    {
                        _Ads._EnableTestRewardedInterstitial = EditorGUILayout.Toggle("Test Ad Unit ID", _Ads._EnableTestRewardedInterstitial);
                        if (_Ads._EnableTestRewardedInterstitial)
                        {
                            EditorGUILayout.HelpBox("Rewarded interstitial ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            _Ads._Android_RewardedInterstitialID = EditorGUILayout.TextField("Android Rewarded Interstitial ID", _Ads._Android_RewardedInterstitialID);
                            _Ads._IOS_RewardedInterstitialID = EditorGUILayout.TextField("iOS Rewarded Interstitial ID", _Ads._IOS_RewardedInterstitialID);
                        }
                    }
                    EditorGUILayout.Space(25);
                    _Ads._EnableNative = EditorGUILayout.Toggle("Enable Native", _Ads._EnableNative);
                    if (_Ads._EnableNative)
                    {
                        EditorGUILayout.HelpBox("Native advanced ads will not show in edit mode.", MessageType.Warning);
                        _Ads._EnableTestNative = EditorGUILayout.Toggle("Test Ad Unit ID", _Ads._EnableTestNative);
                        if (_Ads._EnableTestNative)
                        {
                            EditorGUILayout.HelpBox("Native ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            _Ads._Android_NativeID = EditorGUILayout.TextField("Android Native ID", _Ads._Android_NativeID);
                            _Ads._IOS_NativeID = EditorGUILayout.TextField("iOS Native ID", _Ads._IOS_NativeID);
                        }
                        EditorGUILayout.HelpBox("If GameObject objects registered to ad assets are missing Collider components or have an incorrectly configured one, native advanced ads will not operate correctly.", MessageType.Warning);
                        EditorGUILayout.HelpBox("You can access the Ads module API via the Ads class under the CTJ namespace to register gameobjects or get assets of native advanced ads.", MessageType.Info);
                        EditorGUILayout.HelpBox("Note that ad assets should only be accessed on the main thread, for example, from the Update() method of a Unity script. Also note the following assets are not always guaranteed to be present, and should be checked before being displayed.", MessageType.Info);
                        return;
                    }
                    return;
                }
                return;
            }
            else if (_Ads._Mode == Ads.Mode.Disable)
            {
                if (GUILayout.Button("Edit", GUILayout.Height(25)))
                {
                    _Ads._Mode = Ads.Mode.Enable;
                    return;
                }
                return;
            }
        }
    }
}
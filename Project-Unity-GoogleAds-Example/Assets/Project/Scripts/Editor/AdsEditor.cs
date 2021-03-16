using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(Ads))]
public class AdsEditor : Editor
{
    private Ads _Ads;
    private bool _NativeObject;

    public override void OnInspectorGUI()
    {
        _Ads = target as Ads;
        EditorUtility.SetDirty(_Ads);
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("Editor in play mode.");
            return;
        }
        if (_Ads._Mode == Ads.Mode.Enable)
        {
            if (GUILayout.Button("Collapse"))
            {
                _Ads._Mode = Ads.Mode.Disable;
                return;
            }
            _Ads._AutoInitialize = EditorGUILayout.Toggle("Auto Initialize", _Ads._AutoInitialize);
            if (_Ads._AutoInitialize)
            {
                EditorGUILayout.Space(25);
                _Ads._TestDeviceMode = EditorGUILayout.Toggle("Test Device Mode", _Ads._TestDeviceMode);
                if (_Ads._TestDeviceMode)
                {
                    EditorGUILayout.HelpBox("Test device mode enabled.", MessageType.Warning);
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
                        _Ads._Width = EditorGUILayout.IntField("Banner Width", _Ads._Width);
                        _Ads._Height = EditorGUILayout.IntField("Banner Height", _Ads._Height);
                    }
                    _Ads._AdPosition = (AdPosition)EditorGUILayout.EnumPopup("Banner Ad Position", _Ads._AdPosition);
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
                    _NativeObject = EditorGUILayout.Foldout(_NativeObject, "Native Object");
                    if (_NativeObject)
                    {
                        _Ads._Native_Icon = EditorGUILayout.ObjectField("Native Icon", _Ads._Native_Icon, typeof(RawImage), true) as RawImage;
                        _Ads._Native_ChoicesIcon = EditorGUILayout.ObjectField("Native Choices Icon", _Ads._Native_ChoicesIcon, typeof(RawImage), true) as RawImage;
                        _Ads._Native_Headline = EditorGUILayout.ObjectField("Native Headline", _Ads._Native_Headline, typeof(Text), true) as Text;
                        _Ads._Native_Body = EditorGUILayout.ObjectField("Native Body", _Ads._Native_Body, typeof(Text), true) as Text;
                        _Ads._Native_CallToAction = EditorGUILayout.ObjectField("Native Call To Action", _Ads._Native_CallToAction, typeof(Text), true) as Text;
                        _Ads._Native_Advertiser = EditorGUILayout.ObjectField("Native Advertiser", _Ads._Native_Advertiser, typeof(Text), true) as Text;
                    }
                    return;
                }
                return;
            }
            return;
        }
        else if (_Ads._Mode == Ads.Mode.Disable)
        {
            if (GUILayout.Button("Edit"))
            {
                _Ads._Mode = Ads.Mode.Enable;
                return;
            }
            return;
        }
    }
}

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
    public override void OnInspectorGUI()
    {
        Ads _ads = target as Ads;

        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("Editor in play mode.");
            return;
        }

        if (_ads._Mode == Ads.Mode.Enable)
        {
            if (GUILayout.Button("Collapse"))
            {
                _ads._Mode = Ads.Mode.Disable;
                return;
            }
            _ads._AutoInitialize = EditorGUILayout.Toggle("Auto Initialize", _ads._AutoInitialize);
            if (_ads._AutoInitialize)
            {
                EditorGUILayout.Space(25);
                _ads._AutoAdRequest = EditorGUILayout.Toggle("Auto Ad Request", _ads._AutoAdRequest);
                if (_ads._AutoAdRequest)
                {
                    EditorGUILayout.LabelField("The recommended ad request minimum value is 5.");
                    _ads._AdRequestTime = EditorGUILayout.FloatField("Ad Request Time", _ads._AdRequestTime);
                }
                EditorGUILayout.Space(25);
                _ads._TestMode = EditorGUILayout.Toggle("Test Mode", _ads._TestMode);
                EditorGUILayout.Space(25);
                _ads._EnableBanner = EditorGUILayout.Toggle("Enable Banner", _ads._EnableBanner);
                if (_ads._EnableBanner)
                {
                    _ads._Android_BannerID = EditorGUILayout.TextField("Android Banner ID", _ads._Android_BannerID);
                    _ads._IOS_BannerID = EditorGUILayout.TextField("iOS Banner ID", _ads._IOS_BannerID);
                    _ads._BannerAdSize = (Ads.BannerAdSize)EditorGUILayout.EnumPopup("Banner Ad Size", _ads._BannerAdSize);
                    _ads._AdPosition = (AdPosition)EditorGUILayout.EnumPopup("Banner Ad Position", _ads._AdPosition);
                }
                EditorGUILayout.Space(25);
                _ads._EnableInterstitial = EditorGUILayout.Toggle("Enable Interstitial", _ads._EnableInterstitial);
                if (_ads._EnableInterstitial)
                {
                    _ads._Android_InterstitialID = EditorGUILayout.TextField("Android Interstitial ID", _ads._Android_InterstitialID);
                    _ads._IOS_InterstitialID = EditorGUILayout.TextField("iOS Interstitial ID", _ads._IOS_InterstitialID);
                }
                EditorGUILayout.Space(25);
                _ads._EnableRewarded = EditorGUILayout.Toggle("Enable Rewarded", _ads._EnableRewarded);
                if (_ads._EnableRewarded)
                {
                    _ads._Android_RewardedID = EditorGUILayout.TextField("Android Rewarded ID", _ads._Android_RewardedID);
                    _ads._IOS_RewardedID = EditorGUILayout.TextField("iOS Rewarded ID", _ads._IOS_RewardedID);
                }
                EditorGUILayout.Space(25);
                _ads._EnableRewardedInterstitial = EditorGUILayout.Toggle("Enable Rewarded Interstitial", _ads._EnableRewardedInterstitial);
                if (_ads._EnableRewardedInterstitial)
                {
                    _ads._Android_RewardedInterstitialID = EditorGUILayout.TextField("Android Rewarded Interstitial ID", _ads._Android_RewardedInterstitialID);
                    _ads._IOS_RewardedInterstitialID = EditorGUILayout.TextField("iOS Rewarded Interstitial ID", _ads._IOS_RewardedInterstitialID);
                }
                EditorGUILayout.Space(25);
                _ads._EnableNative = EditorGUILayout.Toggle("Enable Native", _ads._EnableNative);
                if (_ads._EnableNative)
                {
                    _ads._Android_NativeID = EditorGUILayout.TextField("Android Native ID", _ads._Android_NativeID);
                    _ads._IOS_NativeID = EditorGUILayout.TextField("iOS Native ID", _ads._IOS_NativeID);
                    _ads._Native_Icon = EditorGUILayout.ObjectField("Native Icon", _ads._Native_Icon, typeof(RawImage), true) as RawImage;
                    _ads._Native_ChoicesIcon = EditorGUILayout.ObjectField("Native Choices Icon", _ads._Native_ChoicesIcon, typeof(RawImage), true) as RawImage;
                    _ads._Native_Headline = EditorGUILayout.ObjectField("Native Headline", _ads._Native_Headline, typeof(Text), true) as Text;
                    _ads._Native_Body = EditorGUILayout.ObjectField("Native Body", _ads._Native_Body, typeof(Text), true) as Text;
                    _ads._Native_CallToAction = EditorGUILayout.ObjectField("Native Call To Action", _ads._Native_CallToAction, typeof(Text), true) as Text;
                    _ads._Native_Advertiser = EditorGUILayout.ObjectField("Native Advertiser", _ads._Native_Advertiser, typeof(Text), true) as Text;
                    return;
                }
                return;
            }
            return;
        }
        else if (_ads._Mode == Ads.Mode.Disable)
        {
            if (GUILayout.Button("Edit"))
            {
                _ads._Mode = Ads.Mode.Enable;
                return;
            }
            return;
        }
    }
}

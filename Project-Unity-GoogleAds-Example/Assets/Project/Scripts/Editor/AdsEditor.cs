using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CTJ
{
    [CustomEditor(typeof(Ads)), CanEditMultipleObjects]
    public class AdsEditor : Editor
    {
        private const string Version = "0.0.4";

        [MenuItem("GameObject/CTJ/Create Ads")]
        private static void CreatePrefab()
        {
            string _local_path = "Assets/Project/Prefabs/Ads.prefab";
            var _prefab = AssetDatabase.LoadAssetAtPath(_local_path, typeof(GameObject)) as GameObject;
            var _instance = GameObject.Instantiate<GameObject>(_prefab, null);
        }

        private SerializedProperty _Mode;
        private SerializedProperty _AutoInitialize;
        private SerializedProperty _Android_BannerID;
        private SerializedProperty _Android_InterstitialID;
        private SerializedProperty _Android_RewardedID;
        private SerializedProperty _Android_RewardedInterstitialID;
        private SerializedProperty _Android_NativeID;
        private SerializedProperty _IOS_BannerID;
        private SerializedProperty _IOS_InterstitialID;
        private SerializedProperty _IOS_RewardedID;
        private SerializedProperty _IOS_RewardedInterstitialID;
        private SerializedProperty _IOS_NativeID;
        private SerializedProperty _TestDeviceMode;
        private SerializedProperty _MediationTestSuiteMode;
        private SerializedProperty _EnableTestBanner;
        private SerializedProperty _EnableTestInterstitial;
        private SerializedProperty _EnableTestRewarded;
        private SerializedProperty _EnableTestRewardedInterstitial;
        private SerializedProperty _EnableTestNative;
        private SerializedProperty _AutoAdRequest;
        private SerializedProperty _AdRequestTime;
        private SerializedProperty _EnableBanner;
        private SerializedProperty _BannerAdSize;
        private SerializedProperty _WH;
        private SerializedProperty _BannerAdPosition;
        private SerializedProperty _Pos;
        private SerializedProperty _BannerCallbacks;
        private SerializedProperty _BannerOnAdLoaded;
        private SerializedProperty _BannerOnAdFailedToLoad;
        private SerializedProperty _BannerOnAdOpening;
        private SerializedProperty _BannerOnAdClosed;
        private SerializedProperty _BannerOnAdLeavingApplication;
        private SerializedProperty _BannerOnPaidEvent;
        private SerializedProperty _EnableInterstitial;
        private SerializedProperty _InterstitialCallbacks;
        private SerializedProperty _InterstitialOnAdLoaded;
        private SerializedProperty _InterstitialOnAdFailedToLoad;
        private SerializedProperty _InterstitialOnAdOpening;
        private SerializedProperty _InterstitialOnAdClosed;
        private SerializedProperty _InterstitialOnAdLeavingApplication;
        private SerializedProperty _InterstitialOnPaidEvent;
        private SerializedProperty _EnableRewarded;
        private SerializedProperty _RewardedCallbacks;
        private SerializedProperty _RewardedOnAdLoaded;
        private SerializedProperty _RewardedOnAdFailedToLoad;
        private SerializedProperty _RewardedOnAdFailedToShow;
        private SerializedProperty _RewardedOnAdOpening;
        private SerializedProperty _RewardedOnUserEarnedReward;
        private SerializedProperty _RewardedOnAdClosed;
        private SerializedProperty _RewardedOnPaidEvent;
        private SerializedProperty _EnableRewardedInterstitial;
        private SerializedProperty _RewardedInterstitialCallbacks;
        private SerializedProperty _RewardedInterstitialOnAdDidPresentFullScreenContent;
        private SerializedProperty _RewardedInterstitialOnAdFailedToPresentFullScreenContent;
        private SerializedProperty _RewardedInterstitialOnAdDidDismissFullScreenContent;
        private SerializedProperty _RewardedInterstitialOnUserEarnedReward;
        private SerializedProperty _RewardedInterstitialOnPaidEvent;
        private SerializedProperty _EnableNative;
        private SerializedProperty _InitializeEventFunction;
        private SerializedProperty _NativeCallbacks;
        private SerializedProperty _NativeOnUnifiedNativeAdLoaded;
        private SerializedProperty _NativeOnCustomNativeTemplateAdLoaded;
        private SerializedProperty _NativeOnAdFailedToLoad;
        private SerializedProperty _NativeOnNativeAdClicked;
        private SerializedProperty _NativeOnNativeAdOpening;
        private SerializedProperty _NativeOnNativeAdClosed;
        private SerializedProperty _NativeOnNativeAdImpression;
        private SerializedProperty _NativeOnNativeAdLeavingApplication;

        private void OnEnable()
        {
            _Mode = serializedObject.FindProperty("_Mode");
            _AutoInitialize = serializedObject.FindProperty("_AutoInitialize");
            _Android_BannerID = serializedObject.FindProperty("_Android_BannerID");
            _Android_InterstitialID = serializedObject.FindProperty("_Android_InterstitialID");
            _Android_RewardedID = serializedObject.FindProperty("_Android_RewardedID");
            _Android_RewardedInterstitialID = serializedObject.FindProperty("_Android_RewardedInterstitialID");
            _Android_NativeID = serializedObject.FindProperty("_Android_NativeID");
            _IOS_BannerID = serializedObject.FindProperty("_IOS_BannerID");
            _IOS_InterstitialID = serializedObject.FindProperty("_IOS_InterstitialID");
            _IOS_RewardedID = serializedObject.FindProperty("_IOS_RewardedID");
            _IOS_RewardedInterstitialID = serializedObject.FindProperty("_IOS_RewardedInterstitialID");
            _IOS_NativeID = serializedObject.FindProperty("_IOS_NativeID");
            _TestDeviceMode = serializedObject.FindProperty("_TestDeviceMode");
            _MediationTestSuiteMode = serializedObject.FindProperty("_MediationTestSuiteMode");
            _EnableTestBanner = serializedObject.FindProperty("_EnableTestBanner");
            _EnableTestInterstitial = serializedObject.FindProperty("_EnableTestInterstitial");
            _EnableTestRewarded = serializedObject.FindProperty("_EnableTestRewarded");
            _EnableTestRewardedInterstitial = serializedObject.FindProperty("_EnableTestRewardedInterstitial");
            _EnableTestNative = serializedObject.FindProperty("_EnableTestNative");
            _AutoAdRequest = serializedObject.FindProperty("_AutoAdRequest");
            _AdRequestTime = serializedObject.FindProperty("_AdRequestTime");
            _EnableBanner = serializedObject.FindProperty("_EnableBanner");
            _BannerAdSize = serializedObject.FindProperty("_BannerAdSize");
            _WH = serializedObject.FindProperty("_WH");
            _BannerAdPosition = serializedObject.FindProperty("_BannerAdPosition");
            _Pos = serializedObject.FindProperty("_Pos");
            _BannerCallbacks = serializedObject.FindProperty("_BannerCallbacks");
            _BannerOnAdLoaded = serializedObject.FindProperty("_BannerOnAdLoaded");
            _BannerOnAdFailedToLoad = serializedObject.FindProperty("_BannerOnAdFailedToLoad");
            _BannerOnAdOpening = serializedObject.FindProperty("_BannerOnAdOpening");
            _BannerOnAdClosed = serializedObject.FindProperty("_BannerOnAdClosed");
            _BannerOnAdLeavingApplication = serializedObject.FindProperty("_BannerOnAdLeavingApplication");
            _BannerOnPaidEvent = serializedObject.FindProperty("_BannerOnPaidEvent");
            _EnableInterstitial = serializedObject.FindProperty("_EnableInterstitial");
            _InterstitialCallbacks = serializedObject.FindProperty("_InterstitialCallbacks");
            _InterstitialOnAdLoaded = serializedObject.FindProperty("_InterstitialOnAdLoaded");
            _InterstitialOnAdFailedToLoad = serializedObject.FindProperty("_InterstitialOnAdFailedToLoad");
            _InterstitialOnAdOpening = serializedObject.FindProperty("_InterstitialOnAdOpening");
            _InterstitialOnAdClosed = serializedObject.FindProperty("_InterstitialOnAdClosed");
            _InterstitialOnAdLeavingApplication = serializedObject.FindProperty("_InterstitialOnAdLeavingApplication");
            _InterstitialOnPaidEvent = serializedObject.FindProperty("_InterstitialOnPaidEvent");
            _EnableRewarded = serializedObject.FindProperty("_EnableRewarded");
            _RewardedCallbacks = serializedObject.FindProperty("_RewardedCallbacks");
            _RewardedOnAdLoaded = serializedObject.FindProperty("_RewardedOnAdLoaded");
            _RewardedOnAdFailedToLoad = serializedObject.FindProperty("_RewardedOnAdFailedToLoad");
            _RewardedOnAdFailedToShow = serializedObject.FindProperty("_RewardedOnAdFailedToShow");
            _RewardedOnAdOpening = serializedObject.FindProperty("_RewardedOnAdOpening");
            _RewardedOnUserEarnedReward = serializedObject.FindProperty("_RewardedOnUserEarnedReward");
            _RewardedOnAdClosed = serializedObject.FindProperty("_RewardedOnAdClosed");
            _RewardedOnPaidEvent = serializedObject.FindProperty("_RewardedOnPaidEvent");
            _EnableRewardedInterstitial = serializedObject.FindProperty("_EnableRewardedInterstitial");
            _RewardedInterstitialCallbacks = serializedObject.FindProperty("_RewardedInterstitialCallbacks");
            _RewardedInterstitialOnAdDidPresentFullScreenContent = serializedObject.FindProperty("_RewardedInterstitialOnAdDidPresentFullScreenContent");
            _RewardedInterstitialOnAdFailedToPresentFullScreenContent = serializedObject.FindProperty("_RewardedInterstitialOnAdFailedToPresentFullScreenContent");
            _RewardedInterstitialOnAdDidDismissFullScreenContent = serializedObject.FindProperty("_RewardedInterstitialOnAdDidDismissFullScreenContent");
            _RewardedInterstitialOnUserEarnedReward = serializedObject.FindProperty("_RewardedInterstitialOnUserEarnedReward");
            _RewardedInterstitialOnPaidEvent = serializedObject.FindProperty("_RewardedInterstitialOnPaidEvent");
            _EnableNative = serializedObject.FindProperty("_EnableNative");
            _InitializeEventFunction = serializedObject.FindProperty("_InitializeEventFunction");
            _NativeCallbacks = serializedObject.FindProperty("_NativeCallbacks");
            _NativeOnUnifiedNativeAdLoaded = serializedObject.FindProperty("_NativeOnUnifiedNativeAdLoaded");
            _NativeOnCustomNativeTemplateAdLoaded = serializedObject.FindProperty("_NativeOnCustomNativeTemplateAdLoaded");
            _NativeOnAdFailedToLoad = serializedObject.FindProperty("_NativeOnAdFailedToLoad");
            _NativeOnNativeAdClicked = serializedObject.FindProperty("_NativeOnNativeAdClicked");
            _NativeOnNativeAdOpening = serializedObject.FindProperty("_NativeOnNativeAdOpening");
            _NativeOnNativeAdClosed = serializedObject.FindProperty("_NativeOnNativeAdClosed");
            _NativeOnNativeAdImpression = serializedObject.FindProperty("_NativeOnNativeAdImpression");
            _NativeOnNativeAdLeavingApplication = serializedObject.FindProperty("_NativeOnNativeAdLeavingApplication");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("Ads Version" + " " + Version, MessageType.Info);
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Editor in play mode.", MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            if (_Mode.enumValueIndex == 0)
            {
                if (GUILayout.Button("Collapse", GUILayout.Height(25)))
                {
                    _Mode.enumValueIndex = 1;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.Space(25);
                EditorGUILayout.PropertyField(_AutoInitialize, new GUIContent("Auto Initialize"), true);
                EditorGUILayout.HelpBox("You can access the Ads module API via the Ads class under the CTJ namespace.", MessageType.Info);
                if (_AutoInitialize.boolValue)
                {
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_TestDeviceMode, new GUIContent("Test Device Mode"), true);
                    if (_TestDeviceMode.boolValue)
                    {
                        EditorGUILayout.HelpBox("Test device mode enabled.", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(_MediationTestSuiteMode, new GUIContent("Mediation Test Suite Mode"), true);
                    if (_MediationTestSuiteMode.boolValue)
                    {
                        EditorGUILayout.HelpBox("Mediation Test Suite mode enabled.", MessageType.Warning);
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_AutoAdRequest, new GUIContent("Auto Ad Request"), true);
                    if (_AutoAdRequest.boolValue)
                    {
                        EditorGUILayout.PropertyField(_AdRequestTime, new GUIContent("Ad Request Time"), true);
                        EditorGUILayout.HelpBox("Ad request interval per second.", MessageType.Info);
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_EnableBanner, new GUIContent("Enable Banner"), true);
                    if (_EnableBanner.boolValue)
                    {
                        EditorGUILayout.PropertyField(_EnableTestBanner, new GUIContent("Test Ad Unit ID"), true);
                        if (_EnableTestBanner.boolValue)
                        {
                            EditorGUILayout.HelpBox("Banner ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_Android_BannerID, new GUIContent("Android Banner ID"), true);
                            EditorGUILayout.PropertyField(_IOS_BannerID, new GUIContent("iOS Banner ID"), true);
                        }
                        EditorGUILayout.PropertyField(_BannerAdSize, new GUIContent("Banner Ad Size"), true);
                        if (_BannerAdSize.enumValueIndex == 6)
                        {
                            EditorGUILayout.PropertyField(_WH, new GUIContent("Banner Width & Height"), true);
                            EditorGUILayout.HelpBox("Size in dp (W x H).", MessageType.Info);
                        }
                        EditorGUILayout.PropertyField(_BannerAdPosition, new GUIContent("Banner Ad Position"), true);
                        if (_BannerAdPosition.enumValueIndex == 4)
                        {
                            EditorGUILayout.PropertyField(_Pos, new GUIContent("Banner Position X & Y"), true);
                            EditorGUILayout.HelpBox("The top-left corner of the BannerView will be positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.", MessageType.Info);
                        }
                        _BannerCallbacks.boolValue = EditorGUILayout.Foldout(_BannerCallbacks.boolValue, "Banner Callbacks");
                        if (_BannerCallbacks.boolValue)
                        {
                            EditorGUILayout.PropertyField(_BannerOnAdLoaded, true);
                            EditorGUILayout.PropertyField(_BannerOnAdFailedToLoad, true);
                            EditorGUILayout.PropertyField(_BannerOnAdOpening, true);
                            EditorGUILayout.PropertyField(_BannerOnAdClosed, true);
                            EditorGUILayout.PropertyField(_BannerOnAdLeavingApplication, true);
                            EditorGUILayout.PropertyField(_BannerOnPaidEvent, true);
                        }
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_EnableInterstitial, new GUIContent("Enable Interstitial"), true);
                    if (_EnableInterstitial.boolValue)
                    {
                        EditorGUILayout.PropertyField(_EnableTestInterstitial, new GUIContent("Test Ad Unit ID"), true);
                        if (_EnableTestInterstitial.boolValue)
                        {
                            EditorGUILayout.HelpBox("Interstitial ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_Android_InterstitialID, new GUIContent("Android Interstitial ID"), true);
                            EditorGUILayout.PropertyField(_IOS_InterstitialID, new GUIContent("iOS Interstitial ID"), true);
                        }
                        _InterstitialCallbacks.boolValue = EditorGUILayout.Foldout(_InterstitialCallbacks.boolValue, "Interstitial Callbacks");
                        if (_InterstitialCallbacks.boolValue)
                        {
                            EditorGUILayout.PropertyField(_InterstitialOnAdLoaded, true);
                            EditorGUILayout.PropertyField(_InterstitialOnAdFailedToLoad, true);
                            EditorGUILayout.PropertyField(_InterstitialOnAdOpening, true);
                            EditorGUILayout.PropertyField(_InterstitialOnAdClosed, true);
                            EditorGUILayout.PropertyField(_InterstitialOnAdLeavingApplication, true);
                            EditorGUILayout.PropertyField(_InterstitialOnPaidEvent, true);
                        }
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_EnableRewarded, new GUIContent("Enable Rewarded"), true);
                    if (_EnableRewarded.boolValue)
                    {
                        EditorGUILayout.PropertyField(_EnableTestRewarded, new GUIContent("Test Ad Unit ID"), true);
                        if (_EnableTestRewarded.boolValue)
                        {
                            EditorGUILayout.HelpBox("Rewarded ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_Android_RewardedID, new GUIContent("Android Rewarded ID"), true);
                            EditorGUILayout.PropertyField(_IOS_RewardedID, new GUIContent("iOS Rewarded ID"), true);
                        }
                        _RewardedCallbacks.boolValue = EditorGUILayout.Foldout(_RewardedCallbacks.boolValue, "Rewarded Callbacks");
                        if (_RewardedCallbacks.boolValue)
                        {
                            EditorGUILayout.PropertyField(_RewardedOnAdLoaded, true);
                            EditorGUILayout.PropertyField(_RewardedOnAdFailedToLoad, true);
                            EditorGUILayout.PropertyField(_RewardedOnAdFailedToShow, true);
                            EditorGUILayout.PropertyField(_RewardedOnAdOpening, true);
                            EditorGUILayout.PropertyField(_RewardedOnUserEarnedReward, true);
                            EditorGUILayout.PropertyField(_RewardedOnAdClosed, true);
                            EditorGUILayout.PropertyField(_RewardedOnPaidEvent, true);
                        }
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_EnableRewardedInterstitial, new GUIContent("Enable Rewarded Interstitial"), true);
                    if (_EnableRewardedInterstitial.boolValue)
                    {
                        EditorGUILayout.PropertyField(_EnableTestRewardedInterstitial, new GUIContent("Test Ad Unit ID"), true);
                        if (_EnableTestRewardedInterstitial.boolValue)
                        {
                            EditorGUILayout.HelpBox("Rewarded interstitial ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_Android_RewardedInterstitialID, new GUIContent("Android Rewarded Interstitial ID"), true);
                            EditorGUILayout.PropertyField(_IOS_RewardedInterstitialID, new GUIContent("iOS Rewarded Interstitial ID"), true);
                        }
                        _RewardedInterstitialCallbacks.boolValue = EditorGUILayout.Foldout(_RewardedInterstitialCallbacks.boolValue, "Rewarded Interstitial Callbacks");
                        if (_RewardedInterstitialCallbacks.boolValue)
                        {
                            EditorGUILayout.PropertyField(_RewardedInterstitialOnAdDidPresentFullScreenContent, true);
                            EditorGUILayout.PropertyField(_RewardedInterstitialOnAdFailedToPresentFullScreenContent, true);
                            EditorGUILayout.PropertyField(_RewardedInterstitialOnAdDidDismissFullScreenContent, true);
                            EditorGUILayout.PropertyField(_RewardedInterstitialOnUserEarnedReward, true);
                            EditorGUILayout.PropertyField(_RewardedInterstitialOnPaidEvent, true);
                        }
                    }
                    EditorGUILayout.Space(25);
                    EditorGUILayout.PropertyField(_EnableNative, new GUIContent("Enable Native"), true);
                    if (_EnableNative.boolValue)
                    {
                        EditorGUILayout.HelpBox("Native advanced ads will not show in edit mode.", MessageType.Warning);
                        EditorGUILayout.PropertyField(_EnableTestNative, new GUIContent("Test Ad Unit ID"), true);
                        if (_EnableTestNative.boolValue)
                        {
                            EditorGUILayout.HelpBox("Native ad test unit enabled.", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(_Android_NativeID, new GUIContent("Android Native ID"), true);
                            EditorGUILayout.PropertyField(_IOS_NativeID, new GUIContent("iOS Native ID"), true);
                        }
                        EditorGUILayout.HelpBox("If GameObject objects registered to ad assets are missing Collider components or have an incorrectly configured one, native advanced ads will not operate correctly.", MessageType.Warning);
                        EditorGUILayout.HelpBox("Use script functions to get native advanced ads and registered gameobjects.", MessageType.Info);
                        SerializedProperty _persistent_calls = _InitializeEventFunction.FindPropertyRelative("m_PersistentCalls.m_Calls");
                        if (_persistent_calls.arraySize <= 0) EditorGUILayout.HelpBox("You can only register your custom function by using the following events.", MessageType.Error);
                        else EditorGUILayout.HelpBox("You can only register your custom function by using the following events.", MessageType.Info);
                        EditorGUILayout.PropertyField(_InitializeEventFunction, true);
                        _NativeCallbacks.boolValue = EditorGUILayout.Foldout(_NativeCallbacks.boolValue, "Native Callbacks");
                        if (_NativeCallbacks.boolValue)
                        {
                            EditorGUILayout.PropertyField(_NativeOnUnifiedNativeAdLoaded, true);
                            EditorGUILayout.PropertyField(_NativeOnCustomNativeTemplateAdLoaded, true);
                            EditorGUILayout.PropertyField(_NativeOnAdFailedToLoad, true);
                            EditorGUILayout.PropertyField(_NativeOnNativeAdClicked, true);
                            EditorGUILayout.PropertyField(_NativeOnNativeAdOpening, true);
                            EditorGUILayout.PropertyField(_NativeOnNativeAdClosed, true);
                            EditorGUILayout.PropertyField(_NativeOnNativeAdImpression, true);
                            EditorGUILayout.PropertyField(_NativeOnNativeAdLeavingApplication, true);
                        }
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                serializedObject.ApplyModifiedProperties();
                return;
            }
            else if (_Mode.enumValueIndex == 1)
            {
                if (GUILayout.Button("Edit", GUILayout.Height(25)))
                {
                    _Mode.enumValueIndex = 0;
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
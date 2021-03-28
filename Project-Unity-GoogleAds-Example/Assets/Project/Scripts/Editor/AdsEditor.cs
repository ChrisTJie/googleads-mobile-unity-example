using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CTJ
{
    [CustomEditor(typeof(Ads)), CanEditMultipleObjects]
    public class AdsEditor : Editor
    {
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
        private SerializedProperty _EnableInterstitial;
        private SerializedProperty _EnableRewarded;
        private SerializedProperty _RewardedOnUserEarnedReward;
        private SerializedProperty _EnableRewardedInterstitial;
        private SerializedProperty _UserEarnedRewardCallBack;
        private SerializedProperty _EnableNative;
        private SerializedProperty _InitializeEventFunction;

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
            _EnableInterstitial = serializedObject.FindProperty("_EnableInterstitial");
            _EnableRewarded = serializedObject.FindProperty("_EnableRewarded");
            _RewardedOnUserEarnedReward = serializedObject.FindProperty("_RewardedOnUserEarnedReward");
            _EnableRewardedInterstitial = serializedObject.FindProperty("_EnableRewardedInterstitial");
            _UserEarnedRewardCallBack = serializedObject.FindProperty("_UserEarnedRewardCallBack");
            _EnableNative = serializedObject.FindProperty("_EnableNative");
            _InitializeEventFunction = serializedObject.FindProperty("_InitializeEventFunction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
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
                        EditorGUILayout.PropertyField(_RewardedOnUserEarnedReward, true);
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
                        EditorGUILayout.PropertyField(_UserEarnedRewardCallBack, true);
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
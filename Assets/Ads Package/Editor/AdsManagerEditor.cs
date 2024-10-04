using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AdsManagerEditorNameSpace
{
    [CustomEditor(typeof(AdsManager)), CanEditMultipleObjects]
    public class AdsManagerEditor : Editor
    {
        private SerializedProperty _accountState;
        private SerializedProperty _customPrivacyPolicy;
        
        //Banner
        private SerializedProperty _bannerPlacement;
        private SerializedProperty _bannerInitState;
        private SerializedProperty _bannerType;
        private SerializedProperty _bannerNetworkType;
        
        //Interstitial
        private SerializedProperty _interstitialNetworkType;
        
        //Rewarded
        private SerializedProperty _rewardedNetworkType;
        
        //AppOpen
        private SerializedProperty _appOpenNetworkType;
        
        private SerializedProperty _splashSceneSwitch;
        private SerializedProperty _splashSceneSwitcher;
        private SerializedProperty _noRewardAdPanel;

        //Admob
        private SerializedProperty _admobAdPlacements;
        private SerializedProperty _admobAppOpenID;
        private SerializedProperty _admobRewardedID;
        private SerializedProperty _admobBannerID;
        private SerializedProperty _admobInterstitialID;
        private SerializedProperty _enableAdmobTestAds;
        
        private Color _statusColor = Color.red;
        
        private AdsManager _instance;
        
        private static GUIStyle _guiStyle = new GUIStyle();

        private static AdsManager GetInstance()
        {
            return FindObjectOfType<AdsManager>();
        }
        
        private void OnEnable()
        {
            _accountState = serializedObject.FindProperty("accountName");
            
            //Banner
            _bannerInitState = serializedObject.FindProperty("bannerInitState");
            _bannerPlacement = serializedObject.FindProperty("bannerPlacement");
            _bannerType = serializedObject.FindProperty("bannerType");
            _bannerNetworkType = serializedObject.FindProperty("bannerNetworkType");
            
            //Interstitial
            _interstitialNetworkType = serializedObject.FindProperty("interstitialNetworkType");
            
            //Rewarded
            _rewardedNetworkType = serializedObject.FindProperty("rewardedNetworkType");
            
            //AppOpen
            _appOpenNetworkType = serializedObject.FindProperty("appOpenNetworkType");

            _splashSceneSwitch = serializedObject.FindProperty("splashSceneSwitch");
            _splashSceneSwitcher = serializedObject.FindProperty("splashSceneSwitchTime");
            _noRewardAdPanel = serializedObject.FindProperty("noRewardAdPanel");

#if Admob
            _admobBannerID = serializedObject.FindProperty("admobBannerID");
            _admobInterstitialID = serializedObject.FindProperty("admobInterstitialID");
            _admobRewardedID = serializedObject.FindProperty("admobRewardedID");
            _admobAppOpenID = serializedObject.FindProperty("admobAppOpenID");
            _admobAdPlacements = serializedObject.FindProperty("admobAdPlacement");
            _enableAdmobTestAds = serializedObject.FindProperty("enableAdmobTestAds");
#endif

            _instance = GetInstance();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.backgroundColor = _statusColor;
            GUI.contentColor = _statusColor;
            AddSpace(5);
            AddDivider("Account Properties");
            
            EditorGUILayout.PropertyField(_accountState);
            var accountName = (AdsManager.Account)_accountState.enumValueIndex;
            _statusColor = accountName != AdsManager.Account.None ? Color.cyan : Color.red;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            AddTitle("Banner Properties");
            EditorGUILayout.PropertyField(_bannerInitState);
            var bannerInitState = (AdsManager.BannerInitState)_bannerInitState.enumValueIndex;
            var adsManagerObject = (AdsManager)target;
            switch (bannerInitState)
            {
                case AdsManager.BannerInitState.AutoInitialize:
                    EditorGUILayout.PropertyField(_bannerPlacement);
                    EditorGUILayout.PropertyField(_bannerType);
                    EditorGUILayout.PropertyField(_bannerNetworkType);
                    break;
                case AdsManager.BannerInitState.ManualInitialize:
                    EditorGUILayout.PropertyField(_bannerPlacement);
                    EditorGUILayout.PropertyField(_bannerType);
                    EditorGUILayout.PropertyField(_bannerNetworkType);
                    break;
                case AdsManager.BannerInitState.Disable:
                    adsManagerObject.bannerNetworkType = AdsManager.NetworkType.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            AddSpace(10);
            AddTitle("Interstitial Properties");
            EditorGUILayout.PropertyField(_interstitialNetworkType);

            AddSpace(10);
            AddTitle("Rewarded Properties");
            EditorGUILayout.PropertyField(_rewardedNetworkType);
            
            AddSpace(10);
            AddTitle("AppOpen Properties");
            EditorGUILayout.PropertyField(_appOpenNetworkType);
            
            AddSpace(10);
            AddDivider("Splash Properties");
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            EditorGUILayout.PropertyField(_splashSceneSwitch);
            var splashSceneSwitcher = (AdsManager.SplashSceneSwitcher)_splashSceneSwitch.enumValueIndex;
            
            switch (splashSceneSwitcher)
            {
                case AdsManager.SplashSceneSwitcher.Enable:
                    EditorGUILayout.PropertyField(_splashSceneSwitcher);
                    break;
                case AdsManager.SplashSceneSwitcher.Disable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EditorGUILayout.PropertyField(_noRewardAdPanel);

#if Admob
            AddSpace(10);
            AddDivider("Admob Properties");
            EditorGUILayout.PropertyField(_admobAdPlacements);
            var admobAdPlacement = (AdsManager.AdmobAdPlacements)_admobAdPlacements.enumValueIndex;
            var adsManager = (AdsManager)target;
            switch (admobAdPlacement)
            {
                case AdsManager.AdmobAdPlacements.AppOpenPlacement:
                    EditorGUILayout.PropertyField(_admobAppOpenID);
                    adsManager.admobBannerID = "";
                    adsManager.admobInterstitialID = new List<string>();
                    adsManager.admobRewardedID = new List<string>();
                    
                    AddCustomDivider("Be Careful! Don't Ship game with this tick enable", Color.gray, 12, false);
                    EditorGUILayout.PropertyField(_enableAdmobTestAds);
                    break;
                case AdsManager.AdmobAdPlacements.AllPlacements:
                    EditorGUILayout.PropertyField(_admobBannerID);
                    EditorGUILayout.PropertyField(_admobInterstitialID);
                    EditorGUILayout.PropertyField(_admobRewardedID);
                    EditorGUILayout.PropertyField(_admobAppOpenID);
                    
                    AddCustomDivider("Be Careful! Don't Ship game with this tick enable", Color.gray, 12, false);
                    EditorGUILayout.PropertyField(_enableAdmobTestAds);
                    break;
                case AdsManager.AdmobAdPlacements.None:
                    adsManager.admobBannerID = "";
                    adsManager.admobInterstitialID = new List<string>();
                    adsManager.admobRewardedID = new List<string>();
                    adsManager.admobAppOpenID = new List<string>();
                    break;
                case AdsManager.AdmobAdPlacements.RewardedPlacement:
                    EditorGUILayout.PropertyField(_admobRewardedID);
                    adsManager.admobBannerID = "";
                    adsManager.admobInterstitialID = new List<string>();
                    adsManager.admobAppOpenID = new List<string>();
                    
                    AddCustomDivider("Be Careful! Don't Ship game with this tick enable", Color.gray, 12, false);
                    EditorGUILayout.PropertyField(_enableAdmobTestAds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
#endif
 
            serializedObject.ApplyModifiedProperties();
        }

        //Editor Visual Helper Methods
        private static void AddDivider(string dividerName)
        {
            _guiStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal =
                {
                    textColor = Color.yellow
                },
                fontSize = 15
            };
            //Divider
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            AddSpace(10);
            GUILayout.Label(dividerName, _guiStyle);
            AddSpace(10);
        }
        
        private static void AddTitle(string dividerName)
        {
            _guiStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal =
                {
                    textColor = Color.gray
                },
                fontSize = 12
            };
            AddSpace(10);
            GUILayout.Label(dividerName, _guiStyle);
        }
        
        private static void AddCustomDivider(string dividerName, Color textColor, int mFontSize, bool isDrawDivider)
        {
            _guiStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                normal =
                {
                    textColor = textColor
                },
                fontSize = mFontSize
            };
            
            if (isDrawDivider)
            {
                //Divider
                var rect = EditorGUILayout.BeginHorizontal();
                Handles.color = Color.gray;

                Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
                EditorGUILayout.EndHorizontal();
            }

            AddSpace(10);
            GUILayout.Label(dividerName, _guiStyle);
            AddSpace(10);
        }
        
        private static void AddSpace(int spaceMargin)
        {
            GUILayout.Space(spaceMargin);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    #region Enums

    public enum Account
    {
        GooglePlay,
        AppStore,
        Amazon,
        None
    }

    public enum SplashSceneSwitcher
    {
        Enable,
        Disable
    }

    public enum BannerPlacement
    {
        Top,
        Bottom
    }

    public enum BannerInitState
    {
        AutoInitialize,
        ManualInitialize,
        Disable
    }
    
    public enum BannerType
    {
        Adaptive,
        Banner
    }

    public enum NetworkType
    {
        None,
        
#if Admob
        Admob,
#endif
    }

    #endregion

    #region AdNetwork Specific Attributes

#if Admob
    public enum AdmobAdPlacements
    {
        AppOpenPlacement,
        RewardedPlacement,
        AllPlacements,
        None
    }

    public string admobBannerID;
    public List<string> admobInterstitialID;
    public List<string> admobRewardedID;
    public List<string> admobAppOpenID = new List<string>();
    public AdmobAdPlacements admobAdPlacement = AdmobAdPlacements.None;
    public bool enableAdmobTestAds;
#endif
    #endregion

    #region Local Variables

    public Account accountName = Account.None;

    //Banner Fields
    [Tooltip("Select Banner shown state")] public BannerInitState bannerInitState = BannerInitState.AutoInitialize;

    [Tooltip("Select Banner Placement where banner needs to be shown")]
    public BannerPlacement bannerPlacement = BannerPlacement.Bottom;

    [Tooltip("Select Banner Placement where banner needs to be shown")]
    public BannerType bannerType = BannerType.Banner;
    
    [Tooltip("Select Banner type to be Shown")]
    public NetworkType bannerNetworkType = NetworkType.None;

    //Interstitial Fields
    [Tooltip("Select Interstitial type to be Shown")]
    public NetworkType interstitialNetworkType = NetworkType.None;

    //Rewarded Fields
    [Tooltip("Select Rewarded type to be Shown")]
    public NetworkType rewardedNetworkType = NetworkType.None;

    //AppOpen Fields
    [Tooltip("Select AppOpen type to be Shown")]
    public NetworkType appOpenNetworkType = NetworkType.None;

    public bool isReadyToShowAppOpen = true;
    public bool isSceneSwitchedForAppOpen;
    public Action OnLoadSceneOnAppOpenCloseOnStart;
    public Action StartLoadingNextScene;
    private bool _checkSceneIsAlreadyInLoading;

    //Splash Scene
    public SplashSceneSwitcher splashSceneSwitch = SplashSceneSwitcher.Disable;
    [Range(9, 12)] public int splashSceneSwitchTime = 9;
    public GameObject noRewardAdPanel;

    private List<IAdsProvider> _adsProviders = new List<IAdsProvider>();
    private int _userAge;
    private bool _isAllAdsProviderAdded;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializingAds();
            return;
        }

        Destroy(gameObject);
    }

    private void InitializingAds()
    {
        //For App open
        isReadyToShowAppOpen = true;

        StartLoadingNextScene += LoadMainScene;
        OnLoadSceneOnAppOpenCloseOnStart += LoadSceneOnAppOpenCloseOnStart;

        InitializeAds();
    }

    private void InitializeAds()
    {
        if (!_isAllAdsProviderAdded)
        {
#if Admob
            _adsProviders.Add(gameObject.AddComponent<AdMobAdsProvider>());
#endif
            _isAllAdsProviderAdded = true;

            //Initialize the ads from all the ads provider
            foreach (var adsProvider in _adsProviders)
            {
                adsProvider.Initialize();
            }
        }
    }

    #region Scene Switch

    private void LoadMainScene()
    {
        if (_checkSceneIsAlreadyInLoading)
        {
            return;
        }

        _checkSceneIsAlreadyInLoading = true;

        //Switch the splash scene
        if (splashSceneSwitch == SplashSceneSwitcher.Enable)
        {
            Invoke(nameof(LoadScene), splashSceneSwitchTime);
        }
    }

    private void LoadScene()
    {
        Debug.Log("Next Scene Loaded...");
        if (SceneManager.sceneCountInBuildSettings > 1)
            SceneManager.LoadScene(sceneBuildIndex: 1);
        isSceneSwitchedForAppOpen = true;

#if Admob
        if (TryGetComponent<AdMobAdsProvider>(out var admobProvider))
        {
            admobProvider.LoadAdsPlacements();
        }
#endif
        
    }
    
    private void LoadSceneOnAppOpenCloseOnStart()
    {
        if (splashSceneSwitch != SplashSceneSwitcher.Enable) return;

        CancelInvoke(nameof(LoadScene));
        LoadScene();
    }

    #endregion

    #region Banner

    public void ShowBanner(Action<string> onComplete = null, Action<string> onSkipped = null,
        Action<string> onFailed = null)
    {
        if (CheckIsRemoveAdsBought())
        {
            Debug.Log("Remove Ad Bought");
            return;
        }

        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }

        foreach (var adsProvider in _adsProviders)
        {
            if (adsProvider.CheckBannerIsAvailable())
            {
                adsProvider.ShowBannerAd(onComplete, onSkipped, onFailed);
                Debug.Log($"AdsManager : Showing Banner Ad... {adsProvider}");
                break;
            }
            else
            {
                adsProvider.LoadBannerAD();
                Debug.Log($"AdsManager : loading Banner Ad... {adsProvider}");
            }
        }
    }
    
    public void LoadBannerAd(IAdsProvider adProvider)
    {
        foreach (var adsProvider in _adsProviders)
        {
            if (adsProvider == adProvider)
            {
                Debug.Log($"discarding the banner for  : {adsProvider}");
                continue;
            }

            if (bannerInitState == BannerInitState.AutoInitialize)
                adsProvider.LoadBannerAD();
        }
    }

    public void DestroyBanner()
    {
        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }

        foreach (var adsProvider in _adsProviders)
        {
            adsProvider.DestroyBanner();
        }

        Debug.Log("Destroy Banner");
    }

    public void HideBanner()
    {
        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }

        foreach (var adsProvider in _adsProviders)
        {
            adsProvider.HideBanner();
        }
    }

    public void ShowHideBanner()
    {
        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }

        foreach (var adsProvider in _adsProviders)
        {
            adsProvider.ShowHideBanner();
        }
    }
    
    public bool CheckBannerIsAvailable(IAdsProvider adProvider = null)
    {
        foreach (var adsProvider in _adsProviders)
        {
            if (adsProvider == adProvider) continue;

            if (adsProvider.CheckBannerIsAvailable())
                return true;
        }

        return false;
    }
    
    public void ShowBannerAtTop()
    {
        //Destroy the old banner
        DestroyBanner();

        //change the banner placement
        bannerPlacement = BannerPlacement.Top;

        //Show the new banner
        ShowBanner();
    }

    public void ShowBannerAtBottom()
    {
        //Destroy the old banner
        DestroyBanner();

        //change the banner placement
        bannerPlacement = BannerPlacement.Bottom;

        //Show the new banner
        ShowBanner();
    }

    #endregion

    #region Interstitial

    public void ShowInterstitial(Action<string> onComplete = null, Action<string> onSkipped = null,
        Action<string> onFailed = null, string ironSourcePlacement = null)
    {
        if (CheckIsRemoveAdsBought()) return;

        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }
#if IronSource
        this.ironSourcePlacementID = ironSourcePlacement;
#endif
        foreach (var adsProvider in _adsProviders.Where(adsProvider => adsProvider.CheckInterstitialIsAvailable()))
        {
            adsProvider.ShowInterstitialAd(onComplete, onSkipped, onFailed);
            return;
        }
    }
    
    public bool CheckInterstitialIsAvailable()
    {
        if (accountName != Account.None)
        {
            foreach (var adsProvider in _adsProviders)
            {
                if(adsProvider.CheckInterstitialIsAvailable())
                    return true;
            }

            return false;
        }

        return false;
    }

    #endregion

    #region Reward

    public void ShowRewardAd(Action<string> onComplete, Action<string> onSkipped = null, Action<string> onFailed = null)
    {
        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }
        foreach (var adsProvider in _adsProviders.Where(adsProvider => adsProvider.CheckRewardIsAvailable()))
        {
            adsProvider.ShowRewardedAd(onComplete, onSkipped, onFailed);
            return;
        }
        
        //Show No Reward Ad Panel
        if (noRewardAdPanel == null) return;
        
        var noReward = Instantiate(noRewardAdPanel, transform);
        Destroy(noReward, 1.5f);
    }
    
    public bool CheckRewardIsAvailable()
    {
        if (accountName != Account.None)
        {
            foreach (var adsProvider in _adsProviders)
            {
                if (adsProvider.CheckRewardIsAvailable())
                    return true;
            }

            return false;
        }

        Debug.LogError("No Account Selected!");
        return false;
    }

    #endregion

    #region IAP

    public bool CheckIsRemoveAdsBought()
    {
        return PlayerPrefs.GetInt("RemoveAds", 0) > 0;
    }

    public void OnRemoveADBought()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        DestroyBanner();
    }
    
    private void OnRemoveAdBoughtSceneSwitch()
    {
        if (SceneManager.sceneCountInBuildSettings > 1)
            SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    #endregion

    #region Consent

    public void OnConsentUser(bool consentStatus, int userAge)
    {
        if (!consentStatus) return;

        if (accountName == Account.None)
        {
            Debug.LogError("No Account Selected!");
            return;
        }

        PlayerPrefs.SetInt("consent", 1);
        _userAge = userAge;
        InitializeAds();
    }

    public int GetUserCurrentAge()
    {
        return _userAge;
    }

    #endregion
}
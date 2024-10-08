using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhantomAds : MonoBehaviour
{
    public enum BannerPlacement
    {
        Top,
        Bottom
    }

    private const int SplashSceneSwitchTime = 12;
    [Header("Admob IDs")]
    public BannerPlacement bannerPlacement = BannerPlacement.Bottom;
    public string bannerID;
    public string interstitialID;
    public string rewardID;
    public string appOpenID;

    [Space(10)] [Header("Don't Ship with Test check enable")] [Space(3)]
    public bool enableTestAds;
    
    //Interstitial AD Callback
    private Action _onCompleteInterstitialAdCallBack;
    
    //Reward AD Callback
    private Action _onCompleteRewardedAdCallBack;
    
    //Ad Views
    private BannerView _bannerView;
    private BannerView _rectBannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private AppOpenAd _appOpenAd;

    //Local Variables
    private bool _appOpenAdShownOnFirstOpen;
    private bool _isSceneSwitched;
    private bool _isAppOpenAdReadyToShow;
    private float _interstitialRetryAttempt;
    private float _rewardRetryAttempt;
    private float _appOpenRetryAttempt;
    
    // App open ads can be preloaded for up to 4 hours.
    private readonly TimeSpan _timeout = TimeSpan.FromHours(4);
    private DateTime _expireTime;
    
    public static PhantomAds Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            return;
        }
        
        Destroy(gameObject);
    }
    
    private void Start()
    {
        //Initialize the ads
        InitializeAds();
        
        //Start Switching the main scene from splash 
        StartLoadingMainScene();
    }

    private void StartLoadingMainScene()
    {
        if (SceneManager.sceneCount > 1)
        {
            Invoke(nameof(LoadMainScene), SplashSceneSwitchTime);
        }
    }
    
    private void LoadMainScene()
    {
        _isSceneSwitched = true;
        
        SceneManager.LoadScene(1);

        //Show Banner On Next Scene
        Invoke(nameof(LoadBannerAd), 3);
    }

    private void InitializeAds()
    {
#if UNITY_EDITOR
        InitializeSDK();
#else
        var request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
    }
    
    //Consent For EU User
    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            Debug.LogError($"Error from OnConsentInfoUpdated Method start : {consentError.Message}");
            
            //Check the platform and initialize SDK accordingly

            #region Initializing SDK

#if UNITY_ANDROID
            //Initialize and show Ads
            InitializeSDK();
       
#elif UNITY_IOS
                //Show iOS Consent
                if (TryGetComponent<ATTConsent>(out var atTrackingObject))
                {
                    atTrackingObject.ShowIOSNativeConsent();
                }
                
                Invoke(nameof(InitializeSDK), 3);   
#endif

            #endregion
            
            return;
        }
        
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                Debug.LogError($"Error From Load and Show Form Method: {formError.Message}");
                
                #region Initializing SDK

#if UNITY_ANDROID
                //Initialize and show Ads
                InitializeSDK();
       
#elif UNITY_IOS
                //Show iOS Consent
                if (TryGetComponent<ATTConsent>(out var atTrackingObject))
                {
                    atTrackingObject.ShowIOSNativeConsent();
                }
                
                Invoke(nameof(InitializeSDK), 3);   
#endif

                #endregion
                
                return;
            }
        
            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                #region Initializing SDK

#if UNITY_ANDROID
                //Initialize and show Ads
                InitializeSDK();
       
#elif UNITY_IOS
                //Show iOS Consent
                if (TryGetComponent<ATTConsent>(out var atTrackingObject))
                {
                    atTrackingObject.ShowIOSNativeConsent();
                }
                
                Invoke(nameof(InitializeSDK), 3);   
#endif

                #endregion
            }
        });
    }

    private void InitializeSDK()
    {
        // On Android, Unity is paused when displaying interstitial or rewarded video.
        // This setting makes iOS behave consistently with Android.
        MobileAds.SetiOSAppPauseOnBackground(true);

        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Configure TagForChildDirectedTreatment and test device IDs.
        var requestConfiguration =
            new RequestConfiguration();
        
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        Debug.Log("Admob Initializing.");
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            if (initStatus == null)
            {
                Debug.LogError("Admob initialization failed.");
                
                return;
            }
            
            Debug.Log("Admob initialization Successful.");

            //Loading Ads
            LoadAppOpenAd();
            LoadInterstitialAd();
            LoadRewardAd();
        });
    }

    #region Interstitial
    
    private void LoadInterstitialAd()
    {
        //Checking if the interstitial is already loaded and can show
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Interstitial Ad already loaded and ready to show...");
            return;
        }
        
        // Clean up the old ad before loading a new one.
        _interstitialAd?.Destroy();

        Debug.Log("Loading Admob interstitial ad.");

        //Check whether test ads are enabled
        var interstitialId = enableTestAds
            ? "ca-app-pub-3940256099942544/1033173712"
            : interstitialID;
        
        // Send the request to load the ad.
        InterstitialAd.Load( interstitialId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError($"Interstitial ad failed to load an ad with error : {error}");
                _interstitialRetryAttempt++;
                var retryDelay = Math.Pow(2, Math.Min(3, _interstitialRetryAttempt));
                Invoke(nameof(LoadInterstitialAd), (float)retryDelay);
                return;
            }
            
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                _interstitialRetryAttempt++;
                var retryDelay = Math.Pow(2, Math.Min(3, _interstitialRetryAttempt));
                Invoke(nameof(LoadInterstitialAd), (float)retryDelay);
                return;
            }
            
            // The operation completed successfully.
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            _interstitialAd = ad;

            // Register to ad events to extend functionality.
            RegisterInterstitialEventHandlers(ad);
        });
    }
    
    public bool CheckInterstitialIsAvailable()
    {
        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }
    
    public void ShowInterstitialAd(Action onCompleteCallBack = null)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _isAppOpenAdReadyToShow = false;
            
            _onCompleteInterstitialAdCallBack = onCompleteCallBack;
            Debug.Log("Showing Interstitial AD");
            _interstitialAd.Show();
        }
    }
    
    private void RegisterInterstitialEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            _onCompleteInterstitialAdCallBack?.Invoke();
            
            _onCompleteInterstitialAdCallBack = null;

            //Load the next Interstitial Ad
            Invoke(nameof(LoadInterstitialAd), 1f);
            
            //Make the variable for app open ready to show
            Invoke(nameof(AppOpenAdReadyToShow), 1f);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                           + error);
            //Load the Interstitial Ad
            Invoke(nameof(LoadInterstitialAd), 1f);
        };
    }
    
    #endregion
    
    #region AppOpen

    public void LoadAppOpenAd()
    {
        // Clean up the old ad before loading a new one.
        if (_appOpenAd != null)
        {
            Debug.Log("Destroying app open ad to load new one.");
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }

        Debug.Log("Loading app open ad.");
        
        //Check Whether the test check is enable
        var appOpenId = enableTestAds
            ? "ca-app-pub-3940256099942544/9257395921"
            : appOpenID;
        
        // Send the request to load the ad.
        AppOpenAd.Load(appOpenId, new AdRequest(), (appOpenAd, error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("App open ad failed to load an ad with error : "
                                   + error);
                    _appOpenRetryAttempt++;
                    var retryDelay = Math.Pow(2, Math.Min(5, _appOpenRetryAttempt));
                    Invoke(nameof(LoadAppOpenAd), (float)retryDelay);
                    return;
                }
                
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (appOpenAd == null)
                {
                    Debug.LogError("Unexpected error: App open ad load event fired with " +
                                   " null ad and null error.");
                    _appOpenRetryAttempt++;
                    var retryDelay = Math.Pow(2, Math.Min(5, _appOpenRetryAttempt));
                    Invoke(nameof(LoadAppOpenAd), (float)retryDelay);
                    return;
                }
                
                // The operation completed successfully.
                Debug.Log("App open ad loaded with response : " + appOpenAd.GetResponseInfo());
                _appOpenAd = appOpenAd;
                
                // App open ads can be preloaded for up to 4 hours.
                _expireTime = DateTime.Now + _timeout;
                
                // Register to ad events to extend functionality.
                RegisterAppOpenADEventHandlers(appOpenAd);
                
                if (!_appOpenAdShownOnFirstOpen && !_isSceneSwitched)
                {
                    ShowAppOpenAD();

                    _appOpenAdShownOnFirstOpen = true;
                }
            });
    }
    
    private void ShowAppOpenAD()
    {
        if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
        {
            _isAppOpenAdReadyToShow = false;
            Debug.Log("Showing App Open AD");
            HideBanner();
            _appOpenAd.Show();
        }
        else
        {
            LoadAppOpenAd();
            Debug.LogError("App open ad is not ready yet.");
        }
    }
    
    private void RegisterAppOpenADEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"App open ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");

            // It may be useful to load a new ad when the current one is complete.
            Invoke(nameof(AppOpenAdReadyToShow), 1f);
            LoadAppOpenAd();

            if (_isSceneSwitched) return;
            
            CancelInvoke(nameof(StartLoadingMainScene));
            LoadMainScene();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content with error : "
                           + error);
        };
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_ANDROID
        if (hasFocus && _isAppOpenAdReadyToShow)
        {
            ShowAppOpenAD();
        }
#endif
    }

    private void AppOpenAdReadyToShow()
    {
        Debug.Log($"Activating App Open Check");
        _isAppOpenAdReadyToShow = true;
    }

    #endregion
    
    #region Reward

    private void LoadRewardAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        
        Debug.Log("Loading rewarded ad.");
        
        //Check whether test ads are enabled
        var rewardId = enableTestAds
            ? "ca-app-pub-3940256099942544/5224354917"
            : rewardID;
        
        // Send the request to load the ad.
        RewardedAd.Load(rewardId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                _rewardRetryAttempt++;
                var retryDelay = Math.Pow(2, Math.Min(4, _rewardRetryAttempt));
                Invoke(nameof(LoadRewardAd), (float)retryDelay);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                _rewardRetryAttempt++;
                var retryDelay = Math.Pow(2, Math.Min(4, _rewardRetryAttempt));
                Invoke(nameof(LoadRewardAd), (float)retryDelay);
                return;
            }
            
            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;
            // Register to ad events to extend functionality.
            RegisterRewardedADEventHandlers(ad);
            
        });
    }
    
    public bool CheckRewardIsAvailable()
    {
        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }
    
    public void ShowRewardedAd(Action onCompleteCallBack = null)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _isAppOpenAdReadyToShow = false;

            _rewardedAd.Show((Reward reward) =>
            {
                onCompleteCallBack?.Invoke();
                
                //Load The Next reward
                Invoke(nameof(LoadRewardAd), 1f);
                
                //Make the variable for app open ready to show
                Invoke(nameof(AppOpenAdReadyToShow), 1f);
            });
            
            return;
        }
        
        Debug.LogError("Rewarded ad is not ready yet.");
    }
    
    private void RegisterRewardedADEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                           + error);
            Invoke(nameof(LoadRewardAd), 2f);
        };
    }
    
    #endregion

    #region Banner

    public void LoadBannerAd()
    {
        //Check Whether the test check is enable
        var bannerId = enableTestAds
            ? "ca-app-pub-3940256099942544/6300978111"
            : bannerID;

        if (_bannerView == null)
        {
            // If we already have a banner, destroy the old one.
            if(_bannerView != null)
            {
                Debug.Log("Destroying banner view to load new ad.");
                _bannerView.Destroy();
                _bannerView = null;
            }
            
            AdPosition adPosition;
            
            switch (bannerPlacement)
            {
                case BannerPlacement.Top:
                    adPosition = AdPosition.Top;
                    break;
                case BannerPlacement.Bottom:
                default:
                    adPosition = AdPosition.Bottom;
                    break;
            }

            var adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            _bannerView = new BannerView(bannerId, AdSize.Banner, adPosition);
           
            // Listen to events the banner may raise.
            RegisterBannerEventHandlers();
            Debug.Log("Banner view created.");
        }

        // Load a banner ad
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(new AdRequest());
    }
    
    private void ShowBannerAd()
    {
        if (_bannerView == null)
        {
            LoadBannerAd();
            return;
        }
        
        Debug.Log("Showing banner Ad now ....");
        _bannerView.Show();
    }

    private void HideBanner()
    {
        Debug.Log("Hiding banner view.");
        _bannerView?.Hide();
    }

    public void ShowHideBanner()
    {
        _bannerView?.Show();
    }

    public void DestroyBanner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView?.Destroy();
            _bannerView = null;
        }
    }
    
    private void RegisterBannerEventHandlers()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                      + _bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
            Invoke(nameof(LoadBannerAd), 2f);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion
}

using System;
#if Admob
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class AdMobAdsProvider : MonoBehaviour,IAdsProvider
{
    //Banner AD Callback
    private Action<string> _onCompleteBannerAdCallBack;
    private Action<string> _onFailedBannerAdCallBack;
    private Action<string> _onSkippedBannerAdCallBack;
    
    //Interstitial AD Callback
    private Action<string> _onCompleteInterstitialAdCallBack;
    private Action<string> _onFailedInterstitialAdCallBack;
    private Action<string> _onSkippedInterstitialAdCallBack;
    
    //Reward AD Callback
    private Action<string> _onCompleteRewardedAdCallBack;
    private Action<string> _onFailedRewardedAdCallBack;
    private Action<string> _onSkippedRewardedAdCallBack;
    
    private BannerView _bannerView;
    private BannerView _rectBannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private AppOpenAd _appOpenAd;

    private bool _showAppOpenOnFirstOpen;
    private int _appOpenTierIndex;
    private int _interstitialTierIndex;
    private int _bannerTierIndex;
    private int _rewardTierIndex;

    private bool _isBannerLoaded;
    private bool _isInitialized;
#if AdmobNative
    private NativeAd _nativeAd;
#endif
    private bool _isNativeAdLoaded;
    private bool _checkBannerStateForManual = false;
    private bool _firstTimeAppOpenAdShow;

    public void Initialize()
    {
        if (AdsManager.Instance.enableAdmobTestAds)
            Debug.Log("<color=red> Test Ads Enabled for Admob</color>");
#if UNITY_EDITOR
        InitializeAds();
#else
        var request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            Debug.LogError($"Error from OnConsentInfoUpdated Method start : {consentError.Message}");
            
#if UNITY_IOS
            //Show iOS Consent
            if (TryGetComponent<ATTConsent>(out var atTrackingObject))
            {
                atTrackingObject.ShowIOSNativeConsent();
            }
#endif
            Invoke(nameof(InitializeAds), 3);
            return;
        }
        
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                Debug.LogError($"Error From Load and Show Form Method: {consentError.Message}");
                
#if UNITY_IOS
                //Show iOS Consent
                if (TryGetComponent<ATTConsent>(out var atTrackingObject))
                {
                    atTrackingObject.ShowIOSNativeConsent();
                }   
#endif
                Invoke(nameof(InitializeAds), 3);
                return;
            }
        
            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
#if UNITY_ANDROID
                //Initialize and show Ads
                InitializeAds();
#endif

#if UNITY_IOS
                //Show iOS Consent
                if (TryGetComponent<ATTConsent>(out var atTrackingObject))
                {
                    atTrackingObject.ShowIOSNativeConsent();
                }
                
                Invoke(nameof(InitializeAds), 3);   
#endif
            }
        });
    }

    private void InitializeAds()
    {
         #region Admob

         if (_isInitialized) return;

         //Start Loading Next Scene
         AdsManager.Instance.StartLoadingNextScene?.Invoke();
         
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
        Debug.Log("Google Mobile Ads Initializing.");
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            if (initStatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                
                return;
            }

            _isInitialized = true;
            Debug.Log("Google Mobile Ads initialization complete.");

            //Loading ADs Placements
            switch (AdsManager.Instance.admobAdPlacement)
            {
                case AdsManager.AdmobAdPlacements.AppOpenPlacement:
                    if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;
                    LoadAppOpenAD();
                    break;
                case AdsManager.AdmobAdPlacements.AllPlacements:
                    LoadRewardedAD();
                    if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;
                    LoadInterstitialAD();
                    LoadAppOpenAD();
                    break;
                case AdsManager.AdmobAdPlacements.None:
                    break;
                case AdsManager.AdmobAdPlacements.RewardedPlacement:
                    LoadRewardedAD();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });

        #endregion
    }

    public bool CheckADsInitialized()
    {
        return _isInitialized;
    }

    public void LoadAdsPlacements()
    {
        if (!_isInitialized) return;
        
        //Loading ADs Placements
        switch (AdsManager.Instance.admobAdPlacement)
        {
            case AdsManager.AdmobAdPlacements.AllPlacements:
                if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;
                if (AdsManager.Instance.bannerInitState != AdsManager.BannerInitState.Disable)
                    LoadBannerAD();
                break;
        }
    }
    
    private static AdRequest CreateAdRequest()
    {
        return new AdRequest();
    }

    #region BannerAD

    public void LoadBannerAD()
    {
        if (!CheckBannerToShow()) return;
        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;

        //Check Whether the test check is enable
        var bannerId = AdsManager.Instance.enableAdmobTestAds
            ? "ca-app-pub-3940256099942544/6300978111"
            : AdsManager.Instance.admobBannerID;
        
        _bannerView?.Destroy();
        var adPosition = AdPosition.Bottom;
        
        switch (AdsManager.Instance.bannerPlacement)
        {
            case AdsManager.BannerPlacement.Top:
                adPosition = AdPosition.Top;
                break;
            case AdsManager.BannerPlacement.Bottom:
                adPosition = AdPosition.Bottom;
                break;
        }
        
        var adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        var bannerSize = AdsManager.Instance.bannerType == AdsManager.BannerType.Adaptive
            ? adaptiveSize
            : AdSize.Banner;
        
        _bannerView = new BannerView(bannerId, bannerSize, adPosition);
        RegisterBannerEventHandlers();

        // Load a banner ad
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(CreateAdRequest());
    }
    
    public bool ShowBannerAd(Action<string> onCompleteCallBack = null, Action<string> onSkippedCallBack = null, Action<string> onFailedCallBack = null)
    {
        if (!CheckBannerToShow()) return false;
        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return false;
        
        if (AdsManager.Instance.bannerInitState == AdsManager.BannerInitState.ManualInitialize)
            _checkBannerStateForManual = true;
        
        if (_bannerView == null)
        {
            Debug.Log("Banner Ad is not loaded now loading...");
            LoadBannerAD();
            return false;
        }
        
        Debug.Log("Showing banner Ad now ....");
        _bannerView.Show();
        return true;
    }

    public void HideBanner()
    {
        if (!CheckBannerToShow()) return;

        if (AdsManager.Instance.bannerInitState == AdsManager.BannerInitState.ManualInitialize)
            _checkBannerStateForManual = false;
        
        _bannerView?.Hide();
    }

    public void ShowHideBanner()
    {
        if (!CheckBannerToShow()) return;
        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;

        _bannerView?.Show();
    }

    public void DestroyBanner()
    {
        if (!CheckBannerToShow()) return;

        _isBannerLoaded = false;
        
        _bannerView?.Destroy();
        _bannerView = null;
        
        if (_bannerView == null)
        {
            Debug.Log("Banner is destroyed");
        }
    }
    
    public bool CheckBannerIsAvailable()
    {
        if (!CheckBannerToShow()) return false;

        return _isBannerLoaded;
    }

    private bool CheckBannerToShow()
    {
        return AdsManager.Instance.bannerNetworkType == AdsManager.NetworkType.Admob;
    }

    #region BannerCallbacks
    
    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void RegisterBannerEventHandlers()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                      + _bannerView.GetResponseInfo());
            
            if (AdsManager.Instance.bannerInitState == AdsManager.BannerInitState.ManualInitialize)
            {
                Debug.Log("Hide Banner on load for manual initialization...");
                _bannerView.Hide();
            }
            
            _isBannerLoaded = true;
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
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

    #endregion

    #region InterstitialAD

    public void LoadInterstitialAD()
    {
        if (!CheckInterstitialToShow()) return;
        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Interstitial Ad already loaded and ready to show...");
            return;
        }
        
        // var adUnitId = AdsManager.Instance.admobInterstitialID;
        _interstitialAd?.Destroy();

        Debug.Log("Loading interstitial ad.");
        
        if (AdsManager.Instance.admobInterstitialID.Count <= 0)
        {
            Debug.Log("Interstitial ID is empty...");
            return;
        }
        
        //Check whether test ads are enabled
        var interstitialId = AdsManager.Instance.enableAdmobTestAds
            ? "ca-app-pub-3940256099942544/1033173712"
            : AdsManager.Instance.admobInterstitialID[_interstitialTierIndex];
        
        // Send the request to load the ad.
        InterstitialAd.Load( interstitialId, CreateAdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                _interstitialTierIndex++;
                if (_interstitialTierIndex < AdsManager.Instance.admobInterstitialID.Count)
                {
                    LoadInterstitialAD();
                }
                else
                {
                    _interstitialTierIndex = 0;
                }
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                _interstitialTierIndex++;
                if (_interstitialTierIndex < AdsManager.Instance.admobInterstitialID.Count)
                {
                    LoadInterstitialAD();
                }
                else
                {
                    _interstitialTierIndex = 0;
                }
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
        if (!CheckInterstitialToShow()) return false;

        return _interstitialAd != null && _interstitialAd.CanShowAd();
    }
    
    public bool ShowInterstitialAd(Action<string> onCompleteCallBack = null, Action<string> onSkippedCallBack = null,
        Action<string> onFailedCallBack = null)
    {
        if (!CheckInterstitialToShow()) return false;
        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return false;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _onCompleteInterstitialAdCallBack = onCompleteCallBack;
            _onFailedInterstitialAdCallBack = onFailedCallBack;
            _onSkippedInterstitialAdCallBack = onSkippedCallBack;
            
            AdsManager.Instance.isReadyToShowAppOpen = false;

            Debug.Log("Showing Interstitial AD");
            _interstitialAd.Show();
            return true;
        }
        
        LoadInterstitialAD();
        return false;
    }
    
    private bool CheckInterstitialToShow()
    {
        return AdsManager.Instance.interstitialNetworkType == AdsManager.NetworkType.Admob;
    }

    #region InterstitialAdCallBack

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
            _onCompleteInterstitialAdCallBack?.Invoke("Interstitial Shown");
            
            _onCompleteInterstitialAdCallBack = null;
            _onFailedInterstitialAdCallBack = null;
            _onSkippedInterstitialAdCallBack = null;
            
            Invoke(nameof(EnableAppOpenCheck), 1f);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                           + error);
            _onFailedInterstitialAdCallBack?.Invoke("Interstitial Failed To Show");
            
            _onCompleteInterstitialAdCallBack = null;
            _onFailedInterstitialAdCallBack = null;
            _onSkippedInterstitialAdCallBack = null;
        };
    }

    #endregion
    
    #endregion

    #region RewardedAD

    public void LoadRewardedAD()
    {
        if (!CheckRewardToShow()) return;
        
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Rewarded Ad already loaded and ready to show...");
            return;
        }
        //var adUnitId = AdsManager.Instance.admobRewardedID;
        
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        
        Debug.Log("Loading rewarded ad.");
        
        if (AdsManager.Instance.admobRewardedID.Count <= 0)
        {
            Debug.Log("Rewarded ID is empty...");
            return;
        }
        
        //Check whether test ads are enabled
        var rewardId = AdsManager.Instance.enableAdmobTestAds
            ? "ca-app-pub-3940256099942544/5224354917"
            : AdsManager.Instance.admobRewardedID[_rewardTierIndex];
        
        // Send the request to load the ad.
        RewardedAd.Load(rewardId, CreateAdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                _rewardTierIndex++;
                if (_rewardTierIndex < AdsManager.Instance.admobRewardedID.Count)
                {
                    LoadRewardedAD();
                }
                else
                {
                    _rewardTierIndex = 0;
                }
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                _rewardTierIndex++;
                if (_rewardTierIndex < AdsManager.Instance.admobRewardedID.Count)
                {
                    LoadRewardedAD();
                }
                else
                {
                    _rewardTierIndex = 0;
                }
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
        if (!CheckRewardToShow()) return false;

        return _rewardedAd != null && _rewardedAd.CanShowAd();
    }
    
    public bool ShowRewardedAd(Action<string> onCompleteCallBack = null, Action<string> onSkippedCallBack = null, Action<string> onFailedCallBack = null)
    {
        if (!CheckRewardToShow()) return false;

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            AdsManager.Instance.isReadyToShowAppOpen = false;

            _rewardedAd.Show((Reward reward) =>
            {
                onCompleteCallBack?.Invoke("Rewarded Completed");
                Invoke(nameof(EnableAppOpenCheck), 2f);
            });
            
            return true;
        }

        Debug.LogError("Rewarded ad is not ready yet.");
        LoadRewardedAD();
        return false;
    }
    
    private bool CheckRewardToShow()
    {
        return AdsManager.Instance.rewardedNetworkType == AdsManager.NetworkType.Admob;
    }

    #region RewardedCallback

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
        };
    }

    #endregion
    
    #endregion
    
    #region AppOpenAD

    public void LoadAppOpenAD()
    {
        if (!CheckAppOpenToShow()) return;
        
        // Clean up the old ad before loading a new one.
        if (_appOpenAd != null)
        {
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }

        Debug.Log("Loading app open ad.");
        
        if (AdsManager.Instance.admobAppOpenID.Count <= 0)
        {
            Debug.Log("App Open ID is empty...");
            return;
        }

        //Check Whether the test check is enable
        var appOpenId = AdsManager.Instance.enableAdmobTestAds
            ? "ca-app-pub-3940256099942544/9257395921"
            : AdsManager.Instance.admobAppOpenID[_appOpenTierIndex];
        
        // Send the request to load the ad.
        AppOpenAd.Load(appOpenId, CreateAdRequest(), (appOpenAd, error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("App open ad failed to load an ad with error : "
                                   + error);
                    
                    _appOpenTierIndex++;
                    if (_appOpenTierIndex < AdsManager.Instance.admobAppOpenID.Count)
                    {
                        LoadAppOpenAD();
                    }
                    else
                    {
                        _appOpenTierIndex = 0;
                    }
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (appOpenAd == null)
                {
                    Debug.LogError("Unexpected error: App open ad load event fired with " +
                                   " null ad and null error.");
                    
                    _appOpenTierIndex++;
                    if (_appOpenTierIndex < AdsManager.Instance.admobAppOpenID.Count)
                    {
                        LoadAppOpenAD();
                    }
                    else
                    {
                        _appOpenTierIndex = 0;
                    }
                    
                    return;
                }
                // The operation completed successfully.
                Debug.Log("App open ad loaded with response : " + appOpenAd.GetResponseInfo());
                _appOpenAd = appOpenAd;
                _appOpenTierIndex++;
                if (_appOpenTierIndex >= AdsManager.Instance.admobAppOpenID.Count)
                    _appOpenTierIndex = 0;
                // Register to ad events to extend functionality.
                RegisterAppOpenADEventHandlers(appOpenAd);

                if (_showAppOpenOnFirstOpen || AdsManager.Instance.isSceneSwitchedForAppOpen)
                {
                    AdsManager.Instance.OnLoadSceneOnAppOpenCloseOnStart = null;
                    return;
                }
                
                _showAppOpenOnFirstOpen = true;
                
                //Show App Open AD for first open
                ShowAppOpenAD();
            });
    }
    
    private void ShowAppOpenAD()
    {
        if (!CheckAppOpenToShow()) return;

        if (AdsManager.Instance.CheckIsRemoveAdsBought()) return;

        if (AdsManager.Instance.isReadyToShowAppOpen && _appOpenAd != null && _appOpenAd.CanShowAd())
        {
            AdsManager.Instance.isReadyToShowAppOpen = false;

            Debug.Log("Showing App Open AD");
            _bannerView?.Hide();
            _appOpenAd.Show();
        }
        else
        {
            LoadAppOpenAD();
            Debug.LogError("App open ad is not ready yet.");
        }
    }
    
    private bool CheckAppOpenToShow()
    {
        return AdsManager.Instance.appOpenNetworkType == AdsManager.NetworkType.Admob;
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
            Invoke(nameof(EnableAppOpenCheck), 1f);
            
            //Loading the main scene if the app open is shown for the first time
            AdsManager.Instance.OnLoadSceneOnAppOpenCloseOnStart?.Invoke();
            AdsManager.Instance.OnLoadSceneOnAppOpenCloseOnStart = null;
            
            LoadAppOpenAD();
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
        if (hasFocus && AdsManager.Instance.isReadyToShowAppOpen)
        {
            ShowAppOpenAD();
        }
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_IOS
        if (!pauseStatus && AdsManager.Instance.isReadyToShowAppOpen)
        {
            ShowAppOpenAD();
        }
#endif
    }
    
    private void EnableAppOpenCheck()
    {
        Debug.Log($"MY LOG: Activating App Open Check");
        AdsManager.Instance.isReadyToShowAppOpen = true;
        
        if (AdsManager.Instance.admobAdPlacement != AdsManager.AdmobAdPlacements.AllPlacements)
        {
            return;
        }

        switch (AdsManager.Instance.bannerInitState)
        {
            case AdsManager.BannerInitState.AutoInitialize:
                ShowBannerAd();
                break;
            case AdsManager.BannerInitState.ManualInitialize:
                if(_checkBannerStateForManual)
                    ShowHideBanner();
                break;
        }
        
        LoadRewardedAD();
        LoadInterstitialAD();
    }

    #endregion
}
#endif
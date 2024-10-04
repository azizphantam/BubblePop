using System;

public interface IAdsProvider
{
    public void Initialize();

    public bool ShowBannerAd(Action<string> onCompleteCallBack = null,
        Action<string> onSkippedCallBack = null, Action<string> onFailedCallBack = null);
    
    public bool ShowInterstitialAd(Action<string> onCompleteCallBack = null,
        Action<string> onSkippedCallBack = null, Action<string> onFailedCallBack = null);

    public bool ShowRewardedAd(Action<string> onCompleteCallBack = null,
        Action<string> onSkippedCallBack = null, Action<string> onFailedCallBack = null);

    public void LoadBannerAD();
    public void LoadInterstitialAD();

    public void LoadRewardedAD();

    public void LoadAppOpenAD();

    //SDK base Methods
    public bool CheckRewardIsAvailable();
    public bool CheckInterstitialIsAvailable();
    public bool CheckBannerIsAvailable();
    public void HideBanner();
    public void ShowHideBanner();
    public void DestroyBanner();
}


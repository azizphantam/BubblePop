using UnityEngine;
 
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class ATTConsent : MonoBehaviour
{
    public void ShowIOSNativeConsent()
    {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}

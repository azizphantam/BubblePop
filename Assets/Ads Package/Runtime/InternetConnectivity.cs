using UnityEngine;

public class InternetConnectivity : MonoBehaviour
{
    public void OnTryAgainButtonPressed()
    {
#if FirebaseRemoteConfig
        AdsManager.Instance.TryAgainInternetConnectivity();
#endif
    }
}

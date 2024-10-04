using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardedButton : MonoBehaviour
{
    public bool isItemUnlocked;
    public string prefName;
    private Button _itemButton;
    private void OnEnable()
    {
        if (isItemUnlocked)
        {
            gameObject.SetActive(false);
            return;
        }
        _itemButton = GetComponent<Button>();
        _itemButton.onClick.AddListener(ShowRewardAd);
        if (PlayerPrefs.GetInt(prefName) < 1) return;
        isItemUnlocked = true;
        gameObject.SetActive(false);
    }

    private void UnlockItem(string status)
    {
        PlayerPrefs.SetInt(prefName, 1);
        isItemUnlocked = true;
        gameObject.SetActive(false);
    }

    private void ShowRewardAd()
    {
        AdsManager.Instance.ShowRewardAd(UnlockItem);
    }
}
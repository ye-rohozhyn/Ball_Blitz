using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string _androidAdID = "Rewarded_Android";
    private UnityAdsShowCompletionState _lastAdStatus;

    private void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        Debug.Log("Loading ad: " + _androidAdID);
        Advertisement.Load(_androidAdID, this);
    }

    public void ShowAd()
    {
        Advertisement.Show(_androidAdID, this);
    }

    public UnityAdsShowCompletionState GetLastAdStatus()
    {
        return _lastAdStatus;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_androidAdID) & showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            _lastAdStatus = showCompletionState;
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Unity Ad Loaded id: " + placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Unity Ad Failed To Load id: " + placementId);
    }
}

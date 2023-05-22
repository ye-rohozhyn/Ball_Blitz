using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    private string _gameID = "5287307";
    private bool _testMode = true;

    private void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        Advertisement.Initialize(_gameID, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialize complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Unity Ads initialization error: " + error + " - " + message);
    }
}

using UnityEngine;
using com.unity3d.mediation;

public class AdsInitializer : MonoBehaviour
{
    public static AdsInitializer Instance { get; private set; }
    [SerializeField] private string androidAppKey;
    [SerializeField] private string iosAppKey;
    private string appKey;
    private bool isInitialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_ANDROID
        appKey = androidAppKey;
#elif UNITY_IOS
            appKey = iosAppKey;
#else
         appKey = "unexpected_platform";
#endif

        InitLevelPlay();
    }

    public void InitLevelPlay()
    {
        LevelPlay.Init(appKey, adFormats: new[] { LevelPlayAdFormat.REWARDED });

        LevelPlay.OnInitSuccess += OnInitializationComplete;
        LevelPlay.OnInitFailed += OnInitializationFailed;
    }

    void OnInitializationComplete(LevelPlayConfiguration config)
    {
        Debug.Log("Initialization completed successfully with config: " + config);
        isInitialized = true;

        //Add ImpressionSuccess Event
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

        // Enable ads
        BannerAds.Instance.EnableAds();
        InterstitialAds.Instance.EnableAds();
        RewardedAds.Instance.EnableAds();
    }

    void OnInitializationFailed(LevelPlayInitError error)
    {
        Debug.Log("Initialization failed with error: " + error);
        isInitialized = false;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
    }
}
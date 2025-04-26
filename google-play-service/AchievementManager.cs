using UnityEngine;
using GooglePlayGames;
using System.Collections.Generic;


public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    [Header("Achievements Config")]
    [SerializeField] private AchievementModel[] achievements;

    private Dictionary<string, AchievementModel> achievementMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize achievements
            BuildAchievementMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildAchievementMap()
    {
        achievementMap = new Dictionary<string, AchievementModel>();
        foreach (var achievement in achievements)
        {
            // If the achievement is already in the map, skip it
            if (achievementMap.ContainsKey(achievement.achievementKey))
            {
                Debug.LogWarning($"Duplicate achievement key found: {achievement.achievementKey}. Skipping.");
                continue;
            }

            // Add the achievement to the map
            achievementMap.Add(achievement.achievementKey, achievement);
        }
    }

    public void UnlockOrIncrement(string key, int step = 1)
    {
        if (!GPGSManager.Instance.IsAuthenticated()) {
            Debug.LogWarning("User is not authenticated. Cannot unlock or increment achievement.");
            return;
        }

        if (!achievementMap.TryGetValue(key, out var info))
        {
            Debug.LogWarning($"Achievement key not found: {key}");
            return;
        }

        if (info.isIncremental)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(info.achievementID, step, success =>
            {
                Debug.Log(success ? $"Incremented {key}" : $"Failed to increment {key}");
            });
        }
        else
        {
            PlayGamesPlatform.Instance.ReportProgress(info.achievementID, 100.0f, success =>
            {
                Debug.Log(success ? $"Unlocked {key}" : $"Failed to unlock {key}");
            });
        }
    }

    public void ShowAchievementsUI()
    {
        if (GPGSManager.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.LogWarning("User is not authenticated. Cannot show achievements UI.");
        }
    }
}

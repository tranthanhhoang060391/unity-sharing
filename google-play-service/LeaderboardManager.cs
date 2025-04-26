using UnityEngine;
using GooglePlayGames;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Leaderboard ID from Google Play Console")]
    public string leaderboardID = "";
    private const long minScore = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReportScore(long score)
    {
        if (score < 0)
        {
            Debug.LogError("Score cannot be negative.");
            return;
        }

        if (string.IsNullOrEmpty(leaderboardID))
        {
            Debug.LogError("Leaderboard ID is not set.");
            return;
        }

        if (score < minScore)
        {
            Debug.LogWarning($"Score {score} is less than the minimum score of {minScore}. Not reporting.");
            return;
        }

        if (GPGSManager.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Score reported successfully.");
                }
                else
                {
                    Debug.LogError("Failed to report score.");
                }
            });
        }
        else
        {
            Debug.LogWarning("User is not authenticated. Cannot report score.");
        }
    }

    public void ShowLeaderboardUI()
    {
        if (GPGSManager.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
        }
        else
        {
            Debug.LogWarning("User is not authenticated. Cannot show leaderboard UI.");
        }

        // Unlock the achievement for showing the leaderboard
        AchievementManager.Instance.UnlockOrIncrement("leaderboard_bee");
    }
}

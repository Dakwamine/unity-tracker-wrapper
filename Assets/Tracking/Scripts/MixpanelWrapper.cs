using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

/// <summary>
/// A convenient wrapper around the Mixpanel SDK.
/// </summary>
public class MixpanelWrapper
{
    static MixpanelWrapper()
    {
    }

    private MixpanelWrapper()
    {
    }

    private enum DefaultKey
    {
        APP_LAUNCHED__FIRST_TIME,
        APP_LAUNCHED__COUNT,
        GAME_PLAYED__SCORE,
        GAME_PLAYED__GAME_COUNT
    }

    private static string userId;

    /// <summary>
    /// Prevents multiple calls on AppLaunched().
    /// </summary>
    private static bool appLaunchedEvent = false;

    /// <summary>
    /// Call this when the app launches. If no argument is set, values are retrieved from / set to PlayerPrefs.
    /// </summary>
    public static void AppLaunched()
    {
        if (appLaunchedEvent)
        {
            Debug.Log("AppLaunched() already called in this session. Ignoring event tracking.");
            return;
        }

        bool firstTime = true;
        int count = 1;

        // Get the values from local storage
        int firstTime_raw = GetValueFromLocalStorage(DefaultKey.APP_LAUNCHED__FIRST_TIME, 1);
        int count_raw = GetValueFromLocalStorage(DefaultKey.APP_LAUNCHED__COUNT, 0);

        if (firstTime_raw == 1)
        {
            Debug.Log("First launch");
        }
        else
        {
            // This is not the first time the app launches
            firstTime = false;

            if (count_raw <= 0)
            {
                // This should never happen; the count must be positive
                Debug.LogError("count_raw is not a positive number.");
                return;
            }

            count = count_raw + 1;
        }

        if(!AppLaunched(firstTime, count))
        {
            // There is nothing more to do; AppLaunched was already called
            return;
        }
        
        SetValueToLocalStorage(DefaultKey.APP_LAUNCHED__FIRST_TIME, 0);
        SetValueToLocalStorage(DefaultKey.APP_LAUNCHED__COUNT, count);
        
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Call this when the app launches.
    /// </summary>
    /// <param name="firstTime">Tells if the app launches for the first time.</param>
    /// <param name="count">The app launch count.</param>
    /// <returns>true if successful; false if AppLaunched() was already called.</returns>
    public static bool AppLaunched(bool firstTime, int count)
    {
        if (appLaunchedEvent)
        {
            Debug.Log("AppLaunched() already called in this session. Ignoring event tracking.");
            return false;
        }

        // This will prevent further AppLaunched() calls
        appLaunchedEvent = true;

        // Prepare the values
        Value value = new Value();

        value["First Time"] = firstTime;
        value["Count"] = count;

        Mixpanel.Track("App Launched", value);

        // Send the data immediately
        Mixpanel.FlushQueue();

        return true;
    }

    /// <summary>
    /// Call this when a game has been played.
    /// </summary>
    /// <param name="score">The score at the end of the game.</param>
    /// <param name="gameCount">The game count.</param>
    public static void GamePlayed(int score, int gameCount)
    {
        Value value = new Value();

        value["Score"] = score;
        value["Game Count"] = gameCount;

        Mixpanel.Track("Game Played", value);
    }

    /// <summary>
    /// Gets a value from the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to retrieve.</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>The value corresponding to the key; the default value if not found.</returns>
    private static int GetValueFromLocalStorage(DefaultKey defaultKey, int defaultValue)
    {
        return PlayerPrefs.GetInt(defaultKey.ToString(), defaultValue);
    }

    /// <summary>
    /// Gets a value from the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to retrieve.</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>The value corresponding to the key; the default value if not found.</returns>
    private static string GetValueFromLocalStorage(DefaultKey defaultKey, string defaultValue)
    {
        return PlayerPrefs.GetString(defaultKey.ToString(), defaultValue);
    }

    /// <summary>
    /// Gets a value from the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to retrieve.</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>The value corresponding to the key; the default value if not found.</returns>
    private static float GetValueFromLocalStorage(DefaultKey defaultKey, float defaultValue)
    {
        return PlayerPrefs.GetFloat(defaultKey.ToString(), defaultValue);
    }

    /// <summary>
    /// Sets a value to the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to set.</param>
    /// <param name="value">The value to set.</param>
    private static void SetValueToLocalStorage(DefaultKey defaultKey, int value)
    {
        PlayerPrefs.SetInt(defaultKey.ToString(), value);
    }

    /// <summary>
    /// Sets a value to the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to set.</param>
    /// <param name="value">The value to set.</param>
    private static void SetValueToLocalStorage(DefaultKey defaultKey, string value)
    {
        PlayerPrefs.SetString(defaultKey.ToString(), value);
    }

    /// <summary>
    /// Sets a value to the local storage.
    /// </summary>
    /// <param name="defaultKey">Which key to set.</param>
    /// <param name="value">The value to set.</param>
    private static void SetValueToLocalStorage(DefaultKey defaultKey, float value)
    {
        PlayerPrefs.SetFloat(defaultKey.ToString(), value);
    }
}

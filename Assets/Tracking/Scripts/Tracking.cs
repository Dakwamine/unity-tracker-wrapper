using System;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

namespace Kwanito
{
    public enum Provider
    {
        NONE,
        MIXPANEL
    }

    public interface ITrackingProvider
    {
        /// <summary>
        /// Call this when the app launches. If no argument is set, values are retrieved from / set to PlayerPrefs.
        /// </summary>
        /// <returns>true if successful; false if there was an error.</returns>
        bool AppLaunched();

        /// <summary>
        /// Call this when the app launches. Call this once.
        /// </summary>
        /// <param name="firstTime">Tells if the app launches for the first time.</param>
        /// <param name="count">The app launch count.</param>
        /// <returns>false if there is any catched error; true otherwise.</returns>
        bool AppLaunched(bool firstTime, int count);
        
        /// <summary>
        /// Call this when a game has been played.
        /// </summary>
        /// <param name="score">The score at the end of the game.</param>
        /// <param name="gameCount">The game count.</param>
        /// <returns>false if there is any catched error; true otherwise.</returns>
        bool GamePlayed(int score, int gameCount);
    }

    public abstract class AbstractProvider : ITrackingProvider
    {
        protected enum DefaultKey
        {
            APP_LAUNCHED__FIRST_TIME,
            APP_LAUNCHED__COUNT,
            GAME_PLAYED__SCORE,
            GAME_PLAYED__GAME_COUNT
        }

        /// <summary>
        /// Prevents multiple calls on AppLaunched().
        /// </summary>
        protected bool appLaunchedCalledInCurrentSession = false;
        
        public bool AppLaunched()
        {
            if (appLaunchedCalledInCurrentSession)
            {
                Debug.Log("AppLaunched() already called in this session. Ignoring event tracking.");
                return false;
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
                    throw new Exception("count_raw is not a positive number.");
                }

                count = count_raw + 1;
            }

            if (!AppLaunched(firstTime, count))
            {
                // There is nothing more to do; AppLaunched was already called
                Debug.Log("AppLaunched() already called in this session. Ignoring event tracking.");
                return false;
            }

            SetValueToLocalStorage(DefaultKey.APP_LAUNCHED__FIRST_TIME, 0);
            SetValueToLocalStorage(DefaultKey.APP_LAUNCHED__COUNT, count);

            PlayerPrefs.Save();

            return true;
        }
        
        public bool AppLaunched(bool firstTime, int count)
        {
            if (appLaunchedCalledInCurrentSession)
            {
                Debug.Log("AppLaunched() already called in this session. Ignoring event tracking.");
                return false;
            }

            // This will prevent further AppLaunched() calls
            appLaunchedCalledInCurrentSession = true;

            // Call the provider specific method
            return AppLaunchedProvider(firstTime, count);
        }
        
        public bool GamePlayed(int score, int gameCount)
        {
            // Call the provider specific method
            return GamePlayedProvider(score, gameCount);
        }

        /// <summary>
        /// Called by AppLaunched() method. Needs to be implemented for each tracking provider.
        /// </summary>
        /// <param name="firstTime">Tells if the app launches for the first time.</param>
        /// <param name="count">The app launch count.</param>
        /// <returns>false if the implementation can return error states; true otherwise.</returns>
        protected abstract bool AppLaunchedProvider(bool firstTime, int count);

        /// <summary>
        /// Called by GamePlayed() method. Needs to be implemented for each tracking provider.
        /// </summary>
        /// <param name="score">The score at the end of the game.</param>
        /// <param name="gameCount">The game count.</param>
        /// <returns>false if the implementation can return error states; true otherwise.</returns>
        protected abstract bool GamePlayedProvider(int score, int gameCount);

        /// <summary>
        /// Gets a value from the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to retrieve.</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>The value corresponding to the key; the default value if not found.</returns>
        protected static int GetValueFromLocalStorage(DefaultKey defaultKey, int defaultValue)
        {
            return PlayerPrefs.GetInt(defaultKey.ToString(), defaultValue);
        }

        /// <summary>
        /// Gets a value from the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to retrieve.</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>The value corresponding to the key; the default value if not found.</returns>
        protected static string GetValueFromLocalStorage(DefaultKey defaultKey, string defaultValue)
        {
            return PlayerPrefs.GetString(defaultKey.ToString(), defaultValue);
        }

        /// <summary>
        /// Gets a value from the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to retrieve.</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>The value corresponding to the key; the default value if not found.</returns>
        protected static float GetValueFromLocalStorage(DefaultKey defaultKey, float defaultValue)
        {
            return PlayerPrefs.GetFloat(defaultKey.ToString(), defaultValue);
        }

        /// <summary>
        /// Sets a value to the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to set.</param>
        /// <param name="value">The value to set.</param>
        protected static void SetValueToLocalStorage(DefaultKey defaultKey, int value)
        {
            PlayerPrefs.SetInt(defaultKey.ToString(), value);
        }

        /// <summary>
        /// Sets a value to the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to set.</param>
        /// <param name="value">The value to set.</param>
        protected static void SetValueToLocalStorage(DefaultKey defaultKey, string value)
        {
            PlayerPrefs.SetString(defaultKey.ToString(), value);
        }

        /// <summary>
        /// Sets a value to the local storage.
        /// </summary>
        /// <param name="defaultKey">Which key to set.</param>
        /// <param name="value">The value to set.</param>
        protected static void SetValueToLocalStorage(DefaultKey defaultKey, float value)
        {
            PlayerPrefs.SetFloat(defaultKey.ToString(), value);
        }
    }

    /// <summary>
    /// Implementation for Mixpanel service.
    /// </summary>
    public class MixpanelProvider : AbstractProvider
    {
        private static readonly MixpanelProvider instance = new MixpanelProvider();

        static MixpanelProvider()
        {
        }

        private MixpanelProvider()
        {
        }

        public static MixpanelProvider Instance
        {
            get
            {
                return instance;
            }
        }

        protected override bool AppLaunchedProvider(bool firstTime, int count)
        {
            // Prepare the values
            Value value = new Value();

            value["First Time"] = firstTime;
            value["Count"] = count;

            Mixpanel.Track("App Launched", value);

            // Send the data immediately
            Mixpanel.FlushQueue();

            return true;
        }

        protected override bool GamePlayedProvider(int score, int gameCount)
        {
            Value value = new Value();

            value["Score"] = score;
            value["Game Count"] = gameCount;

            Mixpanel.Track("Game Played", value);

            return true;
        }
    }

    public static class TrackingProviderFactory
    {
        /// <summary>
        /// Get the provider object.
        /// </summary>
        /// <param name="provider">Which provider to load.</param>
        /// <returns>The interface to the provider.</returns>
        public static ITrackingProvider GetProvider(Provider provider)
        {
            switch (provider)
            {
                case Provider.MIXPANEL:
                    return MixpanelProvider.Instance;

                default:
                    throw new Exception("Tracking provider is unset. Please select a provider on the Tracking Game Object.");
            }
        }
    }

    public class Tracking : MonoBehaviour
    {
        [Header("Provider configuration")]
        [Tooltip("Please select a tracking provider.")]
        public Provider currentProvider = Provider.NONE;

        private static Tracking instance;

        /// <summary>
        /// Gets the unique instance.
        /// </summary>
        public static Tracking Instance
        {
            get
            {
                return instance;
            }
        }

        [SerializeField]
        [HideInInspector]
        private Mixpanel mixpanelPrefab;

        private void Awake()
        {
            if (instance != null)
            {
                // There is no need for more than one Tracking Game Object
                Destroy(gameObject);
            }

            // Make this gameobject persist across the session
            DontDestroyOnLoad(this);
            instance = this;

            // Load the prefab corresponding to the tracking provider
            switch(currentProvider)
            {
                case Provider.MIXPANEL:
                    Instantiate(mixpanelPrefab);
                    break;

                default:
                    throw new Exception("No tracking provider selected in Inspector.");
            }
        }

        /// <summary>
        /// Call this when the app launches. If no argument is set, values are retrieved from / set to PlayerPrefs.
        /// </summary>
        /// <returns>true if successful; false if there was an error.</returns>
        public bool AppLaunched()
        {
            return TrackingProviderFactory.GetProvider(currentProvider).AppLaunched();
        }

        /// <summary>
        /// Call this when the app launches. Call this once.
        /// </summary>
        /// <param name="firstTime">Tells if the app launches for the first time.</param>
        /// <param name="count">The app launch count.</param>
        /// <returns>false if there is any catched error; true otherwise.</returns>
        public bool AppLaunched(bool firstTime, int count)
        {
            return TrackingProviderFactory.GetProvider(currentProvider).AppLaunched(firstTime, count);
        }

        /// <summary>
        /// Call this when a game has been played.
        /// </summary>
        /// <param name="score">The score at the end of the game.</param>
        /// <param name="gameCount">The game count.</param>
        /// <returns>false if there is any catched error; true otherwise.</returns>
        public bool GamePlayed(int score, int gameCount)
        {
            return TrackingProviderFactory.GetProvider(currentProvider).GamePlayed(score, gameCount);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dakwamine
{
    public abstract class AbstractProvider : MonoBehaviour
    {
        protected static AbstractProvider instance;

        /// <summary>
        /// Unique instance.
        /// </summary>
        public static AbstractProvider Instance
        {
            get
            {
                return instance;
            }
        }

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
}

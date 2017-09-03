using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dakwamine
{
    public class Tracking : MonoBehaviour
    {
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

        [Header("Provider configuration")]

        /// <summary>
        /// Prefab list of providers to call.
        /// </summary>
        [Tooltip("Please add a tracking provider prefab to enable it.")]
        public AbstractProvider[] activeProviders;

        /// <summary>
        /// Instantiated list of providers.
        /// </summary>
        private List<AbstractProvider> providerInstances = new List<AbstractProvider>();

        private void Awake()
        {
            if (instance != null)
            {
                // There is no need for more than one Tracking Game Object
                Destroy(gameObject);
                return;
            }

            // Make this gameobject persist across the session
            DontDestroyOnLoad(this);
            instance = this;

            // This list lets us check if an object has already been instantiated
            List<string> loadedProvidersClassNames = new List<string>();

            // Load the prefab corresponding to the tracking provider
            foreach (AbstractProvider ap in activeProviders)
            {
                if (ap == null)
                    continue;
                
                if(loadedProvidersClassNames.Contains(ap.GetType().ToString()))
                {
                    // Already loaded
                    continue;
                }

                // Instantiate the prefab and reference it
                providerInstances.Add(Instantiate(ap));
                
                // Reference the class full name to prevent same object creation
                loadedProvidersClassNames.Add(ap.GetType().ToString());
            }
        }

        /// <summary>
        /// Call this when the app launches. If no argument is set, values are retrieved from / set to PlayerPrefs.
        /// </summary>
        public void AppLaunched()
        {
            foreach (AbstractProvider provider in providerInstances)
            {
                provider.AppLaunched();
            }
        }

        /// <summary>
        /// Call this when the app launches. Call this once.
        /// </summary>
        /// <param name="firstTime">Tells if the app launches for the first time.</param>
        /// <param name="count">The app launch count.</param>
        public void AppLaunched(bool firstTime, int count)
        {
            foreach (AbstractProvider provider in providerInstances)
            {
                provider.AppLaunched(firstTime, count);
            }
        }

        /// <summary>
        /// Call this when a game has been played.
        /// </summary>
        /// <param name="score">The score at the end of the game.</param>
        /// <param name="gameCount">The game count.</param>
        /// <returns>false if there is any catched error; true otherwise.</returns>
        public void GamePlayed(int score, int gameCount)
        {
            foreach (AbstractProvider provider in providerInstances)
            {
                provider.GamePlayed(score, gameCount);
            }
        }
    }
}

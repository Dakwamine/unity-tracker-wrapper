using System;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

namespace Dakwamine
{
    /// <summary>
    /// Implementation for Mixpanel service.
    /// </summary>
    public class MixpanelProvider : AbstractProvider
    {
        /// <summary>
        /// Mixpanel component attached to the same game object.
        /// </summary>
        private Mixpanel mixpanelComponent;

        private void Awake()
        {
            // Pseudo singleton mechanism
            if (instance != null)
            {
                // There is no need for more than one Tracking Game Object
                Destroy(gameObject);
                return;
            }

            // Make this gameobject persist across the session
            DontDestroyOnLoad(this);
            instance = this;

            // Reference the Mixpanel component
            mixpanelComponent = GetComponent<Mixpanel>();

            // Make basic checks
            if(mixpanelComponent.token == "" || mixpanelComponent.debugToken == "")
            {
                Debug.LogError("No public or private token defined on Mixpanel component.", gameObject);
                return;
            }

            // Enable the Mixpanel component (disabled by default)
            mixpanelComponent.enabled = true;
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
}

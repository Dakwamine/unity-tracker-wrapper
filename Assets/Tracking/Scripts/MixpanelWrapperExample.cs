using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kwanito;

public class MixpanelWrapperExample : MonoBehaviour
{
    /// <summary>
    /// A value to track.
    /// </summary>
    private int gameCount;

    private enum PlayerPrefsKey
    {
        GAME_COUNT
    }

    private void Awake()
    {
        // This is an example of data persistance. You can use your own save system.
        gameCount = PlayerPrefs.GetInt(PlayerPrefsKey.GAME_COUNT.ToString(), 0);
    }

    void Start()
    {
        // This will track the app launch
        Tracking.Instance.AppLaunched();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load Scene"))
        {
            // This will reload the same scene, making sure the AppLaunched event is sent one time only
            SceneManager.LoadScene(0);
        }

        if (GUILayout.Button("Simulate Game played"))
        {
            // The game has been played, increment the game count
            gameCount++;

            // Save the game count locally
            PlayerPrefs.SetInt(PlayerPrefsKey.GAME_COUNT.ToString(), gameCount);
            PlayerPrefs.Save();

            // We can track the event
            Tracking.Instance.GamePlayed(1337, gameCount);
        }
    }
}

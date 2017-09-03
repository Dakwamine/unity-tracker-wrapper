# Tracking (ex MixpanelWrapper) v0.3
* by Quang-Minh Dang *

## Tracking installation

- Add this package to your project (if not already done).
- Add the **Tracking/Game Objects/Tracking** prefab to your scene.

## Provider selection

For each provider you want to use:

- Set the prefab from **Tracking/Game Objects/Provider/**
- Add it to the **Active Providers** field in the inspector of the **Tracking** prefab.

## Tracking usage

- Call Tracking.Instance.AppLaunched() to track app launch.
- Call Tracking.Instance.GamePlayed(int score, int gameCount) to track a game played.

## Additional notes

### Mixpanel configuration

- IMPORTANT: For now, the Unity Editor crashes if your **Debug Token** is unset: https://github.com/mixpanel/mixpanel-unity/issues/26
- Set your tokens on the Mixpanel Game Object in Project as described here: https://mixpanel.com/help/reference/unity

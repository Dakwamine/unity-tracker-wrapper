Tracking (ex MixpanelWrapper) v0.2
by Quang-Minh Dang
====================

Usage
-----

Tracking installation:

- Add this package to your project (if not already done).
- Add the Tracking Prefab to your game.

Mixpanel installation:

- Set your tokens on the Mixpanel Game Object in Project as described here: https://mixpanel.com/help/reference/unity
- Set the current provider on the Tracking Prefab or Game Object to "MIXPANEL".

Tracking calls:

- Call Tracking.Instance.AppLaunched() to track app launch.
- Call Tracking.Instance.GamePlayed(int score, int gameCount) to track a game played.

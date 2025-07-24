// Editor/PlayfabFriendsInitialize.cs
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Sabih.PlayfabFriends;

[InitializeOnLoad]
static class PlayfabFriendsInitialize
{
    static PlayfabFriendsInitialize()
    {
        // Delay one frame so everything’s compiled & loaded
        EditorApplication.delayCall += ShowSetupIfNeeded;
    }

    private static void ShowSetupIfNeeded()
    {
        // Only once per load
        EditorApplication.delayCall -= ShowSetupIfNeeded;

        // Look for your settings asset in Resources
        var settings = Resources.Load<PlayfabFriendsSettings>("PlayfabFriendsSettings");
        if (settings == null)
        {
            // No settings yet → launch the setup window
            PlayfabFriendsSetupWindow.ShowWindow();
        }
    }
}

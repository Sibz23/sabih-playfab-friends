using UnityEditor;
using UnityEngine;
using Sabih.PlayfabFriends;

public class PlayfabFriendsSetupWindow : EditorWindow
{
    private PlayfabFriendsSettings settings;

    [MenuItem("Tools/Playfab Friends/Setup")]
    public static void ShowWindow() =>
        GetWindow<PlayfabFriendsSetupWindow>("Playfab Friends Setup");

    void OnEnable() => LoadOrCreateSettings();

    void OnGUI()
    {
        if (settings == null)
        {
            EditorGUILayout.HelpBox(
                "Unable to locate or create PlayfabFriendsSettings.",
                MessageType.Error
            );
            if (GUILayout.Button("Retry"))
                LoadOrCreateSettings();
            return;
        }

        EditorGUILayout.LabelField("CloudScript Configuration",
            EditorStyles.boldLabel);
        settings.CloudScriptUrl = EditorGUILayout.TextField(
            "CloudScript URL", settings.CloudScriptUrl
        );

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Settings"))
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog(
                "Playfab Friends",
                "Settings saved successfully.",
                "OK"
            );
        }
    }

    void LoadOrCreateSettings()
    {
        // find existing
        var guids = AssetDatabase.FindAssets("t:PlayfabFriendsSettings");
        if (guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath<PlayfabFriendsSettings>(path);
            return;
        }

        // else create under Assets/PlayfabFriends/Resources
        const string baseFolder = "Assets/PlayfabFriends";
        const string resFolder  = baseFolder + "/Resources";
        if (!AssetDatabase.IsValidFolder(baseFolder))
            AssetDatabase.CreateFolder("Assets", "PlayfabFriends");
        if (!AssetDatabase.IsValidFolder(resFolder))
            AssetDatabase.CreateFolder(baseFolder, "Resources");

        var asset = ScriptableObject.CreateInstance<PlayfabFriendsSettings>();
        AssetDatabase.CreateAsset(
            asset,
            $"{resFolder}/PlayfabFriendsSettings.asset"
        );
        AssetDatabase.SaveAssets();
        settings = asset;
    }
}

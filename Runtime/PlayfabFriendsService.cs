using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Sabih.PlayfabFriends
{
    [DisallowMultipleComponent]
    public class PlayfabFriendsService : MonoBehaviour
    {
        public static PlayfabFriendsService Instance { get; private set; }

        public event Action<List<FriendInfo>> OnFriendsListReceived;
        public event Action OnRequestSent;
        public event Action<string> OnError;

        /// <summary>CloudScript URL from settings (stub).</summary>
        public string CloudScriptUrl => settings != null ? settings.CloudScriptUrl : string.Empty;

        private PlayfabFriendsSettings settings;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else Destroy(gameObject);
        }

        void LoadSettings()
        {
            settings = Resources.Load<PlayfabFriendsSettings>("PlayfabFriendsSettings");
            if (settings == null)
                Debug.LogWarning("PlayfabFriendsSettings not found—run Tools→Playfab Friends→Setup.");
        }

        /// <summary>Lookup exact DisplayName, then send AddFriend.</summary>
        public void SearchAndSendRequest(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                OnError?.Invoke("Display name cannot be empty.");
                return;
            }

            var lookup = new GetAccountInfoRequest { TitleDisplayName = displayName };
            PlayFabClientAPI.GetAccountInfo(lookup,
                result => AddFriendById(result.AccountInfo.PlayFabId),
                err    => OnError?.Invoke($"User '{displayName}' not found."));
        }

        void AddFriendById(string friendId)
        {
            var req = new AddFriendRequest { FriendPlayFabId = friendId };
            PlayFabClientAPI.AddFriend(req,
                res => OnRequestSent?.Invoke(),
                err => OnError?.Invoke(err.GenerateErrorReport()));
        }

        /// <summary>Fetch your PlayFab friends (non‑FB, non‑Steam).</summary>
        public void FetchFriendsList()
        {
            var req = new GetFriendsListRequest {
                IncludeFacebookFriends = false,
                IncludeSteamFriends    = false
            };
            PlayFabClientAPI.GetFriendsList(req,
                res => OnFriendsListReceived?.Invoke(res.Friends),
                err => OnError?.Invoke(err.GenerateErrorReport()));
        }

        /// <summary>Remove an existing friend.</summary>
        public void RemoveFriend(string friendPlayFabId)
        {
            var req = new RemoveFriendRequest { FriendPlayFabId = friendPlayFabId };
            PlayFabClientAPI.RemoveFriend(req,
                res => { },  // you could add an OnFriendRemoved event here
                err => OnError?.Invoke(err.GenerateErrorReport()));
        }

        /// <summary>Import all FB friends who have your title.</summary>
        public void ImportFacebookFriends()
        {
            var req = new GetFriendsListRequest {
                IncludeFacebookFriends = true,
                IncludeSteamFriends    = false
            };
            PlayFabClientAPI.GetFriendsList(req,
                res => {
                    foreach (var f in res.Friends)
                        if (f.IsFacebookFriend)
                            AddFriendById(f.FriendPlayFabId);
                },
                err => OnError?.Invoke(err.GenerateErrorReport()));
        }

        /// <summary>Stub for future CloudScript calls.</summary>
        public void ExecuteCloudScript(string functionName, object parameters, Action<ExecuteCloudScriptResult> onSuccess)
        {
            var req = new ExecuteCloudScriptRequest {
                FunctionName           = functionName,
                FunctionParameter      = parameters,
                GeneratePlayStreamEvent= false
            };
            PlayFabClientAPI.ExecuteCloudScript(req,
                result => onSuccess?.Invoke(result),
                err    => OnError?.Invoke(err.GenerateErrorReport()));
        }
    }
}

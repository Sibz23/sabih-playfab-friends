using UnityEngine;

namespace Sabih.PlayfabFriends
{
    [CreateAssetMenu(fileName = "PlayfabFriendsSettings", menuName = "Playfab/PlayfabFriends Settings")]
    public class PlayfabFriendsSettings : ScriptableObject
    {
        [Tooltip("Your CloudScript endpoint URL (for future use).")]
        public string CloudScriptUrl;
    }
}

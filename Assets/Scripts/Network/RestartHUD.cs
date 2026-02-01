using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class RestartHUD : NetworkBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private Button voteButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private TextMeshProUGUI votesText;

#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneToRestart;
#endif
        [SerializeField] private string sceneName;

        private readonly SyncVar<int> votedCount = new();
        private readonly HashSet<NetworkConnection> votedConns = new();

        private bool selfVoted = false;

        public override void OnStartClient()
        {
            votesText.text = votedCount.Value + "/2";
            votedCount.OnChange += VotedCountOnChange;
        }

        private void VotedCountOnChange(int prev, int next, bool asServer)
        {
            if (asServer) return;
            votesText.text = next + "/2";
        }

        private void OnEnable()
        {
            voteButton.onClick.AddListener(Vote);
        }
        private void OnDisable()
        {
            voteButton.onClick.RemoveListener(Vote);
        }

        private void Vote()
        {
            selfVoted = !selfVoted;
            CmdVote(selfVoted);
            buttonText.text = selfVoted ? "Cancel" : "Restart";
        }

        [ServerRpc(RequireOwnership = false)]
        private void CmdVote(bool vote, NetworkConnection conn = null)
        {
            if (vote)
            {
                votedConns.RemoveWhere(x => !x.IsValid);
                votedConns.Add(conn);
                if (votedConns.Count > 1)
                    Restart();
            }
            else
                votedConns.Remove(conn);
            votedCount.Value = votedConns.Count;
        }

        [Server]
        private void Restart()
        {
            SceneManager.UnloadGlobalScenes(new(sceneName));
            SceneManager.LoadGlobalScenes(new(sceneName));
        }

        public void OnBeforeSerialize()
        {
            sceneName = sceneToRestart.name;
        }

        public void OnAfterDeserialize()
        {
        }
    }
}

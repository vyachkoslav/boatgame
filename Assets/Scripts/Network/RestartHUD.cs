using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Network
{
    public class RestartHUD : NetworkBehaviour
    {
        [SerializeField] private InputActionReference voteAction;
        [SerializeField] private Button voteButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private TextMeshProUGUI votesText;

        private readonly SyncVar<int> votedCount = new();
        private readonly HashSet<NetworkConnection> votedConns = new();

        private bool selfVoted = false;

        public override void OnStartClient()
        {
            votesText.text = votedCount.Value + "/2";
            votedCount.OnChange += VotedCountOnChange;
        }

        public override void OnDespawnServer(NetworkConnection connection)
        {
            votedConns.Remove(connection);
            votedCount.Value = votedConns.Count;
        }

        private void VotedCountOnChange(int prev, int next, bool asServer)
        {
            if (asServer) return;
            votesText.text = next + "/2";
        }

        private void OnEnable()
        {
            if(voteAction != null)
                voteAction.action.performed += Vote;
            voteButton?.onClick.AddListener(Vote);

            BoatHealth.OnDeath += Restart;
        }

        private void OnDisable()
        {
            if(voteAction != null)
                voteAction.action.performed -= Vote;
            voteButton?.onClick.RemoveListener(Vote);

            BoatHealth.OnDeath -= Restart;
        }

        private void Vote(InputAction.CallbackContext obj)
        {
            Vote();
        }

        private void Vote()
        {
            selfVoted = !selfVoted;
            CmdVote(selfVoted);
            buttonText.text = selfVoted ? "<b>P</b> - Cancel" : "<b>P</b> - Restart";
        }

        [ServerRpc(RequireOwnership = false)]
        private void CmdVote(bool vote, NetworkConnection conn = null)
        {
            if (vote)
            {
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
            var scene = SceneManager.SceneConnections.First().Key.name;
            SceneManager.UnloadGlobalScenes(new(scene));
            SceneManager.LoadGlobalScenes(new(scene));
        }
    }
}

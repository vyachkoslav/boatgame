using System;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

namespace Network
{
    public class SyncCamera : NetworkBehaviour
    {
        [SerializeField] private Transform center;
        [SerializeField] private new Transform camera;
        [SerializeField] private Transform rightLook;
        [SerializeField] private Transform leftLook;

        private readonly Vector3SyncVar rightDir = new();
        private readonly Vector3SyncVar leftDir = new();

        private void Awake()
        {
            rightDir.UpdateSettings(Channel.Unreliable);
            leftDir.UpdateSettings(Channel.Unreliable);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateDir(Vector3 dir, Channel channel = Channel.Unreliable, NetworkConnection conn = null)
        {
            if (!PlayerManager.Instance.Players.TryGetValue(conn, out var player)) return;
            SetDir(dir, player);
        }

        private void SetDir(Vector3 dir, PlayerManager.PlayerType player)
        {
            switch (player)
            {
                case PlayerManager.PlayerType.Left:
                    leftDir.Value = dir;
                    break;
                case PlayerManager.PlayerType.Right:
                    rightDir.Value = dir;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (IsHostStarted)
                SetDir(camera.forward, PlayerManager.Instance.CurrentPlayer);
            else if (IsClientStarted)
                UpdateDir(camera.forward);
        }

        private void Update()
        {
            switch (PlayerManager.Instance.CurrentPlayer)
            {
                case PlayerManager.PlayerType.Left:
                    leftLook.position = center.position + camera.forward;
                    rightLook.position = center.position + rightDir.InterpolatedValue();
                    break;
                case PlayerManager.PlayerType.Right:
                    rightLook.position = center.position + camera.forward;
                    leftLook.position = center.position + leftDir.InterpolatedValue();
                    break;
                default:
                    rightLook.position = center.position + rightDir.InterpolatedValue();
                    leftLook.position = center.position + leftDir.InterpolatedValue();
                    break;
            }
        }
    }
}
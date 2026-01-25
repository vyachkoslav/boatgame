using FishNet.Connection;
using FishNet.Object;
using Player;
using UnityEngine;

namespace Network
{
    public abstract class NetworkGrabbable : NetworkBehaviour, IGrabbable
    {
        [SerializeField] private NetworkObject takeOwnershipOf;
        public override void OnStartClient()
        {
            if (Owner.IsValid)
                OnGrabbedByOther();
        }

        [ServerRpc(RequireOwnership = false)]
        private void CmdGrab(NetworkConnection sender = null)
        {
            if (!Owner.IsValid)
            {
                Debug.Log("Grab " + sender);
                GiveOwnership(sender);
                takeOwnershipOf?.GiveOwnership(sender);
                RpcOwnershipChanged();
            }
            else
            {
                Debug.Log("Grab fail " + sender);
                RpcGrabFailed(sender);
            }
        }

        [ObserversRpc(ExcludeOwner = false)]
        private void RpcOwnershipChanged()
        {
            if (IsOwner)
                OnGrab();
            else if (Owner.IsValid)
                OnGrabbedByOther();
            else
                OnUngrabbedByOther();
        }

        [TargetRpc]
        private void RpcGrabFailed(NetworkConnection target)
        {
            Debug.LogWarning("Grab failed, object already grabbed.");
        }
        
        [ServerRpc(RequireOwnership = true)]
        private void CmdStopGrab()
        {
            Debug.Log("Ungrab " + Owner);
            RemoveOwnership();
            takeOwnershipOf?.RemoveOwnership();
            RpcOwnershipChanged();
        }

        public void Grab()
        {
            if (Owner.IsValid || !IsClientStarted) return;
            CmdGrab();
        }

        public void Ungrab()
        {
            if (!IsOwner || !IsClientStarted) return;
            CmdStopGrab();
            OnUngrab();
        }
        
        protected abstract void OnGrab();
        protected abstract void OnUngrab();
        protected abstract void OnGrabbedByOther();
        protected abstract void OnUngrabbedByOther();
    }
}
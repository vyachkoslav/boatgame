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
                RpcGrabFailed(sender);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcOwnershipChanged()
        {
            if (Owner.IsValid)
                OnGrabbedByOther();
            else
                OnUngrabbedByOther();
        }

        [TargetRpc]
        private void RpcGrabFailed(NetworkConnection target)
        {
            Debug.LogWarning("Grab failed, object already grabbed.");
            OnUngrab();
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
            if (Owner.IsValid) return;
            CmdGrab();
            OnGrab();
        }

        public void Ungrab()
        {
            if (!IsOwner) return;
            CmdStopGrab();
            OnUngrab();
        }
        
        protected abstract void OnGrab();
        protected abstract void OnUngrab();
        protected abstract void OnGrabbedByOther();
        protected abstract void OnUngrabbedByOther();
    }
}
using FishNet.Connection;
using Network;
using UnityEngine;

namespace Player
{
    public class Paddle : NetworkGrabbable, IMouseGrabbable
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer mesh;
        
        [Header("Visual")]
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material dragMaterial;
        [SerializeField] private Material grabbedByOtherMaterial;

        private Vector3 initPosition;

        private void Awake()
        {
            initPosition = transform.localPosition;
        }
        
        public void Hover()
        {
            if (Owner.IsValid) return;
            mesh.material = hoverMaterial;
        }

        public void Unhover()
        {
            if (Owner.IsValid) return;
            mesh.material = defaultMaterial;
        }

        protected override void OnGrab()
        {
            mesh.material = dragMaterial;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void OnUngrab()
        {
            mesh.material = defaultMaterial;
            transform.localPosition = initPosition;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);
            if (!Owner.IsValid)
                transform.localPosition = initPosition;
        }

        protected override void OnGrabbedByOther()
        {
            mesh.material = grabbedByOtherMaterial;
        }

        protected override void OnUngrabbedByOther()
        {
            mesh.material = ReferenceEquals(MouseHandler.Instance.CurrentHovered, this) ? hoverMaterial : defaultMaterial;
        }
    }
}
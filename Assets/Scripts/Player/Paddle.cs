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

        private bool grabbed = false;
        private bool grabbedByOther = false;
        private Vector3 initPosition;

        private void Awake()
        {
            initPosition = transform.localPosition;
        }
        
        public void Hover()
        {
            if (grabbedByOther) return;
            mesh.material = hoverMaterial;
        }

        public void Unhover()
        {
            if (grabbedByOther) return;
            mesh.material = defaultMaterial;
        }

        protected override void OnGrab()
        {
            if (grabbedByOther) return;
            mesh.material = dragMaterial;
            grabbed = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void OnUngrab()
        {
            mesh.material = defaultMaterial;
            grabbed = false;
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
            if (grabbedByOther) return;
            if (grabbed) Ungrab();
            
            grabbedByOther = true;
            mesh.material = grabbedByOtherMaterial;
        }

        protected override void OnUngrabbedByOther()
        {
            if (!grabbedByOther) return;
            grabbedByOther = false;
            mesh.material = ReferenceEquals(MouseHandler.Instance.CurrentHovered, this) ? hoverMaterial : defaultMaterial;
        }
    }
}
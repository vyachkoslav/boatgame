using FishNet.Connection;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class Paddle : NetworkGrabbable, IMouseGrabbable
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private new Rigidbody rigidbody;

        [Header("Settings")] 
        [SerializeField] private float maxDragRadius;
        [SerializeField] private float airSpeed;
        [SerializeField] private float waterSpeed;
        
        [Header("Visual")]
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material dragMaterial;
        [SerializeField] private Material grabbedByOtherMaterial;

        private bool grabbed = false;
        private bool grabbedByOther = false;
        private Plane horPlane;
        private Vector3 initPosition;
        private float currentSpeed;
        private Vector3 targetPosition;

        private Camera MainCamera => GlobalObjects.MainCamera;

        private void Awake()
        {
            horPlane = new Plane(Vector3.up, transform.position);
            initPosition = transform.localPosition;
            currentSpeed = airSpeed;
        }

        public void EnterWater()
        {
            currentSpeed = waterSpeed;
        }

        public void ExitWater()
        {
            currentSpeed = airSpeed;
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
            rigidbody.useGravity = false;
        }

        protected override void OnUngrab()
        {
            mesh.material = defaultMaterial;
            grabbed = false;
            rigidbody.useGravity = true;
            transform.localPosition = initPosition;
            targetPosition = initPosition;
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
            mesh.material = MouseHandler.Instance.CurrentHovered == this ? hoverMaterial : defaultMaterial;
        }

        private void Update()
        {
            if (!grabbed) return;
            UpdateHandlePosition();
        }

        private void FixedUpdate()
        {
            if (!grabbed) return;
            DragPaddleToHandle();
        }
        
        private void UpdateHandlePosition()
        {
            var camRay = MainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (!horPlane.Raycast(camRay, out var distance))
            {
                Debug.LogWarning("Didn't hit drag raycast, camera under plane?");
                return;
            }
            var pos = camRay.GetPoint(distance);
            var endPos = transform.parent.InverseTransformPoint(pos);
            
            var target = endPos;
            var dir = endPos - initPosition;
            distance = dir.magnitude;
            if (distance > maxDragRadius)
            {
                target = initPosition + (dir/distance)*maxDragRadius;
                distance = maxDragRadius;
            }

            targetPosition = target;
            targetPosition.y += (distance - (maxDragRadius/2))*2;
            
            transform.localPosition = target;
        }

        private void DragPaddleToHandle()
        {
            var selfPos = targetPosition;
            var dir = selfPos - initPosition;
            var rot = Quaternion.LookRotation(dir);
            rot = Quaternion.RotateTowards(rigidbody.rotation, rot, currentSpeed * Time.deltaTime);
            rigidbody.MoveRotation(rot);
        }
    }
}
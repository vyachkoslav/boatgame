using FishNet.Connection;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Paddle : NetworkGrabbable, IMouseGrabbable
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private Transform blade;
        [SerializeField] private Rigidbody boatRb;

        [Header("Settings")] 
        [SerializeField] private InputActionReference raiseAction;
        [SerializeField] private float waterDrag;
        
        [Header("Visual")]
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material dragMaterial;
        [SerializeField] private Material grabbedByOtherMaterial;

        private bool grabbed = false;
        private bool grabbedByOther = false;
        private Vector3 initPosition;
        private float xRot = 0;
        private float zRot;
        private Vector3 bladeLastPos;

        private void Awake()
        {
            initPosition = transform.localPosition;
            bladeLastPos = blade.transform.position;
            zRot = rigidbody.transform.localRotation.eulerAngles.z;
        }

        private void OnEnable()
        {
            raiseAction.action.started += EnterWater;
            raiseAction.action.canceled += ExitWater;
        }

        private void OnDisable()
        {
            raiseAction.action.started -= EnterWater;
            raiseAction.action.canceled -= ExitWater;
        }

        private void EnterWater(InputAction.CallbackContext callbackContext)
        {
            if (!grabbed) return;
            SetRotation(-45);
        }

        private void ExitWater(InputAction.CallbackContext callbackContext)
        {
            if (!grabbed) return;
            SetRotation(0);
        }

        private void SetRotation(float x)
        {
            rigidbody.angularVelocity = Vector3.zero;
            xRot = x;
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

        private void FixedUpdate()
        {
            if (grabbed || grabbedByOther && IsServerInitialized)
            {
                var bladePos = blade.transform.position;
                if (bladePos.y < 0 && bladeLastPos.y < 0) 
                {
                    var velocity = (bladePos - bladeLastPos) / Time.fixedDeltaTime;
                    var force = -velocity * waterDrag;
                    boatRb.AddForceAtPosition(force, bladePos, ForceMode.Force);
                }
                bladeLastPos = blade.transform.position;
            }
            if (!grabbed) return;
            
            var delta = Pointer.current.delta.value;
            var targetVel = new Vector3(0, -delta.y, 0);
            rigidbody.AddRelativeTorque(targetVel, ForceMode.Acceleration);

            var rot = rigidbody.transform.localEulerAngles;
            rot.x = xRot;
            rot.z = zRot;
            rigidbody.rotation = rigidbody.transform.parent.rotation * Quaternion.Euler(rot);
        }
    }
}
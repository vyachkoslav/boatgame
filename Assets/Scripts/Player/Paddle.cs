using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class Paddle : MonoBehaviour, IMouseGrabbable
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

        private bool grabbed = false;
        private Plane controlPlane;
        private Vector3 initPosition;

        private Camera MainCamera => GlobalObjects.MainCamera;

        private void Awake()
        {
            controlPlane = new Plane(Vector3.up, transform.position);
            initPosition = transform.localPosition;
        }
        
        public void Hover()
        {
            mesh.material = hoverMaterial;
        }

        public void Unhover()
        {
            mesh.material = defaultMaterial;
        }

        public void Grab()
        {
            mesh.material = dragMaterial;
            grabbed = true;
            rigidbody.useGravity = false;
        }

        public void Ungrab()
        {
            mesh.material = defaultMaterial;
            grabbed = false;
            rigidbody.useGravity = true;
            transform.localPosition = initPosition;
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
            if (!controlPlane.Raycast(camRay, out var distance))
            {
                Debug.LogWarning("Didn't hit drag raycast, camera under plane?");
                return;
            }
            var pos = camRay.GetPoint(distance);
            var endPos = transform.parent.InverseTransformPoint(pos);
            
            var target = endPos;
            var dir = endPos - initPosition;
            var distanceFromStart = dir.magnitude;
            if (distanceFromStart > maxDragRadius)
                target = initPosition + (dir/distanceFromStart)*maxDragRadius;
            
            transform.localPosition = target;
        }

        private void DragPaddleToHandle()
        {
            var padPos = transform.parent.TransformPoint(initPosition);
            var selfPos = transform.position;
            var dir = selfPos - padPos;
            var rot = Quaternion.LookRotation(dir);
            rigidbody.MoveRotation(rot);
        }
    }
}
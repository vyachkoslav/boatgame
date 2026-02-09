using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class MouseHandler : MonoBehaviour
    {
        [SerializeField] private InputActionReference grabAction;
        private Camera MainCamera => GlobalObjects.MainCamera;
        
        public static MouseHandler Instance { get; private set; }

        public IMouseGrabbable CurrentHovered => currentHovered;
        public IMouseGrabbable CurrentGrabbed => currentGrabbed;

        private IMouseGrabbable currentHovered;
        private IMouseGrabbable currentGrabbed;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            grabAction.action.started += TryGrab;
            grabAction.action.canceled += StopGrab;
        }
        
        private void OnDisable()
        {
            grabAction.action.started -= TryGrab;
            grabAction.action.canceled -= StopGrab;
        }

        private void TryGrab(InputAction.CallbackContext callbackContext)
        {
            if (currentHovered == null) return;
            
            currentGrabbed = currentHovered;
            currentGrabbed.Grab();
            currentHovered = null;
        }

        private void StopGrab(InputAction.CallbackContext callbackContext)
        {
            if (currentGrabbed == null) return;
            
            currentGrabbed.Ungrab();
            currentGrabbed = null;
        }
        
        private void Update()
        {
            UpdateHover();
        }

        private void UpdateHover()
        {
            if (currentGrabbed != null) return;
            
            var camRay = MainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (Physics.Raycast(camRay, out var hit, 100, ~(1<<2), QueryTriggerInteraction.Collide)
                && hit.collider.TryGetComponent(out IMouseGrabbable grabbable))
            {
                ChangeHovered(grabbable);
            }
            else
            {
                ChangeHovered(null);
            }
        }

        private void ChangeHovered(IMouseGrabbable newHovered)
        {
            if (currentHovered == newHovered) return;
            
            currentHovered?.Unhover();
            currentHovered = newHovered;
            currentHovered?.Hover();
        }
    }
}
using UnityEngine;

namespace Player
{
    public class Paddle : MonoBehaviour, IMouseGrabbable
    {
        [Header("Components")]
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Transform pivotParent;

        [Header("Settings")] 
        [SerializeField] private float maxDragRadius;
        [SerializeField] private float airSpeed;
        [SerializeField] private float waterSpeed;
        
        [Header("Visual")]
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        [SerializeField] private Material dragMaterial;

        private bool grabbed = false;
        private int thisLayer;

        private void Awake()
        {
            thisLayer = gameObject.layer;
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
        }

        public void Ungrab()
        {
            mesh.material = defaultMaterial;
            grabbed = false;
        }

        private void Update()
        {
            if (grabbed)
                Drag();
        }
        
        private void Drag()
        {
            Debug.Log("Dragging");
        }
    }
}
using FishNet;
using UnityEngine;

namespace Player
{
    public class BoatMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody boat;
        [SerializeField] private Transform rBlade;
        [SerializeField] private Transform rPaddle;
        [SerializeField] private Transform lBlade;
        [SerializeField] private Transform lPaddle;
        [SerializeField] private float waterDrag;
        
        /// <summary>
        /// When delta angle is less than this value, the boat won't calculate force from paddles
        /// </summary>
        [SerializeField] private float minAngleDelta;
        
        private Vector3 rLast, lLast;
        private float rAngle, lAngle;

        private void Awake()
        {
            rLast = rBlade.position;
            lLast = lBlade.position;
        }

        private void ApplyForceAtPos(Vector3 last, Vector3 cur, float lAng, float cAng)
        {
            if (cur.y < 0 && last.y < 0 && Mathf.Abs(Mathf.DeltaAngle(lAng, cAng)) > minAngleDelta)
            {
                var velocity = (cur - last) / Time.fixedDeltaTime;
                var force = -velocity * waterDrag;
                boat.AddForceAtPosition(force, cur, ForceMode.Force);
            }
        }
        
        private void FixedUpdate()
        {
            if (!InstanceFinder.IsServerStarted) return;
            
            var cur = rBlade.transform.position;
            var angle = rPaddle.transform.localEulerAngles.y;
            ApplyForceAtPos(rLast, cur, rAngle, angle);
            rLast = cur;
            rAngle = angle;
            
            cur = lBlade.transform.position;
            angle = lPaddle.transform.localEulerAngles.y;
            ApplyForceAtPos(lLast, cur, lAngle, angle);
            lLast = cur;
            lAngle = angle;
        }
    }
}
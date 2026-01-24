using System;
using FishNet;
using FishNet.Managing;
using UnityEngine;

namespace Player
{
    public class BoatMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody boat;
        [SerializeField] private Transform rBlade;
        [SerializeField] private Transform lBlade;
        [SerializeField] private float waterDrag;
        
        private Vector3 rLast, lLast;

        private void Awake()
        {
            rLast = rBlade.position;
            lLast = lBlade.position;
        }

        private void ApplyForceAtPos(Vector3 last, Vector3 cur)
        {
            if (cur.y < 0 && last.y < 0) 
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
            ApplyForceAtPos(rLast, cur);
            rLast = cur;
            
            cur = lBlade.transform.position;
            ApplyForceAtPos(lLast, cur);
            lLast = cur;
        }
    }
}
using Network;
using Unity.Cinemachine;
using UnityEngine;

namespace Utility
{
    public class WaitCamera : MonoBehaviour
    {
        // if not waiting reset wait rotation
        private void Start()
        {
            if (WaitForPlayer.IsWaiting) return;
            var orbit = GetComponent<CinemachineOrbitalFollow>();
            orbit.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget;
        }
    }
}
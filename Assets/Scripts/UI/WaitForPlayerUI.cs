using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using Utility;

namespace UI
{
    public class WaitForPlayerUI : MonoBehaviour
    {
        public static WaitForPlayerUI Instance { get; private set; }

        [SerializeField] private Transform uiParent;

        private Camera camera;
        private CinemachineInputAxisController input;
        private CinemachineOrbitalFollow orbit;
        private Coroutine waitRoutine;

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
        }

        public void StartWait()
        {
            if (uiParent.gameObject.activeSelf) return;
            if (GlobalObjects.MainCamera == null)
            {
                waitRoutine ??= StartCoroutine(WaitForCamera());
                return;
            }
            
            uiParent.gameObject.SetActive(true);
            if (camera == null)
            {
                camera = GlobalObjects.MainCamera;
                input = camera.GetComponent<CinemachineInputAxisController>();
                orbit = camera.GetComponent<CinemachineOrbitalFollow>();
            }

            input.enabled = false;
            if (orbit.RecenteringTarget != CinemachineOrbitalFollow.ReferenceFrames.AxisCenter)
            {
                orbit.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.AxisCenter;
                orbit.HorizontalAxis.Center -= 150f;
            }

            orbit.VerticalAxis.Recentering.Enabled = true;
            orbit.RadialAxis.Recentering.Enabled = true;
        }

        private IEnumerator WaitForCamera()
        {
            while (GlobalObjects.MainCamera == null) yield return null;
            waitRoutine = null;
            StartWait();
        }

        public void StopWait()
        {
            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                waitRoutine = null;
            }

            if (!uiParent.gameObject.activeSelf) return;
            
            uiParent.gameObject.SetActive(false);
            input.enabled = true;
            orbit.RecenteringTarget = CinemachineOrbitalFollow.ReferenceFrames.TrackingTarget;
            orbit.HorizontalAxis.Center = 0;
            orbit.VerticalAxis.Recentering.Enabled = false;
            orbit.RadialAxis.Recentering.Enabled = false;
            
            TutorialUI.StartTutorial();
        }
    }
}
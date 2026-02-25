using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private float victorySequenceDuration = 10f;
    
    [Header("Camera Targets")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform damViewTarget;
    
    [Header("Pan Settings")]
    [SerializeField] private float panDuration = 8f;
    [SerializeField] private AnimationCurve panCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Player Control")]
    [SerializeField] private GameObject playerBoat;
    [SerializeField] private MonoBehaviour cameraControlScript;
    
    private bool isActive = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform originalCameraParent;
    private bool cameraWasChild = false;
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            TriggerVictory();
        }
    }
    
    public void TriggerVictory()
    {
        if (isActive) return;
        isActive = true;
        
        // Disable player camera control
        if (cameraControlScript != null)
            cameraControlScript.enabled = false;
        
        // Detach camera from boat
        if (mainCamera != null)
        {
            originalCameraParent = mainCamera.transform.parent;
            if (originalCameraParent != null)
            {
                cameraWasChild = true;
                mainCamera.transform.SetParent(null);
            }
            
            startPosition = mainCamera.transform.position;
            startRotation = mainCamera.transform.rotation;
        }
        
        StartCoroutine(VictorySequence());
    }
    
    IEnumerator VictorySequence()
    {
        float elapsedTime = 0f;
        
        // Show victory text
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
        
        // Pan camera to dam view
        while (elapsedTime < panDuration && damViewTarget != null)
        {
            elapsedTime += Time.deltaTime;
            float t = panCurve.Evaluate(elapsedTime / panDuration);
            
            if (mainCamera != null)
            {
                mainCamera.transform.position = Vector3.Lerp(startPosition, damViewTarget.position, t);
                mainCamera.transform.rotation = Quaternion.Slerp(startRotation, damViewTarget.rotation, t);
            }
            
            yield return null;
        }
        if (mainCamera != null && damViewTarget != null)
        {
            mainCamera.transform.position = damViewTarget.position;
            mainCamera.transform.rotation = damViewTarget.rotation;
        }
        
        float remainingTime = victorySequenceDuration - panDuration;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }
        
        // Hide text
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        
        Debug.Log("Victory sequence complete - Camera now showing dam");
    }
}
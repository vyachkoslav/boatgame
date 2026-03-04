using UnityEngine;
using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using TMPro;

public class VictoryScreen : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private float victorySequenceDuration = 10f;
    
    [Header("Camera Targets")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform damViewTarget;
    
    [Header("Pan Settings")]
    [SerializeField] private float panDuration = 8f;
    [SerializeField] private AnimationCurve panCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Delay")]
    [SerializeField] private float startDelay = 2f;
    
    [Header("Text Settings")]
    [SerializeField] private float typingSpeed = 0.005f;
    
    [Header("Player Control")]
    [SerializeField] private GameObject playerBoat;
    [SerializeField] private MonoBehaviour cameraControlScript;
    
    private readonly SyncVar<bool> isVictoryActive = new SyncVar<bool>();
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform originalCameraParent;
    private string victoryMessage = "You win!";
    private bool hasStarted = false;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        isVictoryActive.OnChange += OnVictoryStateChanged;
    }
    
    private void OnDestroy()
    {
        isVictoryActive.OnChange -= OnVictoryStateChanged;
    }
    
    public void TriggerVictory()
    {
        if (isVictoryActive.Value) return;
        ServerTriggerVictory();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ServerTriggerVictory()
    {
        isVictoryActive.Value = true;
    }
    
    private void OnVictoryStateChanged(bool prev, bool next, bool asServer)
    {
        if (next && !hasStarted)
        {
            hasStarted = true;
            StartCoroutine(VictorySequence());
        }
    }
    
    IEnumerator VictorySequence()
    {
        // Add delay before anything happens
        yield return new WaitForSeconds(startDelay);
        
        float elapsedTime = 0f;
        
        if (cameraControlScript != null)
            cameraControlScript.enabled = false;
        
        if (mainCamera != null)
        {
            originalCameraParent = mainCamera.transform.parent;
            if (originalCameraParent != null)
            {
                mainCamera.transform.SetParent(null);
            }
            
            startPosition = mainCamera.transform.position;
            startRotation = mainCamera.transform.rotation;
        }
        
        // Show panel and type out text
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            // Type out the victory text
            if (victoryText != null)
            {
                victoryText.text = "";
                foreach (char c in victoryMessage)
                {
                    victoryText.text += c;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
        }
        
        if (damViewTarget == null)
            yield break;
        
        // Start camera pan while text is fully displayed
        while (elapsedTime < panDuration)
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
        
        float remainingTime = victorySequenceDuration - panDuration - (victoryMessage.Length * typingSpeed);
        if (remainingTime > 0)
            yield return new WaitForSeconds(remainingTime);
    }
}
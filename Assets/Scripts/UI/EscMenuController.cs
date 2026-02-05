using FishNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class EscMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Main Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;
    
    [Header("Category Buttons")]
    [SerializeField] private Button audioButton;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Button controlsButton;
    
    [Header("Category Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject controlsPanel;
    
    private bool isMenuOpen = false;
    private Keyboard keyboard;
    private GameObject currentlyOpenPanel = null;
    
    void Start()
    {
        keyboard = Keyboard.current;
        
        // Set initial states
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        CloseAllCategoryPanels();
        
        // Setup button actions
        resumeButton.onClick.AddListener(CloseMenu);
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        quitButton.onClick.AddListener(QuitGame);
        
        // Category buttons
        audioButton.onClick.AddListener(() => ToggleDropdown(audioPanel));
        graphicsButton.onClick.AddListener(() => ToggleDropdown(graphicsPanel));
        controlsButton.onClick.AddListener(() => ToggleDropdown(controlsPanel));
    }
    
    void Update()
    {
        if (keyboard == null) return;
        
        // Toggle menu with ESC
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }
    
    void OpenMenu()
    {
        isMenuOpen = true;
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        CloseAllCategoryPanels();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    void CloseMenu()
    {
        isMenuOpen = false;
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        CloseAllCategoryPanels();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        CloseAllCategoryPanels();
    }
    
    void CloseSettings()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
        CloseAllCategoryPanels();
    }
    
    void ToggleDropdown(GameObject panel)
    {
        if (panel == null) return;
        
        // If clicking the same panel that's already open, close it
        if (currentlyOpenPanel == panel)
        {
            panel.SetActive(false);
            currentlyOpenPanel = null;
        }
        else
        {
            // Close any open panel first
            if (currentlyOpenPanel != null)
            {
                currentlyOpenPanel.SetActive(false);
            }
            
            // Open the clicked panel
            panel.SetActive(true);
            currentlyOpenPanel = panel;
        }
    }
    
    void CloseAllCategoryPanels()
    {
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        currentlyOpenPanel = null;
    }
    
    void QuitGame()
    {
        Debug.Log("Quit to menu");
        InstanceFinder.ClientManager.StopConnection();
        InstanceFinder.ServerManager.StopConnection(true);
    }
}
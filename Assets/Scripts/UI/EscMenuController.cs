using FishNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Network;

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
    [SerializeField] private Button resolutionButton;
    [SerializeField] private Button controlsButton;
    
    [Header("Category Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject resolutionPanel;
    [SerializeField] private GameObject controlsPanel;
    
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    
    [Header("Graphics")]
    [SerializeField] private Slider graphicsSlider;
    
    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    [Header("Window Mode")]
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    
    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;
    
    [Header("Paddle References")]
    [SerializeField] private PaddlePrediction leftPaddle;
    [SerializeField] private PaddlePrediction rightPaddle;
    
    private Resolution[] resolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 },
        new Resolution { width = 1280, height = 720 }
    };
    
    private bool isMenuOpen = false;
    private Keyboard keyboard;
    private GameObject currentlyOpenPanel = null;
    
    void Start()
    {
        keyboard = Keyboard.current;
        
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        CloseAllCategoryPanels();
        
        resumeButton.onClick.AddListener(CloseMenu);
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        quitButton.onClick.AddListener(QuitGame);
        
        audioButton.onClick.AddListener(() => ToggleDropdown(audioPanel));
        graphicsButton.onClick.AddListener(() => ToggleDropdown(graphicsPanel));
        resolutionButton.onClick.AddListener(() => ToggleDropdown(resolutionPanel));
        controlsButton.onClick.AddListener(() => ToggleDropdown(controlsPanel));
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (graphicsSlider != null)
            graphicsSlider.onValueChanged.AddListener(OnGraphicsQualityChanged);
        
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        if (windowModeDropdown != null)
            windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
        
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        
        LoadSettings();
    }
    
    void Update()
    {
        if (keyboard == null) return;
        
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
    
    public void OnGraphicsQualityChanged(float value)
    {
        int qualityLevel = Mathf.RoundToInt(value);
        QualitySettings.SetQualityLevel(qualityLevel, true);
        PlayerPrefs.SetInt("GraphicsQuality", qualityLevel);
    }
    
    public void OnResolutionChanged(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
            PlayerPrefs.SetInt("ResolutionIndex", index);
        }
    }
    
    public void OnWindowModeChanged(int index)
    {
        switch(index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.fullScreen = true;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
                if (resolutionDropdown != null)
                {
                    Resolution res = resolutions[resolutionDropdown.value];
                    Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed);
                }
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
        }
        PlayerPrefs.SetInt("WindowMode", index);
    }
    
    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        
        if (leftPaddle != null)
            leftPaddle.SetMouseSensitivity(value);
        
        if (rightPaddle != null)
            rightPaddle.SetMouseSensitivity(value);
    }
    
private void LoadSettings()
{
    if (masterVolumeSlider != null)
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        masterVolumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
    }
    
    if (sfxVolumeSlider != null)
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
    
    if (musicVolumeSlider != null)
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
    
    if (graphicsSlider != null)
    {
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 1);
        graphicsSlider.value = savedQuality;
        QualitySettings.SetQualityLevel(savedQuality, true);
    }
    
    if (windowModeDropdown != null)
    {
        int savedMode = PlayerPrefs.GetInt("WindowMode", 0);
        windowModeDropdown.value = savedMode;
        
        switch(savedMode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.fullScreen = true;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.fullScreen = false;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.fullScreen = true;
                break;
        }
    }
    
    if (resolutionDropdown != null)
    {
        int savedRes = PlayerPrefs.GetInt("ResolutionIndex", 0);
        resolutionDropdown.value = savedRes;
        Resolution res = resolutions[savedRes];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }
    
    if (sensitivitySlider != null)
    {
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.01f);
        sensitivitySlider.value = savedSensitivity;
        
        if (leftPaddle != null)
            leftPaddle.SetMouseSensitivity(savedSensitivity);
        
        if (rightPaddle != null)
            rightPaddle.SetMouseSensitivity(savedSensitivity);
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

        if (InstanceFinder.IsClientStarted)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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
        
        if (currentlyOpenPanel == panel)
        {
            panel.SetActive(false);
            currentlyOpenPanel = null;
        }
        else
        {
            if (currentlyOpenPanel != null)
                currentlyOpenPanel.SetActive(false);
            
            panel.SetActive(true);
            currentlyOpenPanel = panel;
        }
    }
    
    void CloseAllCategoryPanels()
    {
        if (audioPanel != null) audioPanel.SetActive(false);
        if (graphicsPanel != null) graphicsPanel.SetActive(false);
        if (resolutionPanel != null) resolutionPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        currentlyOpenPanel = null;
    }
    
    void QuitGame()
    {
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.StopConnection(true);
        
        if (InstanceFinder.IsClientStarted)
            InstanceFinder.ClientManager.StopConnection();
    }
}
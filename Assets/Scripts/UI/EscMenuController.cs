using System;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Utility;

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
    
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider voiceOutVolumeSlider;
    [SerializeField] private Slider voiceInVolumeSlider;
    
    [Header("Graphics")]
    [SerializeField] private Slider graphicsSlider;
    
    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    [Header("Window Mode")]
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    
    [Header("Controls")]
    [SerializeField] private Slider sensitivitySlider;

    private List<Resolution> resolutions = new();
    
    private bool isMenuOpen = false;
    private Keyboard keyboard;

    private const int Fullscreen = 0;
    private const int Windowed = 1;
    private const int WindowedFullscreen = 2;

    private bool Equals(Resolution x, Resolution y)
    {
        return x.width == y.width && x.height == y.height;
    }
    
    void Start()
    {
        foreach (var res in Screen.resolutions)
        {
            if (resolutions.Count > 0 && Equals(resolutions[^1], res))
            {
                if (res.refreshRateRatio.CompareTo(resolutions[^1].refreshRateRatio) == 1)
                    resolutions[^1] = res;
                continue;
            }
            resolutions.Add(res);
        }
        // higher res on top
        resolutions.Reverse();
        
        resolutionDropdown.options.Clear();
        var currentRes = Screen.currentResolution;
        foreach (var res in  resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{res.width} x {res.height}"));
            if (Equals(currentRes, res) && currentRes.refreshRateRatio.Equals(res.refreshRateRatio))
                resolutionDropdown.SetValueWithoutNotify(resolutionDropdown.options.Count - 1);
        }
        
        keyboard = Keyboard.current;
        
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        resumeButton.onClick.AddListener(CloseMenu);
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        quitButton.onClick.AddListener(QuitGame);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        
        if (voiceInVolumeSlider != null)
            voiceInVolumeSlider.onValueChanged.AddListener(OnVoiceInVolumeChanged);
        
        if (voiceOutVolumeSlider != null)
            voiceOutVolumeSlider.onValueChanged.AddListener(OnVoiceOutVolumeChanged);
        
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
        resolutionDropdown.interactable = Screen.fullScreenMode is FullScreenMode.ExclusiveFullScreen or FullScreenMode.Windowed;
        var mode = Screen.fullScreenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => Fullscreen,
            FullScreenMode.FullScreenWindow => WindowedFullscreen,
            FullScreenMode.MaximizedWindow => Windowed,
            FullScreenMode.Windowed => Windowed,
            _ => throw new ArgumentOutOfRangeException()
        };
        windowModeDropdown.SetValueWithoutNotify(mode);
        
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
    
    public void OnVoiceInVolumeChanged(float value)
    {
        PlayerPrefs.SetInt("VoiceInVolume", (int)value);
        Settings.VoiceVolumeIn = (int)value;
    }
    
    public void OnVoiceOutVolumeChanged(float value)
    {
        PlayerPrefs.SetInt("VoiceOutVolume", (int)value);
        Settings.VoiceVolumeOut = (int)value;
    }
    
    public void OnGraphicsQualityChanged(float value)
    {
        int qualityLevel = Mathf.RoundToInt(value);
        QualitySettings.SetQualityLevel(qualityLevel, true);
        PlayerPrefs.SetInt("GraphicsQuality", qualityLevel);
    }
    
    public void OnResolutionChanged(int index)
    {
        if (index >= 0 && index < resolutions.Count)
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
                break;
            case 1:
                if (resolutionDropdown != null)
                {
                    Resolution res = resolutions[resolutionDropdown.value];
                    Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed);
                }
                break;
            case 2:
                Screen.SetResolution(resolutions[0].width, resolutions[0].height, FullScreenMode.FullScreenWindow);
                break;
        }
        PlayerPrefs.SetInt("WindowMode", index);
    }
    
    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        Settings.Sensitivity = value;
    }
    
private void LoadSettings()
{
    if (masterVolumeSlider != null)
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        masterVolumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
    }
    
    if (voiceInVolumeSlider != null)
        voiceInVolumeSlider.value = PlayerPrefs.GetInt("VoiceInVolume", 0);
    
    if (voiceOutVolumeSlider != null)
        voiceOutVolumeSlider.value = PlayerPrefs.GetInt("VoiceOutVolume", 0);
    
    if (graphicsSlider != null)
    {
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 3);
        graphicsSlider.value = savedQuality;
        QualitySettings.SetQualityLevel(savedQuality, true);
    }
    
    if (windowModeDropdown != null)
    {
        int savedMode = PlayerPrefs.GetInt("WindowMode", WindowedFullscreen);
        windowModeDropdown.value = savedMode;
        
        switch(savedMode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.SetResolution(resolutions[0].width, resolutions[0].height, FullScreenMode.FullScreenWindow);
                break;
        }
    }
    
    if (resolutionDropdown != null && Screen.fullScreenMode is not FullScreenMode.FullScreenWindow)
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
        Settings.Sensitivity = savedSensitivity;
    }
}
    
    void OpenMenu()
    {
        isMenuOpen = true;
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    void CloseMenu()
    {
        isMenuOpen = false;
        menuPanel.SetActive(false);
        settingsPanel.SetActive(false);

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
    }
    
    void CloseSettings()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    
    void QuitGame()
    {
        if (InstanceFinder.IsServerStarted)
            InstanceFinder.ServerManager.StopConnection(true);
        
        if (InstanceFinder.IsClientStarted)
            InstanceFinder.ClientManager.StopConnection();
    }
}
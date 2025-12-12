using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public Button dashButton;
    public Button attackButton;
    public Button helpButton;
    public Button saveButton;
    public Button loadDefaultsButton;
    public Button backButton;
    public GameObject escMenu;
    public Image waitingImage;
    public bool canSave = true;
    public static bool isFromMainMenu = false;
    private bool keyAssigned = true;
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI musicValueText;
    public TextMeshProUGUI sfxValueText;
    public Button ControllsButton;
    public Button AudioButton;
    public GameObject ControllsPanel;
    public GameObject AudioPanel;
    private SaveDataSound savedSoundSettings;
    private bool isLoading = false;
    public void Start()
    {
        isLoading = true;
        upButton.onClick.AddListener(() => StartRebinding(ActionKey.MoveUp));
        UpdateText(ActionKey.MoveUp, "");
        downButton.onClick.AddListener(() => StartRebinding(ActionKey.MoveDown));
        UpdateText(ActionKey.MoveDown, "");
        leftButton.onClick.AddListener(() => StartRebinding(ActionKey.MoveLeft));
        UpdateText(ActionKey.MoveLeft, "");
        rightButton.onClick.AddListener(() => StartRebinding(ActionKey.MoveRight));
        UpdateText(ActionKey.MoveRight, "");
        dashButton.onClick.AddListener(() => StartRebinding(ActionKey.Dash));
        UpdateText(ActionKey.Dash, "");
        attackButton.onClick.AddListener(() => StartRebinding(ActionKey.Attack));
        UpdateText(ActionKey.Attack, "");
        helpButton.onClick.AddListener(() => StartRebinding(ActionKey.Help));
        UpdateText(ActionKey.Help, "");
        saveButton.onClick.AddListener(() => SaveKeyBinds());
        loadDefaultsButton.onClick.AddListener(() => LoadDefaults());
        backButton.onClick.AddListener(() => BackToEscMenu());
        if (!ControlsManager.Instance.soundIsLoaded)
        {
            float musicVolume = PlayerPrefs.GetFloat("Music", 0.5f);
            float sfxVolume = PlayerPrefs.GetFloat("SFX", 0.5f);
            musicVolume = Mathf.Clamp(musicVolume, 0.001f, 1f);
            sfxVolume = Mathf.Clamp(sfxVolume, 0.001f, 1f);
            musicSlider.value = musicVolume;
            sfxSlider.value = sfxVolume;
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);  
        } else 
        {
            musicSlider.value = ControlsManager.Instance.savedSoundSettings.musicVolume;
            sfxSlider.value = ControlsManager.Instance.savedSoundSettings.sfxVolume;
        }
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        isLoading = false; 
        ControllsButton.onClick.AddListener(() => ShowControlls());
        AudioButton.onClick.AddListener(() => ShowAudio());
        ShowControlls();
    }

    void Update()
    {
        if (ControlsManager.Instance.DupicateInDictionary())
        {
            canSave = false;
        }
        else
        {
            canSave = true;
        }
        saveButton.interactable = canSave;
        if (keyAssigned && Input.GetKeyDown(KeyCode.Escape))
        {
            BackToEscMenu();
        }
        musicValueText.text = $"Music: {(musicSlider.value * 100).ToString("0")}%";
        sfxValueText.text = $"SFX: {(sfxSlider.value * 100).ToString("0")}%";
    }

    private void OnEnable()
    {
        ShowControlls();
    }

    private void StartRebinding(ActionKey actionKey)
    {
        StartCoroutine(RebindKey(actionKey));
    }

    IEnumerator RebindKey(ActionKey actionKey)
    {
        keyAssigned = false;
        waitingImage.gameObject.SetActive(true);
        UpdateText(actionKey, "Waiting...");
        while (!keyAssigned)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && keyCode != KeyCode.Escape)
                {
                    ControlsManager.Instance.RebindKey(actionKey, keyCode);
                    UpdateText(actionKey, "");
                    keyAssigned = true;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UpdateText(actionKey, "");
                    keyAssigned = true;
                    break;
                }
            }
            yield return null;
        }
        waitingImage.gameObject.SetActive(false);
    }

    private void LoadDefaults()
    {
        ControlsManager.Instance.LoadDefaults();
        UpdateText(ActionKey.MoveUp, "");
        UpdateText(ActionKey.MoveDown, "");
        UpdateText(ActionKey.MoveLeft, "");
        UpdateText(ActionKey.MoveRight, "");
        UpdateText(ActionKey.Dash, "");
        UpdateText(ActionKey.Attack, "");
        UpdateText(ActionKey.Help, "");
        SaveKeyBinds();
    }
    private void SaveKeyBinds()
    {
        ControlsManager.Instance.SaveKeyBinds();
    }

    private void UpdateText(ActionKey actionKey, string keyName)
    {
        if (keyName == "")
        {
            keyName = ControlsManager.Instance.GetKeyName(actionKey);
        }
        switch (actionKey)
        {
            case ActionKey.MoveUp:
                upButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.MoveDown:
                downButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.MoveLeft:
                leftButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.MoveRight:
                rightButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.Dash:
                dashButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.Attack:
                attackButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
            case ActionKey.Help:
                helpButton.GetComponentInChildren<TextMeshProUGUI>().text = keyName;
                break;
        }
    }
    public void BackToEscMenu()
    {
        ControlsManager.Instance.SaveSoundSettings(musicSlider.value, sfxSlider.value);
        if (isFromMainMenu)
        {
            GameModeManager.isInSettingsMenu = false;
            this.gameObject.SetActive(false);
            isFromMainMenu = false;
            return;
        }else 
        {
            GameModeManager.isInSettingsMenu = false;
            escMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (isLoading) return;
        value = Mathf.Clamp(value, 0.001f, 1f);
        mainMixer.SetFloat("Music", Mathf.Log10(value) * 50);
        PlayerPrefs.SetFloat("Music", value);
    }
    public void SetSFXVolume(float value)
    {
        if (isLoading) return;
        value = Mathf.Clamp(value, 0.001f, 1f);
        mainMixer.SetFloat("SFX", Mathf.Log10(value) * 50);
        PlayerPrefs.SetFloat("SFX", value);
    }

    public void ShowControlls()
    {
        ControllsPanel.SetActive(true);
        AudioPanel.SetActive(false);
    }
    public void ShowAudio()
    {
        ControllsPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }

}

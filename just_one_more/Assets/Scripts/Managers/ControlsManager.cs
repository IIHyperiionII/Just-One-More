using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Audio;

public class ControlsManager : MonoBehaviour
{  
    public static ControlsManager Instance;

    private Dictionary<ActionKey, KeyCode> keyBindings;
    public SaveDataKeyBinds savedKeyBinds;
    public SaveDataSound savedSoundSettings;
    public AudioMixer mainMixer;
    public bool soundIsLoaded = false;
    private string fileName = "keyBindsSave.json";
    private string fileNameSounds = "soundSettings.json";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (keyBindings == null)
        {
            keyBindings = new Dictionary<ActionKey, KeyCode>();
        }
        LoadSaved();
        LoadSoundSettings();
    }

    public void LoadDefaults()
    {
        keyBindings[ActionKey.MoveUp] = KeyCode.W;
        keyBindings[ActionKey.MoveDown] = KeyCode.S;
        keyBindings[ActionKey.MoveLeft] = KeyCode.A;
        keyBindings[ActionKey.MoveRight] = KeyCode.D;
        keyBindings[ActionKey.Attack] = KeyCode.Mouse0;
        keyBindings[ActionKey.Dash] = KeyCode.Space;
        keyBindings[ActionKey.Help] = KeyCode.H;
    }

    string GetFilePath(string fileName)
    {
        string folderPath = Application.persistentDataPath;

        if (!Application.isEditor)
        {
            folderPath = Path.Combine(folderPath, "BuildSaves");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, fileName);
    }

    public void SaveKeyBinds()
    {
        savedKeyBinds.moveUp = keyBindings[ActionKey.MoveUp];
        savedKeyBinds.moveDown = keyBindings[ActionKey.MoveDown];
        savedKeyBinds.moveLeft = keyBindings[ActionKey.MoveLeft];
        savedKeyBinds.moveRight = keyBindings[ActionKey.MoveRight];
        savedKeyBinds.dash = keyBindings[ActionKey.Dash];
        savedKeyBinds.attack = keyBindings[ActionKey.Attack];

        string savePath = GetFilePath(fileName);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        // Convert the SaveData object to JSON
        string json = JsonUtility.ToJson(savedKeyBinds, true);

        // Write JSON to file
        File.WriteAllText(savePath, json);

    }
    public void LoadSaved()
    {
        string savePath = GetFilePath(fileName);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            savedKeyBinds = JsonUtility.FromJson<SaveDataKeyBinds>(json);

            keyBindings = new Dictionary<ActionKey, KeyCode>
            {
                { ActionKey.MoveUp, savedKeyBinds.moveUp },
                { ActionKey.MoveDown, savedKeyBinds.moveDown },
                { ActionKey.MoveLeft, savedKeyBinds.moveLeft },
                { ActionKey.MoveRight, savedKeyBinds.moveRight },
                { ActionKey.Dash, savedKeyBinds.dash },
                { ActionKey.Attack, savedKeyBinds.attack }
            };
        }
        else
        {
            LoadDefaults();
        }
    }

    public float GetAxisHorizontal()
    {
        float axis = 0f;
        if (Input.GetKey(keyBindings[ActionKey.MoveRight]))
        {
            axis += 1f;
        }
        if (Input.GetKey(keyBindings[ActionKey.MoveLeft]))
        {
            axis -= 1f;
        }
        return axis;
    }
    public float GetAxisVertical()
    {
        float axis = 0f;
        if (Input.GetKey(keyBindings[ActionKey.MoveUp]))
        {
            axis += 1f;
        }
        if (Input.GetKey(keyBindings[ActionKey.MoveDown]))
        {
            axis -= 1f;
        }
        return axis;
    }
    public bool GetDashInputDown()
    {
        return Input.GetKeyDown(keyBindings[ActionKey.Dash]);
    }
    public bool GetAttackInputDown()
    {
        return Input.GetKeyDown(keyBindings[ActionKey.Attack]);
    }
    public bool GetAttackInput()
    {
        return Input.GetKey(keyBindings[ActionKey.Attack]);
    }
    public string GetKeyName(ActionKey actionKey)
    {
        return keyBindings[actionKey].ToString();
    }
    public void RebindKey(ActionKey actionKey, KeyCode newKey)
    {
        keyBindings[actionKey] = newKey;
    }
    public bool DupicateInDictionary()
    {
        HashSet<KeyCode> seenKeys = new HashSet<KeyCode>();
        foreach (var keyBinding in keyBindings)
        {
            if (seenKeys.Contains(keyBinding.Value))
            {
                return true;
            }
            seenKeys.Add(keyBinding.Value);
        }
        return false;
    }

    public void SaveSoundSettings(float musicVolume, float sfxVolume)
    {
        savedSoundSettings.musicVolume = musicVolume;
        savedSoundSettings.sfxVolume = sfxVolume;

        string savePath = GetFilePath(fileNameSounds);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        string json = JsonUtility.ToJson(savedSoundSettings, true);

        File.WriteAllText(savePath, json);
    }

    public void LoadSoundSettings()
    {
        string savePath = GetFilePath(fileNameSounds);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            savedSoundSettings = JsonUtility.FromJson<SaveDataSound>(json);
            SetMusicVolume(savedSoundSettings.musicVolume);
            SetSFXVolume(savedSoundSettings.sfxVolume);
            soundIsLoaded = true;
        }
    }
    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.001f, 1f);
        mainMixer.SetFloat("Music", Mathf.Log10(value) * 50);
        PlayerPrefs.SetFloat("Music", value);
    }
    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp(value, 0.001f, 1f);
        mainMixer.SetFloat("SFX", Mathf.Log10(value) * 50);
        PlayerPrefs.SetFloat("SFX", value);
    }

}

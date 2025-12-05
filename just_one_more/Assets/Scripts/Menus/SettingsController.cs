using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
    private bool keyAssigned = false;

    public void Start()
    {
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
        if (isFromMainMenu)
        {
            GameModeManager.isInSettingsMenu = false;
            this.transform.parent.gameObject.SetActive(false);
            isFromMainMenu = false;
            return;
        }else 
        {
            GameModeManager.isInSettingsMenu = false;
            escMenu.SetActive(true);
            this.transform.parent.gameObject.SetActive(false);
        }
    }

}

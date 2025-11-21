using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine.UI;

public class PlayerHudController : MonoBehaviour
{
    [Header("References")]
    private PlayerData playerData;
    private GameObject gameManager;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI msText;
    public TextMeshProUGUI asText;
    public Sprite[] numbers;
    private Dictionary<int, GameObject> numberSprites = new Dictionary<int, GameObject>();
    public GameObject numbersParent;
    private ModeAndWeaponSelection currentSelection;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameController") == null)
        {
            Debug.LogWarning("PlayerHudController: GameManager reference is missing!");
        } else 
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            playerData = gameManager.GetComponent<GameManager>().runtimePlayerData;
        }
        currentSelection = ModeController.Instance.currentSelection;
    }

    void Update()
    {
        if (playerData != null)
        {
            if (playerData.hp > 0){
                if (currentSelection.selectedMode == GameMode.OneShot)
                {
                    playerData.hp = 1;
                }
                GetHp();
            }
            healthText.text = $"Health: {playerData.hp}";
            coinsText.text = $"{playerData.money}";
            msText.text = $"MS: {playerData.moveSpeed}";
            asText.text = $"AS: {playerData.attackSpeed}";
        }
    }
    void GetHp()
    {
        int playerHp = playerData.hp;
        int index = 1;
        int digit;
        bool willContinue = true;
        while (willContinue)
        {
            digit = playerHp % 10;
            if (playerHp < 10)
            {
                willContinue = false;
                if (digit == 0)
                {
                    break;
                }
            }
            if (numberSprites.ContainsKey(index) )
            {
                numberSprites[index].GetComponent<Image>().sprite = numbers[digit];
            } else {
                GameObject number = new GameObject();
                number.AddComponent<Image>();
                number.GetComponent<Image>().sprite = numbers[digit];
                number.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
                numberSprites[index] = number;
                number.transform.SetParent(numbersParent.transform);
            }
            playerHp /= 10;
            index ++;
        }
        foreach (KeyValuePair<int, GameObject> entry in numberSprites)
        {
            if (entry.Key >= index)
            {
                entry.Value.SetActive(false);
            } else {
                entry.Value.SetActive(true);
            }
        }
        int iterator = 0;
        for (int i = index-1; i > 0; i --)
        {
            if (numberSprites.ContainsKey(i))
            {
                numberSprites[i].transform.position = new Vector3(numbersParent.transform.position.x + (numberSprites[i].GetComponent<Image>().sprite.rect.size.x/3f) * iterator, numbersParent.transform.position.y, numbersParent.transform.position.z);
            }
        
            iterator++;

        }
    }
}

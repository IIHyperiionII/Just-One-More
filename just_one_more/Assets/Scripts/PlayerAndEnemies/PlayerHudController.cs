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
    public Sprite[] numbersCards;
    private Dictionary<int, GameObject> numberCardsSprites = new Dictionary<int, GameObject>();
    public GameObject numbersCardsParent;
    public Sprite[] numbersMoney;
    private Dictionary<int, GameObject> numberMoneySprites = new Dictionary<int, GameObject>();
    public GameObject numbersMoneyParent;
    private ModeAndWeaponSelection currentSelection;
    private float screenWidth;
    private float defaultScreenWidth = 1920f;
    private float screenWidthDifference;

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
        if (GameModeManager.timeIsPaused) return;
        screenWidth = Screen.width;
        screenWidthDifference = screenWidth / defaultScreenWidth;
        Debug.Log($"Screen width: {screenWidth}, Screen default width: {defaultScreenWidth}, Difference: {screenWidthDifference}");
        if (playerData != null)
        {
            if (playerData.hp > 0){
                if (currentSelection.selectedMode == GameMode.OneShot)
                {
                    playerData.hp = 1;
                }
                GetHp();
            }
            GetMoney();
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
            if (numberCardsSprites.ContainsKey(index) )
            {
                numberCardsSprites[index].GetComponent<Image>().sprite = numbersCards[digit];
            } else {
                GameObject numberCard = new GameObject();
                numberCard.AddComponent<Image>();
                numberCard.GetComponent<Image>().sprite = numbersCards[digit];
                numberCard.GetComponent<RectTransform>().sizeDelta = new Vector2(256 * screenWidthDifference, 256 * screenWidthDifference);
                numberCardsSprites[index] = numberCard;
                numberCard.transform.SetParent(numbersCardsParent.transform);
            }
            playerHp /= 10;
            index ++;
        }
        foreach (KeyValuePair<int, GameObject> entry in numberCardsSprites)
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
            if (numberCardsSprites.ContainsKey(i))
            {
                numberCardsSprites[i].transform.position = new Vector3(numbersMoneyParent.transform.position.x + 95f * iterator * screenWidthDifference, numbersCardsParent.transform.position.y, numbersCardsParent.transform.position.z);
            }
        
            iterator++;

        }
    }
    void GetMoney()
    {
        int playerMoney = playerData.money;
        int index = 1;
        int digit;
        bool willContinue = true;
        while (willContinue)
        {
            digit = playerMoney % 10;
            if (playerMoney < 10)
            {
                willContinue = false;
                if (digit == 0)
                {
                    break;
                }
            }
            if (numberMoneySprites.ContainsKey(index) )
            {
                numberMoneySprites[index].GetComponent<Image>().sprite = numbersMoney[digit];
            } else {
                GameObject numberMoney = new GameObject();
                numberMoney.AddComponent<Image>();
                numberMoney.GetComponent<Image>().sprite = numbersMoney[digit];
                numberMoney.GetComponent<RectTransform>().sizeDelta = new Vector2(70f * screenWidthDifference, 128f * screenWidthDifference);
                numberMoneySprites[index] = numberMoney;
                numberMoney.transform.SetParent(numbersMoneyParent.transform);
            }
            playerMoney /= 10;
            index ++;
        }
        foreach (KeyValuePair<int, GameObject> entry in numberMoneySprites)
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
            if (numberMoneySprites.ContainsKey(i))
            {
                numberMoneySprites[i].transform.position = new Vector3(numbersMoneyParent.transform.position.x + 60f * iterator * screenWidthDifference, numbersMoneyParent.transform.position.y, numbersMoneyParent.transform.position.z);
            }
        
            iterator++;

        }
    }
}

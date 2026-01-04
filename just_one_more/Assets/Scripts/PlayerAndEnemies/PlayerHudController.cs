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
    public Sprite dollarSignSprite;
    GameObject dollarSign;
    private Dictionary<int, GameObject> numberMoneySprites = new Dictionary<int, GameObject>();
    public GameObject numbersMoneyParent;
    private ModeAndWeaponSelection currentSelection;
    private float screenWidth;
    private float defaultScreenWidth = 1920f;
    private float screenWidthDifference;
    private bool isFirst = true;
    public Sprite[] needToGambleBarSprites;
    public Sprite[] needToGambleHeadSprites;
    public GameObject needToGambleBarImage;
    public GameObject needToGambleHeadImage;
    public int needToGambleLevel = 0;
    public float multiplier = 0.75f;
    private bool isFirstSign = true;

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
        dollarSign = new GameObject("DollarSign");
    }

    void Update()
    {
        if (GameModeManager.timeIsPaused) return;
        playerData = GameManager.Instance.runtimePlayerData;
        // Adjust for screen width
        screenWidth = Screen.width;
        screenWidthDifference = screenWidth / defaultScreenWidth; // Calculate the difference ratio
        if (playerData != null)
        {
            if (playerData.hp > 0){
                if (currentSelection.selectedMode == GameMode.OneShot)
                {
                    playerData.hp = 1;
                }
                GetHp();
            }
            if (!(ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife))
            {
                GetMoney();
            }
            healthText.text = $"Health: {playerData.hp}";
            coinsText.text = $"{playerData.money}";
            msText.text = $"MS: {playerData.moveSpeed}";
            asText.text = $"AS: {playerData.attackSpeed}";
        }
        // Need to gamble bar and head update
        needToGambleLevel = playerData.needToGamble/10;
        needToGambleBarImage.GetComponent<Image>().sprite = needToGambleBarSprites[needToGambleLevel];
        if (needToGambleLevel < 5)
        {
            needToGambleHeadImage.GetComponent<Image>().sprite = needToGambleHeadSprites[0];
        } else if (needToGambleLevel < 7)
        {
            needToGambleHeadImage.GetComponent<Image>().sprite = needToGambleHeadSprites[1];
        } else if (needToGambleLevel < 8)
        {
            needToGambleHeadImage.GetComponent<Image>().sprite = needToGambleHeadSprites[2];
        } else 
        {
            needToGambleHeadImage.GetComponent<Image>().sprite = needToGambleHeadSprites[3];
        }

    }
    void GetHp()
    {
        int playerHp = playerData.hp;
        int index = 1;
        int digit;
        bool willContinue = true;
        // Loop to extract digits and create/update sprites
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
            // Create or update the digit sprite
            if (numberCardsSprites.ContainsKey(index) )
            {
                numberCardsSprites[index].GetComponent<Image>().sprite = numbersCards[digit]; // Update existing sprite
            } else {
                // Create new sprite
                GameObject numberCard = new GameObject();
                numberCard.AddComponent<Image>();
                numberCard.GetComponent<Image>().sprite = numbersCards[digit];
                numberCard.GetComponent<RectTransform>().sizeDelta = new Vector2(256 * screenWidthDifference * multiplier, 256 * screenWidthDifference * multiplier); // Set size based on sprite size
                numberCardsSprites[index] = numberCard;
                numberCard.transform.SetParent(numbersCardsParent.transform);
            }
            playerHp /= 10;
            index ++;
        }
        // Deactivate unused sprites
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
        // Position the digit sprites
        for (int i = index-1; i > 0; i --)
        {
            if (numberCardsSprites.ContainsKey(i))
            {
                numberCardsSprites[i].transform.position = new Vector3(numbersCardsParent.transform.position.x + 95f * iterator * screenWidthDifference * multiplier, numbersCardsParent.transform.position.y, numbersCardsParent.transform.position.z);
            }
        
            iterator++;

        }
    }
    // Update money display
    void GetMoney()
    {
        int playerMoney = playerData.money;
        int index = 1;
        int digit;
        bool willContinue = true;
        if (playerData.money != 0)
        {
            // Loop to extract digits and create/update sprites
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
                // Create or update the digit sprite    
                if (numberMoneySprites.ContainsKey(index) )
                {
                    numberMoneySprites[index].GetComponent<Image>().sprite = numbersMoney[digit];
                    numberMoneySprites[index].GetComponent<RectTransform>().sizeDelta = numbersMoney[digit].rect.size * screenWidthDifference * multiplier; // Set size based on sprite size
                    if (digit == 5)
                    {
                        numberMoneySprites[index].transform.rotation = Quaternion.Euler(0f, 0f, 0.8f);
                    } else {
                        numberMoneySprites[index].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                    // Update existing sprite
                } else {
                    GameObject numberMoney = new GameObject();
                    numberMoney.AddComponent<Image>();
                    numberMoney.GetComponent<Image>().sprite = numbersMoney[digit];
                    numberMoney.GetComponent<RectTransform>().sizeDelta = numbersMoney[digit].rect.size * screenWidthDifference * multiplier; // Set size based on sprite size
                    if (digit == 5)
                    {
                        numberMoney.transform.rotation = Quaternion.Euler(0f, 0f, 0.8f);
                    } else {
                        numberMoney.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                    numberMoneySprites[index] = numberMoney;
                    numberMoney.transform.SetParent(numbersMoneyParent.transform);
                }
                playerMoney /= 10;
                index ++;
            }
        } else {
            // Handle zero case
            if (numberMoneySprites.ContainsKey(1))
            {   
                // Update existing sprite
                numberMoneySprites[1].GetComponent<Image>().sprite = numbersMoney[0];
                numberMoneySprites[1].GetComponent<RectTransform>().sizeDelta =
                    numbersMoney[0].rect.size * screenWidthDifference * multiplier;
            }
            else
            {
                // Create new sprite
                GameObject numberMoney = new GameObject();
                numberMoney.AddComponent<Image>();
                numberMoney.GetComponent<Image>().sprite = numbersMoney[0];
                numberMoney.GetComponent<RectTransform>().sizeDelta =
                    numbersMoney[0].rect.size * screenWidthDifference * multiplier;
                numberMoneySprites[1] = numberMoney;
                numberMoney.transform.SetParent(numbersMoneyParent.transform);
            }

            index = 2;
        }
        if (ModeController.Instance.currentSelection.selectedMode == GameMode.MoneyLife)
        {
            return;
        }
        // Deactivate unused sprites
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
        float position = 0f;
        // Position the digit sprites
        for (int i = index-1; i > 0; i --)
        {
            if (numberMoneySprites.ContainsKey(i))
            {
                if (isFirst)
                {
                    isFirst = false;
                    numberMoneySprites[i].transform.position = new Vector3(numbersMoneyParent.transform.position.x, numbersMoneyParent.transform.position.y, numbersMoneyParent.transform.position.z);
                    position += (numberMoneySprites[i].GetComponent<Image>().sprite.rect.width * screenWidthDifference * multiplier)/2 + 7.5f;
                } else {
                    numberMoneySprites[i].transform.position = new Vector3(numbersMoneyParent.transform.position.x + position + ((numberMoneySprites[i].GetComponent<Image>().sprite.rect.width)/2) * screenWidthDifference * multiplier, numbersMoneyParent.transform.position.y, numbersMoneyParent.transform.position.z);
                    position += (numberMoneySprites[i].GetComponent<Image>().sprite.rect.width * screenWidthDifference * multiplier) + 7.5f;
                }
                
            }
            iterator++;

        }
        // Position the dollar sign
        if (isFirstSign)
        {
            // Create dollar sign sprite
            dollarSign.AddComponent<Image>();
            dollarSign.GetComponent<Image>().sprite = dollarSignSprite;
            dollarSign.GetComponent<RectTransform>().sizeDelta = dollarSignSprite.rect.size * screenWidthDifference * multiplier;
            dollarSign.transform.SetParent(numbersMoneyParent.transform);
            isFirstSign = false;
        }
        // Set dollar sign position
        dollarSign.transform.position = new Vector3(numbersMoneyParent.transform.position.x + position + ((dollarSign.GetComponent<Image>().sprite.rect.width)/2) * screenWidthDifference * multiplier, numbersMoneyParent.transform.position.y, numbersMoneyParent.transform.position.z);
        isFirst = true;
    }
}

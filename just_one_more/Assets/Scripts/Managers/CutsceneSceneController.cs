using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneSceneController : MonoBehaviour
{
    public Image firstCutsceneImage;
    public Image secondCutsceneImage;
    public TextMeshProUGUI firstCutsceneText;
    public TextMeshProUGUI secondCutsceneText;
    public TextMeshProUGUI instructionText;
    private int currentIndex = 0;
    private bool isChanging = false;
    private bool skipRequested = false;

    public void Start()
    {
        firstCutsceneImage.gameObject.SetActive(false);
        secondCutsceneImage.gameObject.SetActive(false);
        firstCutsceneText.gameObject.SetActive(true);
        FadeIn(firstCutsceneText);
        secondCutsceneText.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isChanging)
            {
                skipRequested = true;
                return;
            }
            currentIndex++;
            if (currentIndex == 1)
            {
                ShowFirstCutscene();
            }
            else if (currentIndex == 2)
            {
                showSecondCutsceneText();
            }
            else if (currentIndex == 3)
            {
                ShowSecondCutscene();
            }
            else if (currentIndex == 4)
            {
                EndCutscene();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }

    public void ShowFirstCutscene()
    {
        Fade();
    }
    public void showSecondCutsceneText()
    {
        Fade();
    }
    public void ShowSecondCutscene()
    {
        Fade();
    }
    public void EndCutscene()
    {
        FadeOut(secondCutsceneImage);
        StartCoroutine(LoadMainMenuAfterFadeOut());
    }

    public void SkipCutscene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    IEnumerator LoadMainMenuAfterFadeOut()
    {
        yield return new WaitUntil(() => !isChanging);
        SceneManager.LoadScene("MainMenuScene");
    }
    void FadeIn(TextMeshProUGUI text)
    {
        isChanging = true;
        instructionText.gameObject.SetActive(false);
        StartCoroutine(FadeInText(text));
    }

    // Fade in coroutine for text
    IEnumerator FadeInText(TextMeshProUGUI text)
    {
        float duration = 3.0f;
        float elapsed = 0.0f;
        Color color = text.color;
        color.a = 0.0f;
        text.color = color;

        while (elapsed < duration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            text.color = color;
            yield return null;
        }   

        text.color = new Color(color.r, color.g, color.b, 1.0f);
        isChanging = false;
        skipRequested = false;
        instructionText.gameObject.SetActive(true);
    }

    // Fade out coroutine
    void FadeOut(Image image)
    {
        isChanging = true;
        instructionText.gameObject.SetActive(false);
        StartCoroutine(FadeOutImg(image));
    }

    // Fade out coroutine for image
    IEnumerator FadeOutImg(Image image)
    {
        float duration = 3.0f;
        float elapsed = 0.0f;
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;

        while (elapsed < duration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsed / duration));
            image.color = color;
            yield return null;
        }
        image.color = new Color(color.r, color.g, color.b, 0.0f);
        isChanging = false;
        skipRequested = false;
    }

    // General Fade method to handle transitions
    void Fade()
    {
        isChanging = true;
        if (currentIndex == 1)
            {
                StartCoroutine(FadeTextToImg(firstCutsceneImage, firstCutsceneText));
            }
            else if (currentIndex == 2)
            {
                StartCoroutine(FadeImgToText(firstCutsceneImage, secondCutsceneText));
            }
            else if (currentIndex == 3)
            {
                StartCoroutine(FadeTextToImg(secondCutsceneImage, secondCutsceneText));
            }
    }
    IEnumerator FadeTextToImg(Image image, TextMeshProUGUI text)
    {
        instructionText.gameObject.SetActive(false);
        image.gameObject.SetActive(true);
        float duration = 3.0f;
        float elapsed = 0.0f;
        Color textColor = text.color;
        Color imageColor = image.color;
        textColor.a = 1.0f;
        imageColor.a = 0.0f;
        text.color = textColor;
        image.color = imageColor;

        while (elapsed < duration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // Normalized time
            textColor.a = 1 - t;
            imageColor.a = t;
            text.color = textColor;
            image.color = imageColor;
            yield return null;
        }
        text.color = new Color(textColor.r, textColor.g, textColor.b, 0.0f);
        image.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1.0f);
        isChanging = false;
        skipRequested = false;
        text.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
    }

    IEnumerator FadeImgToText(Image image, TextMeshProUGUI text)
    {
        instructionText.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
        float duration = 3.0f;
        float elapsed = 0.0f;
        Color imageColor = image.color;
        Color textColor = text.color;
        imageColor.a = 1.0f;
        textColor.a = 0.0f;
        image.color = imageColor;
        text.color = textColor;

        while (elapsed < duration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            imageColor.a = 1 - t;
            textColor.a = t;
            image.color = imageColor;
            text.color = textColor;
            yield return null;
        }
        image.color = new Color(imageColor.r, imageColor.g, imageColor.b, 0.0f);
        text.color = new Color(textColor.r, textColor.g, textColor.b, 1.0f);
        isChanging = false;
        skipRequested = false;
        image.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(true);
    }
}

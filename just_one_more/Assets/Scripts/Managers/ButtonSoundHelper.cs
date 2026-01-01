using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundHelper : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }
    
    void PlayButtonSound()
    {
        if (SoundController.Instance != null)
        {
            SoundController.Instance.PlayButtonClick();
        }
    }
}

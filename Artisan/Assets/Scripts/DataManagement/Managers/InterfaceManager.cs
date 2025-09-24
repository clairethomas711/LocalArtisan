using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] Animator canvasAnim;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI timeText;
    public void UpdateUIVisuals()
    {
        moneyText.text = DataManager.instance.money.ToString();
        staminaText.text = DataManager.instance.stamina.ToString();
        dayText.text = DataManager.instance.currentDay.ToString();
    }

    public void UpdateClock()
    {
        timeText.text = DataManager.instance.gameTime.ToString();
    }

    public void FadeOut()
    {
        canvasAnim.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        canvasAnim.SetTrigger("FadeIn");   
    }
    
}

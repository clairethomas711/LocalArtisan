using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] Animator canvasAnim;
    [SerializeField] TextMeshProUGUI moneyText;
    //[SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI specializationText;
    [SerializeField] TextMeshProUGUI progressText;
    public void UpdateUIVisuals()
    {
        moneyText.text = DataManager.instance.money.ToString("n2");
        //staminaText.text = DataManager.instance.stamina.ToString();
        dayText.text = DataManager.instance.currentDay.ToString();
        string specializationId = "";
        string progress = "";
        //float progressToNextLevel = 0.0f;
        if (DataManager.instance.progressionManager.knownSpecializations.ContainsKey("baker"))
        {
            //print("ah yes a baker");
            int exp = DataManager.instance.progressionManager.knownSpecializations["baker"];
            if (exp > 10000)
                specializationId += "Artisan ";
            else if (exp > 5000)
                specializationId += "Professional ";
            else if (exp > 2500)
                specializationId += "Advanced ";
            else if (exp > 1000)
                specializationId += "Novice ";
            else if (exp > 100)
                specializationId += "Hobbyist ";
            else
                specializationId += "Newbie ";
            specializationId += "Baker";
            progress = exp.ToString();
        }
        specializationText.text = specializationId;
        progressText.text = progress;
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

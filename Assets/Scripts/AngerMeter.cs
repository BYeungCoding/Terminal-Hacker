using UnityEngine;
using UnityEngine.UI;

public class AngerMeter : MonoBehaviour
{
    public Image progressBarFill;
    public float angerSpeed = 0.1f;
    private float _angerLevel = 0f; // Value between 0 and 1
    public GameObject debuffIcon;
    public GameObject neutralIcon; 
    private bool isDebuffed = false; // Flag to indicate if the character is debuffed
    // Update is called once per frame
    void Update()
    {
        /*
         * This method updates the anger level over time. You can adjust the rate of increase by changing the multiplier.
         * The value will be clamped between 0 and 1 to ensure it stays within the valid range for the progress bar.
         */
         if(isDebuffed)
         {
            _angerLevel += Time.deltaTime * angerSpeed; 
            _angerLevel = Mathf.Clamp01(_angerLevel);

            progressBarFill.fillAmount = _angerLevel; 
            progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); 
         }
            else
            {
                // If not debuffed, the anger level will decay over time to simulate calming down.
                _angerLevel -= Time.deltaTime * 0.05f; 
                _angerLevel = Mathf.Clamp01(_angerLevel); // Ensure it stays within 0 and 1
    
                progressBarFill.fillAmount = _angerLevel; 
                progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); 
            }
    }

     public void SetAngerLevel(float level)
    {   
         /*
         * This method allows external scripts to set the anger level directly.
         * Level should be between 0 and 1.
         */
         if (level < 0f || level > 1f)
        {
            Debug.LogWarning("Anger level must be between 0 and 1.");
            return;
        }
        _angerLevel = Mathf.Clamp01(level);
        progressBarFill.fillAmount = _angerLevel;
    }

     public void ResetAngerLevel()
    {
        _angerLevel = 0f; // Reset the anger level to 0
        progressBarFill.fillAmount = _angerLevel; // Update the UI to reflect the reset
    }


    public void AppyDebuff(bool debuffed)
    {
        /*
         * This method allows external scripts to apply or remove a debuff which affects the anger level increase rate.
         */
        isDebuffed = debuffed;
        if (debuffIcon != null && debuffed == true){
            debuffIcon.SetActive(true);
            neutralIcon.SetActive(false); // Hide neutral icon when debuffed
        }
        else if (debuffIcon != null && debuffed == false){
            debuffIcon.SetActive(false);
            neutralIcon.SetActive(true); // Show neutral icon when not debuffed
        }
        else 
            Debug.LogWarning("Debuff icon not assigned in the inspector.");
    }
}

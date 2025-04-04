using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class AngerMeter : MonoBehaviour
{

    public UnityEngine.UI.Image progresssBarFill;
    private float _angerLevel = 0f; // Value between 0 and 1

    // Update is called once per frame
    void Update()
    {
        _angerLevel += Time.deltaTime * 0.1f; // Simulate anger increase over time, adjust the rate as needed
        _angerLevel = Mathf.Clamp01(_angerLevel); // Ensure it stays between 0 and 1

        progresssBarFill.fillAmount = _angerLevel; // Update the fill amount of the progress bar  
        progresssBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); // Change color based on anger level
        if (_angerLevel >= 1f)
        {
            Debug.Log("Character is fully angry!");
            transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, 0); // Example of a reaction when fully angry, like a small random movement
            // You can add more reactions here, like playing a sound or triggering an animation
        }
    }

    public void ResetAngerLevel()
    {
        _angerLevel = 0f; // Reset the anger level to 0
        progresssBarFill.fillAmount = _angerLevel; // Update the UI to reflect the reset
    }

    public void SetAngerLevel(float value)
    {
        if (value < 0f || value > 1f)
        {
            Debug.LogWarning("Anger level must be between 0 and 1.");
            return;
        }

        _angerLevel = value; // Set the anger level to the specified value
        progresssBarFill.fillAmount = _angerLevel; // Update the UI to reflect the new anger level
    }
}

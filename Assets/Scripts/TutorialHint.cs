using UnityEngine;
using TMPro;

public class TutorialHint : MonoBehaviour
{
    public GameObject hintUI; // assign in code or Inspector
    public TMP_Text hintText;
    public string message = "Press 'E' to interact.";

    void Start()
    {
        if (hintUI != null)
        {
            hintUI.SetActive(false);
            Debug.Log($"[TutorialHint] Player entered. hintUI: {hintUI != null}, hintText: {hintText != null}");
            if (hintText != null)
                hintText.text = message; // Initialize the text
        }
    }

    void Update()
    {
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
        {
            hintUI.SetActive(true);
            Debug.Log("Hint should be there");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
        {
            hintUI.SetActive(false);
        }
    }
}

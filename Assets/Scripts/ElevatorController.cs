using System;
using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public int floorID;
    public int returnToFloorID;
    public Vector2Int returnGridPosition;
    public bool isReturnElevator = false;

    private bool playerInRange = false;
    private bool isTeleporting = false;

    public levelGen levelGen;
    public CharacterMover playerMover;
    public AudioSource ElevatorDing;

    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;


    void Start()
    {
        CanvasGroup[] allGroups = Resources.FindObjectsOfTypeAll<CanvasGroup>();
        foreach (CanvasGroup cg in allGroups)
        {
            if (cg.gameObject.name == "Black Screen")
            {
                fadeCanvasGroup = cg;
                break;
            }
            else
            {
                Debug.LogWarning("Fade canvas group not found. Make sure the 'Black Screen' object is in the scene.");
            }
        }
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isTeleporting)
        {
            // 🛑 Block input if terminal is open
            if (playerMover != null && playerMover.terminalController != null && playerMover.terminalController.isTerminalVisible)
                return;

            isTeleporting = true;
            TeleportPlayerToFloor();
        }
    }

    void TeleportPlayerToFloor()
    {
        if (playerMover == null) return;

        GameObject player = playerMover.gameObject;
        if (player == null) return;

        // Determine which floor to go to
        int targetFloorID = isReturnElevator ? returnToFloorID : floorID;

        // Calculate floor world offset
        Vector3 floorOffset = (targetFloorID % 2 == 0)
            ? new Vector3(targetFloorID * 500, 0, 0)
            : new Vector3(0, targetFloorID * 500, 0);

        // Calculate local room offset
        Vector3 localOffset = new Vector3(returnGridPosition.x * 75, returnGridPosition.y * 50, 0);
        Vector3 destination = floorOffset + localOffset + new Vector3(0, 0, -1);

        // Play sound
        if (ElevatorDing != null)
            ElevatorDing.Play();

        // Start fade/teleport coroutine
        StartCoroutine(FadeAndTeleport(player, destination));
    }

    IEnumerator FadeAndTeleport(GameObject player, Vector3 destination)
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade canvas group not assigned.");
            player.transform.position = destination;
            Camera.main.transform.position = new Vector3(destination.x, destination.y, Camera.main.transform.position.z);
            playerMover?.UpdateCurrentRoom();
            isTeleporting = false;
            yield break;
        }

        playerMover.PlayerBody.linearVelocity = Vector2.zero;
        playerMover.PlayerBody.angularVelocity = 0f;
        playerMover.enabled = false;


        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        player.transform.position = destination;
        Camera.main.transform.position = new Vector3(destination.x, destination.y, Camera.main.transform.position.z);

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        playerMover.enabled = true;
        playerMover.UpdateCurrentRoom();

        isTeleporting = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < duration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
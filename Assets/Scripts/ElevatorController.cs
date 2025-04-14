using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public int floorID;
    private bool playerInRange = false;

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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E to use elevator to floor: " + floorID);
            TeleportPlayerToFloor();
        }
    }

    void TeleportPlayerToFloor()
    {
        // Optional: Add fade effect or animation before teleporting
        Vector3 destination = new Vector3(floorID * 1000, 0, 0); // Customize for X or Y positioning
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Optionally fade to black, then teleport
            StartCoroutine(FadeAndTeleport(player, destination));
        }
    }

    IEnumerator FadeAndTeleport(GameObject player, Vector3 destination)
    {
        // Fade out
        // (Implement your fade-out here, e.g., by changing camera background or using a canvas)

        yield return new WaitForSeconds(1f);  // Adjust delay as needed

        // Teleport player
        player.transform.position = destination;
        Camera.main.transform.position = new Vector3(destination.x, destination.y, Camera.main.transform.position.z);

        // Fade in
        // (Implement fade-in here)
    }
}
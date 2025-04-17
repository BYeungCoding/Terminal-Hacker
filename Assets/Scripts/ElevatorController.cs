using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public int floorID; //The Floor this elevator leads to 
    public int returnToFloorID; //The Floor this elevator returns to
    private bool playerInRange = false;
    private bool isTeleporting = false; //To prevent spams
    public Vector2Int returnGridPosition; //the grid position the player should appear at when returning to the floor
    
    //when the player is on the elevator, set the flag to true
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //when the player leaves the elevator, set the flag to false
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //Check for player input E to trigger the teleportation
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isTeleporting)
        {
            isTeleporting = true;
            TeleportPlayerToFloor();
        }
    }

    //Teleport the player to the floor
    void TeleportPlayerToFloor()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // choose which floor to teleport to:
        // if returnToFloorID is set not 0, use that; otherwise use the floorID
        int targetFloorID = (returnToFloorID != 0) ? returnToFloorID : floorID;
        
        //Match the floor offset logic from LevelGen
        Vector3 floorOffset = (targetFloorID % 2 == 0)
            ? new Vector3(targetFloorID * 500, 0, 0)
            : new Vector3(0, targetFloorID * 500, 0);

        // Calculate the destination position based on the returnGridPosition
        Vector3 localOffset = new Vector3(returnGridPosition.x * 75, returnGridPosition.y * 50, 0);
        Vector3 destination = floorOffset + localOffset + new Vector3(0, 0, -1);
        
        //Start a corotine to handle the teleportation with a fade effect
        StartCoroutine(FadeAndTeleport(player, destination));

        //Set flag to allow the player to teleport again
        isTeleporting = false;
    }

    //Coroutine handles teleportation with a fade effect(Not implemented yet)
    IEnumerator FadeAndTeleport(GameObject player, Vector3 destination)
    {
        yield return new WaitForSeconds(0.3f); // optional delay/fade

        player.transform.position = destination;

        Camera.main.transform.position = new Vector3(
            destination.x,
            destination.y,
            Camera.main.transform.position.z
        );
    }
}
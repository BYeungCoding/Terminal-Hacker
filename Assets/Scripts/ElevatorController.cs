
using System;
using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public int floorID; //The Floor this elevator leads to 
    public int returnToFloorID; //The Floor this elevator returns to
    private bool playerInRange = false;
    private bool isTeleporting = false; //To prevent spams
    public Vector2Int returnGridPosition; //the grid position the player should appear at when returning to the floor
    public levelGen levelGen; //Reference to the level generator script
    public CharacterMover playerMover; //Reference to the player mover script
    public bool isReturnElevator = false;

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
        if (playerMover == null) return;

        GameObject player = playerMover.gameObject; // get the player from the assigned mover
        if (player == null) return;

        int targetFloorID = isReturnElevator ? returnToFloorID : floorID;
        Vector3 floorOffset = (targetFloorID % 2 == 0)
            ? new Vector3(targetFloorID * 500, 0, 0)
            : new Vector3(0, targetFloorID * 500, 0);

        Vector3 localOffset = new Vector3(returnGridPosition.x * 75, returnGridPosition.y * 50, 0);
        Vector3 destination = floorOffset + localOffset + new Vector3(0, 0, -1);

        StartCoroutine(FadeAndTeleport(player, destination));
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

        playerMover.UpdateCurrentRoom();
    }
}

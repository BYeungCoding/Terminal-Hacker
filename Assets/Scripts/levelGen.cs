using UnityEngine;
using System.Collections.Generic;

public class levelGen : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject playerPrefab;
    public GameObject elevatorPrefab;
    public int numberOfRooms = 10;
    public int maxFloors = 5;
    private int totalFloorsSpawned = 1;

    private int elevatorCounter = 1;

    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    void Start()
    {
        GenerateLevelAt(Vector2Int.zero, 0);
    }

    void GenerateLevelAt(Vector2Int offset, int floorID)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Vector2Int startPos = Vector2Int.zero;
        frontier.Enqueue(startPos);

        Dictionary<Vector2Int, GameObject> localRooms = new Dictionary<Vector2Int, GameObject>();
        int roomCount = 0;

        while (frontier.Count > 0 && roomCount < numberOfRooms)
        {
            Vector2Int currPos = frontier.Dequeue();

            if (localRooms.ContainsKey(currPos))
                continue;

            Vector3 worldPos = new Vector3(currPos.x * 75 + offset.x, currPos.y * 50 + offset.y, 0);
            GameObject room = Instantiate(roomPrefab, worldPos, Quaternion.identity);
            RoomController rc = room.GetComponent<RoomController>();
            rc.gridPosition = currPos;
            localRooms.Add(currPos, room);
            roomCount++;

            ShuffleArray(directions);
            bool addedAtLeastOne = false;
            foreach (var dir in directions)
            {
                Vector2Int nextPos = currPos + dir;
                if (!localRooms.ContainsKey(nextPos))
                {
                    if (Random.value > 0.5f || !addedAtLeastOne)
                    {
                        frontier.Enqueue(nextPos);
                        addedAtLeastOne = true;
                    }
                }
            }
        }

        foreach (var kvp in localRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomController rc = kvp.Value.GetComponent<RoomController>();

            bool top = localRooms.ContainsKey(pos + Vector2Int.up);
            bool bottom = localRooms.ContainsKey(pos + Vector2Int.down);
            bool right = localRooms.ContainsKey(pos + Vector2Int.right);
            bool left = localRooms.ContainsKey(pos + Vector2Int.left);

            rc.SetDoors(top, bottom, right, left);

            // Count neighbors
            int neighborCount = 0;
            if (top) neighborCount++;
            if (bottom) neighborCount++;
            if (right) neighborCount++;
            if (left) neighborCount++;

            // Place elevator in single-connection rooms
            if (neighborCount == 1)
            {
                Transform floorTransform = null;
                foreach (Transform child in kvp.Value.transform)
                {
                    if (child.CompareTag("Floor"))
                    {
                        floorTransform = child;
                        break;
                    }
                }

                if (floorTransform != null && totalFloorsSpawned < maxFloors)
                {
                    Vector3 spawnPosition = floorTransform.position + new Vector3(0f, 0f, -0.5f);
                    GameObject elevator = Instantiate(elevatorPrefab, spawnPosition, Quaternion.identity);

                    int newFloorID = elevatorCounter++;
                    ElevatorController ec = elevator.GetComponent<ElevatorController>();
                    if (ec != null)
                    {
                        ec.floorID = newFloorID;
                    }

                    Vector2Int nextOffset = (newFloorID % 2 == 0)
                        ? new Vector2Int(newFloorID * 1000, 0)
                        : new Vector2Int(0, newFloorID * 1000);

                    totalFloorsSpawned++;
                    GenerateLevelAt(nextOffset, newFloorID);
                }
            }
        }

        // Only spawn player in floor 0
        if (floorID == 0)
        {
            GameObject startRoom = localRooms[Vector2Int.zero];

            Transform floorTransform = null;
            foreach (Transform child in startRoom.transform)
            {
                if (child.CompareTag("Floor"))
                {
                    floorTransform = child;
                    break;
                }
            }

            if (floorTransform != null)
            {
                Vector3 spawnPosition = floorTransform.position + new Vector3(0f, 0f, -1f);
                GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

                Camera.main.transform.position = spawnPosition + new Vector3(0f, 0f, Camera.main.transform.position.z);

                GameObject terminalManager = GameObject.Find("TerminalManager");
                if (terminalManager != null)
                {
                    CharacterMover moveScript = playerInstance.GetComponent<CharacterMover>();
                    if (moveScript != null)
                    {
                        TerminalController terminalController = terminalManager.GetComponent<TerminalController>();
                        if (terminalController != null)
                        {
                            moveScript.terminalController = terminalController;
                        }
                        else
                        {
                            Debug.LogWarning("TerminalController not found on TerminalManager!");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("TerminalManager not found in scene!");
                }
            }
        }
    }

    void ShuffleArray(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector2Int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
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
    private int DeadEndFloorSpawnCounter = 0;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    private struct ElevatorSpawnData
    {
        public Vector3 position;
        public int floorID;
        public Vector2Int offset;
    }

    void Start()
    {
        GenerateLevelAt(Vector2Int.zero, 0, -1); // No return for root
        Debug.Log("Level generation complete. Total floors spawned: " + totalFloorsSpawned);
        Debug.Log("Total elevators spawned: " + elevatorCounter);
    }

    void GenerateLevelAt(Vector2Int offset, int floorID, int returnToFloorID)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Vector2Int startPos = Vector2Int.zero;
        frontier.Enqueue(startPos);

        Dictionary<Vector2Int, GameObject> localRooms = new Dictionary<Vector2Int, GameObject>();
        int roomCount = 0;
        List<ElevatorSpawnData> elevatorsToSpawn = new List<ElevatorSpawnData>();

        while (frontier.Count > 0 && roomCount < numberOfRooms)
        {
            Vector2Int currPos = frontier.Dequeue();
            if (localRooms.ContainsKey(currPos)) continue;

            // Step 1: Spawn room at Vector3.zero temporarily
            GameObject room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            Transform floor = FindFloorTransform(room);

            // Step 2: Calculate world position
            Vector3 targetGridPos = new Vector3(currPos.x * 75 + offset.x, currPos.y * 50 + offset.y, 0);

            // Step 3: Align room so that its Floor center is at the grid position
            if (floor != null)
            {
                Vector3 centerOffset = room.transform.position - floor.position;
                room.transform.position = targetGridPos + centerOffset;
            }
            else
            {
                // Fallback in case Floor not found
                room.transform.position = targetGridPos;
            }

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

        // Add return elevator in the center room
        if (returnToFloorID != -1 && localRooms.ContainsKey(Vector2Int.zero))
        {
            Transform centerFloor = FindFloorTransform(localRooms[Vector2Int.zero]);
            if (centerFloor != null)
            {
                Vector3 returnPos = centerFloor.position + new Vector3(0f, 0f, -0.5f);
                GameObject returnElevator = Instantiate(elevatorPrefab, returnPos, Quaternion.identity);
                ElevatorController ec = returnElevator.GetComponent<ElevatorController>();
                if (ec != null)
                {
                    ec.floorID = returnToFloorID;
                }
            }
        }

        // Set doors and collect elevator candidates
        foreach (var kvp in localRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomController rc = kvp.Value.GetComponent<RoomController>();

            bool top = localRooms.ContainsKey(pos + Vector2Int.up);
            bool bottom = localRooms.ContainsKey(pos + Vector2Int.down);
            bool right = localRooms.ContainsKey(pos + Vector2Int.right);
            bool left = localRooms.ContainsKey(pos + Vector2Int.left);
            rc.SetDoors(top, bottom, right, left);

            int neighborCount = 0;
            if (top) neighborCount++;
            if (bottom) neighborCount++;
            if (right) neighborCount++;
            if (left) neighborCount++;

            if (neighborCount == 1 && totalFloorsSpawned < maxFloors)
            {
                Transform floorTransform = FindFloorTransform(kvp.Value);
                if (floorTransform != null)
                {
                    Vector3 spawnPos = floorTransform.position + new Vector3(0f, 0f, -0.5f);
                    elevatorsToSpawn.Add(new ElevatorSpawnData
                    {
                        position = spawnPos,
                        floorID = elevatorCounter,
                        offset = (elevatorCounter % 2 == 0)
                            ? new Vector2Int(elevatorCounter * 500, 0)
                            : new Vector2Int(0, elevatorCounter * 500)
                    });
                    elevatorCounter++;
                }
            }
        }

        // Spawn elevators and then spawn new floors
        foreach (var data in elevatorsToSpawn)
        {
            GameObject elevator = Instantiate(elevatorPrefab, data.position, Quaternion.identity);
            ElevatorController ec = elevator.GetComponent<ElevatorController>();
            if (ec != null)
            {
                ec.floorID = data.floorID;
            }

            if (totalFloorsSpawned < maxFloors)
            {
                totalFloorsSpawned++;
                GenerateLevelAt(data.offset, data.floorID, floorID);
            }
            else
            {
                DeadEndFloorSpawnCounter++;
                // Spawn a 3x3 dead-end room with only a return elevator
                Generate3x3DeadEndFloor(data.offset, floorID);
            }
        }

        // Spawn player on root floor
        if (floorID == 0 && GameObject.FindGameObjectWithTag("Player") == null)
        {
            if (localRooms.TryGetValue(Vector2Int.zero, out GameObject centerRoom))
            {
                Transform floorTransform = centerRoom.transform.Find("Floor");
                if (floorTransform != null)
                {
                    Vector3 spawnPos = floorTransform.position + new Vector3(0f, 0f, -1f);
                    GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                    Camera.main.transform.position = spawnPos + new Vector3(0f, 0f, Camera.main.transform.position.z);

                    // Hook up terminal controller
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
                        }
                    }
                }
            }
        }
    }
    void Generate3x3DeadEndFloor(Vector2Int offset, int returnToFloorID)
    {
        Dictionary<Vector2Int, GameObject> localRooms = new Dictionary<Vector2Int, GameObject>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                GameObject room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
                Transform floor = FindFloorTransform(room);

                Vector3 targetGridPos = new Vector3(gridPos.x * 75 + offset.x, gridPos.y * 50 + offset.y, 0);
                if (floor != null)
                {
                    Vector3 centerOffset = room.transform.position - floor.position;
                    room.transform.position = targetGridPos + centerOffset;
                }
                else
                {
                    room.transform.position = targetGridPos;
                }

                RoomController rc = room.GetComponent<RoomController>();
                rc.gridPosition = gridPos;
                localRooms[gridPos] = room;
            }
        }

        // Set doors
        foreach (var kvp in localRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomController rc = kvp.Value.GetComponent<RoomController>();

            bool top = localRooms.ContainsKey(pos + Vector2Int.up);
            bool bottom = localRooms.ContainsKey(pos + Vector2Int.down);
            bool right = localRooms.ContainsKey(pos + Vector2Int.right);
            bool left = localRooms.ContainsKey(pos + Vector2Int.left);
            rc.SetDoors(top, bottom, right, left);
        }

        // Place return elevator in center room
        if (localRooms.TryGetValue(Vector2Int.zero, out GameObject centerRoom))
        {
            Transform centerFloor = FindFloorTransform(centerRoom);
            if (centerFloor != null)
            {
                Vector3 returnPos = centerFloor.position + new Vector3(0f, 0f, -0.5f);
                GameObject returnElevator = Instantiate(elevatorPrefab, returnPos, Quaternion.identity);
                ElevatorController ec = returnElevator.GetComponent<ElevatorController>();
                if (ec != null)
                {
                    ec.floorID = returnToFloorID;
                }
            }
        }
    }

    Transform FindFloorTransform(GameObject room)
    {
        foreach (Transform child in room.transform)
        {
            if (child.CompareTag("Floor"))
                return child;
        }
        return null;
    }

    void ShuffleArray(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector2Int temp = array[i];
            int rand = Random.Range(i, array.Length);
            array[i] = array[rand];
            array[rand] = temp;
        }
    }
}
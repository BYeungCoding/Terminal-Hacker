using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class levelGen : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject playerPrefab;
    public GameObject elevatorPrefab;
    public int numberOfRooms = 10; // Number of rooms to generate per floor
    public int maxFloors = 5; // Maximum number of floors to generate per level
    public GameObject[] dummyFiles;
    private int totalFloorsSpawned = 1;
    private int elevatorCounter = 1;
    private int DeadEndFloorSpawnCounter = 0;
    private int nextFileID = 0;



    //Directions used for room conections: up, right, down, left
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
        //Generate the root Floor + rest of the levels
        GenerateLevelAt(Vector2Int.zero, 0, -1);
        Debug.Log("Level generation complete. Total floors spawned: " + totalFloorsSpawned);
        Debug.Log("Total elevators spawned: " + elevatorCounter);
        Debug.Log("Total dead-end floors spawned: " + DeadEndFloorSpawnCounter);
    }

    // Generates a level at the specified offset with the given floor ID and return elevator ID
    void GenerateLevelAt(Vector2Int offset, int floorID, int returnToFloorID)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Vector2Int startPos = Vector2Int.zero;
        frontier.Enqueue(startPos);

        Dictionary<Vector2Int, GameObject> localRooms = new Dictionary<Vector2Int, GameObject>();
        int roomCount = 0;
        List<ElevatorSpawnData> elevatorsToSpawn = new List<ElevatorSpawnData>();

        // Randomly generate Room layout using BFS
        while (frontier.Count > 0 && roomCount < numberOfRooms)
        {
            Vector2Int currPos = frontier.Dequeue();
            if (localRooms.ContainsKey(currPos)) continue;

            // Instantiate room temporarily at origin
            GameObject room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
            Transform floor = FindFloorTransform(room);

            // Set Final world position for the room
            Vector3 targetGridPos = new Vector3(currPos.x * 75 + offset.x, currPos.y * 50 + offset.y, 0);
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

            //Assign grid position and add to localRooms
            RoomController rc = room.GetComponent<RoomController>();
            rc.gridPosition = currPos;
            localRooms.Add(currPos, room);
            roomCount++;

            // Randomly shuffle directions to add variety in room connections
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

        // Spawn A return Elevator in the center room of the level
        // If returnToFloorID is -1, it means this is the root floor and we don't need a return elevator
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

        // Set doors for each room based on neighbors
        foreach (var kvp in localRooms)
        {
            Vector2Int pos = kvp.Key;
            RoomController rc = kvp.Value.GetComponent<RoomController>();

            bool top = localRooms.ContainsKey(pos + Vector2Int.up);
            bool bottom = localRooms.ContainsKey(pos + Vector2Int.down);
            bool right = localRooms.ContainsKey(pos + Vector2Int.right);
            bool left = localRooms.ContainsKey(pos + Vector2Int.left);
            rc.SetDoors(top, bottom, right, left);
            SpawnFiles(kvp.Value);

            //Spawn elevators if room is dead-end and totalFloorsSpawned < maxFloors
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

        // Place elevators and generate new floors or dead-end rooms
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

        // Spawn player on root floor only
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

                    // Hook up terminal controller to the player
                    GameObject terminalManager = GameObject.Find("TerminalManager");
                    GameObject fileManager = GameObject.Find("File Manager");
                    if (terminalManager != null)
                    {
                        CharacterMover moveScript = playerInstance.GetComponent<CharacterMover>();
                        if (moveScript != null)
                        {
                            TerminalController terminalController = terminalManager.GetComponent<TerminalController>();
                            FileManager fileManagerScript = fileManager.GetComponent<FileManager>();
                            if (terminalController != null)
                            {
                                moveScript.terminalController = terminalController;
                            }
                            if (fileManagerScript != null){
                                moveScript.fileManager = fileManagerScript;
                            }
                        }
                    }
                }
            }
        }
    }
    // Generates a 3x3 dead-end floor with only a return elevator
    // This is used when the maximum number of floors has been reached
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
            SpawnFiles(kvp.Value);
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
    // Finds the floor transform within a room
    Transform FindFloorTransform(GameObject room)
    {
        foreach (Transform child in room.transform)
        {
            if (child.CompareTag("Floor"))
                return child;
        }
        return null;
    }

    // Shuffles the array in place using Fisher-Yates algorithm
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

    void SpawnFiles(GameObject room)
    {
        bool thereIsWin = false;
        RoomController rc = room.GetComponent<RoomController>();
        int spawned = 0;

        List<string> walls = new List<string>(rc.emptyWalls);
        ShuffleList(walls);

        foreach(string wall in walls)
        {
            if(spawned >= rc.emptyWalls.Count -1) break;
            if (Random.value < 0.6f)
            {
                GameObject file = Instantiate(dummyFiles[Random.Range(0, dummyFiles.Length)]);
                Transform floor = FindFloorTransform(room);
                
                Vector3 spawnOffset = Vector3.zero;
                Quaternion rotation = Quaternion.identity;
                switch(wall)
                {
                    case "Top":
                        spawnOffset = new Vector3(0, 15f, -0.5f);
                        rotation = Quaternion.Euler(0, 0, 180f);
                        break;
                    case "Bottom":
                        spawnOffset = new Vector3(0, -15f, -0.5f);
                        rotation = Quaternion.Euler(0, 0, 0);
                        break;
                    case "Right":
                        spawnOffset = new Vector3(26f, 0, -0.5f);
                        rotation = Quaternion.Euler(0, 0, 90f);
                        break;
                    case "Left":
                        spawnOffset = new Vector3(-26f, 0, -0.5f);
                        rotation = Quaternion.Euler(0, 0, -90f);
                        break;
                }

                file.transform.position = floor.position + spawnOffset;
                file.transform.rotation = rotation;
                
                DummyFile df = file.GetComponent<DummyFile>();
                GameObject terminalManager = GameObject.Find("TerminalManager");
                if (terminalManager != null)
                {
                    if (df != null)
                    {
                        TerminalController terminalController = terminalManager.GetComponent<TerminalController>();
                        if (terminalController != null)
                        {
                            df.terminalController = terminalController;
                        }
                    }
                }
                if(df != null){
                    df.gameObject.name = "Dummy_file" + nextFileID;
                    nextFileID++;
                    float rand = Random.value;
                    if(rand < 0.2f) df.isCorrupted = true;
                    else if(rand < 0.4f) df.isHidden = true;
                    spawned++;
                    if(spawned >= 8 && thereIsWin == false){
                        float winRand = Random.value;
                        if(winRand < 0.1f){
                            df.isWin = true;
                            thereIsWin = true;
                        }
                    }
                }
            }
        }
    }
    // Define the ShuffleList method
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
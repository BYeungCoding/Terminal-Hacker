
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Data.Common;
using UnityEngine.Rendering.Universal;

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
    public AudioSource LevelMusic;
    public Dictionary<Vector2Int, GameObject> generatedRooms = new Dictionary<Vector2Int, GameObject>();
    public Vector2Int currentPlayerRoom { get; set; }
    public int currentPlayerFloorID { get; set; }
    public List<ElevatorController> allElevators = new List<ElevatorController>();
    private List<DummyFile> allSpawnedFiles = new List<DummyFile>();
    public GameObject tutorialHintPrefab;

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
            generatedRooms[offset + new Vector2Int(currPos.x * 75, currPos.y * 50)] = room;
            roomCount++;
            room.AddComponent<RoomFloorTag>().floorID = floorID;

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
                    ec.floorID = floorID;
                    ec.returnToFloorID = returnToFloorID;
                    ec.levelGen = this;
                    ec.isReturnElevator = true;
                    allElevators.Add(ec);
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
            GameObject terminalManager = GameObject.Find("TerminalManager");
            TerminalController terminalController = terminalManager?.GetComponent<TerminalController>();

            rc.SpawnFiles(dummyFiles, terminalController, ref nextFileID, allSpawnedFiles);

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

            // Find the closest room to the elevator position
            GameObject closestRoom = generatedRooms
                .Where(kvp => kvp.Value.GetComponent<RoomFloorTag>()?.floorID == floorID)
                .OrderBy(kvp => Vector3.Distance(kvp.Value.transform.position, data.position))
                .FirstOrDefault().Value;

            if (closestRoom != null)
            {
                elevator.transform.SetParent(closestRoom.transform);
            }

            ElevatorController ec = elevator.GetComponent<ElevatorController>();
            if (ec != null)
            {
                ec.floorID = data.floorID;
                ec.returnToFloorID = floorID;
                ec.levelGen = this;
                allElevators.Add(ec);
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
                Generate3x3DeadEndFloor(data.offset, data.floorID, floorID);
            }
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
    }

    // Generates a 3x3 dead-end floor with only a return elevator
    // This is used when the maximum number of floors has been reached
    void Generate3x3DeadEndFloor(Vector2Int offset, int floorID, int returnToFloorID)
    {
        int thisFloorID = floorID;
        totalFloorsSpawned++;
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
                    var tag = room.AddComponent<RoomFloorTag>();
                    tag.floorID = thisFloorID;
                    Debug.Log($"[DeadEndGen] Room {gridPos} assigned to floor {tag.floorID}");

                generatedRooms[offset + new Vector2Int(gridPos.x * 75, gridPos.y * 50)] = room;
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

            GameObject terminalManager = GameObject.Find("TerminalManager");
            TerminalController terminalController = terminalManager?.GetComponent<TerminalController>();

            rc.SpawnFiles(dummyFiles, terminalController, ref nextFileID, allSpawnedFiles);
        }

        // Place return elevator in center room
        if (localRooms.TryGetValue(Vector2Int.zero, out GameObject centerRoom))
        {
            Transform centerFloor = FindFloorTransform(centerRoom);
            if (centerFloor != null)
            {
                Vector3 returnPos = centerFloor.position + new Vector3(0f, 0f, -0.5f);
                GameObject returnElevator = Instantiate(elevatorPrefab, returnPos, Quaternion.identity);
                if (centerRoom != null)
                {
                    returnElevator.transform.SetParent(centerRoom.transform);
                }

                    ElevatorController ec = returnElevator.GetComponent<ElevatorController>();
                    if (ec != null)
                    {
                        ec.floorID = thisFloorID;
                        ec.returnToFloorID = returnToFloorID;
                        ec.returnGridPosition = Vector2Int.zero;
                        ec.globalOffset = offset;
                        ec.levelGen = this;
                        ec.isReturnElevator = true;
                        allElevators.Add(ec);
                    }
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
    void AssignWinFile()
    {
        var eligibleFiles = allSpawnedFiles.Where(f => !f.isCorrupted && !f.isWin).ToList();
        if (eligibleFiles.Count > 0)
        {
            var chosen = eligibleFiles[UnityEngine.Random.Range(0, eligibleFiles.Count)];
            chosen.isWin = true;
            Debug.Log("[Win File] Assigned to: " + chosen.fileName);
        }
        else
        {
            Debug.LogWarning("No eligible files found to assign win file.");
        }
    }
    public void ResetLevel()
    {
        foreach (var room in generatedRooms.Values)
            Destroy(room);
        generatedRooms.Clear();

        foreach (var elevator in allElevators)
            if (elevator != null) Destroy(elevator.gameObject);
        allElevators.Clear();

        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
            Destroy(existingPlayer);

        totalFloorsSpawned = 1;
        elevatorCounter = 1;
        DeadEndFloorSpawnCounter = 0;
        nextFileID = 0;

        GenerateLevelAt(Vector2Int.zero, 0, -1);
        SpawnPlayerAtFloorZero();
        int timesToAssign = Random.Range(3, 6); // 3 to 5 times
        for (int i = 0; i < timesToAssign; i++)
        {
            AssignWinFile();
        }
        Debug.Log("Level has been reset!");
    }

    void SpawnPlayerAtFloorZero()
    {
        Vector2Int centerKey = new Vector2Int(0, 0);
        if (generatedRooms.TryGetValue(centerKey * 75, out GameObject centerRoom))
        {
            Transform floorTransform = centerRoom.transform.Find("Floor");
            if (floorTransform != null)
            {
                Vector3 spawnPos = floorTransform.position + new Vector3(0f, 0f, -1f);
                GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, -10f);

                // Reassign connections like before
                GameObject terminalManager = GameObject.Find("TerminalManager");
                CharacterMover moveScript = playerInstance.GetComponent<CharacterMover>();
                if (moveScript != null)
                {
                    if (terminalManager != null)
                    {
                        TerminalController terminalController = terminalManager.GetComponent<TerminalController>();
                        if (terminalController != null)
                        {
                            moveScript.terminalController = terminalController;
                            terminalController.characterMover = moveScript;
                        }
                    }

                    moveScript.levelGen = this;

                    foreach (ElevatorController ec in allElevators)
                    {
                        ec.playerMover = moveScript;
                    }
                }

                currentPlayerRoom = Vector2Int.zero;
                currentPlayerFloorID = 0;
            }
        }
        else
        {
            Debug.LogWarning("SpawnPlayerAtFloorZero: center room not found.");
        }
    }

    public void showHidden()
    {
        Vector2Int roomKey = currentPlayerRoom;

        if (!generatedRooms.TryGetValue(roomKey, out GameObject room))
        {
            Debug.LogWarning($"[showHidden] Room not found at key: {roomKey}. Recalculating from player position...");

            // 🔄 Try to recover by estimating player's current room based on position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 pos = player.transform.position;
                int x = Mathf.RoundToInt(pos.x / 75f) * 75;
                int y = Mathf.RoundToInt(pos.y / 50f) * 50;
                Vector2Int fallbackKey = new Vector2Int(x, y);

                if (generatedRooms.TryGetValue(fallbackKey, out room))
                {
                    Debug.Log($"[showHidden] Fallback succeeded with key: {fallbackKey}");
                    currentPlayerRoom = fallbackKey;  // optionally update it
                }
                else
                {
                    Debug.LogWarning($"[showHidden] Fallback key {fallbackKey} also failed.");
                    return;
                }
            }
            else
            {
                Debug.LogError("[showHidden] No player found in scene.");
                return;
            }
        }

        Debug.Log("Showing hidden files in room: " + room.name);
        DummyFile[] files = room.GetComponentsInChildren<DummyFile>();
        foreach (DummyFile file in files)
        {
            if (file.isHidden)
            {
                file.gameObject.SetActive(true);
                file.isHidden = false;
            }
        }
    }

    public void generateTutorial(TerminalController terminalController)
    {
        // === Manual Reset (No procedural regen) ===
        foreach (var room in generatedRooms.Values)
            Destroy(room);
        generatedRooms.Clear();

        foreach (var elevators in allElevators)
            if (elevators != null) Destroy(elevators.gameObject);
        allElevators.Clear();

        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
            Destroy(existingPlayer);

        nextFileID = 0;
        allSpawnedFiles.Clear();

        // === Tutorial Room Setup ===
        Vector2Int mainPos = Vector2Int.zero;
        Vector2Int secondPos = Vector2Int.up;
        int tutorialFloorID = 0;
        int destFloorID = tutorialFloorID + 1;
        Vector2Int destOffset = new Vector2Int(0, 500);

        GameObject mainRoom = Instantiate(roomPrefab);
        Transform mainFloor = FindFloorTransform(mainRoom);
        Vector3 mainWorldPos = new Vector3(mainPos.x * 75, mainPos.y * 50, 0);
        mainRoom.transform.position = mainFloor ? mainWorldPos + (mainRoom.transform.position - mainFloor.position) : mainWorldPos;
        mainRoom.AddComponent<RoomFloorTag>().floorID = tutorialFloorID;
        RoomController mainRC = mainRoom.GetComponent<RoomController>();
        mainRC.gridPosition = mainPos;
        generatedRooms[mainPos * 75] = mainRoom;

        GameObject secondRoom = Instantiate(roomPrefab);
        Transform secondFloor = FindFloorTransform(secondRoom);
        Vector3 secondWorldPos = new Vector3(secondPos.x * 75, secondPos.y * 50, 0);
        secondRoom.transform.position = secondFloor ? secondWorldPos + (secondRoom.transform.position - secondFloor.position) : secondWorldPos;
        secondRoom.AddComponent<RoomFloorTag>().floorID = tutorialFloorID;
        RoomController secondRC = secondRoom.GetComponent<RoomController>();
        secondRC.gridPosition = secondPos;
        generatedRooms[secondPos * 75] = secondRoom;

        mainRC.SetDoors(top: true, bottom: false, right: false, left: false);
        secondRC.SetDoors(top: false, bottom: true, right: false, left: false);

        Transform door = mainRoom.transform.Find("Door Top");
        if (door != null)
        {
            GameObject doorHint = Instantiate(tutorialHintPrefab);
            doorHint.transform.SetParent(door, worldPositionStays: false);
            doorHint.transform.localPosition = new Vector3(0f, -0.7f, 0f); // adjust as needed
            doorHint.transform.localRotation = Quaternion.identity;
            doorHint.transform.localScale = Vector3.one * 0.05f;

            TutorialHint doorHintScript = doorHint.GetComponent<TutorialHint>();
            if (doorHintScript != null)
            {
                doorHintScript.message = "Doors connect rooms together. Walk through to explore!";
            }
        }

        Transform otherdoor = secondRoom.transform.Find("Door Bottom");
        if (otherdoor != null)
        {
            GameObject otherdoorHint = Instantiate(tutorialHintPrefab);
            otherdoorHint.transform.SetParent(otherdoor, worldPositionStays: false);
            otherdoorHint.transform.localPosition = new Vector3(0f, -0.7f, 0f); // adjust as needed
            otherdoorHint.transform.localRotation = Quaternion.identity;
            otherdoorHint.transform.localScale = Vector3.one * 0.05f;

            TutorialHint otherdoorHintScript = otherdoorHint.GetComponent<TutorialHint>();
            if (otherdoorHintScript != null)
            {
                otherdoorHintScript.message = "This door will take you back to the main room.";
            }
        }

        // === Spawn Tutorial Files ===
        List<string> walls = new List<string> { "Bottom", "Right", "Left" };
        foreach (string wall in walls)
        {
            GameObject file = Instantiate(dummyFiles[Random.Range(0, dummyFiles.Length)]);
            Transform floor = FindFloorTransform(mainRoom);

            Vector3 spawnOffset = wall switch
            {
                "Top" => new Vector3(0, 14f, -2.5f),
                "Bottom" => new Vector3(0, -10f, -2.5f),
                "Right" => new Vector3(18.5f, 0, -2.5f),
                "Left" => new Vector3(-19f, 0, -2.5f),
                _ => Vector3.zero
            };

            file.transform.position = (floor != null ? floor.position : mainRoom.transform.position) + spawnOffset;
            file.transform.rotation = Quaternion.identity;
            file.transform.SetParent(mainRoom.transform);

            DummyFile df = file.GetComponent<DummyFile>();
            if (df != null)
            {
                df.terminalController = terminalController;
                df.gameObject.name = df.fileName;

                if (wall == "Bottom") df.isHidden = true;
                else if (wall == "Right") df.isCorrupted = true;
                else if (wall == "Left") df.isWin = true;

                allSpawnedFiles.Add(df);
            }
            GameObject hint = Instantiate(tutorialHintPrefab);
            hint.transform.SetParent(file.transform, worldPositionStays: false); // <<< KEY
            hint.transform.localPosition = new Vector3(0f, 0f, 0f); // Offset relative to file
            hint.transform.localRotation = Quaternion.identity;
            hint.transform.localScale = Vector3.one * 0.05f; 
            TutorialHint hintScript = hint.GetComponent<TutorialHint>();
            if (hintScript != null)
            {
                hintScript.message = wall switch
                {
                    "Bottom" => "This is a hidden file. Walking close to it will reveal them. Solve the puzzle and slow the Firewall!",
                    "Right"  => "This file is corrupted. You can't open it. And it will apply a Debuff if you interact with it.",
                    "Left"   => "This is a win file! Press 'E' to interact and 'vim <filename>' to solve a puzzle and increase your score!",
                    _        => "This is a file."
                };
            }
        }

        // === Elevator to isolated room ===
        Vector3 elevatorPos = mainFloor.position + new Vector3(0f, 0f, -0.5f);
        GameObject elevator = Instantiate(elevatorPrefab, elevatorPos, Quaternion.identity);
        elevator.transform.SetParent(mainRoom.transform);
        ElevatorController ec = elevator.GetComponent<ElevatorController>();
        if (ec != null)
        {
            ec.floorID = destFloorID;
            ec.returnToFloorID = tutorialFloorID;
            ec.returnGridPosition = Vector2Int.zero;
            ec.levelGen = this;
            allElevators.Add(ec);
        }

        GameObject elevatorHint = Instantiate(tutorialHintPrefab);
        elevatorHint.transform.SetParent(elevator.transform, worldPositionStays: false);
        elevatorHint.transform.localPosition = new Vector3(0f, 0f, 0f); // Adjust as needed
        elevatorHint.transform.localRotation = Quaternion.identity;
        elevatorHint.transform.localScale = Vector3.one * 0.05f;
        TutorialHint elevatorHintScript = elevatorHint.GetComponent<TutorialHint>();
        if (elevatorHintScript != null)
        {
            elevatorHintScript.message = "Interact with 'E' to go to another floor.";
        }

        // === Single Isolated Destination Room ===
        GameObject isolatedRoom = Instantiate(roomPrefab);
        Transform isoFloor = FindFloorTransform(isolatedRoom);
        Vector3 isoWorldPos = new Vector3(destOffset.x, destOffset.y, 0);
        isolatedRoom.transform.position = isoFloor ? isoWorldPos + (isolatedRoom.transform.position - isoFloor.position) : isoWorldPos;
        RoomController isoRC = isolatedRoom.GetComponent<RoomController>();
        isoRC.gridPosition = Vector2Int.zero;
        isolatedRoom.AddComponent<RoomFloorTag>().floorID = destFloorID;
        generatedRooms[new Vector2Int(destOffset.x, destOffset.y)] = isolatedRoom;

        // === Return Elevator in isolated room ===
        Vector3 returnPos = isoFloor.position + new Vector3(0f, 0f, -0.5f);
        GameObject returnElevator = Instantiate(elevatorPrefab, returnPos, Quaternion.identity);
        returnElevator.transform.SetParent(isolatedRoom.transform);
        ElevatorController returnEC = returnElevator.GetComponent<ElevatorController>();
        if (returnEC != null)
        {
            returnEC.floorID = tutorialFloorID;
            returnEC.returnToFloorID = tutorialFloorID;
            returnEC.returnGridPosition = Vector2Int.zero;
            returnEC.levelGen = this;
            returnEC.isReturnElevator = true;
            allElevators.Add(returnEC);

            GameObject returnHint = Instantiate(tutorialHintPrefab);
            returnHint.transform.SetParent(returnElevator.transform, worldPositionStays: false);
            returnHint.transform.localPosition = new Vector3(0f, 0f, 0f);
            returnHint.transform.localRotation = Quaternion.identity;
            returnHint.transform.localScale = Vector3.one * 0.05f;

            TutorialHint returnHintScript = returnHint.GetComponent<TutorialHint>();
            if (returnHintScript != null)
            {
                returnHintScript.message = "Interact with this to return to the tutorial room.";
            }
        }

        // === Player Spawn ===
        if (mainFloor != null)
        {
            Vector3 spawnPos = mainFloor.position + new Vector3(0f, 0f, -1f);
            GameObject playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            Camera.main.transform.position = new Vector3(spawnPos.x, spawnPos.y, -10f);

            GameObject tm = GameObject.Find("TerminalManager");
            CharacterMover moveScript = playerInstance.GetComponent<CharacterMover>();
            if (moveScript != null)
            {
                if (tm != null)
                {
                    TerminalController tc = tm.GetComponent<TerminalController>();
                    if (tc != null)
                    {
                        moveScript.terminalController = tc;
                        tc.characterMover = moveScript;
                    }
                }

                moveScript.levelGen = this;

                foreach (ElevatorController e in allElevators)
                    e.playerMover = moveScript;
            }

            currentPlayerRoom = mainPos;
            currentPlayerFloorID = tutorialFloorID;
        }

        Debug.Log("Tutorial level generated.");
    }

}


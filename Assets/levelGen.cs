using UnityEngine;
using System.Collections.Generic;

public class levelGen : MonoBehaviour
{
    public GameObject roomPrefab;
    public int numberOfRooms = 10;

    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left
    };

    void Start()
    {
        Generatelevel();
    }

    // Update is called once per frame
    void Generatelevel()
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Vector2Int startPos = Vector2Int.zero;
        frontier.Enqueue(startPos);

        int roomCount = 0;

        while (frontier.Count > 0 && roomCount < numberOfRooms)
        {
            Vector2Int currPos = frontier.Dequeue();

            if (spawnedRooms.ContainsKey(currPos))
                continue;

            GameObject room = Instantiate(roomPrefab, new Vector3(currPos.x * 75, currPos.y * 50, 0), Quaternion.identity);
            RoomController rc = room.GetComponent<RoomController>();
            rc.gridPosition = currPos;
            spawnedRooms.Add(currPos, room);

            roomCount++;

            ShuffleArray(directions);
            foreach (var dir in directions)
            {
                Vector2Int nextPos = currPos + dir;
                if(!spawnedRooms.ContainsKey(nextPos) && Random.value > 0.5f)
                {
                    frontier.Enqueue(nextPos);
                }
            }
        }

        foreach (var kvp in spawnedRooms){
            Vector2Int pos = kvp.Key;
            RoomController rc = kvp.Value.GetComponent<RoomController>();

            bool top = spawnedRooms.ContainsKey(pos + Vector2Int.up);
            bool bottom = spawnedRooms.ContainsKey(pos + Vector2Int.down);
            bool right = spawnedRooms.ContainsKey(pos + Vector2Int.right);
            bool left = spawnedRooms.ContainsKey(pos + Vector2Int.left); 

            rc.SetDoors(top, bottom, right, left);
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

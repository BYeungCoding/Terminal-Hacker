using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public int width = 3;   // Number of rooms horizontally
    public int height = 3;  // Number of rooms vertically
    public float roomSpacing = 12f; // Distance between rooms

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 roomPos = new Vector3(x * roomSpacing, y * roomSpacing, 0);
                Instantiate(roomPrefab, roomPos, Quaternion.identity);
            }
        }
    }
}

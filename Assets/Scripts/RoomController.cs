using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool visited = false;
    public GameObject doorTop;
    public GameObject doorBottom;
    public GameObject doorRight;
    public GameObject doorLeft;

    public List<string> emptyWalls = new List<string>();

    public void SetDoors(bool top, bool bottom, bool right, bool left)
    {
        doorTop.SetActive(top);
        doorBottom.SetActive(bottom);
        doorRight.SetActive(right);
        doorLeft.SetActive(left);

        emptyWalls.Clear();
        if (!top) emptyWalls.Add("Top");
        if (!bottom) emptyWalls.Add("Bottom");
        if (!right) emptyWalls.Add("Right");
        if (!left) emptyWalls.Add("Left");
    }

    public int GetFileCount(bool includeHidden = false)
    {
        return GetComponentsInChildren<DummyFile>().Count(f => includeHidden || !f.isHidden);
    }

    public ElevatorController GetElevator()
    {
        return GetComponentInChildren<ElevatorController>();
    }

    public void SpawnFiles(GameObject[] dummyFiles, TerminalController terminalController, ref int nextFileID, List<DummyFile> allSpawnedFiles)
    {
        int spawned = 0;
        List<string> walls = new List<string>(emptyWalls);
        ShuffleList(walls);

        foreach (string wall in walls)
        {
            if (spawned >= emptyWalls.Count - 1) break;
            if (Random.value > 0.6f) continue;

            GameObject file = Instantiate(dummyFiles[Random.Range(0, dummyFiles.Length)]);
            Transform floor = FindFloorTransform();

            Vector3 spawnOffset;
            Quaternion rotation;
            switch (wall)
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
                default:
                    spawnOffset = Vector3.zero;
                    rotation = Quaternion.identity;
                    break;
            }

            file.transform.position = (floor != null ? floor.position : transform.position) + spawnOffset;
            file.transform.rotation = rotation;
            file.transform.SetParent(transform); // Parent to room

            DummyFile df = file.GetComponent<DummyFile>();
            if (df != null)
            {
                df.terminalController = terminalController;
                df.gameObject.name = df.fileName;

                float rand = Random.value;
                if (rand < 0.2f) df.isCorrupted = true;
                else if (rand < 0.4f) df.isHidden = true;

                allSpawnedFiles.Add(df);
                spawned++;
            }
        }
    }

    private Transform FindFloorTransform()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Floor"))
                return child;
        }
        return null;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public List<DummyFile> GetFiles(bool includeHidden = false)
    {
        return GetComponentsInChildren<DummyFile>().Where(f => includeHidden || !f.isHidden).ToList();
    }
}
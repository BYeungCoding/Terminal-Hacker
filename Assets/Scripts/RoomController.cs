using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool visisted = false;

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
}

using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool visisted = false;

    public GameObject doorTop;
    public GameObject doorBottom;
    public GameObject doorRight;
    public GameObject doorLeft;

    public void SetDoors(bool top, bool bottom, bool right, bool left)
    {
        doorTop.SetActive(top);
        doorBottom.SetActive(bottom);
        doorRight.SetActive(right);
        doorLeft.SetActive(left);
    }
}

using UnityEngine;
using System.Linq;
using System.Text;

public class MapPrinter : MonoBehaviour
{
    public levelGen levelGen; // Reference to LevelGen (set this in Inspector)

    public string GetFloorLayout(bool includeHidden = false)
    {
        int width = 9;
        int height = 9;
        int centerX = width / 2;
        int centerY = height / 2;

        string[,] grid = new string[width, height];

        // Fill grid with empty spaces
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = " . ";

        // Go through every generated room on the current floor
        foreach (var roomEntry in levelGen.generatedRooms.Where(r =>
            r.Value.GetComponent<RoomFloorTag>()?.floorID == levelGen.currentPlayerFloorID))
        {
            var room = roomEntry.Value;
            var rc = room.GetComponent<RoomController>();
            if (rc == null) continue;

            int gx = centerX + rc.gridPosition.x;
            int gy = centerY - rc.gridPosition.y;

            if (gx >= 0 && gx < width && gy >= 0 && gy < height)
            {
                string marker = "";

                // Check if player is in this room
                if (rc.gridPosition == levelGen.currentPlayerRoom)
                {
                    marker = "[X]";
                }
                else
                {
                    ElevatorController ec = rc.GetElevator();
                    if (ec != null)
                    {
                        marker = "[A" + ec.floorID + "]";
                    }
                    else
                    {
                        int fileCount = rc.GetFileCount(includeHidden);
                        marker = $"[{fileCount}]";
                    }
                }

                grid[gx, gy] = marker;
            }
        }

        // Build the final string output
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(grid[x, y].PadRight(4));
            }
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine("Legend:");
        sb.AppendLine("[X] - You (current room)");
        sb.AppendLine("[A#] - Elevator to floor #");
        sb.AppendLine("[#] - Number of visible files");
        sb.AppendLine(".   - Empty space (no room)");

        return sb.ToString();
    }

    public string GetDetailedFileList(bool includeHidden = false)
    {
        StringBuilder sb = new StringBuilder();

        var rooms = levelGen.generatedRooms
            .Where(r => r.Value.GetComponent<RoomFloorTag>()?.floorID == levelGen.currentPlayerFloorID)
            .OrderBy(r => r.Key.x).ThenBy(r => r.Key.y);

        foreach (var roomEntry in rooms)
        {
            var room = roomEntry.Value;
            var rc = room.GetComponent<RoomController>();
            if (rc == null) continue;

            var files = rc.GetFiles(includeHidden);
            if (files.Count == 0) continue;

            sb.AppendLine($"Room {rc.gridPosition}:");

            foreach (var file in files)
            {
                string created = file.creationDate.ToString("yyyy-MM-dd");
                string accessed = file.lastAccessed.ToString("yyyy-MM-dd");
                sb.AppendLine($"- {file.fileName.PadRight(18)} Created: {created}  Accessed: {accessed}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
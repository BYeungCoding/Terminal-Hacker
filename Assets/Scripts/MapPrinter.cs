
using UnityEngine;
using System.Linq;
using System.Text;


public class MapPrinter : MonoBehaviour
{
    public levelGen levelGen; // Reference to LevelGen (set this in Inspector)

    public string GetFloorLayout(bool includeHidden = false)
    {
        if (levelGen == null) return "Error: levelGen not assigned.";

        int currentFloor = levelGen.currentPlayerFloorID;
        Vector2Int playerRoomKey = levelGen.currentPlayerRoom;

        var floorRooms = levelGen.generatedRooms
            .Where(kvp =>
            {
                var tag = kvp.Value.GetComponent<RoomFloorTag>();
                return tag != null && tag.floorID == currentFloor;
            })
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        if (floorRooms.Count == 0)
            return $"No rooms found on floor {currentFloor}.";

        int minX = floorRooms.Keys.Min(k => k.x);
        int maxX = floorRooms.Keys.Max(k => k.x);
        int minY = floorRooms.Keys.Min(k => k.y);
        int maxY = floorRooms.Keys.Max(k => k.y);

        string layout = "";

        for (int y = maxY; y >= minY; y -= 50)
        {
            for (int x = minX; x <= maxX; x += 75)
            {
                Vector2Int key = new Vector2Int(x, y);
                string cell = ".    ."; // Default for missing room

                if (floorRooms.TryGetValue(key, out GameObject room))
                {
                    if (key == playerRoomKey)
                    {
                        cell = "[ X ]";
                    }
                    else
                    {
                        ElevatorController elevator = room.GetComponentInChildren<ElevatorController>();
                        if (elevator != null && !elevator.isReturnElevator)
                        {
                            string floorStr = elevator.floorID.ToString();
                            cell = floorStr.Length == 1 ? $"[A{floorStr}]" : $"[A{floorStr}]";
                        }
                        else
                        {
                            int fileCount = 0;
                            foreach (var df in room.GetComponentsInChildren<DummyFile>())
                            {
                                if (!df.isHidden || includeHidden)
                                    fileCount++;
                            }

                            cell = fileCount < 10 ? $"[ {fileCount} ]" : $"[{fileCount}]";
                        }
                    }
                }

                layout += cell + " ";
            }

            layout += "\n";
        }

        string legend =
            "\nLegend:\n" +
            "[ X ] = You\n" +
            "[A# ] = Elevator\n" +
            "[ # ] = File count in room\n";

        return
            "╔═════════ Floor Map ═════════╗\n" + layout + "╚═════════════════════════╝" +
            legend;
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


            var files = rc.GetFiles(includeHidden).Where(f => f.isHidden == includeHidden).ToList();
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

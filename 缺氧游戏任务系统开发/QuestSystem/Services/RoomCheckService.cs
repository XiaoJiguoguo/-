using UnityEngine;

namespace QuestSystem.Services
{
    public class RoomCheckService
    {
        public bool RoomExists(string roomTypeId)
        {
            if (string.IsNullOrEmpty(roomTypeId)) return false;
            return GetRoomOfType(roomTypeId) != null;
        }

        public float GetRoomPressure(string roomTypeId)
        {
            var room = GetRoomOfType(roomTypeId);
            if (room == null) return 0f;

            float totalPressure = 0f;
            int cellCount = 0;
            var cavity = room.cavity;
            if (cavity != null)
            {
                foreach (int cell in cavity.cells)
                {
                    if (Grid.IsValidCell(cell))
                    {
                        var element = Grid.Element[cell];
                        if (element != null && !element.IsVacuum)
                        {
                            totalPressure += Grid.Mass[cell];
                            cellCount++;
                        }
                    }
                }
            }
            return cellCount > 0 ? totalPressure / cellCount * 1000f : 0f;
        }

        public float GetRoomTemperature(string roomTypeId)
        {
            var room = GetRoomOfType(roomTypeId);
            if (room == null) return 0f;

            float totalTemp = 0f;
            int cellCount = 0;
            var cavity = room.cavity;
            if (cavity != null)
            {
                foreach (int cell in cavity.cells)
                {
                    if (Grid.IsValidCell(cell) && !Grid.Element[cell].IsVacuum)
                    {
                        totalTemp += Grid.Temperature[cell] - 273.15f;
                        cellCount++;
                    }
                }
            }
            return cellCount > 0 ? totalTemp / cellCount : 0f;
        }

        public bool IsRoomSealed(string roomTypeId)
        {
            var room = GetRoomOfType(roomTypeId);
            if (room == null) return false;
            var cavity = room.cavity;
            return cavity != null && cavity.cells != null && cavity.cells.Count > 0;
        }

        public bool IsPipeConnected(string buildingId)
        {
            return CountBuildings(buildingId) >= 1;
        }

        public bool IsPipeMediumCorrect(string buildingId)
        {
            return CountBuildings(buildingId) >= 1;
        }

        private float CountBuildings(string id)
        {
            float count = 0f;
            var all = Object.FindObjectsByType<BuildingComplete>(FindObjectsSortMode.None);
            foreach (var b in all)
            {
                if (b != null && b.name.Contains(id))
                    count++;
            }
            return count;
        }

        private Room GetRoomOfType(string roomTypeId)
        {
            foreach (var room in Game.Instance.roomProber.rooms)
            {
                if (room.roomType != null && room.roomType.Id.Contains(roomTypeId))
                    return room;
            }
            return null;
        }
    }
}
using UnityEngine;

namespace QuestSystem.Services
{
    public class BuildingCheckService
    {
        public float CountBuildings(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return 0f;
            float count = 0f;
            var allBuildings = Object.FindObjectsByType<BuildingComplete>(FindObjectsSortMode.None);
            foreach (var building in allBuildings)
            {
                if (building == null || building.gameObject == null) continue;
                var prefabId = building.GetComponent<KPrefabID>();
                if (prefabId != null && prefabId.PrefabTag.Name.Contains(buildingId))
                    count++;
                else if (building.name.Contains(buildingId))
                    count++;
            }
            return count;
        }
    }
}
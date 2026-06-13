using UnityEngine;

namespace QuestSystem.Services
{
    public class ResourceCheckService
    {
        public float GetResourceAmount(string elementId)
        {
            if (string.IsNullOrEmpty(elementId)) return 0f;

            float total = 0f;
            var storages = Object.FindObjectsByType<Storage>(FindObjectsSortMode.None);
            foreach (var storage in storages)
            {
                if (storage == null) continue;
                foreach (var item in storage.items)
                {
                    if (item == null) continue;
                    var primary = item.GetComponent<PrimaryElement>();
                    if (primary != null)
                    {
                        string elementName = primary.ElementID.ToString();
                        if (elementName.Contains(elementId))
                            total += primary.Mass;
                    }
                }
            }

            var activeWorld = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId);
            if (activeWorld != null)
            {
                var worldInventory = activeWorld.GetComponent<WorldInventory>();
                if (worldInventory != null)
                {
                    try
                    {
                        var tag = new Tag(elementId);
                        var amount = worldInventory.GetTotalAmount(tag, true);
                        if (amount > total) total = amount;
                    }
                    catch { }
                }
            }

            return total;
        }

        public void AddResource(string elementId, float amount)
        {
            var activeWorld = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId);
            if (activeWorld == null) return;
            var worldInventory = activeWorld.GetComponent<WorldInventory>();
            if (worldInventory == null) return;
            try
            {
                var tag = TagManager.Create(elementId);
                var gameObj = tag.IsValid ? Assets.GetPrefab(tag) : null;
                if (gameObj != null)
                {
                    var go = GameUtil.KInstantiate(gameObj, Grid.SceneLayer.Ore);
                    var primary = go.GetComponent<PrimaryElement>();
                    if (primary != null) primary.Mass = amount;
                }
            }
            catch { }
        }
    }
}
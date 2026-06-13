using HarmonyLib;
using KMod;
using QuestSystem.Components;
using QuestSystem.Core;
using QuestSystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
    public class ModEntry : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Debug.Log("[QuestSystem] 任务系统 Mod 已加载!");
        }
    }

    [HarmonyPatch(typeof(SaveGame), "OnPrefabInit")]
    public static class QuestSystemInitPatch
    {
        private static bool initialized = false;

        public static void Postfix()
        {
            if (initialized) return;
            initialized = true;

            if (Game.Instance == null || Game.Instance.gameObject == null)
            {
                Debug.LogError("[QuestSystem] Game.Instance 为空，无法初始化任务系统！");
                return;
            }

            Game.Instance.gameObject.AddComponent<QuestSystemManager>();
            Game.Instance.gameObject.AddComponent<QuestWorldTracker>();

            var questPanelGO = new GameObject("QuestPanel");
            questPanelGO.transform.SetParent(Game.Instance.gameObject.transform, false);

            // 在挂 QuestPanel 之前先加 Canvas，避免子物体找不到父 Canvas 导致 NullReference
            if (questPanelGO.GetComponent<Canvas>() == null)
            {
                var panelCanvas = questPanelGO.AddComponent<Canvas>();
                panelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                panelCanvas.sortingOrder = 100;
            }
            if (questPanelGO.GetComponent<CanvasScaler>() == null)
                questPanelGO.AddComponent<CanvasScaler>();
            if (questPanelGO.GetComponent<GraphicRaycaster>() == null)
                questPanelGO.AddComponent<GraphicRaycaster>();

            questPanelGO.AddComponent<QuestPanel>();
            questPanelGO.AddOrGet<CanvasGroup>();

            Debug.Log("[QuestSystem] 任务系统 Mod 初始化完成!");
        }
    }
}
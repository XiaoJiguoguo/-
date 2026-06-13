using HarmonyLib;
using UnityEngine;
using QuestSystem.Core;

namespace QuestSystem.Patches
{
    // BuildingComplete.OnSpawn 是 KMonoBehaviour 的标准生命周期方法，
    // 任何建筑生成时都会调用，参考 ChiYuKe 教程中 StorageNetwork 的组件注入模式。
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public static class BuildingEventsPatch
    {
        public static void Postfix(BuildingComplete __instance)
        {
            if (QuestSystemManager.Instance == null) return;
            QuestSystemManager.Instance.Tick();
        }
    }
}
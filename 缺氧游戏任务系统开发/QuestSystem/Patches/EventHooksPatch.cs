using HarmonyLib;
using UnityEngine;
using QuestSystem.Core;

namespace QuestSystem.Patches
{
    // MinionIdentity.OnSpawn 是 KMonoBehaviour 的生命周期方法，
    // 复制人生成/打印时触发，参考 ChiYuKe 的 MinionAge 项目中对 MinionConfig.CreatePrefab 的 Patch 模式。
    [HarmonyPatch(typeof(MinionIdentity), "OnSpawn")]
    public static class MinionSpawnPatch
    {
        public static void Postfix(MinionIdentity __instance)
        {
            if (QuestSystemManager.Instance == null) return;
            QuestSystemManager.Instance.Tick();
        }
    }
}
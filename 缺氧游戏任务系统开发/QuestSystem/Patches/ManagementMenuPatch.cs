using HarmonyLib;
using UnityEngine;
using QuestSystem.UI;

namespace QuestSystem.Patches
{
    [HarmonyPatch(typeof(ManagementMenu), "OnPrefabInit")]
    public static class ManagementMenuPatch
    {
        public static void Postfix(ManagementMenu __instance)
        {
            KToggle template = __instance.PauseMenuButton;
            if (template == null) return;

            KToggle questToggle = Util.KInstantiateUI<KToggle>(
                template.gameObject,
                template.transform.parent.gameObject,
                true);

            questToggle.gameObject.name = "Toggle:任务系统";
            questToggle.ClearOnClick();
            questToggle.group = null;

            var locText = questToggle.GetComponentInChildren<LocText>();
            if (locText != null) locText.SetText("任务");

            if (questToggle.fgImage != null)
            {
                var sprite = Assets.GetSprite("OverviewUI_research_nav_icon");
                if (sprite != null) questToggle.fgImage.sprite = sprite;
            }

            questToggle.onClick += () =>
            {
                if (QuestPanel.Instance == null) return;
                bool isActive = QuestPanel.Instance.IsActive();
                QuestPanel.Instance.Show(!isActive);
            };

            questToggle.transform.SetSiblingIndex(template.transform.GetSiblingIndex());
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestSystem.Core;

namespace QuestSystem.UI
{
    public class QuestCardWidget : KMonoBehaviour
    {
        public QuestDefinition Quest { get; private set; }
        private System.Action<QuestDefinition> onClick;

        private TextMeshProUGUI titleLabel;
        private Image progressFill;
        private TextMeshProUGUI statusLabel;
        private TextMeshProUGUI progressText;
        private Button button;

        public void Setup(QuestDefinition quest, System.Action<QuestDefinition> callback)
        {
            Quest = quest;
            onClick = callback;

            BuildUI();
            Refresh();
        }

        private void BuildUI()
        {
            var rt = gameObject.AddOrGet<RectTransform>();

            var le = gameObject.AddComponent<LayoutElement>();
            le.minHeight = 110;
            le.preferredHeight = 110;

            var bg = gameObject.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.18f, 0.22f, 1f);

            button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => onClick?.Invoke(Quest));

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(transform, false);
            titleLabel = titleGO.AddComponent<TextMeshProUGUI>();
            titleLabel.text = Quest.Name;
            titleLabel.fontSize = 18;
            var tRT = titleGO.rectTransform();
            tRT.anchorMin = new Vector2(0, 1);
            tRT.anchorMax = new Vector2(1, 1);
            tRT.pivot = new Vector2(0, 1);
            tRT.sizeDelta = new Vector2(-24, 30);
            tRT.anchoredPosition = new Vector2(16, -8);

            var barBgGO = new GameObject("ProgressBg");
            barBgGO.transform.SetParent(transform, false);
            var barBgRT = barBgGO.AddComponent<RectTransform>();
            barBgRT.anchorMin = new Vector2(0, 0.5f);
            barBgRT.anchorMax = new Vector2(1, 0.5f);
            barBgRT.sizeDelta = new Vector2(-32, 20);
            barBgRT.anchoredPosition = new Vector2(16, -2);
            var barBgImg = barBgGO.AddComponent<Image>();
            barBgImg.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

            var barFillGO = new GameObject("ProgressFill");
            barFillGO.transform.SetParent(barBgGO.transform, false);
            var barFillRT = barFillGO.AddComponent<RectTransform>();
            barFillRT.anchorMin = Vector2.zero;
            barFillRT.anchorMax = new Vector2(0, 1);
            barFillRT.sizeDelta = Vector2.zero;
            barFillRT.pivot = new Vector2(0, 0.5f);
            progressFill = barFillGO.AddComponent<Image>();
            progressFill.color = new Color(0.25f, 0.6f, 0.95f, 1f);

            var progTextGO = new GameObject("ProgressText");
            progTextGO.transform.SetParent(barBgGO.transform, false);
            progressText = progTextGO.AddComponent<TextMeshProUGUI>();
            progressText.fontSize = 14;
            progressText.alignment = TMPro.TextAlignmentOptions.Center;
            var progRT = progTextGO.rectTransform();
            progRT.anchorMin = Vector2.zero;
            progRT.anchorMax = Vector2.one;
            progRT.sizeDelta = Vector2.zero;

            var statusGO = new GameObject("Status");
            statusGO.transform.SetParent(transform, false);
            statusLabel = statusGO.AddComponent<TextMeshProUGUI>();
            statusLabel.fontSize = 15;
            var sRT = statusGO.rectTransform();
            sRT.anchorMin = new Vector2(0, 0);
            sRT.anchorMax = new Vector2(1, 0);
            sRT.sizeDelta = new Vector2(-24, 24);
            sRT.anchoredPosition = new Vector2(16, 8);
        }

        public void Refresh()
        {
            if (QuestSystemManager.Instance == null) return;
            var progress = QuestSystemManager.Instance.GetProgress(Quest.Id);
            if (progress == null) return;

            float ratio = 0f;
            string progInfo = "";
            if (Quest.Conditions.Count > 0)
            {
                ratio = progress.IsCompleted ? 1f : (progress.ConditionValues[0] / Quest.Conditions[0].TargetValue);
                ratio = Mathf.Clamp01(ratio);
                progInfo = progress.IsCompleted
                    ? "全部完成"
                    : Quest.Conditions[0].GetProgressText(progress.ConditionValues[0]);
            }

            progressFill.rectTransform().anchorMax = new Vector2(ratio, 1);
            progressText.text = progInfo;

            if (progress.IsCompleted && progress.IsRewardClaimed)
            {
                statusLabel.text = "<color=#55dd55>已领取</color>";
                progressFill.color = new Color(0.3f, 0.75f, 0.3f, 1f);
            }
            else if (progress.IsCompleted)
            {
                statusLabel.text = "<color=#ffdd44>已完成 点击领取</color>";
                progressFill.color = new Color(0.9f, 0.75f, 0.2f, 1f);
            }
            else if (!progress.IsUnlocked)
            {
                statusLabel.text = "<color=#888888>未解锁</color>";
                progressFill.color = new Color(0.3f, 0.3f, 0.35f, 1f);
            }
            else
            {
                statusLabel.text = "<color=#aaddff>进行中</color>";
                progressFill.color = new Color(0.25f, 0.6f, 0.95f, 1f);
            }
        }
    }
}
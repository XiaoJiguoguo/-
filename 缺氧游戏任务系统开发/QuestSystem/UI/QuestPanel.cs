using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QuestSystem.Core;

namespace QuestSystem.UI
{
    public class QuestPanel : KScreen
    {
        public static QuestPanel Instance { get; private set; }

        private CanvasGroup canvasGroup;
        private GameObject backgroundPanel;
        private GameObject leftPanel;
        private GameObject centerPanel;
        private GameObject rightPanel;

        private ScrollRect taskScrollRect;
        private RectTransform taskContent;
        private List<QuestCardWidget> cardWidgets = new();

        private List<GameObject> phaseToggleObjects = new();
        private QuestPhase currentPhase = QuestPhase.Newbie;
        private string currentFilter = "ALL";

        private QuestDefinition selectedQuest;
        private TextMeshProUGUI detailTitle;
        private GameObject detailContent;
        private TextMeshProUGUI detailDesc;
        private TextMeshProUGUI detailProgress;
        private KButton claimButton;

        private bool uiBuilt;
        private TMP_FontAsset gameFont;

        // ── 静态颜色样式（参考 StorageNetwork Style.cs） ──
        private static ColorStyleSetting _pinkStyle;
        private static ColorStyleSetting _blueStyle;

        private static ColorStyleSetting PinkStyle
        {
            get
            {
                if (_pinkStyle == null)
                {
                    _pinkStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                    _pinkStyle.inactiveColor = new Color(0.53f, 0.27f, 0.40f, 1f);
                    _pinkStyle.hoverColor = new Color(0.62f, 0.33f, 0.47f, 1f);
                    _pinkStyle.activeColor = new Color(0.79f, 0.45f, 0.62f, 1f);
                    _pinkStyle.disabledColor = new Color(0.42f, 0.41f, 0.40f, 1f);
                    _pinkStyle.disabledActiveColor = Color.clear;
                    _pinkStyle.disabledhoverColor = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                return _pinkStyle;
            }
        }

        private static ColorStyleSetting BlueStyle
        {
            get
            {
                if (_blueStyle == null)
                {
                    _blueStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                    _blueStyle.inactiveColor = new Color(0.17f, 0.19f, 0.25f, 1f);
                    _blueStyle.hoverColor = new Color(0.25f, 0.28f, 0.35f, 1f);
                    _blueStyle.activeColor = new Color(0.11f, 0.12f, 0.16f, 1f);
                    _blueStyle.disabledColor = new Color(0.42f, 0.41f, 0.40f, 1f);
                    _blueStyle.disabledActiveColor = new Color(0.63f, 0.62f, 0.59f, 1f);
                    _blueStyle.disabledhoverColor = new Color(0.50f, 0.49f, 0.46f, 1f);
                }
                return _blueStyle;
            }
        }

        private TMP_FontAsset GameFont
        {
            get
            {
                if (gameFont == null)
                {
                    try { gameFont = Localization.GetFont(Localization.GetCurrentLanguageCode()) as TMP_FontAsset; }
                    catch { }
                }
                return gameFont;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  KScreen 生命周期
        // ═══════════════════════════════════════════════════════════════

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
            activateOnSpawn = false;
            canvasGroup = gameObject.AddOrGet<CanvasGroup>();
            var rt = gameObject.AddOrGet<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        protected override void OnSpawn() { base.OnSpawn(); }

        public override float GetSortKey() => 25f;

        public override void Show(bool show = true)
        {
            if (!uiBuilt && show)
            {
                TryBuildUI();
                if (!uiBuilt) return;
            }

            canvasGroup.alpha = show ? 1f : 0f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
            if (show)
            {
                RefreshUI();
                if (CameraController.Instance != null)
                    CameraController.Instance.DisableUserCameraControl = true;
            }
            else
            {
                if (CameraController.Instance != null)
                    CameraController.Instance.DisableUserCameraControl = false;
            }
        }

        private void TryBuildUI()
        {
            try
            {
                BuildFullscreenBackground();
                BuildLeftPanel();
                BuildCenterPanel();
                BuildRightPanel();
                uiBuilt = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[QuestSystem] UI构建失败: {e}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  ONI 风格按钮工厂（参考 StorageNetwork Panel.UI.cs）
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// 创建 ONI 原生风格按钮：KImage + ColorStyleSetting + KButton
        /// 带游戏纹理、悬停/点击颜色过渡、点击音效
        /// </summary>
        private static GameObject CreateStyledButton(Transform parent, string name, string text,
            System.Action onClick, ColorStyleSetting style, float minHeight = 36f, float prefHeight = 40f,
            float flexibleHeight = 1f, int fontSize = 14)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();

            // KImage 背景 — 应用游戏内置按钮纹理
            var image = go.AddComponent<KImage>();
            image.type = Image.Type.Sliced;
            ApplyThinButtonSprite(image);
            image.colorStyleSetting = style;
            image.ColorState = KImage.ColorSelector.Inactive;

            // KButton — 手动赋值避免 Awake 崩溃
            var button = go.AddComponent<KButton>();
            button.bgImage = image;
            button.additionalKImages = new KImage[0];
            button.soundPlayer = new ButtonSoundPlayer();
            button.onClick += () => onClick?.Invoke();

            // LayoutElement
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = minHeight;
            le.preferredHeight = prefHeight;
            le.flexibleHeight = flexibleHeight;

            // 文字标签
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var lRT = labelGO.AddComponent<RectTransform>();
            lRT.anchorMin = Vector2.zero;
            lRT.anchorMax = Vector2.one;
            lRT.offsetMin = new Vector2(8, 0);
            lRT.offsetMax = new Vector2(-8, 0);
            var txt = labelGO.AddComponent<TextMeshProUGUI>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.alignment = TMPro.TextAlignmentOptions.Center;
            txt.color = Color.white;
            txt.raycastTarget = false;

            return go;
        }

        /// <summary>
        /// 应用游戏内置按钮纹理（web_button），找不到则保持纯色
        /// </summary>
        private static void ApplyThinButtonSprite(KImage image)
        {
            if (image == null) return;
            Sprite sprite = Assets.GetSprite("web_button");
            if (sprite == null) return;
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
            image.fillCenter = true;
        }

        // ═══════════════════════════════════════════════════════════════
        //  面板构建
        // ═══════════════════════════════════════════════════════════════

        private void BuildFullscreenBackground()
        {
            backgroundPanel = new GameObject("PanelBackground");
            backgroundPanel.transform.SetParent(transform, false);
            var bgRT = backgroundPanel.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = new Vector2(60, 80);
            bgRT.offsetMax = new Vector2(-60, -40);
            var bgImg = backgroundPanel.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.15f, 0.2f, 0.96f);

            // 标题栏
            var titleGO = new GameObject("TitleBar");
            titleGO.transform.SetParent(backgroundPanel.transform, false);
            var titleRT = titleGO.AddComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 1); titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot = new Vector2(0, 1); titleRT.sizeDelta = new Vector2(0, 44);
            var titleImg = titleGO.AddComponent<Image>();
            titleImg.color = new Color(0.18f, 0.22f, 0.28f, 1f);

            var titleLabelGO = new GameObject("TitleLabel");
            titleLabelGO.transform.SetParent(titleGO.transform, false);
            var tLRT = titleLabelGO.AddComponent<RectTransform>();
            tLRT.anchorMin = Vector2.zero; tLRT.anchorMax = Vector2.one;
            tLRT.offsetMin = new Vector2(16, 0); tLRT.offsetMax = new Vector2(0, 0);
            var tLabel = titleLabelGO.AddComponent<TextMeshProUGUI>();
            tLabel.text = "任务系统";
            tLabel.fontSize = 22;
            tLabel.color = new Color(0.9f, 0.85f, 0.7f);
            if (GameFont != null) tLabel.font = GameFont;

            // 关闭按钮 — ONI 风格
            CreateStyledButton(titleGO.transform, "CloseBtn", "X", () => Show(false),
                BlueStyle, minHeight: 32, prefHeight: 32, flexibleHeight: 0, fontSize: 18);
            // 定位到右上角
            var closeObj = titleGO.transform.Find("CloseBtn");
            if (closeObj != null)
            {
                var crt = closeObj.GetComponent<RectTransform>();
                crt.anchorMin = new Vector2(1, 0.5f);
                crt.anchorMax = new Vector2(1, 0.5f);
                crt.pivot = new Vector2(1, 0.5f);
                crt.anchoredPosition = new Vector2(-8, 0);
                crt.sizeDelta = new Vector2(32, 32);
            }
        }

        private void BuildLeftPanel()
        {
            leftPanel = new GameObject("PhaseNavPanel");
            leftPanel.transform.SetParent(backgroundPanel.transform, false);
            var rt = leftPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 0); rt.sizeDelta = new Vector2(200, 0);
            rt.anchoredPosition = new Vector2(8, -52);
            var bg = leftPanel.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.22f, 0.92f);

            var vlg = leftPanel.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 6;
            vlg.padding = new RectOffset(8, 8, 12, 12);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
        }

        private void BuildCenterPanel()
        {
            centerPanel = new GameObject("QuestListPanel");
            centerPanel.transform.SetParent(backgroundPanel.transform, false);
            var rt = centerPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(200, 0); rt.offsetMax = new Vector2(-320, -52);

            // 筛选按钮栏 — ONI 蓝色风格
            var filterBarGO = new GameObject("FilterBar");
            filterBarGO.transform.SetParent(centerPanel.transform, false);
            var fRT = filterBarGO.AddComponent<RectTransform>();
            fRT.anchorMin = new Vector2(0, 1); fRT.anchorMax = new Vector2(1, 1);
            fRT.pivot = new Vector2(0, 1); fRT.sizeDelta = new Vector2(0, 48);
            var hlg = filterBarGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8; hlg.padding = new RectOffset(10, 10, 8, 8);
            hlg.childForceExpandWidth = false; hlg.childForceExpandHeight = true;

            var filterNames = new[] { "全部", "主线", "支线", "故障", "成就" };
            var filterValues = new[] { "ALL", "Main", "Side", "Fault", "Hidden" };
            for (int i = 0; i < filterNames.Length; i++)
            {
                var filterVal = filterValues[i];
                CreateStyledButton(filterBarGO.transform, "Filter:" + filterVal, filterNames[i],
                    () => SetFilter(filterVal), BlueStyle,
                    minHeight: 36, prefHeight: 36, flexibleHeight: 0, fontSize: 16);
            }
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(filterBarGO.GetComponent<RectTransform>());

            // ScrollRect
            var scrollGO = new GameObject("TaskScrollRect");
            scrollGO.transform.SetParent(centerPanel.transform, false);
            var sRT = scrollGO.AddComponent<RectTransform>();
            sRT.anchorMin = Vector2.zero; sRT.anchorMax = Vector2.one;
            sRT.offsetMin = Vector2.zero; sRT.offsetMax = new Vector2(0, -40);
            var sImg = scrollGO.AddComponent<Image>();
            sImg.color = new Color(0.08f, 0.11f, 0.16f, 1f);

            var viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(scrollGO.transform, false);
            var vRT = viewportGO.AddComponent<RectTransform>();
            vRT.anchorMin = Vector2.zero; vRT.anchorMax = Vector2.one; vRT.sizeDelta = Vector2.zero;
            viewportGO.AddComponent<RectMask2D>();
            var vImg = viewportGO.AddComponent<Image>();
            vImg.color = new Color(0, 0, 0, 0);

            var contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform, false);
            var cRT = contentGO.AddComponent<RectTransform>();
            cRT.anchorMin = new Vector2(0, 1); cRT.anchorMax = new Vector2(1, 1);
            cRT.pivot = new Vector2(0.5f, 1); cRT.sizeDelta = new Vector2(0, 0);
            var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8; vlg.padding = new RectOffset(10, 10, 10, 10);
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

            taskScrollRect = scrollGO.AddComponent<ScrollRect>();
            taskScrollRect.vertical = true;
            taskScrollRect.horizontal = false;
            taskScrollRect.movementType = ScrollRect.MovementType.Clamped;
            taskScrollRect.viewport = vRT;
            taskScrollRect.content = cRT;
            taskContent = cRT;
        }

        private void BuildRightPanel()
        {
            rightPanel = new GameObject("QuestDetailPanel");
            rightPanel.transform.SetParent(backgroundPanel.transform, false);
            var rt = rightPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0); rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0); rt.sizeDelta = new Vector2(300, 0);
            rt.anchoredPosition = new Vector2(-8, -52);
            var bg = rightPanel.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.22f, 0.92f);
            var vlg = rightPanel.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8; vlg.padding = new RectOffset(12, 12, 12, 12);
            vlg.childForceExpandWidth = true; vlg.childForceExpandHeight = false;

            // 详情标题
            var titleGO = new GameObject("DetailTitle");
            titleGO.transform.SetParent(rightPanel.transform, false);
            var tLE = titleGO.AddComponent<LayoutElement>();
            tLE.minHeight = 28; tLE.preferredHeight = 28;
            detailTitle = titleGO.AddComponent<TextMeshProUGUI>();
            detailTitle.text = "<i>选择一个任务查看详情</i>";
            detailTitle.fontSize = 22;

            // 描述
            var descGO = new GameObject("DetailDesc");
            descGO.transform.SetParent(rightPanel.transform, false);
            var dLE = descGO.AddComponent<LayoutElement>();
            dLE.minHeight = 24; dLE.preferredHeight = 24;
            detailDesc = descGO.AddComponent<TextMeshProUGUI>();
            detailDesc.text = "";
            detailDesc.fontSize = 16;

            // 目标列表容器
            var detailParent = new GameObject("DetailProgressContainer");
            detailParent.transform.SetParent(rightPanel.transform, false);
            var dpLE = detailParent.AddComponent<LayoutElement>();
            dpLE.minHeight = 100; dpLE.flexibleHeight = 1;
            var dpVLG = detailParent.AddComponent<VerticalLayoutGroup>();
            dpVLG.spacing = 4; dpVLG.childForceExpandWidth = true; dpVLG.childForceExpandHeight = false;

            var pgGO = new GameObject("ProgressText");
            pgGO.transform.SetParent(detailParent.transform, false);
            var pgLE = pgGO.AddComponent<LayoutElement>();
            pgLE.minHeight = 20; pgLE.flexibleHeight = 1;
            detailProgress = pgGO.AddComponent<TextMeshProUGUI>();
            detailProgress.text = "";
            detailProgress.fontSize = 15;

            detailContent = detailParent;

            // 领取奖励按钮 — ONI 粉色风格
            var claimBtnGO = new GameObject("ClaimButtonContainer");
            claimBtnGO.transform.SetParent(rightPanel.transform, false);
            var cbLE = claimBtnGO.AddComponent<LayoutElement>();
            cbLE.minHeight = 44; cbLE.preferredHeight = 44;

            claimButton = CreateStyledButton(claimBtnGO.transform, "ClaimBtn", "领取奖励",
                ClaimSelectedReward, PinkStyle, minHeight: 40, prefHeight: 40, flexibleHeight: 0, fontSize: 16)
                .GetComponent<KButton>();
            claimBtnGO.SetActive(false);

            // 占位
            var dummyGO = new GameObject("Spacer");
            dummyGO.transform.SetParent(rightPanel.transform, false);
            var dumLE = dummyGO.AddComponent<LayoutElement>();
            dumLE.minHeight = 20; dumLE.preferredHeight = 20;
        }

        // ═══════════════════════════════════════════════════════════════
        //  筛选 / 刷新
        // ═══════════════════════════════════════════════════════════════

        private void SetFilter(string filter)
        {
            currentFilter = filter;
            RefreshQuestList();
        }

        public void RefreshUI()
        {
            if (!uiBuilt) return;
            RebuildPhaseToggles();
            RefreshQuestList();
            UpdateSelectedDetail();
        }

        private void RebuildPhaseToggles()
        {
            foreach (var t in phaseToggleObjects)
                if (t != null) Destroy(t);
            phaseToggleObjects.Clear();

            var phases = new (QuestPhase, string)[]
            {
                (QuestPhase.Newbie,       "新手开荒"),
                (QuestPhase.Development,  "基地发展"),
                (QuestPhase.Industry,     "工业腾飞"),
                (QuestPhase.Space,        "星际探索"),
                (QuestPhase.DLC,          "DLC专属"),
                (QuestPhase.Global,       "全周期通用"),
            };

            var first = true;
            foreach (var (phase, label) in phases)
            {
                int count = QuestSystemManager.Instance != null
                    ? QuestSystemManager.Instance.GetPhaseQuests(phase).Count : 0;
                string fullLabel = $"{label}  ({count})";

                var captured = phase;
                var isFirst = first;
                var btn = CreateStyledButton(leftPanel.transform, "Phase:" + phase, fullLabel,
                    () => { currentPhase = captured; RefreshQuestList(); },
                    PinkStyle, minHeight: 44, prefHeight: 52, flexibleHeight: 1, fontSize: 16);

                if (first) first = false;
                phaseToggleObjects.Add(btn);
            }
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(leftPanel.GetComponent<RectTransform>());
        }

        private void RefreshQuestList()
        {
            foreach (var card in cardWidgets)
                if (card != null && card.gameObject != null) Destroy(card.gameObject);
            cardWidgets.Clear();
            if (QuestSystemManager.Instance == null) return;

            var quests = QuestSystemManager.Instance.GetPhaseQuests(currentPhase);
            foreach (var quest in quests)
            {
                if (!MatchesFilter(quest)) continue;
                var go = new GameObject("QuestCard_" + quest.Id);
                go.transform.SetParent(taskContent, false);
                var card = go.AddComponent<QuestCardWidget>();
                card.Setup(quest, OnQuestSelected);
                cardWidgets.Add(card);
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(taskContent);
            float totalHeight = 20 + cardWidgets.Count * 110 + Mathf.Max(0, cardWidgets.Count - 1) * 8;
            taskContent.sizeDelta = new Vector2(0, Mathf.Max(totalHeight, 100));
        }

        private bool MatchesFilter(QuestDefinition quest) => currentFilter switch
        {
            "ALL" => true,
            "Main" => quest.Type == QuestType.Main,
            "Side" => quest.Type == QuestType.Side,
            "Fault" => quest.Type == QuestType.Fault || quest.Type == QuestType.Emergency,
            "Hidden" => quest.Type == QuestType.Hidden,
            _ => true,
        };

        private void OnQuestSelected(QuestDefinition quest)
        {
            selectedQuest = quest;
            UpdateSelectedDetail();
        }

        // ═══════════════════════════════════════════════════════════════
        //  右侧详情
        // ═══════════════════════════════════════════════════════════════

        private void UpdateSelectedDetail()
        {
            if (detailTitle == null) return;

            if (selectedQuest == null)
            {
                detailTitle.text = "<i>选择一个任务查看详情</i>";
                detailProgress.text = "";
                if (claimButton != null) claimButton.gameObject.SetActive(false);
                return;
            }

            detailTitle.text = selectedQuest.Name;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<b>阶段:</b> {selectedQuest.Phase}");
            sb.AppendLine($"<b>类型:</b> {selectedQuest.Type}");
            sb.AppendLine($"<b>难度:</b> {selectedQuest.Difficulty}");
            sb.AppendLine();
            sb.AppendLine($"<b>描述:</b> {selectedQuest.Description}");
            sb.AppendLine();
            sb.AppendLine("<b>任务目标:</b>");

            if (QuestSystemManager.Instance != null)
            {
                var progress = QuestSystemManager.Instance.GetProgress(selectedQuest.Id);
                for (int i = 0; i < selectedQuest.Conditions.Count; i++)
                {
                    var cond = selectedQuest.Conditions[i];
                    float val = progress != null && i < progress.ConditionValues.Count
                        ? progress.ConditionValues[i] : 0f;
                    string check = cond.IsMet(val) ? "✓" : "○";
                    sb.AppendLine($"  {check} {cond.Description}: {cond.GetProgressText(val)}");
                }
            }

            if (selectedQuest.Rewards.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("<b>奖励:</b>");
                foreach (var r in selectedQuest.Rewards) sb.AppendLine($"  · {r}");
            }

            detailProgress.text = sb.ToString();

            if (claimButton != null)
            {
                bool canClaim = false;
                if (QuestSystemManager.Instance != null)
                {
                    var p = QuestSystemManager.Instance.GetProgress(selectedQuest.Id);
                    canClaim = p != null && p.IsCompleted && !p.IsRewardClaimed;
                }
                claimButton.gameObject.SetActive(canClaim);
            }
        }

        private void ClaimSelectedReward()
        {
            if (selectedQuest == null) return;
            QuestSystemManager.Instance?.GrantReward(selectedQuest.Id);
            UpdateSelectedDetail();
            foreach (var card in cardWidgets) card.Refresh();
        }
    }
}
using System.Collections.Generic;
using KSerialization;
using UnityEngine;
using QuestSystem.Components;
using QuestSystem.Config;
using QuestSystem.Services;

namespace QuestSystem.Core
{
    public class QuestSystemManager : KMonoBehaviour
    {
        public static QuestSystemManager Instance { get; private set; }

        [Serialize]
        private List<QuestProgress> progressList = new List<QuestProgress>();

        private Dictionary<string, QuestProgress> progressMap = new Dictionary<string, QuestProgress>();
        private List<QuestDefinition> allDefinitions = new List<QuestDefinition>();
        private Dictionary<QuestPhase, bool> phaseCompleted = new Dictionary<QuestPhase, bool>();

        private BuildingCheckService buildingCheck;
        private ResourceCheckService resourceCheck;
        private RoomCheckService roomCheck;
        private TechCheckService techCheck;
        private MinionCheckService minionCheck;

        private bool isInitialized;
        private float tickTimer;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (!isInitialized)
            {
                LoadQuestDefinitions();
                RebuildProgressMap();
                InitServices();
                isInitialized = true;
                Debug.Log("[QuestSystem] QuestSystemManager 初始化完成!");
            }
        }

        private void Update()
        {
            if (!isInitialized) return;
            tickTimer += Time.deltaTime;
            if (tickTimer >= 1f)
            {
                tickTimer = 0f;
                Tick();
            }
        }

        private void LoadQuestDefinitions()
        {
            allDefinitions = QuestConfig.GetAllQuests();
            foreach (var def in allDefinitions)
            {
                if (!progressMap.ContainsKey(def.Id))
                {
                    var progress = new QuestProgress
                    {
                        QuestId = def.Id,
                        IsUnlocked = string.IsNullOrEmpty(def.PrerequisiteQuestId) && !def.IsPhaseGated,
                        IsCompleted = false,
                        IsRewardClaimed = false,
                        Definition = def
                    };
                    progress.ConditionValues = new List<float>();
                    for (int i = 0; i < def.Conditions.Count; i++)
                        progress.ConditionValues.Add(0f);
                    progressList.Add(progress);
                    progressMap[def.Id] = progress;
                }
            }
        }

        private void RebuildProgressMap()
        {
            progressMap.Clear();
            foreach (var p in progressList)
            {
                var def = allDefinitions.Find(d => d.Id == p.QuestId);
                if (def != null)
                {
                    p.Definition = def;
                    progressMap[p.QuestId] = p;
                }
            }
        }

        private void InitServices()
        {
            buildingCheck = new BuildingCheckService();
            resourceCheck = new ResourceCheckService();
            roomCheck = new RoomCheckService();
            techCheck = new TechCheckService();
            minionCheck = new MinionCheckService();
        }

        public void Tick()
        {
            if (!isInitialized) return;
            UnlockPhaseGatedQuests();
            UpdateAllProgress();
        }

        private void UnlockPhaseGatedQuests()
        {
            bool newbieDone = IsPhaseMainComplete(QuestPhase.Newbie);
            bool devDone = IsPhaseMainComplete(QuestPhase.Development);
            bool industryDone = IsPhaseMainComplete(QuestPhase.Industry);

            foreach (var def in allDefinitions)
            {
                if (!def.IsPhaseGated) continue;
                var progress = GetProgress(def.Id);
                if (progress == null || progress.IsUnlocked) continue;

                bool shouldUnlock = def.Phase switch
                {
                    QuestPhase.Newbie => true,
                    QuestPhase.Development => newbieDone,
                    QuestPhase.Industry => devDone && newbieDone,
                    QuestPhase.Space => industryDone && devDone && newbieDone,
                    QuestPhase.DLC => CheckDlcInstalled(),
                    QuestPhase.Global => true,
                    _ => false
                };

                if (shouldUnlock)
                {
                    progress.IsUnlocked = true;
                }
            }
        }

        private bool CheckDlcInstalled()
        {
            try { return DlcManager.IsContentSubscribed("EXPANSION1_ID"); }
            catch { return false; }
        }

        public void UpdateAllProgress()
        {
            foreach (var def in allDefinitions)
            {
                var progress = GetProgress(def.Id);
                if (progress == null || !progress.IsUnlocked || progress.IsCompleted) continue;

                bool allMet = true;
                for (int i = 0; i < def.Conditions.Count; i++)
                {
                    float val = CheckCondition(def.Conditions[i]);
                    progress.ConditionValues[i] = val;
                    if (!def.Conditions[i].IsMet(val))
                        allMet = false;
                }

                if (allMet && def.Conditions.Count > 0)
                {
                    progress.IsCompleted = true;
                    TryAutoGrantReward(progress);
                    TriggerNotification(progress);
                }
            }
        }

        public float CheckCondition(QuestCondition condition)
        {
            return condition.Type switch
            {
                QuestConditionType.BuildingCount => buildingCheck.CountBuildings(condition.TargetId),
                QuestConditionType.ResourceAmount => resourceCheck.GetResourceAmount(condition.TargetId),
                QuestConditionType.ResearchUnlocked => techCheck.IsResearchUnlocked(condition.TargetId) ? 1f : 0f,
                QuestConditionType.TechTreeComplete => techCheck.IsTechTreeComplete() ? 1f : 0f,
                QuestConditionType.MinionCount => minionCheck.GetMinionCount(),
                QuestConditionType.MinionSkillLevel => minionCheck.GetMaxSkillLevel(condition.TargetId),
                QuestConditionType.RoomExists => roomCheck.RoomExists(condition.TargetId) ? 1f : 0f,
                QuestConditionType.RoomPressureRange => roomCheck.GetRoomPressure(condition.TargetId),
                QuestConditionType.RoomTemperatureRange => roomCheck.GetRoomTemperature(condition.TargetId),
                QuestConditionType.RoomSealed => roomCheck.IsRoomSealed(condition.TargetId) ? 1f : 0f,
                QuestConditionType.PipeConnected => roomCheck.IsPipeConnected(condition.TargetId) ? 1f : 0f,
                QuestConditionType.PipeMediumCorrect => roomCheck.IsPipeMediumCorrect(condition.TargetId) ? 1f : 0f,
                QuestConditionType.CycleCount => GameClock.Instance != null ? GameClock.Instance.GetCycle() : 0f,
                QuestConditionType.DlcInstalled => CheckDlcInstalled() ? 1f : 0f,
                _ => 0f
            };
        }

        public bool IsPhaseMainComplete(QuestPhase phase)
        {
            foreach (var def in allDefinitions)
            {
                if (def.Phase == phase && def.Type == QuestType.Main)
                {
                    var p = GetProgress(def.Id);
                    if (p == null || !p.IsCompleted) return false;
                }
            }
            return true;
        }

        public QuestProgress GetProgress(string questId)
        {
            progressMap.TryGetValue(questId, out var p);
            return p;
        }

        public List<QuestDefinition> GetPhaseQuests(QuestPhase phase)
        {
            return allDefinitions.FindAll(d => d.Phase == phase);
        }

        public List<QuestDefinition> GetAllDefinitions() => allDefinitions;

        public bool HasCompletedQuest(string questId)
        {
            var p = GetProgress(questId);
            return p != null && p.IsCompleted;
        }

        public bool AllMainQuestsCompleted()
        {
            foreach (var def in allDefinitions)
                if (def.Type == QuestType.Main)
                    if (!HasCompletedQuest(def.Id)) return false;
            return true;
        }

        public void GrantReward(string questId)
        {
            var progress = GetProgress(questId);
            if (progress == null || progress.IsRewardClaimed) return;

            var def = progress.Definition;
            foreach (var rewardId in def.Rewards)
            {
                Debug.Log($"[QuestSystem] 发放奖励: {rewardId} for quest {questId}");
                resourceCheck.AddResource(rewardId, 1000f);
            }

            progress.IsRewardClaimed = true;
        }

        private void TryAutoGrantReward(QuestProgress progress)
        {
            GrantReward(progress.QuestId);
        }

        private void TriggerNotification(QuestProgress progress)
        {
            var def = progress.Definition;
            if (def.Type == QuestType.Hidden) return;

            var notif = QuestNotification.Create(def.Name, $"任务完成! 奖励已发放。", () =>
            {
                GrantReward(progress.QuestId);
            });
            notif?.Show();
        }

        public void ForceCompleteQuest(string questId)
        {
            var progress = GetProgress(questId);
            if (progress == null) return;
            progress.IsCompleted = true;
            progress.IsUnlocked = true;
            for (int i = 0; i < progress.ConditionValues.Count; i++)
                progress.ConditionValues[i] = progress.Definition.Conditions[i].TargetValue;
        }
    }
}
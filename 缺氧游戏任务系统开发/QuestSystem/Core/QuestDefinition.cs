using System.Collections.Generic;

namespace QuestSystem.Core
{
    public class QuestDefinition
    {
        public string Id;
        public string Name;
        public string Description;
        public QuestPhase Phase;
        public QuestType Type;
        public string Difficulty;
        public List<QuestCondition> Conditions = new List<QuestCondition>();
        public List<string> PreRewards = new List<string>();
        public List<string> Rewards = new List<string>();
        public List<string> BuildingRequirements = new List<string>();
        public string PrerequisiteQuestId;
        public bool IsPhaseGated;
    }
}
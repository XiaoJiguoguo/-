using System.Collections.Generic;
using KSerialization;

namespace QuestSystem.Core
{
    [System.Serializable]
    public class QuestProgress : KMonoBehaviour
    {
        [Serialize]
        public string QuestId;

        [Serialize]
        public bool IsUnlocked;

        [Serialize]
        public bool IsCompleted;

        [Serialize]
        public bool IsRewardClaimed;

        [Serialize]
        public List<float> ConditionValues = new List<float>();

        public QuestDefinition Definition;
    }
}
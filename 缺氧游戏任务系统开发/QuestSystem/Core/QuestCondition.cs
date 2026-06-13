using UnityEngine;

namespace QuestSystem.Core
{
    [System.Serializable]
    public class QuestCondition
    {
        public QuestConditionType Type;
        public string TargetId;
        public string Description;
        public float TargetValue;
        public float MinValue;
        public float MaxValue;
        public string ExtraParam;

        public bool IsMet(float currentValue)
        {
            if (MinValue > 0 || MaxValue > 0)
                return currentValue >= MinValue && currentValue <= MaxValue;
            return currentValue >= TargetValue;
        }

        public string GetProgressText(float currentValue)
        {
            if (MinValue > 0 || MaxValue > 0)
                return $"{currentValue:F1} ({MinValue}~{MaxValue})";
            return $"{currentValue:F0}/{TargetValue:F0}";
        }
    }
}
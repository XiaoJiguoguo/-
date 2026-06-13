namespace QuestSystem.Core
{
    public class QuestReward
    {
        public string Id;
        public string Name;
        public string Type;
        public string TargetId;
        public float Amount;
        public System.Action OnGrant;
    }
}
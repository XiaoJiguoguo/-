namespace QuestSystem.Services
{
    public class MinionCheckService
    {
        public float GetMinionCount()
        {
            return global::Components.MinionIdentities.Count;
        }

        public float GetMaxSkillLevel(string attributeId)
        {
            if (string.IsNullOrEmpty(attributeId)) return 0f;

            float maxLevel = 0f;
            foreach (MinionIdentity minion in global::Components.MinionIdentities.Items)
            {
                if (minion == null) continue;
                try
                {
                    var attr = Db.Get().Attributes.Get(attributeId);
                    if (attr != null)
                    {
                        var instance = attr.Lookup(minion);
                        if (instance != null)
                        {
                            float level = instance.GetTotalValue();
                            if (level > maxLevel) maxLevel = level;
                        }
                    }
                }
                catch { }
            }
            return maxLevel;
        }
    }
}
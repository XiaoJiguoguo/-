namespace QuestSystem.Services
{
    public class TechCheckService
    {
        public bool IsResearchUnlocked(string techId)
        {
            if (string.IsNullOrEmpty(techId)) return false;
            try
            {
                var tech = Db.Get().Techs.Get(techId);
                return tech != null && tech.IsComplete();
            }
            catch { return false; }
        }

        public bool IsTechTreeComplete()
        {
            try
            {
                foreach (var tech in Db.Get().Techs.resources)
                {
                    if (tech != null && !tech.IsComplete())
                        return false;
                }
                return true;
            }
            catch { return false; }
        }
    }
}
using UnityEngine;
using QuestSystem.Core;

namespace QuestSystem.Components
{
    public class QuestWorldTracker : KMonoBehaviour
    {
        public static QuestWorldTracker Instance { get; private set; }

        private float tickTimer;
        private const float TickInterval = 5f;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            tickTimer = 0f;
        }

        private void Update()
        {
            tickTimer += Time.unscaledDeltaTime;
            if (tickTimer >= TickInterval)
            {
                tickTimer = 0f;
                if (QuestSystemManager.Instance != null)
                    QuestSystemManager.Instance.Tick();
            }
        }
    }
}
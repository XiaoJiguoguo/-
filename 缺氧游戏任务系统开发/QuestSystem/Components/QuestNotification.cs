using UnityEngine;

namespace QuestSystem.Components
{
    public class QuestNotification : KMonoBehaviour
    {
        public static QuestNotification Instance { get; private set; }

        private System.Action onConfirmCallback;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;
        }

        public static QuestNotification Create(string title, string message, System.Action onConfirm)
        {
            var go = new GameObject("QuestNotification");
            var notif = go.AddComponent<QuestNotification>();
            notif.onConfirmCallback = onConfirm;
            return notif;
        }

        public void Show()
        {
            Debug.Log($"[QuestSystem] 任务通知: 请查看任务面板领取奖励!");
        }
    }
}
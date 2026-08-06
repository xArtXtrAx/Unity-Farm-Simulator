using System;
using FarmSimulator.Domain.Time;
using UnityEngine;

namespace FarmSimulator.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class GameSessionRuntime : MonoBehaviour
    {
        private static GameSessionRuntime instance;

        private GameCalendarState calendar;
        private string pendingSpawnId;

        public static GameSessionRuntime Instance
        {
            get
            {
                EnsureInstance();
                return instance;
            }
        }

        public GameDate CurrentDate => Calendar.CurrentDate;

        public event Action<GameDate> DayChanged;

        private GameCalendarState Calendar =>
            calendar ??= new GameCalendarState();

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            EnsureInstance();
        }

        private static void EnsureInstance()
        {
            if (instance != null)
            {
                return;
            }

            GameSessionRuntime existing =
                UnityEngine.Object.FindFirstObjectByType<
                    GameSessionRuntime>();
            if (existing != null)
            {
                instance = existing;
                return;
            }

            var sessionObject =
                new GameObject(nameof(GameSessionRuntime));
            instance =
                sessionObject.AddComponent<GameSessionRuntime>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            calendar ??= new GameCalendarState();
            DontDestroyOnLoad(gameObject);
        }

        public GameDate AdvanceDay()
        {
            GameDate nextDate = Calendar.AdvanceDay();
            DayChanged?.Invoke(nextDate);
            Debug.Log(
                $"[GameSession] New day: year {nextDate.Year}, " +
                $"{nextDate.Season} {nextDate.DayOfSeason}.");
            return nextDate;
        }

        public void SetPendingSpawn(string spawnId)
        {
            pendingSpawnId = string.IsNullOrWhiteSpace(spawnId)
                ? null
                : spawnId;
        }

        public string ConsumePendingSpawn()
        {
            string spawnId = pendingSpawnId;
            pendingSpawnId = null;
            return spawnId;
        }

        public void ResetSession()
        {
            calendar = new GameCalendarState();
            pendingSpawnId = null;
            DayChanged?.Invoke(calendar.CurrentDate);
        }
    }
}

using System;
using FarmSimulator.Domain.Time;
using FarmSimulator.Presentation.World;
using UnityEngine;
using UnityEngine.UI;

namespace FarmSimulator.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class DayLabelView : MonoBehaviour
    {
        [SerializeField]
        private Text dayText;

        private GameSessionRuntime session;

        public Text DayText => dayText;

        private void OnEnable()
        {
            if (!global::UnityEngine.Application
                    .isPlaying)
            {
                return;
            }

            session = GameSessionRuntime.Instance;
            session.DayChanged += Refresh;
            Refresh(session.CurrentDate);
        }

        private void OnDisable()
        {
            if (session != null)
            {
                session.DayChanged -= Refresh;
            }

            session = null;
        }

        public void Configure(Text label)
        {
            dayText = label ??
                throw new ArgumentNullException(
                    nameof(label));
        }

        public void Refresh(GameDate date)
        {
            if (dayText == null)
            {
                return;
            }

            dayText.text =
                $"Año {date.Year} · " +
                $"{SeasonName(date.Season)} " +
                $"{date.DayOfSeason}";
        }

        private static string SeasonName(
            Season season)
        {
            return season switch
            {
                Season.Spring => "Primavera",
                Season.Summer => "Verano",
                Season.Autumn => "Otoño",
                Season.Winter => "Invierno",
                _ => season.ToString()
            };
        }
    }
}

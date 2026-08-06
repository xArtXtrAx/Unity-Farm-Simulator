using System;

namespace FarmSimulator.Domain.Time
{
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3
    }

    public readonly struct GameDate : IEquatable<GameDate>
    {
        public GameDate(
            int year,
            Season season,
            int dayOfSeason,
            int totalDaysElapsed)
        {
            Year = year;
            Season = season;
            DayOfSeason = dayOfSeason;
            TotalDaysElapsed = totalDaysElapsed;
        }

        public int Year { get; }

        public Season Season { get; }

        public int DayOfSeason { get; }

        public int TotalDaysElapsed { get; }

        public bool Equals(GameDate other)
        {
            return Year == other.Year &&
                Season == other.Season &&
                DayOfSeason == other.DayOfSeason &&
                TotalDaysElapsed == other.TotalDaysElapsed;
        }

        public override bool Equals(object obj)
        {
            return obj is GameDate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Year,
                (int)Season,
                DayOfSeason,
                TotalDaysElapsed);
        }

        public static bool operator ==(GameDate left, GameDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GameDate left, GameDate right)
        {
            return !left.Equals(right);
        }
    }

    public sealed class GameCalendarState
    {
        public const int DaysPerSeason = 28;
        public const int SeasonsPerYear = 4;

        private int year;
        private Season season;
        private int dayOfSeason;
        private int totalDaysElapsed;

        public GameCalendarState(
            int initialYear = 1,
            Season initialSeason = Season.Spring,
            int initialDayOfSeason = 1)
        {
            ValidateDate(
                initialYear,
                initialSeason,
                initialDayOfSeason);

            year = initialYear;
            season = initialSeason;
            dayOfSeason = initialDayOfSeason;
            totalDaysElapsed =
                ((initialYear - 1) * SeasonsPerYear +
                 (int)initialSeason) * DaysPerSeason +
                initialDayOfSeason - 1;
        }

        public GameDate CurrentDate =>
            new GameDate(
                year,
                season,
                dayOfSeason,
                totalDaysElapsed);

        public GameDate AdvanceDay()
        {
            totalDaysElapsed++;
            dayOfSeason++;

            if (dayOfSeason <= DaysPerSeason)
            {
                return CurrentDate;
            }

            dayOfSeason = 1;
            int nextSeason = (int)season + 1;
            if (nextSeason < SeasonsPerYear)
            {
                season = (Season)nextSeason;
                return CurrentDate;
            }

            season = Season.Spring;
            year++;
            return CurrentDate;
        }

        private static void ValidateDate(
            int candidateYear,
            Season candidateSeason,
            int candidateDay)
        {
            if (candidateYear < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(candidateYear),
                    "Year must be at least one.");
            }

            if (!Enum.IsDefined(
                    typeof(Season),
                    candidateSeason))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(candidateSeason));
            }

            if (candidateDay < 1 ||
                candidateDay > DaysPerSeason)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(candidateDay),
                    $"Day must be between 1 and {DaysPerSeason}.");
            }
        }
    }
}

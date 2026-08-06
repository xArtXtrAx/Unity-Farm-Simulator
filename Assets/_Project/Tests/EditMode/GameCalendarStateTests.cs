using System;
using FarmSimulator.Domain.Time;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class GameCalendarStateTests
    {
        [Test]
        public void StartsAtYearOneSpringOne()
        {
            var calendar = new GameCalendarState();

            Assert.That(calendar.CurrentDate.Year, Is.EqualTo(1));
            Assert.That(
                calendar.CurrentDate.Season,
                Is.EqualTo(Season.Spring));
            Assert.That(
                calendar.CurrentDate.DayOfSeason,
                Is.EqualTo(1));
            Assert.That(
                calendar.CurrentDate.TotalDaysElapsed,
                Is.Zero);
        }

        [Test]
        public void AdvancesOneNormalDay()
        {
            var calendar = new GameCalendarState();

            GameDate date = calendar.AdvanceDay();

            Assert.That(date.Year, Is.EqualTo(1));
            Assert.That(date.Season, Is.EqualTo(Season.Spring));
            Assert.That(date.DayOfSeason, Is.EqualTo(2));
            Assert.That(date.TotalDaysElapsed, Is.EqualTo(1));
        }

        [Test]
        public void SpringTwentyEightBecomesSummerOne()
        {
            var calendar = new GameCalendarState(
                1,
                Season.Spring,
                GameCalendarState.DaysPerSeason);

            GameDate date = calendar.AdvanceDay();

            Assert.That(date.Year, Is.EqualTo(1));
            Assert.That(date.Season, Is.EqualTo(Season.Summer));
            Assert.That(date.DayOfSeason, Is.EqualTo(1));
        }

        [Test]
        public void WinterTwentyEightBeginsNextYear()
        {
            var calendar = new GameCalendarState(
                3,
                Season.Winter,
                GameCalendarState.DaysPerSeason);

            GameDate date = calendar.AdvanceDay();

            Assert.That(date.Year, Is.EqualTo(4));
            Assert.That(date.Season, Is.EqualTo(Season.Spring));
            Assert.That(date.DayOfSeason, Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(29)]
        public void RejectsInvalidDay(int day)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameCalendarState(
                    1,
                    Season.Spring,
                    day));
        }

        [Test]
        public void RejectsInvalidYear()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameCalendarState(
                    0,
                    Season.Spring,
                    1));
        }
    }
}

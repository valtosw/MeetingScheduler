using MeetingScheduler.Application.DTOs;
using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Domain.Models;
using Moq;

namespace MeetingScheduler.Tests
{
    public class MeetingSchedulerServiceTests
    {
        private readonly Mock<IMeetingRepository> _repositoryMock;
        private readonly MeetingSchedulerService _service;


        public MeetingSchedulerServiceTests()
        {
            _repositoryMock = new Mock<IMeetingRepository>();
            _service = new MeetingSchedulerService(_repositoryMock.Object);
        }

        [Fact]
        public void FindEarliestSlot_NoMeetings_ReturnsEarliestStartAdjusted()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll()).Returns([]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 60,
                EarliestStart = new DateTime(2025, 8, 9, 8, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var slot = _service.FindEarliestSlot(dto);

            // Assert
            Assert.NotNull(slot);
            Assert.Equal(new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc), slot.Value.start);
            Assert.Equal(new DateTime(2025, 8, 9, 10, 0, 0, DateTimeKind.Utc), slot.Value.end);
        }

        [Fact]
        public void FindEarliestSlot_WithMeeting_ReturnsAfterMeeting()
        {
            // Arrange
            var busyMeeting = new Meeting
            {
                ParticipantIds = [1],
                StartTime = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 8, 9, 10, 0, 0, DateTimeKind.Utc)
            };

            _repositoryMock.Setup(r => r.GetAll()).Returns([busyMeeting]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 30,
                EarliestStart = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var slot = _service.FindEarliestSlot(dto);

            // Assert
            Assert.NotNull(slot);
            Assert.Equal(new DateTime(2025, 8, 9, 10, 0, 0, DateTimeKind.Utc), slot.Value.start);
            Assert.Equal(new DateTime(2025, 8, 9, 10, 30, 0, DateTimeKind.Utc), slot.Value.end);
        }

        [Fact]
        public void FindEarliestSlot_NoAvailableSlot_ReturnsNull()
        {
            // Arrange
            var busyMeeting = new Meeting
            {
                ParticipantIds = [1],
                StartTime = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            _repositoryMock.Setup(r => r.GetAll()).Returns([busyMeeting]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 60,
                EarliestStart = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var slot = _service.FindEarliestSlot(dto);

            // Assert
            Assert.Null(slot);
        }

        [Fact]
        public void FindEarliestSlot_MultipleDays_ReturnsNextDay()
        {
            // Arrange
            var busyMeeting = new Meeting
            {
                ParticipantIds = [1],
                StartTime = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            _repositoryMock.Setup(r => r.GetAll()).Returns([busyMeeting]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 60,
                EarliestStart = new DateTime(2025, 8, 9, 8, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 10, 17, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var slot = _service.FindEarliestSlot(dto);

            // Assert
            Assert.NotNull(slot);
            Assert.Equal(new DateTime(2025, 8, 10, 9, 0, 0, DateTimeKind.Utc), slot.Value.start);
            Assert.Equal(new DateTime(2025, 8, 10, 10, 0, 0, DateTimeKind.Utc), slot.Value.end);
        }

        [Fact]
        public void FindEarliestSlot_MeetingsWithOverlap_MergesBeforeFindingSlot()
        {
            _repositoryMock.Setup(r => r.GetAll()).Returns(
            [
                new Meeting
                {
                    ParticipantIds = [1],
                    StartTime = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2025, 8, 9, 10, 30, 0, DateTimeKind.Utc)
                },
                new Meeting
                {
                    ParticipantIds = [1],
                    StartTime = new DateTime(2025, 8, 9, 10, 15, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2025, 8, 9, 11, 0, 0, DateTimeKind.Utc)
                }
            ]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 30,
                EarliestStart = new DateTime(2025, 8, 9, 8, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            var result = _service.FindEarliestSlot(dto);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(2025, 8, 9, 11, 0, 0, DateTimeKind.Utc), result.Value.start);
            Assert.Equal(new DateTime(2025, 8, 9, 11, 30, 0, DateTimeKind.Utc), result.Value.end);
        }

        [Fact]
        public void FindEarliestSlot_GapExactlyDuration_SchedulesInGap()
        {
            _repositoryMock.Setup(r => r.GetAll()).Returns(
            [
                new Meeting
                {
                    ParticipantIds = [1],
                    StartTime = new DateTime(2025, 8, 9, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2025, 8, 9, 10, 0, 0, DateTimeKind.Utc)
                },
                new Meeting
                {
                    ParticipantIds = new List<int> { 1 },
                    StartTime = new DateTime(2025, 8, 9, 11, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2025, 8, 9, 12, 0, 0, DateTimeKind.Utc)
                }
            ]);

            var dto = new MeetingSlotDto
            {
                ParticipantIds = [1],
                DurationMinutes = 60,
                EarliestStart = new DateTime(2025, 8, 9, 8, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 8, 9, 17, 0, 0, DateTimeKind.Utc)
            };

            var result = _service.FindEarliestSlot(dto);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(2025, 8, 9, 10, 0, 0, DateTimeKind.Utc), result.Value.start);
            Assert.Equal(new DateTime(2025, 8, 9, 11, 0, 0, DateTimeKind.Utc), result.Value.end);
        }
    }
}

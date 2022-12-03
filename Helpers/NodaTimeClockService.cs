using NodaTime;
using NodaTime.TimeZones;

namespace bank_on_api.Helpers
{
    public interface IClockService
    {
        DateTimeZone TimeZone { get; }

        Instant Now { get; }

        LocalDateTime LocalNow { get; }

        Instant ToInstant(LocalDateTime local);

        LocalDateTime ToLocal(Instant instant);
        Instant? ToInstant(LocalDateTime? local);

        LocalDateTime? ToLocal(Instant? instant);
        long InTicks { get; set; }
    }

    public class ClockService : IClockService
    {
        private readonly IClock _clock;

        public DateTimeZone TimeZone { get; private set; }

        public ClockService()
            : this(SystemClock.Instance)
        {
        }

        public ClockService(IClock clock)
        {
            _clock = clock;

            // NOTE: Get the current users timezone here instead of hard coding it...
            TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Asia/Hong_Kong");
        }

        public long InTicks
        {
            get => _clock.GetCurrentInstant().ToUnixTimeTicks();
            set => throw new System.NotImplementedException();
        }

        public Instant Now
            => _clock.GetCurrentInstant();

        public LocalDateTime LocalNow
            => Now.InZone(TimeZone).LocalDateTime;

        public Instant ToInstant(LocalDateTime local)
            => local.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

        public LocalDateTime ToLocal(Instant instant)
            => instant.InZone(TimeZone).LocalDateTime;

        public Instant? ToInstant(LocalDateTime? local)
            => local?.InZone(TimeZone, Resolvers.LenientResolver).ToInstant();

        public LocalDateTime? ToLocal(Instant? instant)
            => instant?.InZone(TimeZone).LocalDateTime;

    }
}
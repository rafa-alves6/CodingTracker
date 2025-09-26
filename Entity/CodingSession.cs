namespace CodingTracker.Entity
{
    public class CodingSession
    {
        private long Id { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }
        private TimeSpan Duration { get; set; }
    }
}
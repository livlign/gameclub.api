namespace GameClubAPI.Models;

public class Event
{
    public Guid EventId { get; set; }
    public Guid ClubId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTimeUtc { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }

    public Event(Guid clubId, string title, string description, DateTime scheduledDateTimeUtc){
        EventId = Guid.NewGuid();
        ClubId = clubId;
        Title = title;
        Description = description;
        ScheduledDateTimeUtc = scheduledDateTimeUtc;
        CreatedDatetimeUtc = DateTime.UtcNow;
    }
}
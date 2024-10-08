using System.ComponentModel.DataAnnotations;

namespace GameClubAPI.Models;

public class Club
{
    public Guid ClubId { get; set; }
    [Required]
    [MaxLength(256)]
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }

    public Club(string name, string description){
        ClubId = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedDatetimeUtc = DateTime.UtcNow;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameClubAPI.Models;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace GameClubAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly GameClubContext _context;
        private readonly ILogger<ClubsController> _logger;
        const int PageSize = 5;

        public ClubsController(GameClubContext context, ILogger<ClubsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Clubs
        [HttpGet]
        public async Task<ActionResult<GetClubsWithPagingModel>> GetClubs([FromQuery] int page, [FromQuery] string? keyword = null)
        {
            var query = from c in _context.Clubs
                        select c;
            if(!string.IsNullOrEmpty(keyword)){
                query = from q in query
                        where q.Name.ToLower().Contains(keyword.ToLower()) 
                           || q.Description.ToLower().Contains(keyword.ToLower())
                        select q;
            }

            var allClubs = await query.ToListAsync();

            return new GetClubsWithPagingModel
            {
                Total = allClubs.Count,
                PageData = allClubs.OrderByDescending(x => x.CreatedDatetimeUtc)
                            .Skip((page - 1) * PageSize)
                            .Take(PageSize)
                            .ToList()
            };
        }

        // GET: api/Clubs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClub(Guid id)
        {
            var club = await _context.Clubs.FindAsync(id);

            if (club == null)
            {
                return NotFound();
            }

            return club;
        }

        // PUT: api/Clubs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClub(Guid id, Club club)
        {
            if (id != club.ClubId)
            {
                return BadRequest();
            }

            _context.Entry(club).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Clubs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Club>> PostClub(Club club)
        {
            if(string.IsNullOrEmpty(club.Name)){
                return BadRequest();
            }
            if(await _context.Clubs.AnyAsync(x=>x.Name.ToLower() == club.Name.ToLower())){
                return Conflict();
            }

            club.ClubId = Guid.NewGuid();
            club.CreatedDatetimeUtc = DateTime.UtcNow;
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClub", new { id = club.ClubId }, club);
        }

        // DELETE: api/Clubs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(Guid id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null)
            {
                return NotFound();
            }

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Seed")]
        public async Task<ActionResult> SeedClub()
        {
            for (int i = 1; i <= 100; i++)
            {
                var clubName = $"Club Number {i}";
                var clubDescription = $"Description for club number {i}";
                _context.Clubs.Add(new Club(clubName, clubDescription));
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{clubId}/events")]
        public async Task<ActionResult> AddClubEvent([FromRoute] Guid clubId, [FromBody] AddNewEventModel eventModel)
        {
            _logger.LogInformation($"{clubId} and {eventModel}");
            if(string.IsNullOrEmpty(eventModel.Title) 
                || eventModel.ScheduledDateTimeUtc < DateTime.UtcNow
                )
            {
                return BadRequest();
            }
            var newEvent = new Event(clubId, eventModel.Title, eventModel.Description, eventModel.ScheduledDateTimeUtc);
            _context.Events.Add(newEvent);

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEvent", new { id = newEvent.EventId }, newEvent);
        }

        [HttpGet("{clubId}/events")]
        public async Task<ActionResult<GetEventsWithPagingModel>> GetClubEvent([FromRoute] Guid clubId, int page = 1)
        {
            var allEvents = await _context.Events.Where(x=>x.ClubId == clubId).ToListAsync();

            return new GetEventsWithPagingModel
            {
                Total = allEvents.Count,
                PageData = allEvents
                            .OrderByDescending(x => x.CreatedDatetimeUtc)
                            .Skip((page - 1) * PageSize)
                            .Take(PageSize)
                            .ToList()
            };
        }

        [HttpGet("GetEvent/{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var _event = await _context.Events.FindAsync(id);

            if (_event == null)
            {
                return NotFound();
            }

            return _event;
        }

        [HttpPost("{clubId}/seedevents")]
        public async Task<ActionResult> SeedEvent([FromRoute] Guid clubId)
        {
            for (int i = 1; i <= 50; i++)
            {                
                var eventTitle = $"Event Day {i}";
                var eventDescription = $"Description for event day {i}";
                var eventDateUtc = DateTime.UtcNow.AddDays(i);
                _context.Events.Add(new Event(clubId, eventTitle, eventDescription, eventDateUtc));
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClubExists(Guid id)
        {
            return _context.Clubs.Any(e => e.ClubId == id);
        }

        public class GetClubsWithPagingModel()
        {
            public int Total { get; set; }
            public IEnumerable<Club> PageData { get; set; }
        }

        public class AddNewEventModel()
        {
            [Required]
            [MaxLength(256)]
            public string Title { get; set; }
            [MaxLength(512)]
            public string Description { get; set; }
            public DateTime ScheduledDateTimeUtc { get; set; }
        }

        public class GetEventsWithPagingModel()
        {
            public int Total { get; set; }
            public IEnumerable<Event> PageData { get; set; }
        }
    }
}

using HomeExchange.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HomeExchange.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;

        public AdministratorController (DatabaseContext databaseContext){
            _databaseContext = databaseContext;
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _databaseContext.Users
                .Where(u => !u.IsApproved)
                .Select(u => new {
                    u.Id,
                    u.Roles,
                    u.FirstName,
                    u.LastName,
                    u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _databaseContext.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsApproved = true;
            await _databaseContext.SaveChangesAsync();

            return Ok(new { Message = "Korisnik odobren." });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("pending-ads")]
        public async Task<IActionResult> GetPendingAdvertisements()
        {
            var ads = await _databaseContext.Advertisements
                .Where(a => !a.IsApproved)
                .Select(a => new {
                    a.Id,
                    a.UrlPhotos,
                    a.Title,
                    a.Description,
                    a.Date,
                    a.Address,
                    a.City,
                    a.Country,
                    a.NumberOfRooms,
                    a.HomeArea,
                    a.Garden,
                    a.ParkingSpace,
                    a.SwimmingPool,
                    a.Availability,

                })
                .ToListAsync();

            return Ok(ads);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("approve-ad/{id}")]
        public async Task<IActionResult> ApproveAdvertisement(int id)
        {
            var ad = await _databaseContext.Advertisements.FindAsync(id);
            if (ad == null) return NotFound();

            ad.IsApproved = true;
            await _databaseContext.SaveChangesAsync();

            return Ok(new { Message = "Oglas odobren." });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("all-ads")]
        public async Task<IActionResult> GetAllAdvertisementsForAdmin()
        {
            var ads = await _databaseContext.Advertisements
                .Select(a => new {
                    a.Id,
                    a.UrlPhotos,
                    a.Title,
                    a.Description,
                    a.Date,
                    a.Address,
                    a.City,
                    a.Country,
                    a.NumberOfRooms,
                    a.HomeArea,
                    a.Garden,
                    a.ParkingSpace,
                    a.SwimmingPool,
                    a.Availability,
                    Status = a.IsApproved ? "Approved" : "Pending"   
                })
                .ToListAsync();

            return Ok(ads);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("advertisements/{id}")]
        public async Task<IActionResult> DeleteAdvertisementAsAdmin(int id)
        {
            var ad = await _databaseContext.Advertisements.FindAsync(id);
            if (ad == null) return NotFound();

            _databaseContext.Advertisements.Remove(ad);
            await _databaseContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("advertisements/{advertisementId}/availability")]
        public async Task<IActionResult> UpdateAdvertisementAvailability(int advertisementId, [FromBody] UpdateAvailabilityDTO dto)
        {
            var advertisement = await _databaseContext.Advertisements.FindAsync(advertisementId);
            if (advertisement == null) return NotFound();

            advertisement.Availability = dto.Availability;
            await _databaseContext.SaveChangesAsync();

            return Ok(new { Message = "Advertisement availability updated." });
        }

    }

    public class UpdateAvailabilityDTO
    {
        public string Availability { get; set; }
    }
}
    
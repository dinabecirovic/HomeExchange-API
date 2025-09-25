using HomeExchange.DTOs;
using HomeExchange.Interfaces;
using HomeExchange.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeExchange.Data;
using Microsoft.EntityFrameworkCore;


namespace HomeExchange.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class HomeOwnerController : ControllerBase
    {
        private readonly IHomeOwnerService _homeOwnerService;
        private readonly DatabaseContext _databaseContext;

        public HomeOwnerController(IHomeOwnerService homeOwnerService, DatabaseContext databaseContext)
        {
            _homeOwnerService = homeOwnerService;
            _databaseContext = databaseContext;
        }

        [HttpGet("Advertisements")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var ads = await _homeOwnerService.GetAllAdvertisements();
            return Ok(ads);
        }

        [HttpGet("Advertisements/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdvertisementById(int id)
        {
            var ad = await _homeOwnerService.GetAdvertisementById(id);
            if (ad == null) return NotFound();

            return Ok(ad);
        }
        [HttpGet("MyAdvertisements")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMyAdvertisements()
        {
            var myAds = await _homeOwnerService.GetMyAdvertisements(User);
            return Ok(myAds);
        }


        [HttpPost("CreateAdvertisement")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> CreateAdvertisement([FromForm] AdvertisementRequestDTO request)
        {
            var ad = await _homeOwnerService.CreateAdvertisement(request, User);
            return Ok(ad);
        }

        [HttpDelete("DeleteAdvertisement/{id}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            await _homeOwnerService.DeleteAdvertisement(id);
            return NoContent();
        }

        [HttpPut("UpdateAdvertisement/{id}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> UpdateAdvertisement(int id, [FromBody] AdvertisementUpdateDTO request)
        {
            var updatedAd = await _homeOwnerService.UpdateAdvertisement(id, request);
            return Ok(updatedAd);
        }
        [Authorize(Roles = "HomeOwner")]
        [HttpPost("Reservations")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDTO request)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0"); var ad = await _databaseContext.Advertisements.FindAsync(request.AdvertisementId); if (ad == null) return NotFound("Oglas nije pronađen."); if (ad.HomeOwnerId == userId) return BadRequest("Ne možete rezervisati svoj dom."); var reservation = new Reservation { AdvertisementId = request.AdvertisementId, UserId = userId, StartDate = request.StartDate, EndDate = request.EndDate }; await _databaseContext.Reservations.AddAsync(reservation); await _databaseContext.SaveChangesAsync(); var ownerReservation = await _databaseContext.Reservations.Include(r => r.Advertisement).Where(r => r.UserId == ad.HomeOwnerId && r.Advertisement.HomeOwnerId == userId).Where(r => r.StartDate <= request.EndDate && r.EndDate >= request.StartDate).FirstOrDefaultAsync(); if (ownerReservation != null) { reservation.IsExchangeConfirmed = true; ownerReservation.IsExchangeConfirmed = true; await _databaseContext.SaveChangesAsync(); }
            return Ok(reservation);
        }

        [Authorize(Roles = "HomeOwner")]
        [HttpGet("ReservationsForOwner")]
        public async Task<IActionResult> GetReservationsForOwner([FromQuery] int adId)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var reservations = await _databaseContext.Reservations
                .Where(r => r.AdvertisementId == adId && r.Advertisement.HomeOwnerId == userId)
                .Include(r => r.User)
                .Select(r => new {
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.IsExchangeConfirmed,
                    UserFirstName = r.User.FirstName,
                    UserLastName = r.User.LastName,
                    UserEmail = r.User.Email
                })
                .ToListAsync();

            return Ok(reservations);
        }


        [HttpPost("Ratings")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> CreateRating([FromBody] RatingRequestDTO request)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var reservation = await _databaseContext.Reservations
                .Where(r => r.AdvertisementId == request.AdvertisementId && r.UserId == userId)
                .OrderByDescending(r => r.EndDate)
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return BadRequest("Ne možete ostaviti recenziju jer nemate rezervaciju za ovaj oglas.");
            }

            if (reservation.EndDate > DateTime.UtcNow)
            {
                return BadRequest("Možete ostaviti recenziju tek nakon što istekne period rezervacije.");
            }

            var rating = await _homeOwnerService.CreateRating(request, User);
            return Ok(rating);
        }

        [HttpGet("Ratings/{advertisementId}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetRatingsForAdvertisement(int advertisementId)
        {
            var ratings = await _homeOwnerService.GetRatingsForAdvertisement(advertisementId);
            return Ok(ratings);
        }

        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Advertisement>>> SearchAdvertisements([FromBody] AdvertisementSearchDTO search)
        {
            var query = _databaseContext.Advertisements.AsQueryable();

            if (!string.IsNullOrEmpty(search.City))
                query = query.Where(a => a.City.ToLower() == search.City.ToLower());

            if (!string.IsNullOrEmpty(search.Country))
                query = query.Where(a => a.Country.ToLower() == search.Country.ToLower());

            if (search.MinRooms.HasValue)
                query = query.Where(a => a.NumberOfRooms >= search.MinRooms.Value);

            if (search.MinArea.HasValue)
                query = query.Where(a => a.HomeArea >= search.MinArea.Value);

            return await query
                .Where(a => a.IsApproved) // opcionalno
                .ToListAsync();
        }

    }
}


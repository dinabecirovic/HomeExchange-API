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
            var rating = await _homeOwnerService.CreateRating(request, User);
            return Ok(rating);
        }

        [HttpPost("Search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAdvertisements([FromBody] AdvertisementSearchDTO criteria)
        {
            var results = await _homeOwnerService.SearchAdvertisements(criteria);
            return Ok(results);
        }
    }
}

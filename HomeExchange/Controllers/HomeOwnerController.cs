using HomeExchange.DTOs;
using HomeExchange.Interfaces;
using HomeExchange.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeExchange.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class HomeOwnerController : ControllerBase
    {
        private readonly IHomeOwnerService _homeOwnerService;

        public HomeOwnerController(IHomeOwnerService homeOwnerService)
        {
            _homeOwnerService = homeOwnerService;
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

        [HttpPost("Reservations")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDTO request)
        {
            var reservation = await _homeOwnerService.CreateReservation(request, User);
            return Ok(reservation);
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

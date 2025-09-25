using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HomeExchange.Data;
using HomeExchange.Data.Models;
using HomeExchange.DTOs;
using HomeExchange.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;

namespace HomeExchange.Services
{
    public class HomeOwnerService : IHomeOwnerService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly Cloudinary _cloudinary;

        public HomeOwnerService(DatabaseContext databaseContext, Cloudinary cloudinary)
        {
            _databaseContext = databaseContext;
            _cloudinary = cloudinary;
        }


        public async Task<List<AdvertisementResponseDTO>> GetAllAdvertisements()
        {
            var advertisements = await _databaseContext.Advertisements
                .Where(a => a.IsApproved)
                .OrderByDescending(a => a.Date)
                .Select(a => new AdvertisementResponseDTO
                {
                    Id = a.Id,
                    UrlPhotos = a.UrlPhotos,
                    Title = a.Title,
                    Description = a.Description,
                    Date = a.Date,
                    Address = a.Address,
                    City = a.City,
                    Country = a.Country,
                    NumberOfRooms = a.NumberOfRooms,
                    HomeArea = a.HomeArea,
                    Garden = a.Garden,
                    ParkingSpace = a.ParkingSpace,
                    SwimmingPool = a.SwimmingPool,
                    HomeOwnerId = a.HomeOwnerId
                })
                .ToListAsync();

            return advertisements;
        }

        public async Task<List<AdvertisementResponseDTO>> GetMyAdvertisements(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst("id")?.Value ?? "0");

            var myAds = await _databaseContext.Advertisements
                .Where(a => a.HomeOwnerId == userId)
                .Select(a => new AdvertisementResponseDTO
                {
                    Id = a.Id,
                    UrlPhotos = a.UrlPhotos,
                    Title = a.Title,
                    Description = a.Description,
                    Date = a.Date,
                    Address = a.Address,
                    City = a.City,
                    Country = a.Country,
                    NumberOfRooms = a.NumberOfRooms,
                    HomeArea = a.HomeArea,
                    Garden = a.Garden,
                    ParkingSpace = a.ParkingSpace,
                    SwimmingPool = a.SwimmingPool,
                    HomeOwnerId = a.HomeOwnerId
                })
                .ToListAsync();

            return myAds;
        }

        public async Task<AdvertisementResponseDTO?> GetAdvertisementById(int id)
        {
            var ad = await _databaseContext.Advertisements
                .Where(a => a.Id == id && a.IsApproved)
                .FirstOrDefaultAsync();

            if (ad == null) return null;

            return new AdvertisementResponseDTO
            {
                Id = ad.Id,
                UrlPhotos = ad.UrlPhotos,
                Title = ad.Title,
                Description = ad.Description,
                Date = ad.Date,
                Address = ad.Address,
                City = ad.City,
                Country = ad.Country,
                NumberOfRooms = ad.NumberOfRooms,
                HomeArea = ad.HomeArea,
                Garden = ad.Garden,
                ParkingSpace = ad.ParkingSpace,
                SwimmingPool = ad.SwimmingPool,
            };
        }

        private async Task<string> UploadImageToCloudinary(IFormFile file)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UploadPreset = "ml_default" //preset
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.AbsoluteUri; // vracam sigurni URL slike
            }

            return null;
        }
        public async Task<AdvertisementResponseDTO> CreateAdvertisement(AdvertisementRequestDTO request, ClaimsPrincipal user)
        {
            var uploadedUrls = new List<string>();

            // obradjujem svaku sliku i uploadujem je na Cloudinary
            foreach (var file in request.Photos)
            {
                var uploadResult = await UploadImageToCloudinary(file);
                if (uploadResult != null)
                {
                    uploadedUrls.Add(uploadResult);
                }
            }

            var userId = int.Parse(user.FindFirst("id")?.Value ?? "0");

            var advertisement = new Advertisement
            {
                UrlPhotos = uploadedUrls,
                Title = request.Title,
                Description = request.Description,
                Date = DateTime.Now,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                NumberOfRooms = request.NumberOfRooms,
                HomeArea = request.HomeArea,
                Garden = request.Garden,
                ParkingSpace = request.ParkingSpace,
                SwimmingPool = request.SwimmingPool,
                Availability = "Available",
                IsApproved = false,
                HomeOwnerId = userId
            };

            await _databaseContext.Advertisements.AddAsync(advertisement);
            await _databaseContext.SaveChangesAsync();

            return new AdvertisementResponseDTO
            {
                Id = advertisement.Id,
                UrlPhotos = uploadedUrls,
                Title = request.Title,
                Description = request.Description,
                Date = DateTime.Now,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                NumberOfRooms = request.NumberOfRooms,
                HomeArea = request.HomeArea,
                Garden = request.Garden,
                ParkingSpace = request.ParkingSpace,
                SwimmingPool = request.SwimmingPool,
            };
        }

        public async Task DeleteAdvertisement(int advertisementId)
        {
            var advertisement = await _databaseContext.Advertisements
                .FirstOrDefaultAsync(a => a.Id == advertisementId);

            if (advertisement == null)
            {
                throw new System.Collections.Generic.KeyNotFoundException("Advertisement not found.");
            }
            _databaseContext.Advertisements.Remove(advertisement);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<AdvertisementResponseDTO> UpdateAdvertisement(int advertisementId, AdvertisementUpdateDTO request)
        {
            var advertisement = await _databaseContext.Advertisements
                .FirstOrDefaultAsync(a => a.Id == advertisementId);

            if (advertisement == null)
                throw new KeyNotFoundException("Advertisement not found.");

            advertisement.Description = request.Description;
            advertisement.Garden = request.Garden;
            advertisement.SwimmingPool = request.SwimmingPool;
            advertisement.ParkingSpace = request.ParkingSpace;

            await _databaseContext.SaveChangesAsync();

            return new AdvertisementResponseDTO
            {
                Id = advertisement.Id,
                UrlPhotos = advertisement.UrlPhotos,
                Title = advertisement.Title,
                Description = advertisement.Description,
                Date = advertisement.Date,
                Address = advertisement.Address,
                City = advertisement.City,
                Country = advertisement.Country,
                NumberOfRooms = advertisement.NumberOfRooms,
                HomeArea = advertisement.HomeArea,
                Garden = advertisement.Garden,
                ParkingSpace = advertisement.ParkingSpace,
                SwimmingPool = advertisement.SwimmingPool
            };
        }
        public async Task<Reservation> CreateReservation(ReservationRequestDTO request, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst("id")?.Value ?? "0");

            var reservation = new Reservation
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserId = userId,
                AdvertisementId = request.AdvertisementId
            };

            await _databaseContext.Reservations.AddAsync(reservation);
            await _databaseContext.SaveChangesAsync();

            return reservation;
        }
        public async Task<Rating> CreateRating(RatingRequestDTO request, ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst("id")?.Value ?? "0");

            var hasEndedReservation = await _databaseContext.Reservations
                .AnyAsync(r => r.AdvertisementId == request.AdvertisementId
                               && r.UserId == userId
                               && r.EndDate <= DateTime.Now); 

            if (!hasEndedReservation)
            {
                throw new InvalidOperationException("Možete ostaviti recenziju tek nakon što istekne period rezervacije.");
            }

            var rating = new Rating
            {
                Score = request.Score,
                Comment = request.Comment,
                UserId = userId,
                AdvertisementId = request.AdvertisementId,
            };

            await _databaseContext.Ratings.AddAsync(rating);
            await _databaseContext.SaveChangesAsync();
            return rating;
        }


        public async Task<List<RatingResponseDTO>> GetRatingsForAdvertisement(int advertisementId)
        {
            return await _databaseContext.Ratings
                .Where(r => r.AdvertisementId == advertisementId)
                .Include(r => r.User)
                .OrderByDescending(r => r.Id)
                .Select(r => new RatingResponseDTO
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    AdvertisementId = r.AdvertisementId,
                    UserId = r.UserId,
                    UserFirstName = r.User.FirstName,
                    UserLastName = r.User.LastName
                })
                .ToListAsync();
        }

    }
}
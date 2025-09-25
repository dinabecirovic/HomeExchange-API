using HomeExchange.Data.Models;
using HomeExchange.DTOs;
using System.Security.Claims;

public interface IHomeOwnerService
{
    Task<List<AdvertisementResponseDTO>> GetAllAdvertisements();
    Task<AdvertisementResponseDTO> CreateAdvertisement(AdvertisementRequestDTO request, ClaimsPrincipal user);
    Task DeleteAdvertisement(int advertisementId);
    Task<AdvertisementResponseDTO> UpdateAdvertisement(int advertisementId, AdvertisementUpdateDTO request);
    Task<Reservation> CreateReservation(ReservationRequestDTO request, ClaimsPrincipal user);
    Task<Rating> CreateRating(RatingRequestDTO request, ClaimsPrincipal user);
    Task<List<RatingResponseDTO>> GetRatingsForAdvertisement(int advertisementId);

    Task<AdvertisementResponseDTO?> GetAdvertisementById(int id);
    Task<List<AdvertisementResponseDTO>> GetMyAdvertisements(ClaimsPrincipal user);
}

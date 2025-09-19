using Microsoft.AspNetCore.Mvc;
using HomeExchange.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using HomeExchange.DTOs;
using HomeExchange.Data.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using HomeExchange.Data.Models;

namespace HomeExchange.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IMapper mapper, IUserService userService)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("valid-roles")]
        public IActionResult GetValidRoles()
        {
            var validRoles = new List<string> { "HomeOwner", "Administrator" };
            return Ok(validRoles);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequestDTO request)
        {
            var emailExist = await _userService.GetUserByEmail(request.Email);
            if (emailExist is not null)
            {
                return BadRequest(new { Message = "Email već postoji." });
            }

            var validRoles = new List<string> { "Administrator", "HomeOwner" };

            if (!validRoles.Contains(request.Roles))
            {
                return BadRequest(new { Message = "Nepoznata uloga!" });
            }

            var user = new Users
            {
                Roles = request.Roles,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = _userService.HashPassword(request.Password),
                IsApproved = false,
            };

            await _userService.RegisterUser(user);

            return Ok(new { Message = "Registracija uspešna! Čekajte odobrenje administratora." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserRequestDTO request)
        {
            var userExist = await _userService.GetUserByEmail(request.Email);


            if (userExist is null)
            {
                return NotFound(new ErrorResponseDTO
                {
                    Message = "Korisnik nije registrovan."
                });
            }

            if (userExist.Password != _userService.HashPassword(request.Password))
            {
                return BadRequest(new ErrorResponseDTO
                {
                    Message = "Pogrešna lozinka."
                });
            }

            if (!userExist.IsApproved)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponseDTO
                {
                    Message = "Nalog još nije odobren od strane administratora."
                });
            }


            var token = _userService.GenerateToken(userExist);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Users = _mapper.Map<UserResponseDTO>(userExist),
                Role = userExist.Roles,

            });
        }
    }
}
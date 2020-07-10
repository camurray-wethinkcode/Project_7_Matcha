using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Matcha.API.Data;
using Matcha.API.Dtos;
using Matcha.API.Helpers;
using Matcha.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Matcha.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IToken _token;
        private readonly IMailer _mailer;
        private readonly IDatingRepository _datingRepo;

        public AuthController(
            IAuthRepository repo,
            IConfiguration config,
            IMapper mapper,
            IToken token,
            IMailer mailer,
            IDatingRepository datingRepo)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
            _token = token;
            _mailer = mailer;
            _datingRepo = datingRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            userToCreate.Token = _token.GenerateToken(128);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            try
            {
                var verifyLink = string.Format("{0}://{1}{2}/verify?token={3}",
                    Request.Scheme,
                    Request.Host,
                    Request.Path.Value.Remove(Request.Path.Value.LastIndexOf('/')),
                    HttpUtility.UrlEncode(createdUser.Token));

                await _mailer.SendVerificationMail(
                    new MailUser
                    {
                        Email = createdUser.Email,
                        Name = createdUser.Name
                    },
                    verifyLink
                );
            }
            catch (Exception)
            {
                return BadRequest("User created successfully, but verify email failed to send");
            }

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtRoute("GetUser", new { controller = "Users", 
                id = createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username
                .ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    
        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _datingRepo.GetUserByVerifyToken(token);

            if (user == null)
                return Unauthorized("Token not found!");

            user.Token = null;
            user.Activated = 1;

            await _datingRepo.SaveAll();

            return Ok("User Successfully Verified");
        }

        [HttpPost("sendreset")]
        public async Task<IActionResult> SendReset(string email)
        {
            var user = await _datingRepo.GetUserByEmail(email);

            if (user == null)
                return NotFound("Email Address not registered!");

            user.Reset = _token.GenerateToken(128);

            try
            {
                var resetLink = string.Format("{0}://{1}{2}/reset?token={3}",
                    Request.Scheme,
                    Request.Host,
                    Request.Path.Value.Remove(Request.Path.Value.LastIndexOf('/')),
                    HttpUtility.UrlEncode(user.Reset));

                await _mailer.SendPasswordResetMail(
                    new MailUser
                    {
                        Email = user.Email,
                        Name = user.Name
                    },
                    resetLink
                );

                return Ok("Reset Email sent Successfully");
            }
            catch (Exception)
            {
                return BadRequest("Error sending reset link email");
            }
        }

        public async Task<IActionResult> CheckReset(string token)
        {
            var user = await _datingRepo.GetUserByResetToken(token);

            if (user == null)
                return NotFound("Reset Token Not Found");

            return Ok("Reset Token Found");
        }

        public async Task<IActionResult> DoReset(string token, string newPassword)
        {
            var user = await _datingRepo.GetUserByResetToken(token);

            if (user == null)
                return NotFound("Reset Token Not Found");

            await _repo.ResetPassword(user, newPassword);

            return Ok("Password Successfully Reset");
        }
    }
}
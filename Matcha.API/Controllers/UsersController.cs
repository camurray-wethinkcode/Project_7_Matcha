using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Matcha.API.Data;
using Matcha.API.Dtos;
using Matcha.API.Helpers;
using Matcha.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matcha.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IMailer _mailer;
        public UsersController(IDatingRepository repo, IMapper mapper, IMailer mailer)
        {
            _mapper = mapper;
            _repo = repo;
            _mailer = mailer;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                var gender = "test";
                if (userFromRepo.Gender == "male")
                    gender = "male";
                else if (userFromRepo.Gender == "female")
                    gender = "female";
                else if (userFromRepo.Gender == "other")
                    gender = "other";
                userParams.Gender = gender;
                //userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
                 users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.Update(userFromRepo))
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if (like != null)
                return BadRequest("You already like this user");

            if (await _repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            if (await _repo.Add(like))
            {
                var likedUser = await _repo.GetUser(recipientId);
                var likedByUser = await _repo.GetUser(id);
                try
                {
                    await _mailer.SendLikeMail(
                        new MailUser
                        {
                            Email = likedUser.Email,
                            Name = likedUser.Name
                        },
                        likedByUser.Name
                    );
                }
                catch (Exception)
                {
                    return BadRequest("Successfully liked user, Unable to send email");
                }

                return Ok();
            }

            return BadRequest("Failed to like user");
        }

        [HttpPost("{id}/unlike/{recipientId}")]
        public async Task<IActionResult> UnlikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if (like == null)
                return BadRequest("You haven't liked this user");

            if (await _repo.Delete(like))
            {
                var unlikedUser = await _repo.GetUser(recipientId);
                var unlikedByUser = await _repo.GetUser(id);
                try
                {
                    await _mailer.SendUnlikeMail(
                        new MailUser
                        {
                            Email = unlikedUser.Email,
                            Name = unlikedUser.Name
                        },
                        unlikedByUser.Name
                    );
                }
                catch (Exception)
                {
                    return BadRequest("Successfully unliked user, Unable to send email");
                }

                return Ok();
            }

            return BadRequest("Failed to unlike user");
        }
    }
}
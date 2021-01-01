using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesReqository _likesReqository;
        public LikesController(IUserRepository userRepository, ILikesReqository likesReqository)
        {
            _likesReqository = likesReqository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            var sourceuser = await _likesReqository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();

            if (sourceuser.UserName.Equals(username))
                return BadRequest("You cannot like yourself");

            var userlike = await _likesReqository.GetUserLike(sourceUserId, likedUser.Id);
            if (userlike != null)
                return BadRequest("You already like this user");

            userlike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            // Änderung zum Kurs. LikedByUser-List war null, weil sie leer war.
            _likesReqository.AddLikeEntity(userlike);
            // sourceuser.LikedByUsers.Add(userlike);

            if (await _userRepository.SaveAllAsync())
                return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesReqository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}
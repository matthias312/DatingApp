using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesReqository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
        
        /// <summary>
        /// Methode wird zum Bugfixen eines Fehler des Kurses verwendet.
        /// </summary>
        /// <param name="like"></param>
        void AddLikeEntity(UserLike like);
    }
}
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;

namespace RuoYi.System.Repositories
{
    /// <summary>
    ///  用户与岗位关联表 Repository
    ///  author ruoyi.net
    ///  date   2023-08-23 09:43:50
    /// </summary>
    public class SysUserPostRepository : BaseRepository<SysUserPost, SysUserPostDto>
    {
        public SysUserPostRepository(ISqlSugarRepository<SysUserPost> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<SysUserPost> Queryable(SysUserPostDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.UserId > 0, (t) => t.UserId == dto.UserId)
                .WhereIF(dto.PostId > 0, (t) => t.PostId == dto.PostId)
            ;
        }

        public override ISugarQueryable<SysUserPostDto> DtoQueryable(SysUserPostDto dto)
        {
            return Repo.AsQueryable()
                .WhereIF(dto.UserId > 0, (t) => t.UserId == dto.UserId)
                .WhereIF(dto.PostId > 0, (t) => t.PostId == dto.PostId)
                .Select((t) => new SysUserPostDto
                {
                    UserId = t.UserId,
                    PostId = t.PostId
                });
        }

        public async Task<int> DeleteUserPostByUserId(long userId)
        {
            return await Repo.DeleteAsync(up => up.UserId == userId);
        }

        public int DeleteUserPost(List<long> userIds)
        {
            return Repo.Delete(up => userIds.Contains(up.UserId));
        }

        /// <summary>
        /// 通过岗位ID查询岗位使用数量
        /// </summary>
        /// <param name="postId">岗位ID</param>
        public async Task<int> CountUserPostByIdAsync(long postId)
        {
            return await Repo.CountAsync(up => up.PostId == postId);
        }


        public async Task<List<SysUserPostDto>> GetDtoListWithNewInstanceAsync(SysUserPostDto dto)
        {
            return await Repo.GetNewRepository().AsQueryable()
               .WhereIF(dto.UserId > 0, (t) => t.UserId == dto.UserId)
               .WhereIF(dto.PostId > 0, (t) => t.PostId == dto.PostId)
               .Select((t) => new SysUserPostDto
               {
                   UserId = t.UserId,
                   PostId = t.PostId
               }).ToListAsync();
        }
    }
}
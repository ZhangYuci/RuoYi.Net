using RuoYi.Quartz.Enums;

namespace RuoYi.Quartz.Repositories;

/// <summary>
///  定时任务调度日志表 Repository
///  author ruoyi.net
///  date   2023-10-12 17:38:31
/// </summary>
public class SysJobLogRepository : BaseRepository<SysJobLog, SysJobLogDto>
{
    public SysJobLogRepository(ISqlSugarRepository<SysJobLog> sqlSugarRepository)
    {
        Repo = sqlSugarRepository;
    }

    public override ISugarQueryable<SysJobLog> Queryable(SysJobLogDto dto)
    {
        return Repo.AsQueryable()
            .WhereIF(dto.JobLogId > 0, (t) => t.JobLogId == dto.JobLogId)
        ;
    }

    public override ISugarQueryable<SysJobLogDto> DtoQueryable(SysJobLogDto dto)
    {
        return Repo.AsQueryable()
            .WhereIF(dto.JobLogId > 0, (t) => t.JobLogId == dto.JobLogId)
            .Select((t) => new SysJobLogDto
            {
                JobLogId = t.JobLogId
            }, true);
    }

    public void TruncateTable()
    {
        Repo.Context.DbMaintenance.TruncateTable<SysJobLog>();
    }

    protected override async Task FillRelatedDataAsync(IEnumerable<SysJobLogDto> dtos)
    {
        await base.FillRelatedDataAsync(dtos);

        foreach (var dto in dtos)
        {
            dto.StatusDesc = !string.IsNullOrEmpty(dto.Status) ? Enum.Parse<JobResultStatus>(dto.Status!).ToDesc() : null;
            dto.ExecuteTime = dto.CreateTime.HasValue ? dto.CreateTime.Value.ToString("yyyy/MM/dd HH:mm:ss") : null;
        }
    }
}
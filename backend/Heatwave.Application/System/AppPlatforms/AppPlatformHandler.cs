using System.Linq.Expressions;
using AutoMapper;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.AppPlatforms;

/// <summary>
/// 查询平台列表
/// </summary>
public class AppPlatformPageListQuery : PaginatedInputBase, IQuery<PaginatedList<AppPlatformDigest>> {
    public string Name { get; set; }
}

/// <summary>
/// 更新或者编辑平台
/// </summary>
public class AppPlatformRequestCommand : AppPlatform, ICommand { }

/// <summary>
/// 获取指定的平台信息
/// </summary>
/// <param name="Id"></param>
public record AppPlatformDataQuery(long Id) : IQuery<AppPlatformDisplay>;

/// <summary>
/// 删除指定的平台
/// </summary>
/// <param name="Id">指定的平台</param>
public record AppPlatformDeleteCommand(long Id) : ICommand;

public class AppPlatformHandler (IDbAccessor dbAccessor, IMapper mapper): 
    IQueryHandler<AppPlatformPageListQuery,PaginatedList<AppPlatformDigest>>,
    IQueryHandler<AppPlatformDataQuery, AppPlatformDisplay>,
    ICommand<AppPlatformRequestCommand>,
    ICommand<AppPlatformDeleteCommand>
{
    public async Task<PaginatedList<AppPlatformDigest>> Handle(AppPlatformPageListQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<AppPlatform, AppPlatformDigest>> selectExpr = a => new AppPlatformDigest { };
        selectExpr = selectExpr.BuildExtendSelectExpre();

        return await dbAccessor.GetIQueryable<AppPlatform>()
            .WhereIf(request.Name.IsNotNullOrAny(), v=> v.Name.Contains(request.Name))
            .Select(selectExpr)
            .ToPageAsync(request);
    }

    public async Task<AppPlatformDisplay> Handle(AppPlatformDataQuery request, CancellationToken cancellationToken)
    {
        var data = await dbAccessor.GetIQueryable<AppPlatform>()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken: cancellationToken);
        if (data is null)
            throw new BusException("未找到相关数据", 404);
        return mapper.Map<AppPlatformDisplay>(data);
    }

    public async Task Handle(AppPlatformRequestCommand requestCommand, CancellationToken cancellationToken)
    {
        if (requestCommand.Id > 0)
        {
            // 更新
            requestCommand.Id = IdHelper.GetLong();
            await dbAccessor.InsertAsync<AppPlatform>(requestCommand, cancellation: cancellationToken);
        }
        else
        {
            // 编辑
            await dbAccessor.UpdateAsync<AppPlatform>(requestCommand,[nameof(AppPlatform.Name),nameof(AppPlatform.Key),nameof(AppPlatform.Secret)], cancellation: cancellationToken);
        }
        
        await dbAccessor.DeleteAsync<AppPlatform>(t => t.Id == requestCommand.Id, cancellationToken);
    }
    
    public async Task Handle(AppPlatformDeleteCommand requestCommand, CancellationToken cancellationToken)
    {
        await dbAccessor.DeleteAsync<AppPlatform>(t => t.Id == requestCommand.Id, cancellationToken);
    }
}



public class AppPlatformDigest : AppPlatform, IMapFrom<AppPlatform> { }

public class AppPlatformDisplay: AppPlatform, IMapFrom<AppPlatform> {}
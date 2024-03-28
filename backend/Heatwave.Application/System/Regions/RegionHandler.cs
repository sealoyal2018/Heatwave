using Heatwave.Domain.System;

namespace Heatwave.Application.System.Regions;


public class RegionHandler(IDbAccessor dbAccessor) : 
    IQueryHandler<RegionTreeQuery, ICollection<RegionNode>>,
    IQueryHandler<RegionListQuery, ICollection<Region>>,
    IQueryHandler<RegionDataQuery, Region>,
    ICommandHandler<RegionDeleteQuery>,
    ICommandHandler<RegionEditCommand>
{
    /// <summary>
    /// 获取树型区域
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ICollection<RegionNode>> Handle(RegionTreeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取区域列表
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ICollection<Region>> Handle(RegionListQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取指定区域
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Region> Handle(RegionDataQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 删除指定区域
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Handle(RegionDeleteQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 新增或者更新区域
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Handle(RegionEditCommand request, CancellationToken cancellationToken)
    {
        if (request.Id < 1)
        {
            await dbAccessor.InsertAsync(request, cancellation: cancellationToken);
            return;
        }

        await dbAccessor.UpdateAsync(request, [nameof(Region.ParentId), nameof(Region.Title), nameof(Region.Rank)], cancellation: cancellationToken);
    }
}

/// <summary>
/// 获取树形区域
/// </summary>
public class RegionTreeQuery : IQuery<ICollection<RegionNode>>
{
    /// <summary>
    /// 上级
    /// </summary>
    public long? ParentId { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// 获取列表型区域
/// </summary>
public class RegionListQuery : IQuery<ICollection<Region>>
{
    /// <summary>
    /// 上级
    /// </summary>
    public long? ParentId { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 当前
    /// </summary>
    public List<long> Ids { get; set; }
}

/// <summary>
/// 获取某个区域
/// </summary>
/// <param name="Id"></param>
public record RegionDataQuery(long Id) : IQuery<Region>;

/// <summary>
/// 删除指定区域
/// </summary>
/// <param name="Id"></param>
public record RegionDeleteQuery(long Id) : ICommand;

/// <summary>
/// 编辑或者新增区域
/// </summary>
public class RegionEditCommand : Region, ICommand;

public class RegionNode: Region, INode<RegionNode>
{
    public ICollection<RegionNode> Children { get; set; }
}
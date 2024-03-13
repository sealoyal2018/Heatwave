using AutoMapper;

using Chocolate.Application.Extensions;
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Resources;
public record ResourceTreeQuery: IQuery<ICollection<ResourceNode>>;

public class ResourceTreeQueryHandler : IQueryHandler<ResourceTreeQuery, ICollection<ResourceNode>>
{
    private readonly IFreeSql freeSql;
    private readonly IMapper _mapper;

    public ResourceTreeQueryHandler(IFreeSql freeSql, IMapper mapper)
    {
        this.freeSql = freeSql;
        _mapper = mapper;
    }

    public async Task<ICollection<ResourceNode>> Handle(ResourceTreeQuery request, CancellationToken cancellationToken)
    {
        var resources = await freeSql.GetRepository<Resource>()
            .Select.ToListAsync();
        var resourceNodes = _mapper.Map<List<Resource>, List<ResourceNode>>(resources);
        return resourceNodes.Build();
    }
}


public class ResourceNode : Resource, INode<ResourceNode>, IMapFrom<Resource>
{
    public long? Pid => this.ParentId;

    public string Text => this.Name;

    public bool Checked { get; set; }

    public int Sort => 0;

    public ICollection<ResourceNode> Children { get; set; } = [];
}

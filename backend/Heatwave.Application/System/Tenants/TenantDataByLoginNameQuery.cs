using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Tenants;
public record TenantDataByLoginNameQuery(string UserName): IQuery<ICollection<TenantDigest>>;

public class TenantDataByLoginNameQueryHandler : IQueryHandler<TenantDataByLoginNameQuery, ICollection<TenantDigest>>
{
    private readonly IDbAccessor dbAccessor;

    private readonly IMapper mapper;

    public TenantDataByLoginNameQueryHandler(IDbAccessor dbAccessor, IMapper mapper)
    {
        this.dbAccessor = dbAccessor;
        this.mapper = mapper;
    }

    public async Task<ICollection<TenantDigest>> Handle(TenantDataByLoginNameQuery request, CancellationToken cancellationToken)
    {
        var users = await dbAccessor.GetIQueryable<User>()
            .Select(v=> new User { 
                Id = v.Id,
                Email = v.Email,
                PhoneNumber = v.PhoneNumber
            })
            .Where(v => v.Email == request.UserName || v.PhoneNumber == request.UserName)
            .ToListAsync();
        var userIds = users.Select(v => v.Id).ToList();

        var userTeants = await dbAccessor.GetIQueryable<TenantUser>()
            .Include(v => v.Tenant)
            .Where(v => userIds.Contains(v.UserId))
            .ToListAsync();

        return mapper.Map<List<TenantDigest>>(userTeants.Select(v => v.Tenant).ToList());
    }
}

public class TenantDigest: IMapFrom<Tenant>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}


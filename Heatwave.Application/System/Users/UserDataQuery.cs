using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.Users;
public record UserDataQuery(long Id): IQuery<UserDisplay>;

public class UserDataQueryHandler : IQueryHandler<UserDataQuery, UserDisplay>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IMapper mapper;
    public UserDataQueryHandler(IMapper mapper, IDbAccessor dbAccessor)
    {
        this.mapper = mapper;
        this.dbAccessor = dbAccessor;
    }

    public async Task<UserDisplay> Handle(UserDataQuery request, CancellationToken cancellationToken)
    {
        var user = await dbAccessor.GetIQueryable<User>()
            .Where(v => v.Id == request.Id)
            .FirstAsync();

        var data = mapper.Map<UserDisplay>(user);
        var roles =  await dbAccessor.GetIQueryable<TenantUserRole>()
            .Include(v => v.Role)
            .Where(v => v.UserId == request.Id)
            .Select(v=> v.Role)
            .ToListAsync();

        data.Roles = roles;
        return data;
    }
}

public class UserDisplay :  User, IMapFrom<User>
{
    public ICollection<TenantRole> Roles { get; set; }
}

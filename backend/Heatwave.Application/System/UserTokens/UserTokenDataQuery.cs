using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.UserTokens;

public class UserTokenDataQuery: IQuery<UserTokenDisplay>
{
    public long? UserId { get; set; }
    public string TokenHash { get; set; }
}

internal class UserTokenDataQueryHandler : IQueryHandler<UserTokenDataQuery, UserTokenDisplay>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IMapper mapper;

    public UserTokenDataQueryHandler(IDbAccessor dbAccessor, IMapper mapper)
    {
        this.dbAccessor = dbAccessor;
        this.mapper = mapper;
    }

    public async Task<UserTokenDisplay> Handle(UserTokenDataQuery request, CancellationToken cancellationToken)
    {
        var queryable = dbAccessor.GetIQueryable<UserToken>()
            .Include(t => t.User)
            .WhereIf(request.UserId.HasValue, v => v.UserId == request.UserId.Value)
            .WhereIf(request.TokenHash.IsNotNullOrEmpty(), v => v.TokenHash == request.TokenHash);

        var data = await queryable.FirstOrDefaultAsync();
        return mapper.Map<UserTokenDisplay>(data);
    }
}

public class UserTokenDisplay: UserToken, IMapFrom<UserToken>
{

}

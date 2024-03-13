using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.UserTokens;

public class UserTokenDataQuery: IQuery<UserTokenDisplay>
{
    public long? UserId { get; set; }
    public string Token { get; set; }
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
            .Include(t=> t.User)
            .WhereIf(request.UserId.HasValue, v => v.UserId == request.UserId.Value);
        if (request.Token.IsNotNullOrEmpty())
        {
            var hash = request.Token.EncodeMD5();
            queryable = queryable.Where(v => v.TokenHash == hash);
        }
        var data = await queryable.FirstOrDefaultAsync();
        return mapper.Map<UserTokenDisplay>(data);
    }
}

public class UserTokenDisplay: UserToken, IMapFrom<UserToken>
{

}

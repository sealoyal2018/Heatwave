using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.Users;
public class GetUserByIdQuery : IQuery<User>
{
    public long Id { get; set; }
}

internal class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User>
{
    private readonly IDbAccessor dbAccessor;

    public GetUserByIdQueryHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await dbAccessor.GetIQueryable<User>().FirstOrDefaultAsync(v => v.Id == request.Id);
    }
}


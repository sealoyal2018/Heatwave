using Heatwave.Domain.System;
using AutoMapper;
using System.Linq.Expressions;
using Heatwave.Domain;
using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.Users;
public class UserPageListQuery : PaginatedInputBase, IQuery<PaginatedList<UserDegist>>
{
    public string NickName { get; set; }
    public string PhoneNumber { get; set; }
    public UserType? UserType { get; set; }
}

public class UserPageListQueryHandler : IQueryHandler<UserPageListQuery, PaginatedList<UserDegist>>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IMapper _mapper;

    public UserPageListQueryHandler(IMapper mapper, IDbAccessor dbAccessor)
    {
        _mapper = mapper;
        this.dbAccessor = dbAccessor;
    }

    public async Task<PaginatedList<UserDegist>> Handle(UserPageListQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<User, UserDegist>> selectExpr = a => new UserDegist { };
        selectExpr = selectExpr.BuildExtendSelectExpre();
        var res = await dbAccessor.GetIQueryable<User>()
            .Select(selectExpr)
            .WhereIf(request.NickName.IsNotNullOrEmpty(), v => v.NickName.Contains(v.NickName))
            .WhereIf(request.PhoneNumber.IsNotNullOrEmpty(), v => v.PhoneNumber.Contains(v.PhoneNumber))
            .WhereIf(request.UserType.HasValue, v => v.UserType == request.UserType)
            .ToPageAsync(request);

        if (res.Items.IsNotNullOrEmpty())
        {
            var userIds = res.Items.Select(v => v.Id).ToList();
            var userRoles = await dbAccessor.GetIQueryable<TenantUserRole>().Include(v => v.Role).Where(v => userIds.Contains(v.UserId)).ToListAsync();
            foreach(var item in res.Items)
            {
                if(userRoles is not null)
                    item.RoleNames = userRoles.Where(v => v.UserId == item.Id).Where(v=> v.Role is not null).Select(v => v.Role?.Name).ToList();
            }
        }
        return res;
    }
}

public class UserDegist
{
    public long Id { get; set; }

    public string? NickName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public UserType UserType { get; set; } = UserType.Default;

    public string UserTypeText => UserType.GetDescription();

    public List<string> RoleNames { get; set; }
}
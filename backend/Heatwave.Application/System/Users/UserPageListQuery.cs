using Heatwave.Domain.System;
using AutoMapper;
using System.Linq.Expressions;
using LinqKit;

namespace Heatwave.Application.System.Users;

/// <summary>
/// 用户分页获取
/// </summary>
public class UserPageListQuery : PaginatedInputBase, IQuery<PaginatedList<UserDigest>>
{
    public long TenantId { get; set; }
    public string NickName { get; set; }
    public string PhoneNumber { get; set; }
    public UserType? UserType { get; set; }
}

public class UserPageListQueryHandler : IQueryHandler<UserPageListQuery, PaginatedList<UserDigest>>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IMapper _mapper;

    public UserPageListQueryHandler(IMapper mapper, IDbAccessor dbAccessor)
    {
        _mapper = mapper;
        this.dbAccessor = dbAccessor;
    }

    public async Task<PaginatedList<UserDigest>> Handle(UserPageListQuery request, CancellationToken cancellationToken)
    {        
        Expression<Func<User, TenantUser, UserDigest>> selectExpr = (a,t) => new UserDigest
        {
            UserType = t.UserType,
        };
        selectExpr = selectExpr.BuildExtendSelectExpre();
        var whereExpr = Linq.Expr<UserDigest, bool>(v=> true)
            .AndIf(request.NickName.IsNotNullOrAny(), v => v.NickName.Contains(request.NickName))
            .AndIf(request.PhoneNumber.IsNotNullOrAny(), v => v.PhoneNumber.Contains(request.PhoneNumber))
            .AndIf(request.UserType.HasValue, v => v.UserType == request.UserType);
        
        var queryable = from u in dbAccessor.GetIQueryable<User>(cancellation: cancellationToken).AsExpandable()
            join t in dbAccessor.GetIQueryable<TenantUser>(cancellation: cancellationToken).Select(v=> new TenantUser{Id = v.Id, UserId = v.UserId}) on u.Id equals t.UserId 
            select selectExpr.Invoke(u,t);
                
        var res = await queryable.Where(whereExpr).ToPageAsync(request);

        if (!res.Items.IsNotNullOrEmpty())
            return res;
        var userIds = res.Items.Select(v => v.Id).ToList();
        var userRoles = await dbAccessor.GetIQueryable<TenantUserRole>().Include(v => v.Role).Where(v => userIds.Contains(v.UserId)).ToListAsync();
        foreach(var item in res.Items)
        {
            if(userRoles is not null)
                item.RoleNames = userRoles.Where(v => v.UserId == item.Id).Where(v=> v.Role is not null).Select(v => v.Role?.Name).ToList();
        }
        return res;
    }
}

public class UserDigest
{
    public long Id { get; set; }

    public string? NickName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public UserType UserType { get; set; } = UserType.Default;

    public string UserTypeText => UserType.GetDescription();

    public List<string> RoleNames { get; set; }
}
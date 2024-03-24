
using FluentValidation;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.UserTokens;
public class DeletedUesrTokenCommand : ICommand
{
    public List<long> Ids { get; set; } = [];
    public long? UserId { get; set; }
    public string TokenHash { get; set; }
}

public class DeletedUesrTokenCommandValidetor: AbstractValidator<DeletedUesrTokenCommand>
{
    public DeletedUesrTokenCommandValidetor()
    {
        RuleFor(v => v.TokenHash)
            .NotEmpty()
            .WithMessage("token hash 不能为空");
    }
}

public class DeletedUesrTokenCommandHandler : ICommandHandler<DeletedUesrTokenCommand>
{
    private readonly IDbAccessor _dbAccessor;

    public DeletedUesrTokenCommandHandler(IDbAccessor dbAccessor)
    {
        _dbAccessor = dbAccessor;
    }

    public async Task Handle(DeletedUesrTokenCommand request, CancellationToken cancellationToken)
    {
        var userTokenQueryable = _dbAccessor.GetIQueryable<UserToken>()
            .Where(v => v.TokenHash == request.TokenHash)
            .WhereIf(request.UserId.HasValue, v => v.UserId == request.UserId);
        if (request.Ids.IsNotNullOrAny())
            userTokenQueryable = userTokenQueryable.Where(v => request.Ids.Contains(v.Id));
        var userTokens = await userTokenQueryable.ToListAsync();
        if (userTokens.IsNullOrEmpty())
            return;
        var userTokenIds = userTokens.Select(v => v.Id).ToList();
        await _dbAccessor.DeleteAsync<UserToken>(userTokenIds);
    }
}

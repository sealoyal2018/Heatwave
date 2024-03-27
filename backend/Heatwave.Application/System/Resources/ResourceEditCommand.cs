using FluentValidation;
using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;

public class ResourceEditCommand : Resource, ICommand;

public class ResourceEditCommandValidator : AbstractValidator<ResourceEditCommand>
{
    public ResourceEditCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().Length(50);
    }
}

public class EditResourceCommandHandler: ICommandHandler<ResourceEditCommand>
{
    private readonly IDbAccessor _dbAccessor;

    public EditResourceCommandHandler(IDbAccessor dbAccessor)
    {
        _dbAccessor = dbAccessor;
    }

    public async Task Handle(ResourceEditCommand request, CancellationToken cancellationToken)
    {
        if (request.Type == ResourceType.Request)
        {
            if (!request.PermissionCode.IsNotNullOrAny())
                throw new BusException("权限不能为空");
            if (!request.ParentId.HasValue)
                throw new BusException("父级数据不能为空");
        }
        else if (request.Type == ResourceType.Catalogue)
        {
            if (request.Component.IsNullOrEmpty())
                throw new BusException("路由组件不能为空");   
            if (request.Path.IsNullOrEmpty())
                throw new BusException("路由地址不能为空"); 
        }
        else
        {
            if (request.Component.IsNullOrEmpty())
                throw new BusException("路由地址不能为空"); 
            
            if (request.Path.IsNullOrEmpty())
                throw new BusException("路由地址不能为空"); 
        }
        
        
        if (request.Id > 0)
        {
            // 新增
            request.Id = IdHelper.GetLong();
            await _dbAccessor.InsertAsync(request, cancellation: cancellationToken);
        }
        else
        {
            // 编辑
            await _dbAccessor.UpdateAsync(request, cancellation: cancellationToken);
        }
    }
} 
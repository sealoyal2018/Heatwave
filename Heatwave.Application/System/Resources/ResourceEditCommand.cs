using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;
public class ResourceEditCommand : Resource, ICommand, IMapFrom<Resource>
{
}


public class ResourceEditCommandHandler : ICommandHandler<ResourceEditCommand>
{
    private readonly IDbAccessor dbAccessor;

    public ResourceEditCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(ResourceEditCommand request, CancellationToken cancellationToken)
    {
        if(request.Id < 1)
        {
            request.Id = IdHelper.GetLong();
            await dbAccessor.InsertAsync<Resource>(request);
        }
        else
        {
            await dbAccessor.UpdateAsync<Resource>(request);
        }
    }
}

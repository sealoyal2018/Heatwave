using System.ComponentModel.DataAnnotations;

using MediatR;

namespace Heatwave.Application.Common;
public interface IQuery<out TResult>: IRequest<TResult>
{

}

public abstract class IdQueryBase<TResult>: IQuery<TResult> {
    
    [Required]
    public long Id { get; set; }
}

public abstract class TimeRangeQueryBase<TResult>: IQuery<TResult>
{
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}



public interface IQueryHandler<in TQuery, TResult>:
    IRequestHandler<TQuery, TResult>
    where TQuery: IQuery<TResult>
{

}


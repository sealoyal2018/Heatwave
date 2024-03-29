﻿using MediatR;

namespace Heatwave.Application.Common;
public interface ICommand: IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{

}

public interface ICommandHandler<in TCommand> :
    IRequestHandler<TCommand>
    where TCommand: ICommand
{

}

public interface ICommandHandler<in TCommand, TResult> :
    IRequestHandler<TCommand, TResult>
    where TCommand: ICommand<TResult>
{

}


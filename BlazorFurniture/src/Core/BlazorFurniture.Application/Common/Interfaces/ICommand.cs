namespace BlazorFurniture.Application.Common.Interfaces;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResult> : IBaseCommand
{
}
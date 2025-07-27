namespace BlazorFurniture.Common.Abstractions;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResult> : IBaseCommand
{
}
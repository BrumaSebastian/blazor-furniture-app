namespace BlazorFurniture.Application.Common.Models.Email;

public abstract class EmailModel
{
    public virtual Dictionary<string, string> ToParameters() => [];
}

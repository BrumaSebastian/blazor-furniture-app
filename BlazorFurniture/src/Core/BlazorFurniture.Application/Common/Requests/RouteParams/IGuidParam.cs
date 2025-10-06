using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Application.Common.Requests.RouteParams;

public interface IGuidParam
{
    [FromRoute]
    public Guid Id { get; set; }
}

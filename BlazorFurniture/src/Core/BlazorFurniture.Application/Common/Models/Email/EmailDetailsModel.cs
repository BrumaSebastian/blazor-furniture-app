using System.Globalization;

namespace BlazorFurniture.Application.Common.Models.Email;

public class EmailDetailsModel
{
    public required object Template { get; set; }
    public required IReadOnlyDictionary<CultureInfo, IEnumerable<string>> CultureToAddresses { get; set; }
    public required IReadOnlyDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}

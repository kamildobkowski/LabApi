using LabAPI.Domain.Common;

namespace LabAPI.Domain.Entities;

public sealed class OrderResult : BaseEntity
{
	public string OrderNumber { get; set; } = null!;
	public Dictionary<string, Dictionary<string, string>> Results { get; set; } = null!;
}
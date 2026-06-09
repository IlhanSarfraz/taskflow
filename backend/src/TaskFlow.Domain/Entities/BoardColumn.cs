using TaskFlow.Domain.Common;

namespace TaskFlow.Domain.Entities;

public sealed class BoardColumn : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public Guid BoardId { get; set; }

    public Board Board { get; set; } = null!;
}
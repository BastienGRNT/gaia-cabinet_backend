using GaiaSolution.Domain.Base;

namespace GaiaSolution.Domain.Entities;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = null!;
}
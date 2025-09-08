using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.ValueObjects;

namespace GaiaSolution.Domain.Entities;

public class DoctorProfile : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string PostalAddress { get; set; } = null!;
    
    public string Rpps { get; set; } = null!;
    public EmailNormalized Mss { get; set; } = null!;
    
    public int DaysAdvance { get; set; }
}
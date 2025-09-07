namespace GaiaSolution.Domain.Entities;

public class DoctorProfile
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string PostalAddress { get; set; } = null!;
    
    public string RegistrationNumber { get; set; } = null!;
    public string ProfessionalEmail { get; set; } = null!;
    
    public short? DaysAdvance { get; set; }
}
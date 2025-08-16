namespace gaiacabinet_api.Models;

// Table des rôles (Médecin, Femme de ménage, Admin, etc.)
public class Role
{
    public int RoleId { get; set; }
    
    public string Label { get; set; }
    
    public ICollection<User> Users { get; set; } = new List<User>();
}
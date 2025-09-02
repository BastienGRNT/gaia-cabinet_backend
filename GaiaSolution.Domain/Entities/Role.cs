namespace GaiaSolution.Domain.Entities;
/*
 * Représente un rôle attribué à un utilisateur dans le système (ex: Admin, Utilisateur, etc.).
 * Utilisé pour la gestion des permissions et de l'accès.
 */

public enum RoleEnum
{
    Admin,
    Doctor,
    Cleaning,
    Developer,
}
public class Role
{
    public int RoleId { get; set; }
    public RoleEnum RoleName { get; set; }
}
using System.ComponentModel;

namespace Domain.Enums
{
    public enum Claims
    {
        [Description("Accesso á Usuários.")]
        User = 1,
        [Description("Accesso á Tenants.")]
        Tenants = 2,
    }
}

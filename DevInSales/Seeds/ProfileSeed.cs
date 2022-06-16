using DevInSales.Models;

namespace DevInSales.Seeds
{
    public class ProfileSeed
    {
        public static List<Profile> Seed { get; set; } = new List<Profile>() { new Profile(1, "Usuario"), new Profile(2, "Gerente"), new Profile(3, "Administrador") };
    }
}

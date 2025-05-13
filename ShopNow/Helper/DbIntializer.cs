using Microsoft.AspNetCore.Identity;

namespace ShopNow.Helper
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Define roles
            string[] roles = { "Admin", "Seller", "Customer" };

            // Create roles if not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin user
            await SeedUserAsync(userManager, "sujalkareliya27@gmail.com", "Sujal@123", "Admin");

            //// Seed Seller user
            //await SeedUserAsync(userManager, "seller@shopnow.com", "Seller@123", "Seller");

            //// Seed Customer user
            //await SeedUserAsync(userManager, "customer@shopnow.com", "Customer@123", "Customer");
        }

        private static async Task SeedUserAsync(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

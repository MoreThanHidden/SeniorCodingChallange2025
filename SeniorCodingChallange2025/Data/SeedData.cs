using Microsoft.AspNetCore.Identity;

namespace SeniorCodingChallange2025.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Clerk", "Administrator", "Doctor", "Nurse" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                await CreateUser(userManager, "clerk1", "Clerk");
                await CreateUser(userManager, "admin1", "Administrator");
                await CreateUser(userManager, "doctor1", "Doctor");
                await CreateUser(userManager, "nurse1", "Nurse");
            }
        }

        private static async Task CreateUser(UserManager<IdentityUser> userManager, string username, string role)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser { UserName = username, Email = $"{username}@test.com", EmailConfirmed = true };
                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}


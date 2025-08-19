using ApprovalSystem.Dtos;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace ApprovalSystem.Helpers
{
    public static class RuntimeSeeding
    {
        public static void SeedDatabase(IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider().CreateScope())
            {
                var roleManager = provider.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                foreach (var item in RolesData())
                {
                    roleManager.CreateAsync(item).GetAwaiter().GetResult();
                }

                int i = 0;
                string[] passwords = { "password", "password1", "password2", "password3", "password4" };
                var userManager = provider.ServiceProvider.GetRequiredService<UserManager<User>>();
                foreach (var item in UsersData())
                {
                    i = 0;
                    userManager.CreateAsync(item, passwords[i++]).GetAwaiter().GetResult();
                }

                var typeRepo = provider.ServiceProvider.GetRequiredService<IApprovalSetup>();
                foreach (var item in ApprovalTypeData())
                {
                    typeRepo.CreateApprovalType(item);
                }
            }
        }

        private static IEnumerable<Role> RolesData()
        {
            IEnumerable<Role> roles = 
            [
                new Role(){ Name = "Admin", ApprovalStatus = ApprovalStatus.New, Description = "The General Administrative privilege", Status = EntityStatus.Active, NormalizedName = "ADMIN" },
                new Role(){ Name = "IT Support", ApprovalStatus = ApprovalStatus.New, Description = "IT Support user role. For generally technical issues", Status = EntityStatus.Active, NormalizedName = "ITSUPPORT" },
                new Role(){ Name = "Auditor", ApprovalStatus = ApprovalStatus.New, Description = "System audit personnels", Status = EntityStatus.Active, NormalizedName = "AUDITOR" },
                new Role(){ Name = "Business Operations", ApprovalStatus = ApprovalStatus.New, Description = "Business Operations and approval", Status = EntityStatus.Active, NormalizedName = "BUSINESSOPERATIONS" },
                new Role(){ Name = "Project Manager", ApprovalStatus = ApprovalStatus.New, Description = "Project Management team", Status = EntityStatus.Active, NormalizedName = "PROJECTMANAGER" },
            ];

            return roles;
        }

        private static IEnumerable<User> UsersData()
        {
            IEnumerable<User> users =
            [
                new User()
                {
                    FirstName = "Tavershima",
                    LastName = "Ako",
                    Email = "taver@approval.com",
                    Status = EntityStatus.Active,
                    UserName = "taver@approval.com",
                },
                new User()
                {
                    FirstName = "Peace",
                    LastName = "Sever",
                    Email = "peace@approval.com",
                    Status = EntityStatus.Active,
                    UserName = "peace@approval.com",
                },
                new User()
                {
                    FirstName = "Queen",
                    LastName = "Ako",
                    Email = "queen@approval.com",
                    Status = EntityStatus.Active,
                    UserName = "queen@approval.com",
                },
                new User()
                {
                    FirstName = "Grace",
                    LastName = "Uchechukwu",
                    Email = "grace@approval.com",
                    Status = EntityStatus.Active,
                    UserName = "grace@approval.com",
                },
                new User()
                {
                    FirstName = "Jennifer",
                    LastName = "Kato",
                    Email = "jennifer@approval.com",
                    Status = EntityStatus.Active,
                    UserName = "jennifer@approval.com",
                },
            ];

            return users;
        }

        private static IEnumerable<ApprovalTypeModel> ApprovalTypeData()
        {
            IEnumerable<ApprovalTypeModel> apTypes =
            [
                new ApprovalTypeModel()
                {
                    Name = "Role Approval",
                    Description = "The approval process for approving creation, modification of delete of a role",
                    FullImplementingInterface = "ApprovalSystem.Interfaces.IRoleService"
                }
                // add more approval types here
            ];

            return apTypes;
        }
    }
}

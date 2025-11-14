using FridgeSystem.Data;
using Microsoft.AspNetCore.Identity;

namespace FridgeSystem.Models
{
    public class Seed
    {
        public static async Task SeedUsersAndDataAsync(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = provider.GetRequiredService<ApplicationDbContext>();

            string[] roles = 
            {
              "Admin",
              "Customer Liaison",
              "Inventory Liaison",
              "Fault Technician",
              "Maintenance Technician",
              "Customer" // kept for system use (e.g. auto-assigned when customers register)
            };

            foreach (var RoleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(RoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(RoleName));
                }
            }

            await context.SaveChangesAsync();

            string firstname = "Entle";
            string lastname = "Skama";
            string username = "admin@cooltech.com";
            string password = "Admin@123";
            string roleName = "Admin";

           
            //if the role doesnt exist we then create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            //if user doesnt exist create the user
            if (await userManager.FindByNameAsync(username) == null)
            {
                ApplicationUser user = new ApplicationUser { FirstName = firstname, LastName = lastname, UserName = username, Email = "admin@cooltech.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
            // -------------------- CUSTOMER LIAISON --------------------
            string clFirst = "Lerato";
            string clLast = "Mokoena";
            string clEmail = "customerliaison@cooltech.com";
            string clPassword = "Customer@123";
            string clRole = "Customer Liaison";

            var clUser = await userManager.FindByEmailAsync(clEmail);
            if (clUser == null)
            {
                var newClUser = new ApplicationUser
                {
                    UserName = clEmail,
                    Email = clEmail,
                    EmailConfirmed = true
                };
                var resultCl = await userManager.CreateAsync(newClUser, clPassword);
                if (resultCl.Succeeded)
                {
                    await userManager.AddToRoleAsync(newClUser, clRole);
                    clUser = newClUser;
                }
            }

            if (clUser != null && !context.Employees.Any(e => e.ApplicationUserId == clUser.Id))
            {
                context.Employees.Add(new Employee
                {
                    ApplicationUserId = clUser.Id,
                    FirstName = clFirst,
                    LastName = clLast,
                    Email = clEmail,
                    PhoneNumber = "0123456789",
                    City = "Cape Town",
                    Province = "Western Cape",
                    HiredDate = DateTime.Now.AddMonths(-6),
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }

            // -------------------- INVENTORY LIAISON --------------------
            string ilFirst = "Thabo";
            string ilLast = "Mahlangu";
            string ilEmail = "inventoryliaison@cooltech.com";
            string ilPassword = "Inventory@123";
            string ilRole = "Inventory Liaison";

            var ilUser = await userManager.FindByEmailAsync(ilEmail);
            if (ilUser == null)
            {
                var newIlUser = new ApplicationUser
                {
                    UserName = ilEmail,
                    Email = ilEmail,
                    EmailConfirmed = true
                };
                var resultIl = await userManager.CreateAsync(newIlUser, ilPassword);
                if (resultIl.Succeeded)
                {
                    await userManager.AddToRoleAsync(newIlUser, ilRole);
                    ilUser = newIlUser;
                }
            }

            if (ilUser != null && !context.Employees.Any(e => e.ApplicationUserId == ilUser.Id))
            {
                context.Employees.Add(new Employee
                {
                    ApplicationUserId = ilUser.Id,
                    FirstName = ilFirst,
                    LastName = ilLast,
                    Email = ilEmail,
                    PhoneNumber = "0987654321",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    HiredDate = DateTime.Now.AddMonths(-12),
                    IsActive = true
                });
                await context.SaveChangesAsync();
            }


            // -------------------- FAULT TECHNICIAN --------------------
            // -------------------- FAULT TECHNICIAN --------------------
            string ftFirst = "Rain";
            string ftLast = "Stormz";
            string ftEmail = "faulttech@cooltech.com";
            string ftPassword = "FaultTech@123";
            string ftRole = "Fault Technician";

            // 1️⃣ Check if the Identity user exists
            var ftUser = await userManager.FindByNameAsync(ftEmail);

            // 2️⃣ Create the user if not exists
            if (ftUser == null)
            {
                var newFtUser = new ApplicationUser
                {
                    UserName = ftEmail,
                    Email = ftEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newFtUser, ftPassword);
                if (createResult.Succeeded)
                {
                    // Ensure role exists
                    if (!await roleManager.RoleExistsAsync(ftRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole(ftRole));
                    }

                    // Assign role to user
                    await userManager.AddToRoleAsync(newFtUser, ftRole);

                    // 3️⃣ Create linked Employee profile
                    var ftEmployee = new Employee
                    {
                        ApplicationUserId = newFtUser.Id, // Link to Identity user
                        FirstName = ftFirst,
                        LastName = ftLast,
                        Email = ftEmail,
                        PhoneNumber = "0123456789",
                        City = "Port Elizabeth",
                        Province = "Gauteng",
                        HiredDate = DateTime.Now.AddMonths(-3),
                        IsActive = true
                    };

                    context.Employees.Add(ftEmployee);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.BusinessTypes.Any())
            {
                context.BusinessTypes.AddRange(
                    new BusinessType { TypeName = "Supermarket" },
                    new BusinessType { TypeName = "Hypermarket" },
                    new BusinessType { TypeName = "Spaza Shop" },
                    new BusinessType { TypeName = "Liquor Store" },
                    new BusinessType { TypeName = "Bar" },
                    new BusinessType { TypeName = "Guesthouse" },
                    new BusinessType { TypeName = "Shebeen" }

                );

                await context.SaveChangesAsync();
            }


            // -------------------- CUSTOMER --------------------
            string custEmail = "contact@spar.co.za";
            string custPassword = "Customer@123";
            string custRole = "Customer";
            string businessName = "Spar";
            string contactPerson = "Nolwazi Dlamini";

            // 1️⃣ Ensure the role exists
            if (await roleManager.FindByNameAsync(custRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(custRole));
            }

            // 2️⃣ Create ApplicationUser (no first/last name needed)
            var custUser = await userManager.FindByEmailAsync(custEmail);
            if (custUser == null)
            {
                var newCustUser = new ApplicationUser
                {
                    UserName = custEmail,
                    Email = custEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newCustUser, custPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newCustUser, custRole);

                    // Create linked Customer business record
                    var customer = new Customer
                    {
                        ApplicationUserId = newCustUser.Id,
                        BusinessName = businessName,
                        Email = custEmail,
                        Telephone = "0712345678",
                        BillingAddress = "123 Main Street, Durban",
                        ContactPersonName = contactPerson,
                        BusinessTypeId = context.BusinessTypes.FirstOrDefault()?.BusinessTypeId ?? 1,
                        ProfileActive = true,
                        DateCreated = DateTime.Now,
                        isDeleted = false
                    };

                    context.Customers.Add(customer);
                    await context.SaveChangesAsync();
                }
            }






            if (!context.Fridges.Any())
            {
                context.Fridges.AddRange(
                    new Fridge
                    {
                        Model = "748L Double Glass door Beverage Cooler",
                        Specification = "Fan-forced cooling, LED lighting, 8 shelves.",
                        FridgeColor = "Black",
                        FridgeImage = "/Images/DD1.JPG",
                        StockQuantity = 1,
                        Category = FridgeCategory.DoubleDoor

                    },

                    new Fridge
                    {
                        Model = "600L Double Glass door Beverage Cooler",
                        Specification = "Fan-forced cooling, LED lighting, 8 shelves.",
                        FridgeColor = "Silver",
                        FridgeImage = "/Images/DD2.PNG",
                        StockQuantity = 1,
                        Category = FridgeCategory.DoubleDoor
                    },

                     new Fridge
                     {
                         Model = "400L Single Glass Door Beverage Cooler",
                         Specification = "Fan-forced cooling, LED lighting, 4 adjustable shelves.",
                         FridgeColor = "White",
                         FridgeImage = "/Images/SD 1.JPG",
                         StockQuantity = 1,
                         Category = FridgeCategory.SingleDoor
                     },

                       new Fridge
                       {
                           Model = "380L Single Glass Door Beverage Cooler",
                           Specification = "Fan-forced cooling, LED lighting, 4 adjustable wire shelves.",
                           FridgeColor = "White",
                           FridgeImage = "/Images/SD2.PNG",
                           StockQuantity = 1,
                           Category = FridgeCategory.SingleDoor
                       },

                         new Fridge
                         {
                             Model = "500L 3 Glass Doors Beverage Cooler",
                             Specification = "Fan-forced cooling, LED lighting,3 shelves.",
                             FridgeColor = "Grey",
                             FridgeImage = "/Images/UB 1.JPG",
                             StockQuantity = 1,
                             Category = FridgeCategory.UnderBar
                         },

                          new Fridge
                          {
                              Model = "350L Solid Double door Beverage Cooler",
                              Specification = "Fan-forced cooling, LED lighting,3 shelves.",
                              FridgeColor = "Silver",
                              FridgeImage = "/Images/UB 2.JPG",
                              StockQuantity = 1,
                              Category = FridgeCategory.UnderBar
                          }
                );

                await context.SaveChangesAsync();

            }
            else
            {
                foreach (var fridge in context.Fridges.Where(f => f.StockQuantity == 0))
                {
                    fridge.StockQuantity = 1;
                }
            }


            /*if (!context.BusinessTypes.Any())
            {
                context.BusinessTypes.AddRange(
                    new BusinessType { TypeName = "Supermarket" },
                    new BusinessType { TypeName = "Hypermarket" },
                    new BusinessType { TypeName = "Spaza Shop" },
                    new BusinessType { TypeName = "Liquor Store" },
                    new BusinessType { TypeName = "Bar" },
                    new BusinessType { TypeName = "Guesthouse" },
                    new BusinessType { TypeName = "Shebeen" }

                );

                await context.SaveChangesAsync();
            }*/

        }
    }
}

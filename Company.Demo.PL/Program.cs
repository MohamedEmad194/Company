using Company.Demo.BLL.Interfaces;
using Company.Demo.BLL.Repositories; 
using Company.Demo.BLL.Services;
using Company.Demo.DAL.Data.Contexts;
using Company.Demo.PL.Mapping.Employees;
using Company.Demo.PL.Models;
using Company.Demo.PL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.AspNetCore.Identity;

namespace Company.Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();//Allow Dependency injection for DepartmentRepository 
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();//Allow Dependency injection for EmployeeRepositor
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddMemoryCache();

            // Configure Email Settings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Allow HTTP cookies in development
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                });
            }
            else
            {
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                });
            }

            builder.Services.AddRazorPages();

            builder.Services.AddAutoMapper(typeof(EmployeeProfile));

            // Service Lifetime Examples:
            // Singleton: One instance for the entire application lifetime
            // builder.Services.AddSingleton<IExampleSingletonService, ExampleSingletonService>();
            // Scoped: One instance per HTTP request (default for most app services)
            // builder.Services.AddScoped<IExampleScopedService, ExampleScopedService>();
            // Transient: New instance every time requested
            // builder.Services.AddTransient<IExampleTransientService, ExampleTransientService>();

            ////life Time
            //builder.Services.AddScoped();  //per Request, UnReachable Object
            //builder.Services.AddSingleton();//per App
            //builder.Services.AddTransient(); //per operation

            //builder.Services.AddScoped<IScopedService, ScopedService>();

            #endregion

            var app = builder.Build();

            // Seed admin user and role
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedAdminUser(services).GetAwaiter().GetResult();
            }

            // إضافة أدمن جديد مؤقتًا
            Task.Run(async () =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    string adminEmail2 = "mohamed@gmail.com";
                    string adminPassword2 = "mohamed@123";
                    string adminRole2 = "Admin";

                    try
                    {
                        Console.WriteLine("Starting to seed second admin user...");
                        
                        // تأكد من وجود الـ Role "Admin"
                        if (!await roleManager.RoleExistsAsync(adminRole2))
                        {
                            await roleManager.CreateAsync(new IdentityRole(adminRole2));
                            Console.WriteLine($"Created role: {adminRole2}");
                        }
                        else
                        {
                            Console.WriteLine($"Role {adminRole2} already exists");
                        }

                        // أنشئ المستخدم إذا لم يكن موجودًا
                        var user2 = await userManager.FindByEmailAsync(adminEmail2);
                        if (user2 == null)
                        {
                            user2 = new IdentityUser { UserName = adminEmail2, Email = adminEmail2, EmailConfirmed = true };
                            var result2 = await userManager.CreateAsync(user2, adminPassword2);
                            if (!result2.Succeeded)
                            {
                                Console.WriteLine("Failed to create second admin user:");
                                foreach (var error in result2.Errors)
                                    Console.WriteLine($"Error: {error.Description}");
                            }
                            else
                            {
                                Console.WriteLine($"Successfully created second admin user: {adminEmail2}");
                                // أضف المستخدم للـ Role "Admin" فقط بعد إنشائه بنجاح
                                if (!await userManager.IsInRoleAsync(user2, adminRole2))
                                {
                                    await userManager.AddToRoleAsync(user2, adminRole2);
                                    Console.WriteLine($"Added user {adminEmail2} to role {adminRole2}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Second admin user {adminEmail2} already exists");
                            // أضف المستخدم للـ Role "Admin" إذا لم يكن مضافًا
                            if (!await userManager.IsInRoleAsync(user2, adminRole2))
                            {
                                await userManager.AddToRoleAsync(user2, adminRole2);
                                Console.WriteLine($"Added user {adminEmail2} to role {adminRole2}");
                            }
                        }
                        
                        Console.WriteLine("Second admin user seeding completed successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding second admin user: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
            }).GetAwaiter().GetResult();

            #region Configure Kestrel Middleware
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection(); // Commented out for development to avoid HTTPS issues
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            #endregion

            app.Run();
        }

        private static async Task SeedAdminUser(IServiceProvider services)
        {
            try
            {
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                string adminEmail = "mohamedemadbrkawy@gmail.com";
                string adminPassword = "MOHAMEDemad123";
                string adminRole = "Admin";

                Console.WriteLine("Starting to seed admin user...");

                // Ensure Admin role exists
                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(adminRole));
                    Console.WriteLine($"Created role: {adminRole}");
                }
                else
                {
                    Console.WriteLine($"Role {adminRole} already exists");
                }

                // Ensure user exists
                var user = await userManager.FindByEmailAsync(adminEmail);
                if (user == null)
                {
                    user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (!result.Succeeded)
                    {
                        Console.WriteLine("Failed to create admin user:");
                        foreach (var error in result.Errors)
                            Console.WriteLine($"Error: {error.Description}");
                        return; // Don't proceed if user creation failed
                    }
                    Console.WriteLine($"Successfully created admin user: {adminEmail}");
                }
                else
                {
                    Console.WriteLine($"Admin user {adminEmail} already exists");
                }

                // Ensure user is in Admin role
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                    Console.WriteLine($"Added user {adminEmail} to role {adminRole}");
                }
                else
                {
                    Console.WriteLine($"User {adminEmail} is already in role {adminRole}");
                }

                Console.WriteLine("Admin user seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding admin user: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

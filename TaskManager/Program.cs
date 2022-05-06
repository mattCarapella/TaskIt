using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Areas.Identity.Data;
using TaskManager.Authorization;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TaskManagerContextConnection");

builder.Services.AddDbContext<TaskManagerContext>(options =>
    options
        .UseSqlServer(
            connectionString,
            // Enable split queries with .AsSingleQuery().ToList()
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TaskManagerContext>();

builder.Services.AddControllersWithViews();

#region Authorization

    AddAuthorizationPolicies(builder.Services);

#endregion

AddScoped();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


void AddAuthorizationPolicies(IServiceCollection services)
{
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
        options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));
        options.AddPolicy("ElevatedRights", policy => policy.RequireRole("Administrator", "Manager"));
        options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeID"));
        options.AddPolicy("DevelopersOnly", policy => policy.RequireClaim("JobTitle", "Developer"));
    });
}

void AddScoped()
{
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
    builder.Services.AddScoped<ITicketRepository, TicketRepository>();
    builder.Services.AddScoped<IProjectAssignmentRepository, ProjectAssignmentRepository>();
    builder.Services.AddScoped<ITicketAssignmentRepository, TicketAssignmentRepository>();
    builder.Services.AddScoped<ITNoteRepository, TNoteRepository>();
    builder.Services.AddScoped<IPNoteRepository, PNoteRepository>();

    builder.Services.AddScoped<IAuthorizationHandler, TicketIsSubmitterAuthorizationHandler>();

    builder.Services.AddScoped<IAuthorizationHandler, PNoteIsSubmitterAuthorizationHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, PNoteManagerAuthorizationHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, PNoteAdminAuthorizationHandler>();
}

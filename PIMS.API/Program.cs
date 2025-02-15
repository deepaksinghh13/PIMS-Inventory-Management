using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PIMS.Core.Models;
using PIMS.EntityFramework.DataSeed;
using System;
using PIMS.EntityFramework.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Configured DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PrimsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configured Identity
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<PrimsDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    
});

builder.Services.AddControllers();

// Swagger API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seeding Initial Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PrimsDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    context.Database.Migrate();

    await DataSeeder.SeedData(context, userManager, roleManager);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();  

app.MapControllers();

app.Run();
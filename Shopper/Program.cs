using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shopper.Data;
using Shopper.Infrastructure;
using Shopper.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
    .AddBearerToken(IdentityConstants.BearerScheme
    //, o =>
    //{
    //    o.BearerTokenExpiration = TimeSpan.FromSeconds(60);
    //}
    );
    //.AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ShopperContext>()
    .AddApiEndpoints();

builder.Services.AddDbContext<ShopperContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ShopperContext")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopper", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });

builder.Services.AddTransient<ICartService, CartService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IRestaurantService, RestaurantService>();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(builder => builder
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowAnyOrigin()
);

app.MapControllers();

app.Run();
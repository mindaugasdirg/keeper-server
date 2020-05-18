using System;
using System.Text;
using Keeper.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Keeper.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if(Env.IsDevelopment())
                services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("InMemory"));
            else
                services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(GetConnectionString()));

            Console.WriteLine(GetConnectionString());

            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Issuer"],
                        ValidAudience = Configuration["Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddTransient<ITransactionsService, TransactionsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<ISecurityService, SecurityService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!(Environment.GetEnvironmentVariable(Constants.CONN_STR) is null))
                UpdateDatabase(app);
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            Console.WriteLine("Applying migrations");
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DatabaseContext>())
                {
                    context.Database.Migrate();
                }
            }
            Console.WriteLine("Migrations applied");
        }

        private string GetConnectionString()
        {
            var address = new Uri(Environment.GetEnvironmentVariable(Constants.CONN_STR));
            var user = address.UserInfo.Split(":")[0];
            var pass = address.UserInfo.Split(":")[1];
            var host = address.Host;
            var port = address.Port;
            var database = address.PathAndQuery.Trim('/');
            return string.Format("Host={0};Port={1};Database={2};Username={3};Password={4}", host, port, database, user, pass);
        }
    }
}

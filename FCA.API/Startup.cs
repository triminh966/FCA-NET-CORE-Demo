using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2;
using FCA.API.Services;
using FCA.Core.Secrets;
using FCA.Data;
using FCA.Data.DbContext;
using FCA.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FCA.API
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";
        private readonly IFcaSecrets _fcaSecrets;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _fcaSecrets = new FcaSecrets(hostingEnvironment, configuration);
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            var fcaConnectionString = _fcaSecrets.OtfRdsDataFca;
            var otbaseConnectionString = _fcaSecrets.OtfRdsDataOTbase;
            services.AddDbContext<FCAContext>(x => x.UseMySQL(fcaConnectionString));
            services.AddDbContext<OTbaseContext>(x => x.UseMySQL(otbaseConnectionString));

            // Register Repositories
            services.AddScoped<IChallengeRepository, ChallengeRepository>();
            services.AddScoped<IConnectionSocketRepository, ConnectionSocketRepository>();
            services.AddScoped<IStudioRepository, StudioRepository>();
            services.AddScoped<IStudioChallengeResultAvgRepository, StudioChallengeResultAvgRepository>();

            // Register Services
            services.AddScoped<IFcaSecrets, FcaSecrets>();
            services.AddScoped<IChallengeTrackerService, ChallengeTrackerService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Add DynamoDB to the ASP.NET Core dependency injection framework
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();

            // Add Authentication
            services.AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Audience = _fcaSecrets.ClientId;
                options.Authority = _fcaSecrets.AuthorityUrl;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            #region Automapper

            AutoMapper.Mapper.Initialize(cfg =>
            {
                // Account and Login
                cfg.CreateMap<AdminInitiateAuthResponse, Models.AccountLoginResponseModel>()
                    .ForMember(dest => dest.RefreshToken,
                        opt => opt.MapFrom(src => src.AuthenticationResult.RefreshToken))
                    .ForMember(dest => dest.TokenType,
                        opt => opt.MapFrom(src => src.AuthenticationResult.TokenType))
                    .ForMember(dest => dest.ExpiresIn,
                        opt => opt.MapFrom(src => src.AuthenticationResult.ExpiresIn))
                    .ForMember(dest => dest.IdToken,
                        opt => opt.MapFrom(src => src.AuthenticationResult.IdToken))
                    .ForMember(dest => dest.AccessToken,
                        opt => opt.MapFrom(src => src.AuthenticationResult.AccessToken));
            });
            

            #endregion

            app.UseCors("AllowMyOrigin");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

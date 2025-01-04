using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.MappingProfiles;
using NS.Quizzy.Server.BL.Services;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.DAL.Extensions;
using NS.Security;
using NS.Shared.CacheProvider.Extensions;
using NS.Shared.Logging;
using NS.Shared.Logging.Extensions;
using System.Security.Claims;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddQuizzyBLServices(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddNSLogger(configuration);
            services.AddNSCacheProvider();
            services.AddQuizzyDALServices();
            services.AddMappingProfiles();

            services.AddHttpContextAccessor();
            services.AddSingleton<JwtHelper>();
            var jwtHelper = services.BuildServiceProvider().GetRequiredService<JwtHelper>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtHelper.GetJwtIssuer(),
                        ValidAudience = jwtHelper.GetJwtAudience(),
                        IssuerSigningKey = new SymmetricSecurityKey(jwtHelper.GetJwtKey()),
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("Token validated successfully");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies[BLConsts.AUTH_TOKEN_KEY];
                            if (string.IsNullOrEmpty(token))
                            {
                                token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                            }

                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }

                            Console.WriteLine("Custom token retrieval logic executed");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddScoped<IClassExamService, ClassExamService>();
            services.AddScoped<IClassesService, ClassesService>();
            services.AddScoped<IExamsService, ExamsService>();
            services.AddScoped<IExamTypesService, ExamTypesService>();
            services.AddScoped<IQuestionnairesService, QuestionnairesService>();
            services.AddScoped<ISubjectsService, SubjectsService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<ITestService, TestService>();

            return services;
        }

        public static IApplicationBuilder UseQuizzyBL(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseAuthentication();
            //app.UseAuthorization();

            return app;
        }

        private static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }


        internal static Guid? GetUserId(this HttpContext? context)
        {
            // Retrieve the user ID from the claims
            var userIdStr = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdStr == null)
            {
                return null;
            }

            return Guid.Parse(userIdStr);
        }

        internal static Guid? GetTokenId(this HttpContext? context)
        {
            // Retrieve the user ID from the claims
            var tokenIdStr = context?.User?.FindFirst(ClaimTypes.Sid)?.Value;

            if (tokenIdStr == null)
            {
                return null;
            }

            return Guid.Parse(tokenIdStr);
        }
        public static object? GetValueByType(this AppSetting? item, INSLogger logger)
        {
            try
            {
                if (item == null)
                {
                    return default;
                }
                var value = item.Value;

                if (item.IsSecured)
                {
                    var securityLogic = new SecurityLogic(BLConsts.APP_SETTING_SECURITY_KEY);
                    value = securityLogic.Decrypt(value);
                }

                switch (item.ValueType)
                {
                    case DAL.DALEnums.AppSettingValueTypes.Integer:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return int.Parse(value);
                        }
                        else
                        {
                            return default(int);
                        }
                    case DAL.DALEnums.AppSettingValueTypes.Double:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return double.Parse(value);
                        }
                        else
                        {
                            return default(double);
                        }
                    case DAL.DALEnums.AppSettingValueTypes.Float:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return float.Parse(value);
                        }
                        else
                        {
                            return default(float);
                        }
                    case DAL.DALEnums.AppSettingValueTypes.Boolean:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return bool.Parse(value);
                        }
                        else
                        {
                            return default(bool);
                        }
                    case DAL.DALEnums.AppSettingValueTypes.Char:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return char.Parse(value);
                        }
                        else
                        {
                            return default(char);
                        }
                    case DAL.DALEnums.AppSettingValueTypes.String:
                        return value;
                    case DAL.DALEnums.AppSettingValueTypes.Object:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return JsonConvert.DeserializeObject(value);
                        }
                        else
                        {
                            return default(string);
                        }
                    default:
                        throw new Exception($"Unknown type '{item.ValueType}'");
                }
            }
            catch (Exception ex)
            {
                logger.Fatal($"AppSetting value parsing exception '{ex.Message}'", new { AppSettingItem = item }, ex);
                throw new Exception($"AppSetting value parsing error '{ex.Message}'");
            }
        }
    }
}

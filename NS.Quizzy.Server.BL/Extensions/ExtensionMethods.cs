using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.AppConfiguration;
using NS.Quizzy.Server.BL.CustomizationsForSwagger;
using NS.Quizzy.Server.BL.HostedServices;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.MappingProfiles;
using NS.Quizzy.Server.BL.QueueSubscriptions;
using NS.Quizzy.Server.BL.Services;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.DAL.Extensions;
using NS.Security;
using NS.Shared.CacheProvider.Extensions;
using NS.Shared.CacheProvider.models;
using NS.Shared.Logging;
using NS.Shared.Logging.Configs;
using NS.Shared.Logging.Extensions;
using NS.Shared.QueueManager.Extensions;
using NS.Shared.QueueManager.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddQuizzyBLServices(this IServiceCollection services, IConfigurationBuilder configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            IConfiguration config = (IConfiguration)configuration;

            #region NSLogger
            var loggerConfig = config.GetSection("NSLoggerConfig").Get<NSLoggerConfig>();
            if (loggerConfig != null)
            {
                loggerConfig.GetContextInfos = (httpContext) =>
                {
                    var res = new Dictionary<string, object?>
                    {
                        { LoggerConsts.LOGGER_GLOBAL_PROPERTY__USER_ID, httpContext.GetUserId() },
                        { LoggerConsts.LOGGER_GLOBAL_PROPERTY__TOKEN_ID, httpContext.GetTokenId() }
                    };
                    return res;
                };

                var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName();
                loggerConfig.GlobalProperties = new Dictionary<string, object?>()
                {
                    { LoggerConsts.LOGGER_GLOBAL_PROPERTY__SERVICE_NAME, assemblyName?.Name },
                    { "AppVersion", assemblyName?.Version?.ToString() },
                    { "Application", "Quizzy" },
                    { "AppContext", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") },
                };
            }
            services.AddNSLogger(loggerConfig);
            #endregion

            services.AddQuizzyDALServices();

            var appName = config.GetValue<string>("AppName");
            var environment = config.GetValue<string>("Environment");


            var cacheProviderConfigs = config.GetSection("NSCacheProviderConfigs").Get<NSCacheProviderConfigs>();
            cacheProviderConfigs.CachePrefix = $"{appName}:{environment}";
            services.AddNSCacheProvider(cacheProviderConfigs);

            var queueManagerConfig = config.GetSection("NSQueueManagerConfig").Get<NSQueueManagerConfig>();
            services.AddNSQueueManager(queueManagerConfig);

            services.AddNSQueueSubscription(setup =>
            {
                setup.AddSubscriptionClass<UpdateUsersQueueSubscription>();
                setup.AddSubscriptionClass<PushNotificationsQueueSubscription>();
                setup.AddSubscriptionClass<ExamEventsQueueSubscription>();
            });

            #region DbConfiguration
            services.AddSingleton<IConfigurationSource, DbConfigurationSource>();

            // Add DB Configuration Source
            configuration.Add(new DbConfigurationSource(services.BuildServiceProvider()));
            #endregion

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

            services.AddScoped<IHealthService, HealthService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IGradesService, GradesService>();
            services.AddScoped<IClassesService, ClassesService>();
            services.AddScoped<IExamsService, ExamsService>();
            services.AddScoped<IExamTypesService, ExamTypesService>();
            services.AddScoped<IMoedsService, MoedsService>();
            services.AddScoped<IQuestionnairesService, QuestionnairesService>();
            services.AddScoped<ISubjectsService, SubjectsService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IFcmService, FcmService>();
            services.AddScoped<INotificationGroupsService, NotificationGroupsService>();
            services.AddScoped<INotificationTemplatesService, NotificationTemplatesService>();
            services.AddScoped<INotificationsService, NotificationsService>();
            services.AddScoped<IDevicesService, DevicesService>();
            services.AddScoped<IDeveloperService, DeveloperService>();
            services.AddScoped<IOutlookCalendarService, OutlookCalendarService>();

            services.AddHostedService<AppLifetimeInfoHostedService>();

            return services;
        }

        public static IApplicationBuilder UseQuizzyBL(this IApplicationBuilder app)
        {
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            forwardOptions.KnownNetworks.Clear(); // Allows any network to be a known source
            forwardOptions.KnownProxies.Clear();  // Allows any proxy to be a known proxy
            app.UseForwardedHeaders(forwardOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        private static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(
                mc =>
                {
                    mc.AddProfile(new MappingGlobalProfile());
                },
                new NullLoggerFactory()
            );
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }

        public static SwaggerGenOptions AddCustomOperationFilters(this SwaggerGenOptions c)
        {
            c.OperationFilter<RoleRequirementOperationFilter>();
            c.OperationFilter<AllowAnonymousOperationFilter>();
            return c;
        }

        internal static Guid? GetUserId(this HttpContext? context)
        {
            // Retrieve the user Id from the claims
            var userIdStr = context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdStr == null)
            {
                return null;
            }

            return Guid.Parse(userIdStr);
        }

        internal static DAL.DALEnums.Roles? GetUserRole(this HttpContext? context)
        {
            // Retrieve the user Id from the claims
            var stringValue = context?.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (stringValue == null)
            {
                return null;
            }

            return Enum.Parse<DAL.DALEnums.Roles>(stringValue);
        }

        internal static Guid? GetTokenId(this HttpContext? context)
        {
            // Retrieve the user Id from the claims
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
                    case DAL.DALEnums.AppSettingValueTypes.Long:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return long.Parse(value);
                        }
                        else
                        {
                            return default(long);
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
                logger.Fatal(ex, $"AppSetting value parsing exception '{ex.Message}'", new { AppSettingItem = item });
                throw new Exception($"AppSetting value parsing error '{ex.Message}'");
            }
        }

        internal static uint? GetFullCode(this Class? @class)
        {
            if (@class == null || @class.Grade == null)
            {
                return null;
            }

            return (@class.Grade.Code * 100) + @class.Code;
        }

        internal static Roles ToUserRole(this string role)
        {
            return (role ?? string.Empty) switch
            {
                "תלמיד" => Roles.Student,
                "מורה" => Roles.Teacher,
                "אחראי" => Roles.Admin,
                "מפתח" => Roles.Developer,
                "מנהל מערכת" => Roles.SuperAdmin,
                _ => throw new Exception($"Unknown role '{role}'"),
            };
        }

        internal static string ToHebrewRole(this Roles role)
        {
            return role switch
            {
                Roles.Student => "תלמיד",
                Roles.Teacher => "מורה",
                Roles.Admin => "אחראי",
                Roles.Developer => "מפתח",
                Roles.SuperAdmin => "מנהל מערכת",
                _ => throw new Exception($"Unknown role '{role}'"),
            };
        }

        internal static string ToExamStringTime(this DateTimeOffset dt)
        {
            var val = dt.DateTime;
            if (val.Hour == 0 && val.Minute == 0)
            {
                return val.ToString("dd/MM/yyyy");
            }
            return val.ToString("dd/MM/yyyy HH:mm");
        }

        internal static string GetQueueMessageStatusCacheKey(this Guid messageId)
        {
            return $"QueueMsgStatus:{messageId}";
        }
    }
}

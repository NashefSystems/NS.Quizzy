using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.MappingProfiles;
using NS.Quizzy.Server.BL.Services;
using NS.Quizzy.Server.DAL.Extensions;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddQuizzyBLServices(this IServiceCollection services)
        {
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
                            var token = context.Request.Cookies[Consts.AUTH_TOKEN_KEY];
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
    }
}

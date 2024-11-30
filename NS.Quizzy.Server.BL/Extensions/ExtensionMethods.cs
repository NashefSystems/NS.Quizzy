using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Services;
using NS.Quizzy.Server.DAL.Extensions;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddQuizzyBLServices(this IServiceCollection services)
        {
            services.AddQuizzyDALServices();

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
                        }
                    };
                });

            services.AddScoped<IClassExamService, ClassExamService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamTypeService, ExamTypeService>();
            services.AddScoped<IQuestionnaireService, QuestionnaireService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAccountService, AccountService>();

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
    }
}


using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using NS.Quizzy.Server.BL.Extensions;

namespace NS.Quizzy.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            builder.Services.AddSwaggerGenNewtonsoftSupport();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.UseInlineDefinitionsForEnums();

                var docXmlPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
                     .Where(x => x.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase) && Path.GetFileName(x).StartsWith("ns.", StringComparison.CurrentCultureIgnoreCase))
                     .ToList();

                foreach (var docXmlPath in docXmlPaths)
                {
                    c.IncludeXmlComments(docXmlPath, includeControllerXmlComments: true);
                }

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Bearer Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            builder.Services.AddQuizzyBLServices(builder.Configuration);

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseQuizzyBL();
            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}

using System;
using Alura.WebApi.Api.Filtros;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Alura.ListaLeitura.Api.Formatters;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Collections.Generic;

namespace Alura.WebApi.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LeituraContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new LivrosCsvFormatters());
                options.Filters.Add(new ErrorResponseFilter());
            }
            ).AddXmlSerializerFormatters();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = "JwtBearer";
                opt.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("alura-webapi-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = "Alura.WebApi",
                    ValidAudience = "Insomnia"
                };
            });

            services.AddApiVersioning();
            //services.AddApiVersioning(options =>
            //{
            //    options.ApiVersionReader = ApiVersionReader.Combine(
            //        new QueryStringApiVersionReader("api-version"),
            //        new HeaderApiVersionReader("api-version")
            //        );
            //});

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Livros Api",
                    Description = "Documentação da API de livros"
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "2.0",
                    Title = "Livros Api",
                    Description = "Documentação da API de livros"
                });
                c.OperationFilter<AuthResponsesOperationFilter>();


                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Description = "Autenticação Bearer via JWT"
                //});
                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>{
                //    { "Bearer", new string[]{ } }
                //});

            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão 1.0");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Versão 2.0");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseMvc();


        }
    }
}

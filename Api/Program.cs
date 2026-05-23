using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoRegisterAnnotation;
using FluentValidation;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;

// Bootstrap Logger
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

// Dapper snake_case to PascalCase property mapping configuration
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// FluentValidation culture settings
ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.InvariantCulture;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog(
        (context, provider, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .Enrich.FromLogContext();
        }
    );

    // Configure DI Valication
    builder.Host.UseDefaultServiceProvider(opts =>
    {
        opts.ValidateScopes = true;
        opts.ValidateOnBuild = true;
    });

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddProblemDetails();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    builder.Services.AddFluentValidationRulesToOpenApi();
    builder.Services.AddAutoRegisterServices<Program>();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi(opts =>
    {
        opts.AddOperationTransformer(
            (operation, context, ct) =>
            {
                operation.Summary = null;
                operation.Description = null;
                return Task.CompletedTask;
            }
        );

        opts.AddFluentValidationRules();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(opts =>
        {
            opts.EnableDarkMode()
                .WithTitle("WatchStore Api Reference")
                .WithTheme(ScalarTheme.BluePlanet)
                .ShowOperationId()
                .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
                .WithDocumentDownloadType(DocumentDownloadType.Json)
                .WithJsonDocumentDownload();
        });
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler();
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

using System.IO.Compression;
using bank_on_api.Adapters;
using bank_on_api.GraphQL.Mutations;
using bank_on_api.GraphQL.Queries;
using bank_on_api.Helpers;
using bank_on_api.Models.Entities.BankOn;
using HotChocolate.Data.Filters;
using HotChocolate.Execution.Options;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.NodaTime;
using HotChocolate.Types.Pagination;
using MicroElements.Swashbuckle.NodaTime;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using static bank_on_api.Helpers.FileContentResultTypeAttribute;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContextPool<BankOn>(options =>
{

    options.UseSqlServer(builder.Configuration.GetConnectionString("BANKONDBCLOUD"), x => { x.UseNodaTime(); });
    if (builder.Environment.IsProduction()) return;
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
});

builder.Services.AddPooledDbContextFactory<BankOn>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BANKONDBCLOUD"), x => { x.UseNodaTime(); });
    if (builder.Environment.IsProduction()) return;
    options.EnableSensitiveDataLogging();
    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "LoosePolicy",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5177")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                          policy.WithOrigins("http://localhost:7177")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                          policy.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                          policy.WithOrigins("https://localhost:5177")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                          policy.WithOrigins("https://localhost:7177")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                          policy.WithOrigins("https://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                      });
});


builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;

}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "YYYY-MM-DD";
    options.SerializerSettings.ContractResolver = new DefaultContractResolver(); ;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

}).AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);



builder.Services.AddSingleton<IClockService, ClockService>();
builder.Services.AddScoped<BankOnAdapter>();

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FileResultContentTypeOperationFilter>();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankOn API", Version = "v1.0.0" });
    c.ConfigureForNodaTime();
});

builder.Services.AddGraphQLServer()
    .SetRequestOptions(_ => new RequestExecutorOptions { ExecutionTimeout = TimeSpan.FromMinutes(10) })
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSorting()
    .AddFiltering()
    .AddProjections()
    .AddAuthorization()
    .AddInMemorySubscriptions()
    .AddType(new UuidType('D'))
    .AddType<InstantType>()
    .AddType<LocalDateType>()
    .AddType<LocalTimeType>()
    .AddType<UploadType>()
    .AddConvention<INamingConventions>(new GraphQLNamingConvention())
    .AddConvention<IFilterConvention>(new FilterConventionExtension(configure =>
    {
        configure.BindRuntimeType<Instant, ComparableOperationFilterInputType<Instant>>();
        configure.BindRuntimeType<Instant?, ComparableOperationFilterInputType<Instant?>>();
        configure.BindRuntimeType<LocalDate, ComparableOperationFilterInputType<LocalDate>>();
        configure.BindRuntimeType<LocalDate?, ComparableOperationFilterInputType<LocalDate?>>();
        configure.BindRuntimeType<LocalTime, ComparableOperationFilterInputType<LocalTime>>();
        configure.BindRuntimeType<LocalTime?, ComparableOperationFilterInputType<LocalTime?>>();
    }))
    .SetPagingOptions(
        new PagingOptions
        {
            IncludeTotalCount = true
        });

var app = builder.Build();

app.UseDeveloperExceptionPage();




app.UseStaticFiles();
app.UseResponseCompression();
app.UseRouting();
app.UseCors("LoosePolicy");
// app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("api", "{controller=Home}/{action=Index}/{id?}")
        .RequireCors("LoosePolicy");
    endpoints.MapControllerRoute("api", "api/{controller}/{action}/{id?}")
        .RequireCors("LoosePolicy");
    endpoints.MapGraphQL("/graphql");
});

app.Run();

using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
	Console.WriteLine("---> Using SQL database.");
	builder.Services.AddDbContext<AppDbContext>(options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"));
	});
}
else
{
	Console.WriteLine("---> Using InMemory database.");
	builder.Services.AddDbContext<AppDbContext>(options =>
	{
		options.UseInMemoryDatabase("InMemory");
	});
}

// Add services to the container.
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, CommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();

builder.Services.AddGrpc();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
	endpoints.MapGrpcService<GrpcPlatformService>();

	//Don't need to do that for Grpc to work.
	endpoints.MapGet("/protos/platforms.proto", async context =>
	{
		await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
	});
});

app.MapControllers();

SeedDb.PreparationPopulation(app, app.Environment.IsProduction());

app.Run();

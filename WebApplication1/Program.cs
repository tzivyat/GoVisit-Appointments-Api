using MongoDB.Driver;
using WebApplication1.Services;
using WebApplication1.Repositories;

var builder = WebApplication.CreateBuilder(args);

// GCP Cloud Run port configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// Add services to the container.
builder.Services.AddControllers();

// MongoDB Configuration - Optional for demo
try
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB") ?? 
                          builder.Configuration["MongoDB:ConnectionString"] ??
                          "mongodb://localhost:27017";
    
    builder.Services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        return new MongoClient(settings);
    });

    builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        var databaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "GoVisitAppointments";
        return client.GetDatabase(databaseName);
    });
}
catch
{
    // MongoDB not available - use mock services
}

// Memory Cache לביצועים
builder.Services.AddMemoryCache();

// Register repositories and services
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IndexManagementService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "GoVisit Appointments API", 
        Version = "v1",
        Description = "מערכת זימון תורים עבור משרדי הממשלה"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoVisit Appointments API v1");
    c.RoutePrefix = "swagger";
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseCors("AllowAll");
// app.UseHttpsRedirection(); // מבוטל לפיתוח
app.UseAuthorization();
app.MapControllers();

app.Run();

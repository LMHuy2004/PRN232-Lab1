using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Repositories ─────────────────────────────────────────────────
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// ── Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// ── Controllers + JSON ───────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opt.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(opt =>
    {
        opt.InvalidModelStateResponseFactory = ctx =>
        {
            var errors = ctx.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
            var response = new Microsoft.AspNetCore.Mvc.ObjectResult(
                new { success = false, message = "Validation failed", errors })
            { StatusCode = 400 };
            return response;
        };
    });

// ── Swagger ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PRN232 - LMS API",
        Version = "v1",
        Description = "Learning Management System REST API - Lab 1",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "PRN232 Student",
            Email = "student@fpt.edu.vn"
        }
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ── CORS ─────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Auto migrate with retry (Docker: DB may not be ready yet) ────
var retries = 10;
while (retries > 0)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");
        break;
    }
    catch (Exception ex)
    {
        retries--;
        Console.WriteLine($"DB not ready, retrying ({10 - retries}/10): {ex.Message}");
        await Task.Delay(5000);
    }
}

// ── Global Exception Handler (500) ───────────────────────────────
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/json";
    var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var msg = app.Environment.IsDevelopment()
        ? feature?.Error.Message ?? "Internal Server Error"
        : "Internal Server Error";
    await ctx.Response.WriteAsJsonAsync(new { success = false, message = msg, errors = (object?)null });
}));

// ── Middleware ───────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
    c.RoutePrefix = string.Empty;   // Swagger at root "/"
});

app.UseCors();
// Bỏ UseHttpsRedirection khi chạy Docker (Production)
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

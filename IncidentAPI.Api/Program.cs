using Microsoft.EntityFrameworkCore;
using Serilog;
using IncidentAPI.Api.Data;
using IncidentAPI.Api.Middleware;
using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Repositories.Implementations;
using IncidentAPI.Api.Services.Interfaces;
using IncidentAPI.Api.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;


/* Configura Serilog como sistema de logs
    WriteTo.console -> Muestra logs en la terminal
    WriteTo.File -> guarda logs en un archivo cada dia en la carpeta logs
*/
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/incidentapi.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Reemplaza y activa el sistema de logs por defecto
builder.Host.UseSerilog();

// Activa los controllers para que la APIA reciba peticiones HTPP
builder.Services.AddControllers();

// Necesario para que Swagger lea los Endspoints automaticamente
builder.Services.AddEndpointsApiExplorer();

// Configura Swagger con el nombre y version de la API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "IncidentAPI",
        Version = "v1",
        Description = "API Restful para incidentes técnicos"
    });
});


/* Conecta entity framework core con SQLite
    Crea el archivo incidents.db si no existe
*/
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlite("Data Source=incidents.db"));

// Conecta POSTGRESQL 
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/* Registor de repositories con AddScoped
    AddScoped significa que crea una instancia nueva por cada 
    peticion HTPP
    Se registra la interfaz junto con su implementación
    Repository Pattern
*/
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();



/* Registro de Services con AddScoped
    El controller llama al service, el service llama al repository
*/
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

// ── JWT ──────────────────────────────────────────────────────────

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key no está configurada");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };

            // Leer JWT desde cookie en lugar de header authorization
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue("jwt", out var token))
                        context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });



builder.Services.AddScoped<IEmailService, EmailService>();  // 

// Habilitar CORS para que el navegador no bloquee peticiones:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // origen exacto de tu React
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ← obligatorio para cookies/credentials
    });
});


// Rate limiting para endpoints de autenticacion sensibles
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

// Activa Middleware global de errores
// Debe ir primero para atrapar cualquier error
app.UseMiddleware<ErrorHandlingMiddleware>();

// CORS Evitar errores
app.UseCors("AllowFrontend");

app.UseRateLimiter();

// Activa Swagger solo en entorno desarrollo
// En produccion no se expone a la documentación por seguridad
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige automaticamente de HTTP a Https
app.UseHttpsRedirection();

// JWT
app.UseAuthentication(); 

// Activa sistema de autorizacion (Necesario aunque no haya JWT)
app.UseAuthorization();

// .NET sirva los archivos estaticos desde wwwroot/uploads/;
app.UseStaticFiles();

// Mapea las rutas de los controllers automaticamente
// Por ejemplo: UserController /api/user
app.MapControllers();

/* Crea y aplica migraciones al iniciar la aplicacion
   Si la base de datos no existe, la crea, si tiene migraciones
   pedneintes las aplica
*/
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Inicia el servidor y empieza a escuchar peticiones HTTP
app.Run();
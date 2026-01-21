using HabitMatrix.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Add services to the container
// ---------------------------
builder.Services.AddControllersWithViews();

// Register DatabaseService (scoped)
builder.Services.AddScoped<DatabaseService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Missing connection string");
    return new DatabaseService(connStr);
});

// Register repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<HabitRepository>();
builder.Services.AddScoped<HabitLogRepository>();
builder.Services.AddScoped<AchievementRepository>();
builder.Services.AddScoped<ReminderRepository>();
builder.Services.AddScoped<HabitSuggestionRepository>();
builder.Services.AddScoped<HabitCategoryRepository>();

// ---------------------------
// Add Session
// ---------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ---------------------------
// 🔥 NEW: Add Authentication + Cookie Scheme
// ---------------------------
builder.Services.AddAuthentication("Cookies") // <-- DEFAULT scheme set here
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";   // redirect if not logged in
        options.LogoutPath = "/Account/Logout"; // redirect after logout
        options.AccessDeniedPath = "/Account/Login"; // optional: handle denied access
    });

var app = builder.Build();

// ---------------------------
// Configure middleware
// ---------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// ---------------------------
// 🔥 NEW: Authentication middleware BEFORE Authorization
// ---------------------------
app.UseAuthentication();   // <-- REQUIRED for claims/cookies
app.UseAuthorization();

// ---------------------------
// Routes
// ---------------------------

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Explicit route for login (root URL goes to login)
app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new { controller = "Account", action = "Login" });

app.Run();
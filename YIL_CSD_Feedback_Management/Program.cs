using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Implementations;
// Admin Dashboard
using YIL_CSD_Feedback_Management.Areas.Admin.Repositories.Interfaces;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Implementations;
using YIL_CSD_Feedback_Management.Areas.Admin.Services.Interfaces;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Repositories.Implementations;
// Customer Portal Repositories
using YIL_CSD_Feedback_Management.Repositories.Interfaces;

using YIL_CSD_Feedback_Management.Services.Implementations;
// Customer Portal Services
using YIL_CSD_Feedback_Management.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// ==========================================================
// DATABASE
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});

// ==========================================================
// MVC
// ==========================================================

builder.Services.AddControllersWithViews();


// ==========================================================
// SESSION
// ==========================================================

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ==========================================================
// CUSTOMER PORTAL SERVICES
// ==========================================================

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<IFeedbackQuestionRepository, FeedbackQuestionRepository>();
builder.Services.AddScoped<IFeedbackQuestionService, FeedbackQuestionService>();

builder.Services.AddScoped<IFeedbackUploadRepository, FeedbackUploadRepository>();
builder.Services.AddScoped<IFeedbackUploadService, FeedbackUploadService>();

builder.Services.AddScoped<ICustomerFeedbackRepository, CustomerFeedbackRepository>();
builder.Services.AddScoped<ICustomerFeedbackService, CustomerFeedbackService>();


// ==========================================================
// OCR SERVICES
// ==========================================================

builder.Services.AddScoped<IOCRService, OCRService>();
builder.Services.AddScoped<ITemplateDetectionService, TemplateDetectionService>();
builder.Services.AddScoped<IFormParserService, FormParserService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

builder.Services.AddScoped<IFeedbackService, FeedbackService>();

builder.Services.AddScoped<
    IFeedbackRepository,
    FeedbackRepository>();

builder.Services.AddScoped<
    IFeedbackService,
    FeedbackService>();


builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.AddScoped<IFeedbackStatusRepository, FeedbackStatusRepository>();

builder.Services.AddScoped<
    IFeedbackStatusService,
    FeedbackStatusService>();

builder.Services.AddScoped<IClosedCasesRepository,
    ClosedCasesRepository>();

builder.Services.AddScoped<IClosedCasesService,
    ClosedCasesService>();

builder.Services.AddScoped<IRegionDashboardRepository,
    RegionDashboardRepository>();

builder.Services.AddScoped<IRegionDashboardService,
    RegionDashboardService>();

builder.Services.AddScoped<IReportsRepository,
    ReportsRepository>();

builder.Services.AddScoped<IReportsService,
    ReportsService>();

builder.Services.AddScoped<IEngineerPerformanceRepository,
    EngineerPerformanceRepository>();

builder.Services.AddScoped<IEngineerPerformanceService,
    EngineerPerformanceService>();


builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IServiceFeedbackAnalyticsRepository,
    ServiceFeedbackAnalyticsRepository>();

builder.Services.AddScoped<IServiceFeedbackAnalyticsService,
    ServiceFeedbackAnalyticsService>();

builder.Services.AddScoped<IRegionRepository, RegionRepository>();

builder.Services.AddScoped<IRegionService, RegionService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.Cookie.Name = "YILFeedbackAuth";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });


builder.Services.AddScoped<ICriticalFeedbackRepository, CriticalFeedbackRepository>();

builder.Services.AddScoped<ICriticalFeedbackService, CriticalFeedbackService>();
// ==========================================================
// BUILD APPLICATION
// ==========================================================

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseSession();

app.UseAuthorization();

// ==========================================================
// HTTP REQUEST PIPELINE
// ==========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
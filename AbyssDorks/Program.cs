using AbyssDorks.BayesClassifier;
using AbyssDorks.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<JsonDataService>();
builder.Services.AddSingleton<DorkGeneratorService>();
builder.Services.AddSingleton<TagsService>();
builder.Services.AddSingleton<ValidationService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<ClassifierManager>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AbyssDorks.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dork}/{action=SetCredentials}/{id?}");

app.Run();

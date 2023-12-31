using SyslogRemoteStore.Web.Data;
using SyslogRemoteStore.Web.Services;
using SyslogRemoteStore.Web.Stores;
using SyslogRemoteStore.Web.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<RadioService>();
builder.Services.AddSingleton<LogFilterService>();
builder.Services.AddSingleton<RadioFilterService>();
builder.Services.AddScoped<ILogsViewModel, LogsViewModel>();
builder.Services.AddScoped<IRadioViewModel, RadioViewModel>();
builder.Services.AddScoped<ISettingsViewModel, SettingsViewModel>();

// Add singletons to the container

builder.Services.AddSingleton<ConfigurationStore>();
builder.Services.AddSingleton<CollectionStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


RadioService _radioService = app.Services.GetRequiredService<RadioService>();
_radioService.BeginListening();

app.Run();




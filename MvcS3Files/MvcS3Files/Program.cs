using Microsoft.EntityFrameworkCore;
using MvcS3Files.Data;
using MvcS3Files.Services;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new AmazonS3Client(
        config["AWS:AccessKey"],
        config["AWS:SecretKey"],
        RegionEndpoint.GetBySystemName(config["AWS:Region"])
    );
});


builder.Services.AddScoped<S3FileService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Files}/{action=Index}/{id?}");

app.Run();
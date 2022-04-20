using TetrisAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TetrisAPI.Models;
using Microsoft.Net.Http.Headers;
using System;

Console.WriteLine("CurrentDirectory in Main: {0}", System.IO.Directory.GetCurrentDirectory());
var builder = WebApplication.CreateBuilder(args);
string sConnection = builder.Configuration.GetConnectionString("TetrisConnection");
if(sConnection==null || string.IsNullOrEmpty(sConnection))
{
    sConnection = "Server=LAPTOP-MRJHEJRN\\SQLEXPRESS;Database=Tetris;Trusted_Connection=True";
}
Console.WriteLine("TestConnection" + sConnection);
builder.Services.AddDbContext<TetrisAPIDbContext>(opt => opt.UseSqlServer(sConnection));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tetris", Version = "v1"});
});
builder.Services.AddHttpClient("Tetris", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7009/index.html");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.UserAgent, "https://localhost:7009/index.html");
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tetris v1"));
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultControllerRoute();

app.Run();

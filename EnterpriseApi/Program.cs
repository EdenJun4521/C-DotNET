using System.Threading.Channels;
using EnterpriseApi.Data;
using EnterpriseApi.Models;
using EnterpriseApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 註冊 Controller 支援
builder.Services.AddControllers();

// 2. 註冊 AppDbContext
// 這裡使用 InMemoryDatabase 做為開發/測試之用 (命名為 EnterpriseDb)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EnterpriseDb"));

/* 
 * SQL Server 註冊範例 (未來更換資料庫時參考)
 * builder.Services.AddDbContext<AppDbContext>(options =>
 *     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
 */

// 3. 註冊 Channel<OrderRequest> 為 Singleton (生產者與消費者共享)
builder.Services.AddSingleton(Channel.CreateUnbounded<OrderRequest>());

// 4. 註冊 OrderProcessingService 為 HostedService (背景消費者程式)
builder.Services.AddHostedService<OrderProcessingService>();

var app = builder.Build();

// 標註必要的 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 5. 對應 Controllers
app.MapControllers();

app.Run();

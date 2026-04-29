using System.Threading.Channels;
using EnterpriseApi.Data;
using EnterpriseApi.Models;
using EnterpriseApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Jenkins CI/CD 測試成功！ ---
Console.WriteLine(">>> 服務已透過 Jenkins 自動化部署啟動 (目標框架: .NET 10.0) <<<");

// 1. 註冊 CORS 支援 (允許前端跨網域存取)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite 預設 Port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. 註冊 Controller 支援
builder.Services.AddControllers();

// 3. 註冊 AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EnterpriseDb"));

// 4. 註冊 Channel<OrderRequest> 為 Singleton
builder.Services.AddSingleton(Channel.CreateUnbounded<OrderRequest>());

// 5. 註冊 OrderProcessingService 為 HostedService
builder.Services.AddHostedService<OrderProcessingService>();

var app = builder.Build();

// 標註必要的 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 使用 CORS 政策 (必須在 MapControllers 之前)
app.UseCors("AllowFrontend");

// 暫時關閉 HTTPS 重定向，方便本地 HTTP 測試
// app.UseHttpsRedirection();

app.UseAuthorization();

// 6. 對應 Controllers
app.MapControllers();

app.Run();

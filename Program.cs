using Agullto_IMS.Data;       // Added: Points to your data layer
using Agullto_IMS.Services;   // Added: Points to your business logic layer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CUSTOM SERVICES REGISTERED HERE ---
builder.Services.AddSingleton<InventoryData>(); // Registers the shared RAM cache
builder.Services.AddScoped<ProductService>();     // Registers the product operations engine
// ----------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Builds the UI on https://localhost:7186/swagger/index.html
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
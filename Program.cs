using ItemsApi.Models;
using ItemsApi.Services;
using Microsoft.Azure.Cosmos;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

// Register Cosmos DB client as singleton (expensive to create, safe to reuse)
var connectionString = builder.Configuration["CosmosDb:ConnectionString"]!;
builder.Services.AddSingleton(new CosmosClient(connectionString));
builder.Services.AddSingleton<ItemService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

// GET /items
app.MapGet("/items", async (ItemService svc) =>
{
    var items = await svc.GetAllAsync();
    return Results.Ok(items);
});

// GET /items/{id}
app.MapGet("/items/{id}", async (string id, ItemService svc) =>
{
    var item = await svc.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

// POST /items
app.MapPost("/items", async (CreateItemRequest request, ItemService svc) =>
{
    var item = await svc.CreateAsync(request.Title);
    return Results.Created($"/items/{item.Id}", item);
});

// PUT /items/{id}
app.MapPut("/items/{id}", async (string id, UpdateItemRequest request, ItemService svc) =>
{
    var item = await svc.UpdateAsync(id, request.Title, request.IsComplete);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

// DELETE /items/{id}
app.MapDelete("/items/{id}", async (string id, ItemService svc) =>
{
    var deleted = await svc.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();


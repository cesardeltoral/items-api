using ItemsApi.Models;
using Scalar.AspNetCore;         

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();    
var app = builder.Build();

// In-memory list using our model
var items = new List<Item>
{
    new Item { Id = 1, Title = "Apple", IsComplete = false },
    new Item { Id = 2, Title = "Banana", IsComplete = false },
    new Item { Id = 3, Title = "Cherry", IsComplete = true },
};
var nextId = 4;

app.MapOpenApi();                  
app.MapScalarApiReference();       

// GET /items — return all items
app.MapGet("/items", async () =>
{
    await Task.Delay(100);
    return Results.Ok(items);
});

// POST /items — add a new item (body: { "title": "..." })
app.MapPost("/items", async (CreateItemRequest request) =>
{
    await Task.Delay(100);
    var item = new Item { Id = nextId++, Title = request.Title, IsComplete = false };
    items.Add(item);
    return Results.Created($"/items/{item.Id}", item);
});

// DELETE /items/{id} — delete by numeric Id
app.MapDelete("/items/{id}", async (int id) =>
{
    await Task.Delay(100);
    var item = items.FirstOrDefault(x => x.Id == id);
    if (item is null) return Results.NotFound();
    items.Remove(item);
    return Results.NoContent();
});

 // PUT /items/{id} — update title and/or isComplete
 app.MapPut("/items/{id}", async (int id, UpdateItemRequest request) =>
 {
     await Task.Delay(100);
     var item = items.FirstOrDefault(x => x.Id == id);
     if (item is null) return Results.NotFound();
     item.Title = request.Title;
     item.IsComplete = request.IsComplete;
     return Results.Ok(item);
 });

app.Run();

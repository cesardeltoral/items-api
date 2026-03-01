using Microsoft.Azure.Cosmos;
using ItemsApi.Models;

namespace ItemsApi.Services;

public class ItemService
{
    private readonly Container _container;

    public ItemService(CosmosClient client, IConfiguration config)
    {
        var dbName = config["CosmosDb:DatabaseName"]!;
        var containerName = config["CosmosDb:ContainerName"]!;
        _container = client.GetContainer(dbName, containerName);
    }

    public async Task<List<Item>> GetAllAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = _container.GetItemQueryIterator<Item>(query);
        var results = new List<Item>();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            results.AddRange(page);
        }
        return results;
    }

    public async Task<Item?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Item>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Item> CreateAsync(string title)
    {
        var item = new Item { Title = title };
        var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        return response.Resource;
    }

    public async Task<Item?> UpdateAsync(string id, string title, bool isComplete)
    {
        var existing = await GetByIdAsync(id);
        if (existing is null) return null;
        existing.Title = title;
        existing.IsComplete = isComplete;
        var response = await _container.UpsertItemAsync(existing, new PartitionKey(id));
        return response.Resource;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository : InterfaceItemsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> itemsCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(collectionName);
        }
        async Task InterfaceItemsRepository.CreateItemAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        async Task InterfaceItemsRepository.DeleteItemAsync(Guid Id)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, Id);
            await itemsCollection.DeleteOneAsync(filter);
        }

        async Task<Item> InterfaceItemsRepository.GetItemAsync(Guid Id)
        {
            var filter = filterBuilder.Eq(item => item.Id, Id);
            return await itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        async Task<IEnumerable<Item>> InterfaceItemsRepository.GetItemsAsync()
        {
            return await itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        async Task InterfaceItemsRepository.UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}
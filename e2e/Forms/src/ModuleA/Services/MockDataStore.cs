using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuleA.Models;

namespace ModuleA.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>()
            {
                new Item { Id = 1, Text = "First item", Description="This is an item description." },
                new Item { Id = 2, Text = "Second item", Description="This is an item description." },
                new Item { Id = 3, Text = "Third item", Description="This is an item description." },
                new Item { Id = 4, Text = "Fourth item", Description="This is an item description." },
                new Item { Id = 5, Text = "Fifth item", Description="This is an item description." },
                new Item { Id = 6, Text = "Sixth item", Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(int id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
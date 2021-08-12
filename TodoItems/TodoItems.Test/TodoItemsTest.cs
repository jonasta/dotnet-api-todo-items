using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TodoItems.API;
using TodoItems.Models.DTO;
using TodoItems.Models.Entities;

namespace TodoItems.Test
{
    [TestClass]
    public class TodoItemsTest
    {
        private readonly TodoItemsWebAppFactory<Startup> _factory;
        private const string API_URI = "/api/TodoItems/";

        public TodoItemsTest()
        {
            _factory = new TodoItemsWebAppFactory<Startup>();
        }

        private TodoItem CreateTodoItem()
        {
            return new TodoItem() { Name = "Dar banho no Cachorro", IsComplete = false };
        }

        [TestMethod]
        public async Task Insert()
        {
            var client = _factory.CreateClient();
            var res = await client.PostAsJsonAsync(API_URI, CreateTodoItem());
            //var resObj = await res.Content.ReadFromJsonAsync<TodoItem>();
            Assert.IsTrue(res.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task InsertFail()
        {
            var client = _factory.CreateClient();
            var res = await client.PostAsJsonAsync(API_URI, new TodoItemPostDTO { });
            Assert.IsTrue(!res.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task List()
        {
            var client = _factory.CreateClient();
            var res = await client.GetFromJsonAsync<ICollection<TodoItem>>(API_URI);
            Assert.IsTrue(res.Count > 0, $"Count equals {res.Count}");
        }

        [TestMethod]
        public async Task UpdateFail()
        {
            var client = _factory.CreateClient();
            var todo = (await client.GetFromJsonAsync<ICollection<TodoItem>>(API_URI)).Last();
            todo.Id = long.MaxValue;
            var res = await client.PutAsJsonAsync($"{API_URI}{todo.Id}", todo);
            Assert.IsTrue(!res.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task Update()
        {
            var client = _factory.CreateClient();
            var todo = (await client.GetFromJsonAsync<ICollection<TodoItem>>(API_URI)).Last();
            todo.Name = "Lavar Gato";
            var res = await client.PutAsJsonAsync($"{API_URI}{todo.Id}", todo);
            Assert.IsTrue(res.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task Delete()
        {
            var client = _factory.CreateClient();
            var todo = (await client.GetFromJsonAsync<ICollection<TodoItem>>(API_URI)).Last();
            var res = await client.DeleteAsync($"{API_URI}{todo.Id}");
            Assert.IsTrue(res.IsSuccessStatusCode);
        }
    }
}
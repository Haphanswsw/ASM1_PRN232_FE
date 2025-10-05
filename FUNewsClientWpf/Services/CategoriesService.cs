using FUNewsClientWpf.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FUNewsClientWpf.Services
{
    public sealed class CategoriesService
    {
        private readonly ApiClient _client;
        public CategoriesService(ApiClient client) => _client = client;

        public Task<List<Category>?> GetAllAsync()
            => _client.Http.GetFromJsonAsync<List<Category>>("api/categories");

        public async Task<Category?> CreateAsync(Category c)
        {
            var req = new
            {
                CategoryName = c.CategoryName!,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive ?? true
            };
            var resp = await _client.Http.PostAsJsonAsync("api/categories", req);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Category>();
        }

        public async Task<bool> UpdateAsync(short id, Category c)
        {
            var req = new
            {
                CategoryName = c.CategoryName,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive
            };
            var resp = await _client.Http.PutAsJsonAsync($"api/categories/{id}", req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var resp = await _client.Http.DeleteAsync($"api/categories/{id}");
            return resp.IsSuccessStatusCode;
        }
    }
}
using FUNewsClientWpf.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FUNewsClientWpf.Services
{
    public sealed class NewsService
    {
        private readonly ApiClient _client;
        public NewsService(ApiClient client) => _client = client;

        public Task<List<NewsArticle>?> GetAllAsync()
            => _client.Http.GetFromJsonAsync<List<NewsArticle>>("api/NewsArticles");

        public Task<List<NewsArticle>?> GetMineAsync()
            => _client.Http.GetFromJsonAsync<List<NewsArticle>>("api/NewsArticles/mine");

        public Task<List<NewsArticle>?> GetReportAsync(DateTime start, DateTime end)
            => _client.Http.GetFromJsonAsync<List<NewsArticle>>($"api/NewsArticles/report?startDate={start:o}&endDate={end:o}");

        public async Task<NewsArticle?> CreateAsync(CreateNewsArticleRequest req)
        {
            var resp = await _client.Http.PostAsJsonAsync("api/newsarticles", req);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<NewsArticle>();
        }

        public Task<NewsArticle?> GetByIdAsync(string id)
    => _client.Http.GetFromJsonAsync<NewsArticle>($"api/newsarticles/{id}");

        public async Task<bool> UpdateAsync(string id, UpdateNewsArticleRequest req)
        {
            var resp = await _client.Http.PutAsJsonAsync($"api/newsarticles/{id}", req);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var resp = await _client.Http.DeleteAsync($"api/newsarticles/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> ReplaceTagsAsync(string id, List<int> tagIds)
        {
            var resp = await _client.Http.PutAsJsonAsync($"api/newsarticles/{id}/tags", new UpdateTagsRequest { TagIds = tagIds });
            return resp.IsSuccessStatusCode;
        }
    }
}
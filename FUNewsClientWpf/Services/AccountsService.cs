using FUNewsClientWpf.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FUNewsClientWpf.Services
{
    public sealed class AccountsService
    {
        private readonly ApiClient _client;
        public AccountsService(ApiClient client) => _client = client;

        public Task<List<SystemAccount>?> GetAllAsync()
            => _client.Http.GetFromJsonAsync<List<SystemAccount>>("api/systemaccounts");

        public Task<SystemAccount?> GetByIdAsync(short id)
            => _client.Http.GetFromJsonAsync<SystemAccount>($"api/systemaccounts/{id}");

        public async Task<SystemAccount?> CreateAsync(SystemAccount a)
        {
            var resp = await _client.Http.PostAsJsonAsync("api/systemaccounts", a);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<SystemAccount>();
        }

        public async Task<bool> UpdateAsync(short id, SystemAccount a)
        {
            var resp = await _client.Http.PutAsJsonAsync($"api/systemaccounts/{id}", a);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var resp = await _client.Http.DeleteAsync($"api/systemaccounts/{id}");
            return resp.IsSuccessStatusCode;
        }
    }
}
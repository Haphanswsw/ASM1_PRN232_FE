using FUNewsClientWpf.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FUNewsClientWpf.Services
{
    public sealed class TagsService
    {
        private readonly ApiClient _client;
        public TagsService(ApiClient client) => _client = client;

        public Task<List<Tag>?> GetAllAsync()
            => _client.Http.GetFromJsonAsync<List<Tag>>("api/tags");
    }
}
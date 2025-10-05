using FUNewsClientWpf.Models;
using FUNewsClientWpf.Services;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FUNewsClientWpf.Services
{
    public sealed class AuthService
{
    private readonly ApiClient _client;
    public MeResult? CurrentUser { get; private set; }

    public AuthService(ApiClient client)
    {
        _client = client;
    }

    public async Task<LoginResult?> LoginAsync(string email, string password)
    {
        var req = new LoginRequest { Email = email, Password = password };
        var resp = await _client.Http.PostAsJsonAsync("api/Auth/login", req);
        if (!resp.IsSuccessStatusCode) return null;
        var result = await resp.Content.ReadFromJsonAsync<LoginResult>();
        CurrentUser = await MeAsync();
        return result;
    }

    public async Task<bool> LogoutAsync()
    {
        var resp = await _client.Http.PostAsync("api/Auth/logout", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<MeResult?> MeAsync()
    {
        var resp = await _client.Http.GetAsync("api/Auth/me");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<MeResult>();
    }

    public async Task<bool> UpdateMeAsync(string? name, string? password)
    {
        var payload = new { Name = name, Password = password };
        var resp = await _client.Http.PutAsJsonAsync("api/Auth/me", payload);
        return resp.IsSuccessStatusCode;
    }
}
}
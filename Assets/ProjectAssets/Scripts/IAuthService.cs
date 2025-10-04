using System.Threading.Tasks;
using System;

public interface IAuthService
{
    event Action<string> OnLoginSuccess;
    event Action<string> OnLoginFailed;
    Task<bool> LoginAsync();
    Task<string> GetPlayerNameAsync();
    Task<bool> UpdatePlayerNameAsync(string newName);
}
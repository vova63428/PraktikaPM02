using System.Threading.Tasks;
using LabDesktop.Models;

namespace LabDesktop.Services
{
    public class AuthService
    {
        private readonly ApiService _api;
        public User CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.Token);

        public AuthService(ApiService api)
        {
            _api = api;
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            try
            {
                var user = await _api.LoginAsync(login, password);
                if (user != null && !string.IsNullOrEmpty(user.Token) &&
                    (user.Role == "lab technician" || user.Role == "admin"))
                {
                    CurrentUser = user;
                    _api.SetToken(user.Token);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
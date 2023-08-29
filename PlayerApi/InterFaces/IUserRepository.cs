using PlayerApi.DTO;
using PlayerApi.Models;

namespace PlayerApi.InterFaces
{
    public interface IUserRepository
    {
        AuthResponse Authenticate(string publicToken);
        PlayerInfo GetPlayerInfo(string Token);
        object GetBalance(string userName);
    }
}

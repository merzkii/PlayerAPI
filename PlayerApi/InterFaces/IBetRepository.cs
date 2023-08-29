using PlayerApi.Models;

namespace PlayerApi.InterFaces
{
    public interface IBetRepository
    {
        BetResponse Bet(BetRequest request);
        BetResponse Win(WinRequest request);
        BetResponse CancelBet(CancelBetRequest request);    
        BetResponse ChangeWin(ChangeWinRequest request);
    }
}

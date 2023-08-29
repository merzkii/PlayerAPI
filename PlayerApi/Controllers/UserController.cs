using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayerApi.DTO;
using PlayerApi.InterFaces;

namespace PlayerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet("authorize")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Authorize(string publicToken)
        {
            try
            {
                var user =  _userRepository.Authenticate(publicToken);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("player-info")]
        public IActionResult GetPlayerInfo(string privateToken)
        {
            try
            {
                var playerInfo = _userRepository.GetPlayerInfo(privateToken);

                if (playerInfo == null)
                {
                    return NotFound("Player not found");
                }

                return Ok(playerInfo);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("balance")]
        public IActionResult GetBalance(string token)
        {
            try
            {
                var balance = _userRepository.GetBalance(token);

                if (balance == null)
                {
                    return NotFound("Balance not found");
                }

                return Ok(balance);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred");
            }
        }
    }
}

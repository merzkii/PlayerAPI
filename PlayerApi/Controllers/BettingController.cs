using Microsoft.AspNetCore.Mvc;
using PlayerApi.InterFaces;
using PlayerApi.Models;

namespace PlayerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController : ControllerBase
    {
        private IBetRepository BetRepository;
        public BettingController(IBetRepository betRepository)
        {
            BetRepository = betRepository;
        }


        [HttpPost("Bet")]
        public IActionResult Bet([FromBody] BetRequest request)
        {
            try
            {
                BetResponse response = BetRepository.Bet(request);


                switch (response.StatusCode)
                {
                    case 200:

                        return Ok(response);

                    case 201:

                        return StatusCode(201, response);

                    case 401:
                    case 402:
                    case 403:
                    case 404:
                    case 409:

                        return StatusCode(response.StatusCode, response);

                    default:

                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new BetResponse { StatusCode = 500 });
            }
        }


        [HttpPost("Win")]
        public IActionResult Win([FromBody] WinRequest request)
        {

            try
            {
                BetResponse response = BetRepository.Win(request);



                switch (response.StatusCode)
                {
                    case 200:

                        return Ok(response);

                    case 201:

                        return StatusCode(201, response);


                    case 402:
                    case 403:
                    case 404:


                        return StatusCode(response.StatusCode, response);

                    default:

                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new BetResponse { StatusCode = 500 });
            }

        }

        [HttpPost("CancelBet")]
        public IActionResult CancelBet(CancelBetRequest request)
        {
            try
            {
                BetResponse response = BetRepository.CancelBet(request);
                switch (response.StatusCode)
                {
                    case 200:

                        return Ok(response);

                    case 201:

                        return StatusCode(201, response);


                    case 402:
                    case 403:
                    case 404:


                        return StatusCode(response.StatusCode, response);

                    default:

                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new BetResponse { StatusCode = 500 });
            }
        }
        [HttpPost("ChangeWin")]
        public IActionResult ChangeWin(ChangeWinRequest request)
        {
            
            try
            {
                BetResponse response = BetRepository.ChangeWin(request);
                switch (response.StatusCode)
                {
                    case 200:

                        return Ok(response);

                    case 201:

                        return StatusCode(201, response);


                    case 402:
                    case 403:
                    case 404:


                        return StatusCode(response.StatusCode, response);

                    default:

                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new BetResponse { StatusCode = 500 });
            }

        }

    }
 }          




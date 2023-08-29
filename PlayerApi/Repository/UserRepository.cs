using PlayerApi.Models;
using System.Data.SqlClient;
using Dapper;
using PlayerApi.InterFaces;
using System.Data;

namespace PlayerApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
        }

        public AuthResponse Authenticate(string publicToken)
        {

            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.Add("PublicToken", publicToken);
            parameters.Add("PrivateToken", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            connection.Execute("GetPrivateTokenAndDeactivatePublicToken", param: parameters, commandType: CommandType.StoredProcedure);
            string privateToken = parameters.Get<string>("PrivateToken");
            int returnValue = parameters.Get<int>("ReturnValue");
            connection.Close();
            switch (returnValue)
            {
                case 404:
                    return new AuthResponse { StatusCode = 404 };
                    break;
                case 401:
                    return new AuthResponse { StatusCode = 401 };
                    break;
                case 200:
                    return new AuthResponse { StatusCode = 200, Data = new AuthData { PrivateToken = privateToken } }; ;
                    break;
                default:
                    return new AuthResponse { StatusCode = 500 };
                    break;
                    
            }


        }









        public PlayerInfo GetPlayerInfo(string privateToken)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.Add("PrivateToken", privateToken);
            parameters.Add("Id", dbType: DbType.String, size: 36, direction: ParameterDirection.Output);
            parameters.Add("UserName", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("Email", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("Balance", dbType: DbType.Decimal, precision: 18, scale: 2, direction: ParameterDirection.Output);
            parameters.Add("StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("GetPlayerInfo", param: parameters, commandType: CommandType.StoredProcedure);

            int statusCode = parameters.Get<int>("StatusCode");
            connection.Close();
            switch (statusCode)
            {
                case 200:
                    var playerInfo = new User
                    {
                        Id = parameters.Get<string>("Id"),
                        UserName = parameters.Get<string>("UserName"),
                        Email = parameters.Get<string>("Email"),
                        Balance = parameters.Get<decimal>("Balance"),  
                        FirstName = "anything",
                        LastName = "anything",
                        CountryCode = "anything",
                        CountryName = "anything",
                        Gender = 3,
                        Currency = "gel"
                    };
                    return new PlayerInfo { StatusCode = statusCode, Data = playerInfo };
                case 401:
                    return new PlayerInfo { StatusCode = 401 };
                case 402:
                    return new PlayerInfo { StatusCode = 402 };
                default:
                    return new PlayerInfo { StatusCode = statusCode };
                    break;
            }

        }

        public object GetBalance(string privateToken)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var parameters = new DynamicParameters();
            parameters.Add("PrivateToken", privateToken);
            parameters.Add("Balance", dbType: DbType.Decimal, precision: 18, scale: 2, direction: ParameterDirection.Output);
            parameters.Add("StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("GetBalanceByPublicToken", param: parameters, commandType: CommandType.StoredProcedure);

            int statusCode = parameters.Get<int>("StatusCode");
            decimal balance = parameters.Get<decimal>("Balance");
            connection.Close();

            switch (statusCode)
            {
                case 200:
                    var response = new
                    {
                        StatusCode = statusCode,
                        Data = new
                        {
                            CurrentBalance = balance
                        }
                    };
                    return response;
                case 401:
                    return new PlayerInfo { StatusCode = 401 };
                case 402:
                    return new PlayerInfo { StatusCode = 402 };
                default:
                    return new { StatusCode = statusCode };
                    break;
            }
        }


    }
}

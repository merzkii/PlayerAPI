using Dapper;
using PlayerApi.InterFaces;
using PlayerApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PlayerApi.Repository
{
    public class BetRepository : IBetRepository
    {
        private readonly IConfiguration _config;

        public BetRepository(IConfiguration config)
        {
            _config = config;
        }

        public BetResponse Bet(BetRequest request)
        {
            string rawHash = $"{request.Amount}|{request.BetTypeId}|{request.CampaignId}|{request.CampaignName}|{request.Currency}|{request.GameId}|{request.ProductId}|{request.RoundId}|{request.Token}|{request.TransactionId}";
            string hash = GetSha256(rawHash);
            BetResponse response = new BetResponse();
            bool isTransactionIdDuplicated = IsDuplicateTransaction(request.TransactionId);

            if (isTransactionIdDuplicated)
            {
                return new BetResponse { StatusCode = 408 }; 
            }

            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                
                using (SqlCommand command = new SqlCommand("Bet", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    
                    command.Parameters.AddWithValue("@Token", request.Token);
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@TransactionId", request.TransactionId);
                    command.Parameters.AddWithValue("@BetTypeId", request.BetTypeId);
                    command.Parameters.AddWithValue("@GameId", request.GameId);
                    command.Parameters.AddWithValue("@ProductId", request.ProductId);
                    command.Parameters.AddWithValue("@RoundId", request.RoundId);
                    command.Parameters.AddWithValue("@Hash", hash);
                    command.Parameters.AddWithValue("@Currency", request.Currency);
                    command.Parameters.AddWithValue("@CampaignId", request.CampaignId);
                    command.Parameters.AddWithValue("@CampaignName", request.CampaignName);


                   
                    SqlParameter transactionIdOutputParam = new SqlParameter("@TransactionIdOutput", SqlDbType.NVarChar, 100);
                    transactionIdOutputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(transactionIdOutputParam);

                    SqlParameter currentBalanceParam = new SqlParameter("@CurrentBalance", SqlDbType.Decimal);
                    currentBalanceParam.Precision = 18;
                    currentBalanceParam.Scale = 2;
                    currentBalanceParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(currentBalanceParam);

                    SqlParameter statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int);
                    statusCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(statusCodeParam);

                   
                    command.ExecuteNonQuery();
                    response.Data = new BetData();

                   
                    response.StatusCode = (int)statusCodeParam.Value;
                    response.Data.TransactionId = (string)command.Parameters["@TransactionIdOutput"].Value;
                    response.Data.CurrentBalance = (decimal)currentBalanceParam.Value;
                }

                connection.Close();
            }

            
            switch (response.StatusCode)
            {
                case 201:
                    return new BetResponse { StatusCode = 201 };
                case 401:
                    
                    return new BetResponse { StatusCode = 401 };
                case 403:
                    return new BetResponse { StatusCode = 403 };
                case 404:
                    return new BetResponse { StatusCode =404 };
                case 402:
                    return new BetResponse { StatusCode = 402 };
                case 409:
                    return new BetResponse { StatusCode = 409 };
                case 200:
                    return new BetResponse { StatusCode =200, Data = new BetData { TransactionId = response.Data.TransactionId, CurrentBalance = response.Data.CurrentBalance } };
                default:
                    return new BetResponse { StatusCode = 500 };
            }

            return response;
        }

        public BetResponse Win(WinRequest request)
        {
            BetResponse response = new BetResponse();


            string rawHash = $"{request.Amount}|{request.CampaignId}|{request.WinTypeId}|{request.CampaignName}|{request.Currency}|{request.GameId}|{request.ProductId}|{request.RoundId}|{request.Token}|{request.TransactionId}";
            string hash = GetSha256(rawHash);
            bool isTransactionIdDuplicated = IsDuplicateTransaction(request.TransactionId);

            if (isTransactionIdDuplicated)
            {
                return new BetResponse { StatusCode = 408 };
            }

            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Win", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Token", request.Token);
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@TransactionId", request.TransactionId);
                    command.Parameters.AddWithValue("@WinTypeId", request.WinTypeId);
                    command.Parameters.AddWithValue("@GameId", request.GameId);
                    command.Parameters.AddWithValue("@ProductId", request.ProductId);
                    command.Parameters.AddWithValue("@RoundId", request.RoundId);
                    command.Parameters.AddWithValue("@Hash", hash);
                    command.Parameters.AddWithValue("@Currency", request.Currency);
                    command.Parameters.AddWithValue("@CampaignId", request.CampaignId);
                    command.Parameters.AddWithValue("@CampaignName", request.CampaignName);

                    SqlParameter transactionIdOutputParam = new SqlParameter("@TransactionIdOutput", SqlDbType.NVarChar, 100);
                    transactionIdOutputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(transactionIdOutputParam);

                    SqlParameter currentBalanceParam = new SqlParameter("@CurrentBalance", SqlDbType.Decimal);
                    currentBalanceParam.Precision = 18;
                    currentBalanceParam.Scale = 2;
                    currentBalanceParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(currentBalanceParam);

                    SqlParameter statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int);
                    statusCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(statusCodeParam);

                    command.ExecuteNonQuery();

                    response.StatusCode = (int)statusCodeParam.Value;
                    response.Data = new BetData
                    {
                        TransactionId = (string)command.Parameters["@TransactionIdOutput"].Value,
                        CurrentBalance = (decimal)currentBalanceParam.Value
                    };
                }

                connection.Close();
            }

            switch (response.StatusCode)
            {
                case 200:
                    return new BetResponse { StatusCode = 200, Data = new BetData { TransactionId = response.Data.TransactionId, CurrentBalance = response.Data.CurrentBalance } };
                case 201:
                    return new BetResponse { StatusCode = 201 };
                
                case 403:
                    return new BetResponse { StatusCode = 403 };
                case 404:
                    return new BetResponse { StatusCode = 404 };
                case 401:
                    return new BetResponse { StatusCode = 401 };
                default:
                    return new BetResponse { StatusCode = 500 };
            }
        }

        public BetResponse CancelBet(CancelBetRequest request)
        {
            BetResponse response = new BetResponse();


            string rawHash = $"{request.Amount}|{request.BetTransactionId}|{request.BetTypeId}|{request.Currency}|{request.GameId}|{request.ProductId}|{request.RoundId}|{request.Token}|{request.TransactionId}";
            string hash = GetSha256(rawHash);
            bool isTransactionIdDuplicated = IsDuplicateTransaction(request.TransactionId);

            if (isTransactionIdDuplicated)
            {
                return new BetResponse { StatusCode = 408 };
            }

            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("CancelBet", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                  
                    command.Parameters.AddWithValue("@Token", request.Token);
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@TransactionId", request.TransactionId);
                    command.Parameters.AddWithValue("@BetTypeId", request.BetTypeId);
                    command.Parameters.AddWithValue("@GameId", request.GameId);
                    command.Parameters.AddWithValue("@ProductId", request.ProductId);
                    command.Parameters.AddWithValue("@RoundId", request.RoundId);
                    command.Parameters.AddWithValue("@Hash", hash); // Use the calculated hash
                    command.Parameters.AddWithValue("@Currency", request.Currency);
                    command.Parameters.AddWithValue("@BetTransactionId", request.BetTransactionId);

                  
                    SqlParameter transactionIdOutputParam = new SqlParameter("@TransactionIdOutput", SqlDbType.NVarChar, 100);
                    transactionIdOutputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(transactionIdOutputParam);

                    SqlParameter currentBalanceParam = new SqlParameter("@CurrentBalance", SqlDbType.Decimal);
                    currentBalanceParam.Precision = 18;
                    currentBalanceParam.Scale = 2;
                    currentBalanceParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(currentBalanceParam);

                    SqlParameter statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int);
                    statusCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(statusCodeParam);

                
                    command.ExecuteNonQuery();

                 
                    response.StatusCode = (int)statusCodeParam.Value;
                    response.Data = new BetData();

                    response.Data.TransactionId = (string)command.Parameters["@TransactionIdOutput"].Value;
                    response.Data.CurrentBalance = (decimal)currentBalanceParam.Value;
                    
                }

                connection.Close();
            }

        
            switch (response.StatusCode)
            {
                case 200:
                    return new BetResponse { StatusCode=200,Data = new BetData {TransactionId=response.Data.TransactionId,CurrentBalance=response.Data.CurrentBalance} };
                    break;
                case 201:
                    return new BetResponse { StatusCode = 201 };
                    break;
                case 401:
                    return new BetResponse { StatusCode = 401 };
                    break;
                case 403:
                    return new BetResponse { StatusCode = 403 };
                    break;
                case 404:
                    return new BetResponse { StatusCode = 404 };
                case 409:
                    return new BetResponse { StatusCode = 409 };
                    break;
                default:
                    return new BetResponse { StatusCode = 500 };
                    break;
            }

            
        }

        public BetResponse ChangeWin(ChangeWinRequest request)
        {
            BetResponse response = new BetResponse();

            
            string rawHash = $"{request.Amount}|{request.ChangeWinTypeId}|{request.Currency}|{request.GameId}|{request.PreviousAmount}|{request.PreviousTransactionId}|{request.ProductId}|{request.RoundId}|{request.Token}|{request.TransactionId}";
            string hash = GetSha256(rawHash);
            bool isTransactionIdDuplicated = IsDuplicateTransaction(request.TransactionId);

            if (isTransactionIdDuplicated)
            {
                return new BetResponse { StatusCode = 408 };
            }

            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("ChangeWin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    
                    command.Parameters.AddWithValue("@Token", request.Token);
                    command.Parameters.AddWithValue("@Amount", request.Amount);
                    command.Parameters.AddWithValue("@PreviousAmount", request.PreviousAmount);
                    command.Parameters.AddWithValue("@TransactionId", request.TransactionId);
                    command.Parameters.AddWithValue("@PreviousTransactionId", request.PreviousTransactionId);
                    command.Parameters.AddWithValue("@ChangeWinTypeId", request.ChangeWinTypeId);
                    command.Parameters.AddWithValue("@GameId", request.GameId);
                    command.Parameters.AddWithValue("@ProductId", request.ProductId);
                    command.Parameters.AddWithValue("@RoundId", request.RoundId);
                    command.Parameters.AddWithValue("@Hash", hash); 
                    command.Parameters.AddWithValue("@Currency", request.Currency);

                   
                    SqlParameter transactionIdOutputParam = new SqlParameter("@TransactionIdOutput", SqlDbType.NVarChar, 100);
                    transactionIdOutputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(transactionIdOutputParam);

                    SqlParameter currentBalanceParam = new SqlParameter("@CurrentBalance", SqlDbType.Decimal);
                    currentBalanceParam.Precision = 18;
                    currentBalanceParam.Scale = 2;
                    currentBalanceParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(currentBalanceParam);

                    SqlParameter statusCodeParam = new SqlParameter("@StatusCode", SqlDbType.Int);
                    statusCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(statusCodeParam);

                    
                    command.ExecuteNonQuery();

                   
                    response.StatusCode = (int)statusCodeParam.Value;
                    response.Data = new BetData
                    {
                        TransactionId = (string)command.Parameters["@TransactionIdOutput"].Value,
                        CurrentBalance = (decimal)currentBalanceParam.Value
                    };
                }

                connection.Close();
                switch (response.StatusCode)
                {
                    case 200:
                        return new BetResponse { StatusCode = 200, Data = new BetData { TransactionId = response.Data.TransactionId, CurrentBalance = response.Data.CurrentBalance } };
                    case 201:
                        return new BetResponse { StatusCode = 201 };
                    case 401:
                        return new BetResponse { StatusCode = 401 };
                    case 402:
                        return new BetResponse { StatusCode = 402 };
                    case 403:
                        return new BetResponse { StatusCode = 403 };
                    case 404:
                        return new BetResponse { StatusCode = 404 };
                    case 409:
                        return new BetResponse { StatusCode = 409 };
                    default:
                        return new BetResponse { StatusCode = 500 };
                }
            }

            

            
        }

        private string GetSha256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private bool IsDuplicateTransaction(string transactionId)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT 1 FROM TransactionHistory WHERE RemoteTransactionId = @TransactionId", connection))
                {
                    command.Parameters.AddWithValue("@TransactionId", transactionId);
                    return command.ExecuteScalar() != null;
                }
            }
        }


    }
}


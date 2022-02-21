using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;
using Microsoft.Solutions.NFT;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService
{
    public class TokenManager
    {
        private TokenNumberManager.TokenNumberManager tokenNumberManager;
        private TransactionIndexer.TransactionIndexer tokenTransactionIndexer;
        private TokenServiceWrapper tokenServiceWrapper;
        private string ServiceEndpointUrl;
        private HttpClient httpClient;

        public TokenManager()
        {
            throw new MongoConfigurationException("Should Pass Connectionstring with constructor parameter");
        }

        public TokenManager(HttpClient HttpClient, IConfiguration Configuration, CosmosConnectionStrings cosmosConnectionStrings)
        {
            tokenTransactionIndexer = new TransactionIndexer.TransactionIndexer(cosmosConnectionStrings.PrimaryReadWriteKey, "TransactionIndexer");
            tokenNumberManager = new TokenNumberManager.TokenNumberManager(cosmosConnectionStrings.PrimaryReadWriteKey, "TradableDigitalGoods");
            tokenServiceWrapper = new TokenServiceWrapper(HttpClient, Configuration["Values:NFTServiceEndpoint"]);
            ServiceEndpointUrl = Configuration["Values:NFTServiceEndpoint"];
            httpClient = HttpClient;
        }


        /// <summary>
        /// Mint Token to Recipient
        /// </summary>
        /// <param name="TokenID">Created(Instanced) Token ID</param>
        /// <param name="TokenMinter">Who caller</param>
        /// <param name="SenderID">Token</param>
        /// <param name="RecipientID"></param>
        /// <param name="tokenInfo"></param>
        /// <returns></returns>
        public async Task<DigitalGoodToken> MintToken(string TokenID, string TokenMinter, string RecipientID, DigitalGoodToken tokenInfo)
        {
            //Get Next Token Number
            long tokenNumber = tokenNumberManager.GetNextNumber(TokenID);
            //Convert to HEX string and write to TokenNumber
            tokenInfo.TokenNumber = tokenNumber.ToString("X16");

            //Mint Token 
            var response = await tokenServiceWrapper.MintToken(
                TokenID,
                TokenMinter,
                RecipientID,
                Newtonsoft.Json.JsonConvert.SerializeObject(tokenInfo),
                tokenNumber
                );


            var tokenMeta = await tokenServiceWrapper.GetTokenMetaData(TokenID, RecipientID, tokenNumber);

            //log Transaction
            await tokenTransactionIndexer.AddTokenTransactionInformation(new TokenTransactionIndexer.Models.TokenTransaction()
            {
                Type = TokenTransactionIndexer.Models.TransactionType.MintingToken,
                TokenInstanceId = TokenID,
                Caller = TokenMinter,
                Sender = TokenMinter,
                Recipient = RecipientID,
                TransactionTime = DateTime.Now,
                SerializedTokenData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenMeta),
                TransactionInfo = null
            });



            return tokenInfo;
        }


        public async Task<bool> TransferCryptoGoods(string TokenID, long TokenNumber, string CallerID, string Sender, string Recipient)
        {
            //Transfer Token
            var response = await tokenServiceWrapper.TranferToken(TokenID, TokenNumber, CallerID, Sender, Recipient);


            //Get TokenInfo from Recipient for logging purpose
            //Token owner already changed by previous TransferToken invokation.
            //So we can get tokenMata information from Recipient's token
            var tokenMeta = await tokenServiceWrapper.GetTokenMetaData(TokenID, Recipient, TokenNumber);

            //log Transaction
            return await tokenTransactionIndexer.AddTokenTransactionInformation(new TokenTransactionIndexer.Models.TokenTransaction()
            {
                Type = TokenTransactionIndexer.Models.TransactionType.TransferToken,
                TokenInstanceId = TokenID,
                Caller = CallerID,
                Sender = Sender,
                Recipient = Recipient,
                TransactionTime = DateTime.Now,
                SerializedTokenData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenMeta),
                TransactionInfo = null
            });


        }

        public async Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID)
        {
            //Create Token
            var response = await tokenServiceWrapper.CreateToken(
                TokenName,
                TokenSymbol,
                CallerID
                );


            var tokenMeta = new TokenMeta()
            {
                TokenTemplateID = "nfMBRGT",
                TokenID = response.ContractAddress,
                TokenName = TokenName,
                TokenSymbol = TokenSymbol,
                TokenCreator = CallerID,
                TokenCreatedDate = DateTime.Now
            };

            //log Transaction
            await tokenTransactionIndexer.AddTokenTransactionInformation(new TokenTransactionIndexer.Models.TokenTransaction()
            {
                Type = TokenTransactionIndexer.Models.TransactionType.TokenCreation,
                TokenInstanceId = tokenMeta.TokenID,
                Caller = CallerID,
                Sender = "",
                Recipient = "",
                TransactionTime = DateTime.Now,
                SerializedTokenData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenMeta),
                TransactionInfo = null
            });

            return tokenMeta;
        }

        public async Task<string> GetCryptoGoodInfoFromToken(string TokenId, string CallerID, long TokenNumber)
        {
            var result = await tokenServiceWrapper.GetTokenMetaData(TokenId, CallerID, TokenNumber);
            return result;
        }


        public async Task<string> GetTokenMetaData(string TokenId, string Caller, long TokenNumber)
        {
            return await tokenServiceWrapper.GetTokenMetaData(TokenId, Caller, TokenNumber);
        }

        public async Task<TokenMeta> GetTokenInfo(string TokenId, string CallerId)
        {
            var txInfo = tokenTransactionIndexer.GetTokenMeta(TokenId);

            if (txInfo != null && txInfo.SerializedTokenData != "")
            {
                var tokenMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenMeta>(txInfo.SerializedTokenData);
                return tokenMeta;

            }
            else
            {
                var tokenName = await tokenServiceWrapper.GetName(TokenId, CallerId);
                var tokenSymbol = await tokenServiceWrapper.GetSymbol(TokenId, CallerId);

                return new TokenMeta()
                {
                    TokenID = TokenId,
                    TokenName = tokenName,
                    TokenSymbol = tokenSymbol,
                    TokenCreator = "" 
                };
            }

        }

        public async Task<bool> IsItMyToken(string tokenID, string callerID, long? mintedTokenNum)
        {
            var addressofOwner =
                await tokenServiceWrapper.WhoisOwner(tokenID, callerID, mintedTokenNum);

            //UsersClient usersClient = new UsersClient(ServiceEndpointUrl);
            ServiceClient usersClient = new (ServiceEndpointUrl, httpClient);
            //var user = await usersClient.UserPostAsync(Guid.Parse(callerID));
            var user = await usersClient.GetUserByIdAsync(Guid.Parse(callerID));

            return (user.PublicAddress.ToLower() ==
                addressofOwner.ToLower());
        }
    }
}

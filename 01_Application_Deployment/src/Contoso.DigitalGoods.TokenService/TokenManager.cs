using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Contoso.DigitalGoods.TokenService.TokenNumberManager;
using Contoso.DigitalGoods.TokenService.TransactionIndexer;
using Microsoft.Azure.TokenService.Management;
using Microsoft.Azure.TokenService.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.TokenService
{
    public class TokenManager
    {
        private TokenNumberManager.TokenNumberManager tokenNumberManager;
        private TransactionIndexer.TransactionIndexer tokenTransactionIndexer;
        private TokenServiceWrapper tokenServiceWrapper;
        private string groupName;

        public TokenManager()
        {
            throw new MongoConfigurationException("Should Pass Connectionstring with constructor parameter");
        }

        public TokenManager(string DBConnectionString, string GroupName)
        {
            InitializeConnections(DBConnectionString);
            this.groupName = GroupName;
        }

        private void InitializeConnections(string DBConnectionString)
        {
            tokenTransactionIndexer = new TransactionIndexer.TransactionIndexer(DBConnectionString, "TransactionIndexer");
            tokenNumberManager = new TokenNumberManager.TokenNumberManager(DBConnectionString, "TokenNumberManager");
            tokenServiceWrapper = new TokenServiceWrapper((new TokenAPIService()).Initialize());
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
        public async Task<CryptoKickToken> MintToken(string TokenID, string TokenMinter, string RecipientID, CryptoKickToken tokenInfo)
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
                tokenNumber,
                this.groupName
                );

            //Waiting for a while Transaction completed.
            //Should be removed once we can support by ABT.
            Libs.WaitProcess.HoldsOnSeconds(8);

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
                SerializedTokenData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenMeta.Output),
                TransactionInfo = null
            });



            return tokenInfo;
        }


        public async Task<bool> TransferCryptoGoods(string TokenID, long TokenNumber, string CallerID, string Sender, string Recipient)
        {
            //Transfer Token
            var response = await tokenServiceWrapper.TranferToken(TokenID, TokenNumber, CallerID, Sender, Recipient, this.groupName);

            //Waiting for a while Transaction completed.
            //Should be removed once we can support by ABT.
            Libs.WaitProcess.HoldsOnSeconds(8);

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
                SerializedTokenData = Newtonsoft.Json.JsonConvert.SerializeObject(tokenMeta.Output),
                TransactionInfo = null
            });


        }

        public async Task<TokenMeta> CreateToken(string TokenName, string TokenSymbol, string CallerID)
        {
            //Create Token
            var response = await tokenServiceWrapper.CreateToken(
                TokenName,
                TokenSymbol,
                CallerID,
                this.groupName
                );

            //Waiting for a while Transaction completed.
            //Should be removed once we can support by ABT.
            Libs.WaitProcess.HoldsOnSeconds(8);

            var tokenMeta = new TokenMeta()
            {
                TokenTemplateID = "nfMBRGT",
                TokenID = response.Id.Replace("tokens.", ""),
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
            var result = await tokenServiceWrapper.GetTokenMetaData(TokenId, CallerID, TokenNumber, this.groupName);
            return result.Output;
        }


        public async Task<TokenURINFMBRGResponse> GetTokenMetaData(string TokenId, string Caller, long TokenNumber)
        {
            return await tokenServiceWrapper.GetTokenMetaData(TokenId, Caller, TokenNumber, this.groupName);
        }

        public async Task<TokenMeta> GetTokenInfo(string TokenId, string CallerId)
        {
            var txInfo = tokenTransactionIndexer.GetTokenMeta(TokenId);

            if (txInfo.SerializedTokenData != null && txInfo.SerializedTokenData != "")
            {
                var tokenMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenMeta>(txInfo.SerializedTokenData);
                return tokenMeta;

            }
            else
            {
                var tokenName = await tokenServiceWrapper.GetName(TokenId, CallerId, this.groupName);
                var tokenSymbol = await tokenServiceWrapper.GetSymbol(TokenId, CallerId, this.groupName);

                return new TokenMeta()
                {
                    TokenID = TokenId,
                    TokenName = tokenName,
                    TokenSymbol = tokenSymbol,
                    TokenCreator = txInfo.Caller,
                    TokenCreatedDate = txInfo.TransactionTime
                };
            }

        }

        public async Task<bool> IsItMyToken(string tokenID, string callerID, long? mintedTokenNum)
        {
            var addressofOwner =
                await tokenServiceWrapper.IsItMyToken(tokenID, callerID, mintedTokenNum, this.groupName);

            AccountResource account = new AccountResource();
            account.GroupName = this.groupName;
            var user = await account.GetAsync(callerID);

            return (user.value[0].properties.publicAddress.ToLower() ==
                addressofOwner.Output.ToLower());
        }
    }
}

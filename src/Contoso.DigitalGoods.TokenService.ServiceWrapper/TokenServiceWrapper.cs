using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Solutions.NFT;

namespace Contoso.DigitalGoods.TokenService.ServiceWrapper
{
    public class TokenServiceWrapper 
    {
        private ServiceClient tokenServiceAPI;

        public TokenServiceWrapper(HttpClient HttpClient, string ServiceEndpoint)
        {
            tokenServiceAPI = new ServiceClient(ServiceEndpoint, HttpClient);
        }

        /// <summary>
        /// Create Token by Non Fungible Burnable, Delegateable, Transferable token template
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="tokenSymbol"></param>
        /// <param name="CallerID"></param>
        /// <returns></returns>
        public async Task<TransactionReciept> CreateToken(string tokenName, string tokenSymbol, string CallerID)
        {
            return await tokenServiceAPI.DeployNewTokenAsync(CallerID, tokenName, tokenSymbol);
         
        }

        #region "ERC 20 Compatibles"

        /// <summary>
        /// Check User's Token Balance
        /// </summary>
        /// <param name="TokenID"></param>
        /// <param name="CallerID"></param>
        /// <returns></returns>
        public async Task<long> GetBalance(string TokenID, string CallerID)
        {
            return await tokenServiceAPI.BalanceOfAsync(TokenID, CallerID);
        }


        public async Task<string> WhoisOwner(string TokenID, string CallerID, long? TokenSequence)
        {
            return await tokenServiceAPI.OwnerOfAsync(TokenID, CallerID, TokenSequence);
        }

        /// <summary>
        /// Returns Token's Name
        /// </summary>
        /// <param name="TokenID"></param>
        /// <param name="CallerID"></param>
        /// <returns></returns>
        public async Task<string> GetName(string TokenID, string CallerID)
        {
            return await tokenServiceAPI.NameAsync(TokenID, CallerID);
        }

        /// <summary>
        /// Returns Token's Symbol
        /// </summary>
        /// <param name="TokenID"></param>
        /// <param name="CallerID"></param>
        /// <returns></returns>
        public async Task<string> GetSymbol(string TokenID, string CallerID)
        {
            return await tokenServiceAPI.SymbolAsync(TokenID, CallerID);
        }

        
        #endregion

        /// <summary>
        /// Mint token with Datadata and TokenNumber
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="tokenCreator"></param>
        /// <param name="mintee"></param>
        /// <param name="metaDataString"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public async Task<TransactionReciept> MintToken(string tokenID, string tokenCreator, string mintee, string metaDataString, long? sequence)
        {
            Debug.Assert(tokenServiceAPI != null, "Token Service API should be assigned before invoke it");

            //AccountResource ar = new AccountResource();
            //ar.GroupName = tokenCreatorGroupName;
            //var response = await ar.GetAsync(mintee);

        
            return await tokenServiceAPI.MintTokenAsync(tokenID, tokenCreator, mintee, sequence, metaDataString);
        }

        /// <summary>
        /// Query Meta which was stored in minted token
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="caller"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public async Task<string> GetTokenMetaData(string tokenID, string caller, long? sequence)
        {
            try
            {
                return await tokenServiceAPI.TokenURIAsync(tokenID, caller, sequence);
            }
            catch (Exception)
            {

                return "";
            }
        }

    
             /// <summary>
        /// Transfer Token from User A to User B
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="mintedTokenNumber"></param>
        /// <param name="callerID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<TransactionReciept> TranferToken(string tokenID, long? mintedTokenNumber, string callerID, string from, string to)
        {
            return await tokenServiceAPI.TransferAsync(tokenID, from, to, mintedTokenNumber);
        }

        /// <summary>
        /// Burn minted Token
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="callerID"></param>
        /// <param name="mintedTokenNum"></param>
        /// <returns></returns>
        public async Task<TransactionReciept> DeleteToken(string tokenID, string callerID, long? mintedTokenNum)
        {
            return await tokenServiceAPI.BurnAsync(tokenID, callerID, mintedTokenNum);
        }

    }
}

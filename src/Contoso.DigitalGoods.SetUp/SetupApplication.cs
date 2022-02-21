using Contoso.DigitalGoods.DigitalLocker.Service;
using Contoso.DigitalGoods.TokenService;
using Contoso.DigitalGoods.TokenService.Models;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Solutions.CosmosDB.Security.ManagedIdentity;
using Microsoft.Solutions.NFT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.SetUp
{
    internal class SetupApplication : IHostedService
    {
        readonly DigitalGoodsSetUp setup;

        public SetupApplication(DigitalGoodsSetUp AppSetup)
        {
            setup = AppSetup;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await SetUpEnvironment(setup);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task SetUpEnvironment(DigitalGoodsSetUp setup)
        {
            Console.WriteLine("\nStart to registering..........");

            Console.WriteLine("\nRegistering group.....");
            var party = await setup.SetupParty();
            Console.WriteLine(JsonConvert.SerializeObject(party));

            Console.WriteLine("\nRegistering blockchian network.....");
            var bcNetwork = await setup.SetupBlockChainNetwork();
            Console.WriteLine(JsonConvert.SerializeObject(bcNetwork));

            Console.WriteLine("\nRegistering ProductManager account.....");
            var productManager_account = await setup.RegisterUserInNFTService(party.Id.ToString(), bcNetwork.Id.ToString(), "Contoso ProductManager");
            Console.WriteLine(productManager_account);

            Console.WriteLine("\nRegistering Contoto account.......");
            var contoso_account = await setup.ProvisioningContosoProfile("Contoso", party.Id.ToString(), bcNetwork.Id.ToString());
            Console.WriteLine(contoso_account);

            Console.WriteLine("\nInitialize Token");
            var token = await setup.InitializeToken(productManager_account);
            Console.WriteLine(JsonConvert.SerializeObject(token));

            Console.WriteLine("Registering process done. Update your configuration in Application and Token Services\n\n\n");

            Console.WriteLine("===================   Configuration Information  =====================");
            Console.WriteLine($"TokenID : {token.TokenID}");
            Console.WriteLine($"ContosoProductManager ID : {productManager_account}");
            Console.WriteLine($"Contoso ID : {contoso_account}");
            Console.WriteLine($"Party Id : {party.Id}");
            Console.WriteLine($"Blockchain Id : {bcNetwork.Id}");
            Console.WriteLine("======================================================================");

            Console.WriteLine("==> Copy these values then Hit Enter to close.");
            Console.ReadLine();
        }
    }

    class DigitalGoodsSetUp
    {
        private IConfiguration config;
        private HttpClient httpClient;
        private ServiceClient tokenServiceProxyClient;
        private CosmosConnectionStrings cosmosConnectionStrings;
        private TokenManager tokenManager;
        public DigitalGoodsSetUp(IConfiguration Config, HttpClient HttpClient, CosmosConnectionStrings CosmosConnectionStrings, TokenManager TokenManager)
        {
            this.config = Config;
            this.httpClient = HttpClient;
            this.tokenServiceProxyClient = new ServiceClient(Config["Values:NFTServiceEndpoint"], HttpClient);
            this.cosmosConnectionStrings = CosmosConnectionStrings;
            this.tokenManager = TokenManager;
        }

        public async Task<BlockchainNetwork> SetupBlockChainNetwork()
        {
            var result = await this.tokenServiceProxyClient.RegisterBlockchainNetworkAsync(new BlockchainNetworkInfo()
            {
                Name = this.config["Values:BlockchainNetworkName"],
                Description = this.config["Values:BlockchainNetworkDescription"],
                NodeURL = this.config["Values:BlockchainNetworkTxNode"]
            });

            return result;
        }

        public async Task<Party> SetupParty()
        {
            string serviceEndpoint = this.config["Values:NFTServiceEndpoint"];
            string _partyName = this.config["Values:PartyName"];
            string _partyDescription = this.config["Values:PartyDescription"];

            var result = await this.tokenServiceProxyClient.RegisterPartyAsync(new PartyInfo()
            {
                PartyName = _partyName,
                Description = _partyDescription
            });

            return result;
        }

        public Task SetupProductCatalog()
        {
            throw new NotImplementedException();
        }

        public async Task<string> RegisterUserInNFTService(string PartyID, string BlockchainNetworkID, string UserName)
        {
            var result = await this.tokenServiceProxyClient.RegisterUserAsync(new UserInfo()
            {
                PartyID = Guid.Parse(PartyID),
                BlockchainNetworkID = Guid.Parse(BlockchainNetworkID),
                Name = UserName
            });

            return result.Id.ToString();
        }

        public async Task<string> ProvisioningContosoProfile(string UserName, string PartyID, string BlockchainNetworkID)
        {
            var result = await RegisterUserInNFTService(PartyID, BlockchainNetworkID, UserName);

            DigitalLockerManager digitalLockerManager = new(this.cosmosConnectionStrings.PrimaryReadWriteKey, "TradableDigitalGoods");

            var isProvisioned = await digitalLockerManager.ProvisionLocker(result);

            return result;
        }

        public async Task<TokenMeta> InitializeToken(string productManagerAccount)
        {
            return await this.tokenManager.CreateToken("ContosoToken", "CTT", productManagerAccount);
        }
    }
}

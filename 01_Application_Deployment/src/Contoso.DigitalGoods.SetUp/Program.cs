using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Collections.Generic;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Azure.TokenService.Proxy;
using Contoso.DigitalGoods.TokenService;
using Contoso.DigitalGoods.DigitalLocker.Service;

namespace Contoso.DigitalGoods.SetUp
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(Path.Combine(fileInfo.Directory.FullName, "appsettings.json")))
            {
                throw new FileNotFoundException("there is no appsettings.json file");
            }

            IConfigurationRoot _config = new ConfigurationBuilder()
                                               .SetBasePath(fileInfo.Directory.FullName)
                                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                               .Build();

            DigitalGoodsSetUp setup = new DigitalGoodsSetUp(_config);


            Console.WriteLine("\nStart to registering..........");

            Console.WriteLine("\nRegistering group.....");
            var party = setup.SetupParty().Result;
            Console.WriteLine(JsonConvert.SerializeObject(party));

            Console.WriteLine("\nRegistering blockchian network.....");
            var bcNetwork = setup.SetupBlockChainNetwork().Result;
            Console.WriteLine(JsonConvert.SerializeObject(bcNetwork));

            Console.WriteLine("\nRegistering ProductManager account.....");
            var productManager_account = setup.SetupContosoProductManagerProfile(party.Id.ToString(), bcNetwork.Id.ToString()).Result;
            Console.WriteLine(productManager_account);

            Console.WriteLine("\nRegistering Contoto account.......");
            var contoso_account = setup.SetupContosoProfile("Contoso", party.Id.ToString(), bcNetwork.Id.ToString()).Result;
            Console.WriteLine(contoso_account);

            Console.WriteLine("\nInitialize Token");
            var token = setup.InitializeToken(productManager_account).Result;
            Console.WriteLine(JsonConvert.SerializeObject(token));

            Console.WriteLine("Registering process done. Update your configuration in Application and Token Services\n\n\n");

            Console.WriteLine("===================   Configuration Information  =====================");
            Console.WriteLine($"TokenID : {token.ContractAddress}");
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
        private IConfigurationRoot config;
        public DigitalGoodsSetUp(IConfigurationRoot Config)
        {
            this.config = Config;
        }

        public async Task<bool> ValidateRegistry()
        {
            string serviceEndpoint = this.config["Settings:ServiceEndpoint"];

            List<string> messageBag = new List<string>();
            int exceptions = 0;

            //Check BlockchainNetworkID
            Console.WriteLine("Checking BlockchainNetworkID.................");
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];

            BlockchainNetworksClient bcClient = new BlockchainNetworksClient(serviceEndpoint);
            var bc_response = await bcClient.BlockchainNetworkPostAsync(Guid.Parse(_blockchainNetworkId));
            if (bc_response != null)
            {
                messageBag.Add($"\nThe BlockchainNetworkId : {_blockchainNetworkId} is already existed. try again with different ID");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            //Check Party
            Console.WriteLine("Checking Party..............");
            string _partyId = this.config["Settings:PartyId"];
            PartiesClient partyClient = new PartiesClient(serviceEndpoint);

            var gr_response = await partyClient.PartyPostAsync(Guid.Parse(_partyId));
            if (gr_response != null)
            {
                messageBag.Add($"\nThis PartyId : {_partyId} is already existed. try again with different ID");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            //Check ProductManager ID
            Console.WriteLine("Checking Product Manager........");
            string _productManager = this.config["Settings:ProductManagerId"];
            UsersClient userClient = new UsersClient(serviceEndpoint);

            var ar_response = await userClient.UserPostAsync(Guid.Parse(_productManager));
            if (ar_response != null)
            {
                messageBag.Add($"\nThe Product Manager : {_productManager} is already existed.");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            return exceptions == 0 ? true : false;

        }

        public async Task<BlockchainNetwork> SetupBlockChainNetwork()
        {
            BlockchainNetworksClient bcClient = new BlockchainNetworksClient(config["Settings:ServiceEndpoint"]);

            var result = await bcClient.BlockchainNetworkPostAsync(new BlockchainNetworkInfo()
            {
                Name = config["Settings:BlockchainNetworkName"],
                Description = config["Settings:BlockchainNetworkDescription"],
                NodeURL = config["Settings:BlockchainNetworkTxNode"]
            });

            return result;
        }

        public async Task<Party> SetupParty()
        {
            string serviceEndpoint = config["Settings:ServiceEndpoint"];
            string _partyName = config["Settings:PartyName"];
            string _partyDescription = config["Settings:PartyDescription"];

            PartiesClient partyClient = new PartiesClient(serviceEndpoint);

            var result = await partyClient.PartyPostAsync(new PartyInfo()
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

        public async Task<string> SetupContosoProductManagerProfile(string PartyID, string BlockchainNetworkID)
        {
            UserManager userManager = new UserManager(config["DBConnections:TokenDBConnection"], 
                config["Settings:ServiceEndpoint"],
                PartyID,
                BlockchainNetworkID);

            var result = await userManager.ProvisionUser("Contoso ProductManager");        
            return result;
        }

        public async Task<string> SetupContosoProfile(string UserName, string PartyID, string BlockchainNetworkID)
        {
            UserManager userManager = new UserManager(config["DBConnections:TokenDBConnection"],
              config["Settings:ServiceEndpoint"],
              PartyID,
              BlockchainNetworkID);

            var result = await userManager.ProvisionUser(UserName);

            DigitalLockerManager digitalLockerManager = new DigitalLockerManager(config["DBConnections:ApplicationDBConnection"], "DigitalLockers");
            
            var isProvisioned = await digitalLockerManager.ProvisionLocker(result);

           
            return result;
        }

        public async Task<TransactionReciept> InitializeToken(string productManagerAccount)
        {
            TokenServiceWrapper svcWrapper = new TokenServiceWrapper(this.config["Settings:ServiceEndPoint"]);
            return await svcWrapper.CreateToken("ContosoToken", "CTS", productManagerAccount);
        }
    }


}

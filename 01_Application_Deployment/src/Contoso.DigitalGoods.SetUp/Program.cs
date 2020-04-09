using Newtonsoft.Json;
using System;
using System.IO;
using Contoso.DigitalGoods.SetUp.Interface;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Azure.TokenService.Management;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Azure.TokenService.Management.Model;
using System.Collections.Generic;
using Contoso.DigitalGoods.TokenService.ServiceWrapper;
using Microsoft.Azure.TokenService;
using Microsoft.Azure.TokenService.Models;

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

            CryptoGoodsGetUp setup = new CryptoGoodsGetUp(_config);

            if (setup.ValidateRegistry().Result)
            {
                Console.WriteLine("\nStart to registering..........");

                Console.WriteLine("\nRegistering group.....");
                var group = setup.SetupGroup().Result;
                Console.WriteLine(JsonConvert.SerializeObject(group));

                Console.WriteLine("\nRegistering blockchian network.....");
                var bcNetwork = setup.SetupBlockChainNetwork().Result;
                Console.WriteLine(JsonConvert.SerializeObject(bcNetwork));

                Console.WriteLine("\nRegistering ProductManager account.....");
                var productManager_account = setup.SetupContosoProductManagerProfile().Result;
                Console.WriteLine(JsonConvert.SerializeObject(productManager_account));

                Console.WriteLine("\nRegistering Contoto A account.......");
                var contoso_accountA = setup.SetupContosoProfile().Result;
                Console.WriteLine(JsonConvert.SerializeObject(contoso_accountA));

                Console.WriteLine("\nRegistering Contoto B account.......");
                var contoso_accountB = setup.SetupContosoProfile().Result;
                Console.WriteLine(JsonConvert.SerializeObject(contoso_accountB));

                Console.WriteLine("\nInitialize Token");
                var token = setup.InitializeToken(contoso_accountA.Name).Result;
                Console.WriteLine(JsonConvert.SerializeObject(token));

                Console.WriteLine("Registering process done. Update your configuration in Application and Token Services\n\n\n");

                Console.WriteLine("===================   Configuration Information  =====================");
                Console.WriteLine($"TokenID : {token.Id.Replace("tokens.", "")}");
                Console.WriteLine($"Contoso A ID : {contoso_accountA.Name}");
                Console.WriteLine($"Contoso B ID : {contoso_accountB.Name}");
                Console.WriteLine($"ContosoProductManager : {productManager_account.Name}");
                Console.WriteLine($"GroupName : {group.Name}");
                Console.WriteLine("======================================================================");
            }
            else
            {
                _ = setup.Clear().Result;
                Console.WriteLine("Your registering process had something wrong. change configuration values and try again.\n\n\n");
                Console.WriteLine("Cleaning resources.......\n");

            }
            Console.WriteLine("==> Hit Enter to close.");
            Console.ReadLine();
        }

        //private static void ReadDataFiles()
        //{
        //    string jsonProfile = File.ReadAllText(".\\Data\\profiles.json");
        //    var profile = JsonConvert.DeserializeObject<Profiles>(jsonProfile);
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        cfg.CreateMap<CryptoGoods.SetUp.Model.Profile, ContosoProfile>();
        //    });

        //    var mapper = config.CreateMapper();
        //    profile.Profile.ForEach(x =>
        //    {
        //        var mappedProfile = mapper.Map<CryptoGoods.SetUp.Model.Profile, ContosoProfile>(x);
        //        Console.WriteLine(mappedProfile.ContosoID);
        //    });
        //}
    }

    class CryptoGoodsGetUp : ICryptoGoodsSetup
    {
        private IConfigurationRoot config;
        public CryptoGoodsGetUp(IConfigurationRoot Config)
        {
            this.config = Config;
        }

        public async Task<bool> Clear()
        {
            //unregister blockchainnetwork
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            await blockchainNetwork.UnRegisterAsync(_blockchainNetworkId);

            //unregister group information
            string _groupName = this.config["Settings:GroupName"];
            GroupResource gr = new GroupResource();
            await gr.UnRegisterAsync(_groupName);

            //unregister product manager
            string _productManager = $"{_groupName}ProductManager";
            AccountResource ar = new AccountResource();
            ar.GroupName = _groupName;
            await ar.UnRegisterAsync(_productManager);

            return true;
        }

        public async Task<bool> ValidateRegistry()
        {
            List<string> messageBag = new List<string>();
            int exceptions = 0;

            //Check BlockchainNetworkID
            Console.WriteLine("Checking BlockchainNetworkID.................");
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];
            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();
            var bc_response = await blockchainNetwork.GetAsync(_blockchainNetworkId);
            if (bc_response != null)
            {
                messageBag.Add($"\nThe BlockchainNetworkId : {_blockchainNetworkId} is already existed. try again with different ID");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            //Check GroupName
            Console.WriteLine("Checking GroupName..............");
            string _groupName = this.config["Settings:GroupName"];
            GroupResource gr = new GroupResource();
            var gr_response = await gr.GetAsync(_groupName);
            if (gr_response != null)
            {
                messageBag.Add($"\nThis GroupName : {_groupName} is already existed. try again with different ID");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            //Check ProductManager ID
            Console.WriteLine("Checking Product Manager........");
            string _productManager = $"{_groupName}ProductManager";
            AccountResource ar = new AccountResource();
            ar.GroupName = _groupName;
            var ar_response = ar.GetAsync(_productManager).Result;
            if (ar_response != null)
            {
                messageBag.Add($"\nThe Product Manager : {_productManager} is already existed.");
                Console.WriteLine(messageBag[exceptions]);
                ++exceptions;
            }

            return exceptions == 0 ? true : false;

        }

        public async Task<GenericResource> SetupBlockChainNetwork()
        {
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];
            string _blockchainNetworkTxNode = this.config["Settings:BlockchainNetworkTxNode"];
            string _blockchainNodeDescription = this.config["Settings:BlockchainNodeDescription"];

            BlockchainNetwork blockchainNetwork = new BlockchainNetwork();

            var result = await blockchainNetwork.RegisterOrUpdateAsync(new BlockchainNetworkRequestPropertyBag()
            {
                BlockchainNetworkId = _blockchainNetworkId,
                blockchainNode = _blockchainNetworkTxNode,
                description = _blockchainNodeDescription
            });

            return result;
        }

        public Task SetupDigitalLocker()
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResource> SetupGroup()
        {
            string _groupName = this.config["Settings:GroupName"];
            string _groupDescription = this.config["Settings:GroupDescription"];
            GroupResource gr = new GroupResource();
            var result = await gr.RegisterOrUpdateAsync(new GroupRequestPropertyBag()
            {
                GroupName = _groupName,
                description = _groupDescription
            });

            return result;
        }

        public Task SetupProductCatalog()
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResource> SetupContosoProductManagerProfile()
        {
            string _groupName = this.config["Settings:GroupName"];
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];
            string _productManager = $"{_groupName}ProductManager";

            AccountResource accountResource = new AccountResource();
            accountResource.GroupName = _groupName;
            var result = await accountResource.RegisterOrUpdateAsync(new AccountRequestPropertyBag()
            {
                AccountName = _productManager,
                blockchainNetworkId = _blockchainNetworkId,
                description = $"{_groupName} Product Manager"
            });

            return result;
        }

        public async Task<GenericResource> SetupContosoProfile()
        {
            string _groupName = this.config["Settings:GroupName"];
            string _blockchainNetworkId = this.config["Settings:BlockchainNetworkId"];
            string _contosoID = Guid.NewGuid().ToString();

            AccountResource accountResource = new AccountResource();
            accountResource.GroupName = _groupName;
            var result = await accountResource.RegisterOrUpdateAsync(new AccountRequestPropertyBag()
            {
                AccountName = _contosoID,
                blockchainNetworkId = _blockchainNetworkId,
                description = $"Contoso ID - {_contosoID}"
            });

            return result;
        }

        public async Task<AsyncResponse> InitializeToken(string productManager)
        {
            //throw new NotImplementedException();
            TokenAPIService tokenAPIConnection = new TokenAPIService();
            AzureTokenServiceAPI api = tokenAPIConnection.Initialize();

            TokenServiceWrapper svcWrapper = new TokenServiceWrapper(api);
            return await svcWrapper.CreateToken("ContosoToken", "CTS", productManager);
        }
    }


}

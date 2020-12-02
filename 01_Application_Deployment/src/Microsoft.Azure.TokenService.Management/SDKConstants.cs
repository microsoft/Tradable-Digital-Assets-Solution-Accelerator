namespace Microsoft.Azure.TokenService.Management
{
    //public enum UserPersona { RPUser, MicrosoftUser1, MicrosoftUser2, MicrosoftUser3, MicrosoftUser4, BlockchainUser1, BlockchainUser2 }

    public static class SDKConstants
    {
        public const string ClientSecret = "";
        public const string ClientId = "";
        public const string AzureTenantId = "";
        public const string AzureSubscriptionId = "";

        //public const string ApiVersion1 = "api/v1/";
        //public const string AccountsUri = ApiVersion1 + "accounts";
        //public const string TokenServicesUri = ApiVersion1 + "tokenservices";
        //public const string BlockchainNetworksUri = ApiVersion1 + "blockchainnetworks";
        //public const string PartysUri = ApiVersion1 + "parties";
        //public const string TemplateUri = ApiVersion1 + "templates";
        //public const string Party1Name = "Party1";


        // Azure PPE constants
        public static string ActiveDirectoryEndpoint = "";
        public static string ActiveDirectoryServiceEndpointResourceId = ";
        public static string ManagementEndPoint = "";

        // Munich constants
        public static string TokenServiceProviderNamespace = "Microsoft.BlockchainTokens";
        public static string TokenServiceResourceType = "tokenServices";
        public static string BlockchainNetworkResourceType = "blockchainNetworks";
        public static string PartyResourceType = "groups";
        public static string AccountResourceType = "accounts";
        public static string TokenTemplateResourceType = "tokenTemplates";
        public static string TokenServiceAPIVersion = "2019-07-19-preview";
        public static string ABTResourceGroupName = "ABTTest";
        public static string ResourceLocation = "West Central US";
        public static string ServiceResourceName = "echopreview";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileUpload
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // helpful links:
        // https://docs.microsoft.com/en-us/azure/key-vault/vs-key-vault-add-connected-service
        // https://docs.microsoft.com/en-us/azure/key-vault/managed-identity
        // signalR: https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-3.1&tabs=visual-studio
        //          https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-dotnet-core

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Host.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((context, config) =>
                   {
                       var keyVaultEndpoint = GetKeyVaultEndpoint();
                       if (!string.IsNullOrEmpty(keyVaultEndpoint))
                       {
                           var azureServiceTokenProvider = new AzureServiceTokenProvider();
                           var keyVaultClient = new KeyVaultClient(
                               new KeyVaultClient.AuthenticationCallback(
                                   azureServiceTokenProvider.KeyVaultTokenCallback));
                           config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                       }
                   })
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<Startup>();
                   });
        private static string GetKeyVaultEndpoint() => "https://halzelkv.vault.azure.net";
    }
}

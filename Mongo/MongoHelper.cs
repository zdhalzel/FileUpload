using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileUpload.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using System.Security.Authentication;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

// https://docs.microsoft.com/en-us/azure/cosmos-db/create-mongodb-dotnet#update-your-connection-string
// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio
// make retryablewrites = false in the connection string

namespace FileUpload.Mongo
{
    public class MongoHelper : IDisposable
    {
        private IConfiguration _configuration;
        private bool disposed = false;

        private string dbName = "chat";
        private string collectionName = "chatLog";

        public MongoHelper(IConfiguration configuration) {
            _configuration = configuration;
        }

        public List<MyChat> GetAllChats()
        {
            try
            {
                var collection = GetChatCollection();
                return collection.Find(new BsonDocument()).ToList();
            }
            catch (MongoConnectionException)
            {
                return new List<MyChat>();
            }
        }

        public void CreateChat(MyChat chat)
        {
            var collection = GetChatCollectionForEdit();
            try
            {
                collection.InsertOne(chat);
            }
            catch (MongoConnectionException ex)
            {
                string msg = ex.Message;
                Trace.WriteLine(msg);
            }
        }

        private IMongoCollection<MyChat> GetChatCollection()
        {
            var client = new MongoClient(_configuration["halzelmongosecret"]);
            var database = client.GetDatabase(dbName);

            var chatCollection = database.GetCollection<MyChat>(collectionName);
            return chatCollection;
        }

        private IMongoCollection<MyChat> GetChatCollectionForEdit()
        {
            var client = new MongoClient(_configuration["halzelmongosecret"]);
            var database = client.GetDatabase(dbName);

            var chatCollection = database.GetCollection<MyChat>(collectionName);
            return chatCollection;
        }

        # region IDisposable
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) 
                { 
                }
            }
            this.disposed = true;
        }
        # endregion
    }
}

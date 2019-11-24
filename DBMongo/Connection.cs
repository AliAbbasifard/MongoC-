using MongoDB.Driver;

namespace DBMongo
{
    sealed class Connection
    {

        #region Properties
        public MongoClient Client { get; }
        #endregion

        #region Constructors
        public Connection() { }
        public Connection(string database = "", string username = "", string password = "")
        {
            MongoCredential Credential = MongoCredential.CreateCredential(
                databaseName: database,
                username: username,
                password: password);

            MongoClientSettings Settings = new MongoClientSettings() { Credential = Credential };

            Client = new MongoClient(Settings);
        }
        #endregion

    }
}

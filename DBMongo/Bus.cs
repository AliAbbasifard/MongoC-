using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;


namespace DBMongo
{
    // We assume that, we have only one sort
    [BsonIgnoreExtraElements]
    sealed class Bus
    {
        #region Properties
        public string Code { get; set; }
        public string Company { get; set; }
        public int Chairs { get; set; }
        public string Type { get; set; }
        public List<Travel> Travels { get; set; } = new List<Travel>();
        #endregion

        #region Constructors
        public Bus() { }
        public Bus(
            string busCode = "",
            string company = "",
            int chairs = 0,
            string type = ""
        )
        {
            Code = busCode;
            Company = company;
            Chairs = chairs;
            Type = type;
        }
        #endregion

        #region Methods
        public static string CreateBus(IMongoDatabase database, IMongoCollection<Bus> collection, Bus bus)
        {
            try
            {
                collection.InsertOne(bus);
                return "Bus Created Successfully";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion
    }
}

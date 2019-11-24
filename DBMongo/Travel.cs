using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace DBMongo
{
    [BsonIgnoreExtraElements]
    sealed class Travel
    {
        
        #region Properties
        public string TravelId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public Bus Bus { get; set; }
        public double Cost { get; set; }
        public int Capacity { get; set; }
        public List<Ticket> Tickets { get; set; }
        #endregion

        #region Constructors
        public Travel(
            string travelCode = "",
            string origin = "",
            string destination = "",
            DateTime? departureTime = null,
            Bus bus = null,
            double cost = 0,
            int capacity = 0)
        {
            TravelId = travelCode;
            Origin = origin;
            Destination = destination;
            DepartureTime = (DateTime)departureTime;
            Bus = bus;
            Cost = cost;
            Capacity = capacity;
            Tickets = new List<Ticket>();
        }
        #endregion

        #region Methods
        public static string CreateTravel(IMongoDatabase database, IMongoCollection<Travel> collection, Travel travel)
        {
            try
            {
                collection.InsertOne(travel);
                return "Travel Created Successfully";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static Travel GetTravel(IMongoDatabase database, IMongoCollection<Travel> collection, string travelId)
        {

            try
            {
                Travel travel = collection.Find(t => t.TravelId == travelId).First();
                return travel;
            }
            catch
            {
                return null;
            }
        }

        public static List<Travel> GetTravels(
            IMongoDatabase database, 
            IMongoCollection<Travel> collection, 
            string origin, 
            string destination, 
            string date)
        {
            DateTime Day = Convert.ToDateTime(date);

            TimeSpan StartTime = new TimeSpan(0, 0, 0, 0);
            TimeSpan EndTime = new TimeSpan(23, 59, 59, 59);

            // Create Start-Date & End-Date. Ex: 2019-05-06 => 2019-05-06T00:00:000 and 2019-05-06T23:59:59
            DateTime StartOfDay = Day + StartTime;
            DateTime EndOfDay = Day + EndTime;

            List<Travel> Travels = collection.Find(travel => 
            (travel.DepartureTime >= StartOfDay && travel.DepartureTime <= EndOfDay) 
            && 
            (travel.Origin == origin)
            &&
            (travel.Destination == destination)
            ).ToList();

            return Travels;
        }
        #endregion
    }
}

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMongo
{
    sealed class Ticket
    {
        #region Properties
        public User User { get; set; }
        public Travel Travel { get; set; }
        #endregion

        #region Constructors
        public Ticket() { }
        public Ticket(User user, Travel travel)
        {
            User = user;
            Travel = travel;
        }
        #endregion

        #region Methods
        public static string CreateTicket(IMongoDatabase database, IMongoCollection<Ticket> collection, Ticket ticket)
        {
            try
            {
                collection.InsertOne(ticket);
                return "Ticket Created Successfully";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion
    }
}

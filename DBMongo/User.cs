using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace DBMongo
{
    [BsonIgnoreExtraElements]
    sealed class User
    {

        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Ticket> Tickets { get; set; }

        #endregion

        #region Constructors
        public User() { }
        public User(
            string firstName,
            string lastName,
            string userName,
            string password,
            string email,
            string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Tickets = new List<Ticket>();
        }
        #endregion

        #region Methods

        public static string RegisterUser(IMongoDatabase database, IMongoCollection<User> collection, User user)
        {
            try
            {
                collection.InsertOne(user);
                return "User Registered Successfully";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }   
        }

        public static User GetUser(IMongoDatabase database, IMongoCollection<User> collection, string username)
        {
            try
            {
                User user = collection.Find(u => u.UserName == username).First();
                return user;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}

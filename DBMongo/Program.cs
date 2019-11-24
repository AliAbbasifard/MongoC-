using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace DBMongo
{
    class Program
    {
        static Connection Connection;

        static void Main(string[] args)
        {
            #region Try To Create A Mongo Connection

            try
            {
                // Get MongoDB Client Connection
                Connection = new Connection(
                    database: "tixys",
                    username: "ali",
                    password: "1072354"
                );
            }

            catch (Exception ex)
            {
                Console.WriteLine("Connection Failed: {0}", ex);
                return;
            }

            #endregion

            #region Get Database And Collections

            // Get Tixy Database
            IMongoDatabase Tixys = Connection.Client.GetDatabase("tixys");

            var BusCollection = Tixys.GetCollection<Bus>("buses");

            var UserCollection = Tixys.GetCollection<User>("users");

            var TravelCollection = Tixys.GetCollection<Travel>("travels");

            var TicketCollection = Tixys.GetCollection<Ticket>("tickets");

            #endregion


            // Set answer to "y" which means you want to do other thing in app
            string Answer = "y";

            while (Answer == "y")
            {
                #region Show Options To User And Get Option Number

                Console.WriteLine(
               @"
What kind of service do you need?

    #1: User Registration
    #2: Create Travel
    #3: Create Ticket
    #4: Search
    #5: Insert Bus
"
                );
                string Option = Console.ReadLine();

                #endregion
                
                switch (Option)
                {
                    case "1":
                        #region User Registration

                        #region Get User Registration Data
                        Console.Write("FirstName: ");
                        string FirstName = Console.ReadLine();

                        Console.Write("LastName: ");
                        string LastName = Console.ReadLine();

                        Console.Write("UserName: ");
                        string UserName = Console.ReadLine();

                        Console.Write("Password: ");
                        string Password = Console.ReadLine();

                        Console.Write("Email: ");
                        string Email = Console.ReadLine();

                        Console.Write("Phone: ");
                        string Phone = Console.ReadLine();

                        #endregion

                        // Create A User 
                        var User = new User(
                            FirstName,
                            LastName,
                            UserName,
                            Password,
                            Email,
                            Phone
                        );

                        // Register User and return an status: Successfull OR Not
                        string UserCreationStatus = User.RegisterUser(Tixys, UserCollection, User);

                        Console.WriteLine("\n");
                        Console.WriteLine(UserCreationStatus);

                        Answer = GetAnswer();

                        break;
                    #endregion
                    case "2":
                        #region Travel Creation

                        #region Get Travel Data
                        Console.Write("Travel ID: ");
                        string TravelID = Console.ReadLine();

                        Console.Write("Origin: ");
                        string Origin = Console.ReadLine();

                        Console.Write("Destination: ");
                        string Destination = Console.ReadLine();

                        Console.Write("DepartureTime: ");
                        DateTime DepartureTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                        DepartureTime = DepartureTime.AddHours(4).AddMinutes(30);

                        Console.Write("Bus Code: ");
                        string Code = Console.ReadLine();

                        Console.Write("Cost: ");
                        double Cost = Convert.ToDouble(Console.ReadLine());

                        Console.Write("Capacity: ");
                        int Capacity = int.Parse(Console.ReadLine());
                        #endregion

                        // Get Bus Object From Collection
                        Bus TBus = BusCollection.Find(b => b.Code == Code).First();

                        // Create Travel Object
                        Travel Travel = new Travel(
                            travelCode: TravelID,
                            origin: Origin,
                            destination: Destination,
                            departureTime: DepartureTime,
                            cost: Cost,
                            capacity: Capacity,
                            bus: TBus
                        );

                        string TravelCreationStatus = Travel.CreateTravel(Tixys, TravelCollection, Travel);

                        if (TravelCreationStatus == "Travel Created Successfully")
                        {
                            BusCollection.UpdateOne(
                                Builders<Bus>.Filter.Eq("Code", Code),
                                Builders<Bus>.Update.Push("Travels", Travel)
                            );
                            Console.WriteLine(TravelCreationStatus);
                        }

                        Answer = GetAnswer();

                        break;

                    #endregion
                    case "3":
                        #region Ticket Creation

                        #region Get Ticket Data

                        Console.Write("Ticket ID: ");
                        string TicketID = Console.ReadLine();

                        Console.Write("UserName: ");
                        string TicketUserName = Console.ReadLine();

                        Console.Write("Travel ID: ");
                        string TicketTravelId = Console.ReadLine();

                        #endregion

                        // TicketTravel is Just Travel
                        Travel TicketTravel = Travel.GetTravel(Tixys, TravelCollection, TicketTravelId);
                        if (TicketTravel == null)
                        {
                            Console.WriteLine("Failed To Find Travel");
                            return;
                        }

                        // Check Whether Travel Have Capacity Or Not
                        int TravelCapacity = (TicketTravel.Capacity) - 1;

                        // Is Full
                        if (TravelCapacity < 0)
                        {
                            Console.WriteLine("Capacity Is Full. Sorry Man");
                            Answer = GetAnswer();
                            break;

                        } else
                        {
                            // Is Not Full

                            // Get User
                            User TicketUser = User.GetUser(Tixys, UserCollection, TicketUserName);
                            if (TicketUser == null)
                            {
                                Console.WriteLine("Failed To Find User");
                                return;
                            }

                            // Create Ticket Object
                            Ticket Ticket = new Ticket(TicketUser, TicketTravel);

                            // Insert Ticket In Collection
                            string TicketCreationStatus = Ticket.CreateTicket(Tixys, TicketCollection, Ticket);

                            if (TicketCreationStatus == "Ticket Created Successfully")
                            {

                                // Add Ticket To User History
                                UserCollection.UpdateOne(
                                       Builders<User>.Filter.Eq("UserName", TicketUserName),
                                       Builders<User>.Update.Push("Tickets", Ticket)
                                );

                                // Add Ticket To Travel History
                                TravelCollection.UpdateOne(
                                       Builders<Travel>.Filter.Eq("TravelId", TicketTravelId),
                                       Builders<Travel>.Update.Push("Tickets", Ticket)
                                );

                                // Update Capacity
                                TravelCollection.UpdateOne(
                                       Builders<Travel>.Filter.Eq("TravelId", TicketTravelId),
                                       Builders<Travel>.Update.Set("Capacity", TravelCapacity)
                                );
                            }

                            Answer = GetAnswer();

                            break;
                        }
                    #endregion
                    case "4":
                        #region Search

                        #region Get Search Date
                        Console.Write("Origin: ");
                        string SearchOrigin = Console.ReadLine();

                        Console.Write("Destination: ");
                        string SearchDestination = Console.ReadLine();

                        Console.Write("Date: ");
                        string SearchDate = Console.ReadLine();
                        #endregion

                        List<Travel> travels = Travel.GetTravels(Tixys, TravelCollection, SearchOrigin, SearchDestination, SearchDate);

                        Console.WriteLine();
                        foreach (Travel travel in travels)
                        {
                            Console.WriteLine("Time: " + travel.DepartureTime.TimeOfDay);
                            Console.WriteLine("Cost: " + travel.Cost);
                            Console.WriteLine("Capacity: " + travel.Capacity);
                            Console.WriteLine();
                        }
                        Answer = GetAnswer();

                        break;
                        #endregion
                    case "5":
                        #region Insert Bus

                        #region Get Bus Data
                        Console.Write("Bus Code: ");
                        string BusCode = Console.ReadLine();

                        Console.Write("Company: ");
                        string Company = Console.ReadLine();

                        Console.Write("Number Of Chairs: ");
                        int Chairs = int.Parse(Console.ReadLine());

                        Console.Write("BusType: ");
                        string Type = Console.ReadLine();
                        #endregion

                        Bus Bus = new Bus(
                            busCode: BusCode,
                            company: Company,
                            chairs: Chairs,
                            type: Type
                        );

                        string BusCreationStatus = Bus.CreateBus(Tixys, BusCollection, Bus);

                        Console.WriteLine("\n");
                        Console.WriteLine(BusCreationStatus);

                        Answer = GetAnswer();

                        break;
                        #endregion
                }
            }
        }

        public static string GetAnswer()
        {
            Console.WriteLine("\n Do you need Another Service ? (y/n)");
            string Answer = Console.ReadLine();
            return Answer;
        }

    }
}

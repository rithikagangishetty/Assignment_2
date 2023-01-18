using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Text;
using System.Reflection.Metadata;

namespace Assignment_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
    
        public IActionResult Index()
        {
            string Date = Set_Date();
            string Ip_address = Set_Ip_Address();
            Console.WriteLine("NO:", Ip_address);
            string Time = Set_Time();
            ViewData["string"] = Add_Date_Time_Ip( Date, Time,  Ip_address);
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["string"]=GetUserData();
            return View();
        }
        [HttpGet]
        public IActionResult CreateForm()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateForm(FormDetails product)
        {
            MongoClient client_db = new MongoClient("mongodb://localhost:27017");

            var user_db = client_db.GetDatabase("UserFormData");
            var collection = user_db.GetCollection<BsonDocument>("data");
            var document = new BsonDocument { { "Name", product.Name }, { "Country", product.Country } };
            collection.InsertOne(document);

            return Redirect("/");
        }
        public ActionResult ViewInfo()
        {
            MongoClient Client = new MongoClient("mongodb://localhost:27017");
            var db = Client.GetDatabase("UserFormData");
            var collection = db.GetCollection<BsonDocument>("data");
            var list_db = collection.Find(new BsonDocument()).ToList();
            BsonDocument document2 = new BsonDocument();
            foreach (var item in list_db)
            {
                document2 = item;
            }
            FormDetails form = new FormDetails();
            form.Name = document2["Name"].ToString();
            form.Country = document2["Country"].ToString();
            return View(form);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public string Set_Date()
        {
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            return Date;
        }
        public string Set_Time()
          {
            string Time = DateTime.Now.ToString("HH:mm:ss");
            return Time;
        }
    public string Set_Ip_Address()
        {
            string Ip_Address = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            return Ip_Address;

        }
        public string Add_Date_Time_Ip(string Date,  string Time, string Ip_Address)
        {
            MongoClient client_db = new MongoClient("mongodb://localhost:27017");

            var user_db = client_db.GetDatabase("UserDataCollection");
            var collection = user_db.GetCollection<BsonDocument>("Info");
            var document = new BsonDocument { { "Date", Date }, { "Time",Time},{ "IP", Ip_Address } };
            collection.InsertOne(document);
            return "string";
        }
        public string GetUserData()
        {
            MongoClient client_db = new MongoClient("mongodb://localhost:27017");

            var db_user = client_db.GetDatabase("UserDataCollection");
            var collect = db_user.GetCollection<BsonDocument>("Info");

            var list_db = collect.Find(new BsonDocument()).ToList();
            BsonDocument document2 = new BsonDocument();
            foreach (var item in list_db)
            {
                document2 = item;
            }
            return $"Date :{document2["Date"].ToString()}, Time : {document2["Time"].ToString()},Ip Address :{document2["IP"].ToString()}";
        }
   
        public string DisplayCountry(string country)
        {
            return country;
        }


    }
}


























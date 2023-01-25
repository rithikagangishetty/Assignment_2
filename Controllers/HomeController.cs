using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using MongoDB.Driver.GridFS;
using System.Text;
using System.Reflection.Metadata;
using Microsoft.Identity.Client;
using System.IO;
using System;
using System.Net.Sockets;
using System.Drawing;

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
            string Date = SetDate();
            string Ip_address = SetIpAddress();
            Console.WriteLine("NO:", Ip_address);
            string Time = SetTime();
            ViewData["string"] = AddDateTimeIp( Date, Time,  Ip_address);
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
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
            var dbUser = dbClient.GetDatabase("UserFormData");
            var collection = dbUser.GetCollection<BsonDocument>("data");
            var document = new BsonDocument { { "Name", product.Name }, { "Country", product.Country } };
            collection.InsertOne(document);

            return Redirect("/");
        }
     
        public IActionResult ViewImage()
        { 
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ViewImage(CreateImage item)
        {
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
            var database = dbClient.GetDatabase("Images");
            IGridFSBucket bucket = new GridFSBucket(database);
            var ImageData = new CreateImage();
            ImageData.Image = item.Image;
            ImageData.Title = item.Title;
            ImageData.Description = item.Description;
            ImageData.Id = item.Id;
           var options = new GridFSUploadOptions
         
            {
                ChunkSizeBytes = 64512, // 63KB
                Metadata = new BsonDocument
    {
        { "resolution", "1080P" },
                    { "copyrighted", true }
    }
            };



            using var stream = await bucket.OpenUploadStreamAsync(item.Title, options);
            var id = stream.Id;
            ImageData.Image.CopyTo(stream);
            await  stream.CloseAsync();
            
           
            //RedirectResult redirectResult = Redirect("https://localhost:7043/Home");

            return View("Index");
        }

        public ActionResult ViewInfo()
        {
            MongoClient Client = new MongoClient("mongodb://localhost:27017");
            var db = Client.GetDatabase("UserFormData");
            var collection = db.GetCollection<BsonDocument>("data");
            var dbList = collection.Find(new BsonDocument()).ToList();
            BsonDocument document2 = new BsonDocument();
            foreach (var item in dbList)
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
        public string SetDate()
        {
            string Date = DateTime.Now.ToString("dd-MM-yyyy");
            return Date;
        }
        public string SetTime()
          {
            string Time = DateTime.Now.ToString("HH:mm:ss");
            return Time;
        }
    public string SetIpAddress()
        {
            string IpAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();
            return IpAddress;

        }
        public string AddDateTimeIp(string Date,  string Time, string IpAddress)
        {
            MongoClient clientDb = new MongoClient("mongodb://localhost:27017");

            var userDb = clientDb.GetDatabase("UserDataCollection");
            var collection = userDb.GetCollection<BsonDocument>("Info");
            var document = new BsonDocument { { "Date", Date }, { "Time",Time},{ "IP", IpAddress } };
            collection.InsertOne(document);
            return "string";
        }
        public string GetUserData()
        {
            MongoClient clientDb = new MongoClient("mongodb://localhost:27017");

            var user = clientDb.GetDatabase("UserDataCollection");
            var collect = user.GetCollection<BsonDocument>("Info");

            var listDb = collect.Find(new BsonDocument()).ToList();
            BsonDocument document2 = new BsonDocument();
            foreach (var item in listDb)
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


























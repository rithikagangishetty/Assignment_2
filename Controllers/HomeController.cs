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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

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
        public string AddDateTimeIp(string Date, string Time, string IpAddress)
        {
            MongoClient clientDb = new MongoClient("mongodb://localhost:27017");

            var userDb = clientDb.GetDatabase("UserDataCollection");
            var collection = userDb.GetCollection<BsonDocument>("Info");
            var document = new BsonDocument { { "Date", Date }, { "Time", Time }, { "IP", IpAddress } };
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
            return View("ViewInfo",form);
        }

        public string DisplayCountry(string country)
        {
            return country;
        }
        [HttpGet]
        public IActionResult ViewImage()
        { 
            return View();
        }
        [HttpPost]
        public IActionResult ViewImage(CreateImage item)
        {
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
            var database = dbClient.GetDatabase("Images");
            var collection = database.GetCollection<BsonDocument>("data");
            

            GridFSBucket bucket = new GridFSBucket(database);
           var options = new GridFSUploadOptions
         
            {
                ChunkSizeBytes = 516096, // 504KB
                Metadata = new BsonDocument
    {
        { "resolution", "1080P" },
                    { "copyrighted", true }
    }
            };

            using var stream =  bucket.OpenUploadStream(item.Title, options);
           var id = stream.Id;
          
            item.Image.CopyTo(stream);
            stream.Close();
            var document = new BsonDocument { { "Title", item.Title }, { "Description", item.Description }, { "Id", id } };
            collection.InsertOne(document);
            return Redirect("/");
        }

        public ActionResult DisplayContent()
        {
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017");
            var database = dbClient.GetDatabase("Images");
            GridFSBucket bucket = new GridFSBucket(database);
            var collection = database.GetCollection<BsonDocument>("data");
            var dbList = collection.Find(new BsonDocument()).ToList();
            BsonDocument document2 = new BsonDocument();
            List<CreateImage> ImageData = new List<CreateImage>();
            foreach (var item in dbList)
            {
                document2 = item;
                var Id = document2["Id"];
                var byteArray = bucket.DownloadAsBytes(Id);
                string Image = Convert.ToBase64String(byteArray);
                string Url = string.Format("data:image/png;base64,{0}", Image);
                ImageData.Add(new CreateImage() { Url = Url, Description = document2["Description"].ToString() });

            }


            return View(ImageData);

            
        }


        public string DisplayDescription(string description)
        {
            return description;
        }


    }
}


























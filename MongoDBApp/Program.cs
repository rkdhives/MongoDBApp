
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core.Configuration;
using static System.Net.WebRequestMethods;
using System.Numerics;
using System.Security.Cryptography;


namespace MongoDBApp;

internal class Program
{
    public static void ConnectDB(string dbName)
    {

        var connectionString = "mongodb://localhost:27017";
        if (connectionString == null)
        {
            Console.WriteLine(connectionString);
            Environment.Exit(0);
        }

        var client = new MongoClient(connectionString);

        
        var collection = client.GetDatabase("testDB").GetCollection<BsonDocument>("testCollection");

        var filter = Builders<BsonDocument>.Filter.Eq("title", "testing new title");

        //var addDocument = Builders<BsonDocument>.SetFields();

        var document = collection.Find(filter).First();

        Console.WriteLine(document);
        Console.ReadLine();
    }
    static void Main(string[] args)
    {
        var connectionString = "mongodb://localhost:27017";
        Console.Write("Enter database name to create for testing: ");
        string dbName = Console.ReadLine();
        dbName = dbName.Trim().Replace(" ", "");
        Console.WriteLine("Creating database with the name as: " + dbName);
        Console.Write("How many dealers / agencies do you want to create: ");
        Int64 agencyCount = Convert.ToInt64(Console.ReadLine());
        CreateDB(dbName, connectionString);
        Console.WriteLine("Creating agencies -- started " + DateTime.Now.ToString());
        BsonDocument[] dataCol = GenerateAgencyData(agencyCount + 1, connectionString, dbName, dbName);
        InsertBulk(dataCol, connectionString, dbName, dbName).Wait();
        Console.WriteLine("Creating agencies -- completed " + DateTime.Now.ToString());
        Console.ReadKey();
    }
    public static async Task InsertData(BsonDocument data, string connectionString, string dbName, string colName)
    {        
        var client = new MongoClient(connectionString);
        var collection = client.GetDatabase(dbName).GetCollection<BsonDocument>(colName);
        await collection.InsertOneAsync(data);
    }

    /// <summary>
    /// This method is used to Insert Bulk documents
    /// </summary>
    /// <param name="data"></param>
    /// <param name="connectionString"></param>
    /// <param name="dbName"></param>
    /// <param name="colName"></param>
    /// <returns></returns>
    public static async Task InsertBulk(BsonDocument[] data, string connectionString, string dbName, string colName)
    {
        var client = new MongoClient(connectionString);
        var collection = client.GetDatabase(dbName).GetCollection<BsonDocument>(colName);
        collection.InsertManyAsync(data, new InsertManyOptions { BypassDocumentValidation = true});
    }
    public static void CreateDB(string dbName, string connectionString)
    {
        try
        {
            if (connectionString == null)
            {
                Console.WriteLine(connectionString);
                Environment.Exit(0);
            }

            var client = new MongoClient(connectionString);
            client.GetDatabase(dbName).CreateCollection(dbName);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : " + ex.Message);
        }        
    }

    public static BsonDocument[] GenerateAgencyData(Int64 agencyCount, string connectionString, string dbName, string colName)
    {
        BsonDocument[] dataCol = new BsonDocument[agencyCount];
        for (int i = 1;i < agencyCount; i++)
        {
            BsonDocument agencyBson = new BsonDocument()
            {
                {"Agency Name", "Hives Estate" + i.ToString() },
                {"Agency Email", "hivesestates@gmail.com" + i.ToString() },
                {"Address", "Fairway Commercial, CP-72, Defence Raya Golf Resort Sector M DHA Phase 6, Lahore" + i.ToString() },
                {"City", "Lahore" },
                {"Logo Image", "https://d3hyuo55idl2el.cloudfront.net/hivesestates%40gmail.com/logo.jpg?AWSAccessKeyId=AKIATLNT7BZN673NLY74&Expires=2008969198&Signature=AJfWlZ5fjQQhEnf1ejcDwB1IFIo%3D"  + i.ToString() },
                {"Lat", "31.4747293763" + i.ToString() },
                {"Long", "74.469959512" + i.ToString() },
                {"Verifed", "1" },                                                                                                                                               //},
             };
            dataCol[i] = agencyBson;
            InsertData(agencyBson, connectionString, dbName, colName);
        }
        return dataCol;
    }
}






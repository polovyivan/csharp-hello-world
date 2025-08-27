// This is a .NET 5 (and earlier) console app template
// (See https://aka.ms/new-console-template for more information)
using NLog;
using NLog.Targets;
using System.Text;

// Using vulnerable dependencies for Dependency Track testing
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using System.Text.Encodings.Web;
using System.Net.Http;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;

namespace MyApp
{
    internal class HelloWorld
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        // Also initialize log4net logger (vulnerable version)
        private static readonly log4net.ILog Log4NetLogger = log4net.LogManager.GetLogger(typeof(HelloWorld));

        static void Main(string[] args)
        {
            Logger.Info("Starting program ...");
            
            // Initialize log4net (vulnerable version 2.0.8)
            // Create a console appender and configure log4net
            var consoleAppender = new log4net.Appender.ConsoleAppender();
            consoleAppender.Layout = new log4net.Layout.SimpleLayout();
            Log4NetLogger.Info("Log4net initialized (vulnerable version for testing)");
            
            string LOGDIR = Environment.GetEnvironmentVariable("LOGDIR");
            Logger.Debug("LOGDIR is set to {0}", LOGDIR);
            
            // Just for testing:
            // Logger.Trace("Trace");
            // Logger.Debug("Debug");
            // Logger.Info("Info");
            // Logger.Warn("Warn");
            // Logger.Error("Error");
            // Logger.Fatal("Fatal");
            
            Console.WriteLine("Hello, World!");
            
            // Use vulnerable Newtonsoft.Json (version 12.0.1)
            TestVulnerableJsonSerialization();
            
            // Use vulnerable System.Text.Encodings.Web
            TestVulnerableEncoder();
            
            // Use vulnerable System.Net.Http
            TestVulnerableHttpClient();
            
            // Use vulnerable System.Drawing.Common
            TestVulnerableImageProcessing();
            
            // Use vulnerable Microsoft.AspNetCore.Http
            TestVulnerableHttpContext();
            
            double x = 1.234;
            double y = 4.321;
            Logger.Debug("calling Library.MyMath.Add(x, y) with x={0} and y={1} ...", x, y);
            double sum = Library.MyMath.Add(x, y);
            
            Logger.Debug("calling Library.MyMath.Multiply(x, y) with x={0} and y={1} ...", x, y);
            double prod = Library.MyMath.Multiply(x, y);
            
            Console.WriteLine(String.Format("{0} plus {1} makes {2}", x, y, sum));
            Console.WriteLine(String.Format("{0} times {1} makes {2}", x, y, prod));
            
            Library.DataStore<int, string> myData = new Library.DataStore<int, string>();
            for (int i = 0; i < 100; i++)
            {
                string text = string.Format("This is element {0}.", i);
                myData.Add(i, text);
            }
            
            PrintElement(myData, 42);
            PrintElement(myData, 100);
            PrintElement(myData, 101);
            PrintElement(myData, 102);
            
            Logger.Info("Terminating program ...");
        }
        
        // Method using vulnerable Newtonsoft.Json
        private static void TestVulnerableJsonSerialization()
        {
            Logger.Info("Testing vulnerable JSON serialization...");
            
            var testObject = new
            {
                Name = "Test User",
                Age = 30,
                Email = "test@example.com",
                Timestamp = DateTime.Now
            };
            
            // Serialize with vulnerable Newtonsoft.Json 12.0.1
            string json = JsonConvert.SerializeObject(testObject, Formatting.Indented);
            Logger.Debug("Serialized JSON: {0}", json);
            
            // Parse JSON
            JObject parsed = JObject.Parse(json);
            string name = (string)parsed["Name"];
            Logger.Debug("Parsed name from JSON: {0}", name);
            
            // Deserialize
            dynamic deserialized = JsonConvert.DeserializeObject(json);
            Logger.Debug("Deserialized object name: {0}", deserialized.Name);
        }
        
        // Method using vulnerable System.Text.Encodings.Web
        private static void TestVulnerableEncoder()
        {
            Logger.Info("Testing vulnerable text encoder...");
            
            // Create encoder (vulnerable version 4.7.0)
            var encoder = HtmlEncoder.Default;
            
            string userInput = "<script>alert('XSS')</script>";
            string encoded = encoder.Encode(userInput);
            Logger.Debug("Encoded HTML: {0}", encoded);
            
            // Also test JavaScript encoder
            var jsEncoder = JavaScriptEncoder.Default;
            string jsEncoded = jsEncoder.Encode("alert('test')");
            Logger.Debug("Encoded JavaScript: {0}", jsEncoded);
        }
        
        // Method using vulnerable System.Net.Http
        private static void TestVulnerableHttpClient()
        {
            Logger.Info("Testing vulnerable HTTP client...");
            
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    
                    // Just create a request, don't actually send it
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
                    request.Headers.Add("User-Agent", "VulnerableApp/1.0");
                    
                    Logger.Debug("Created HTTP request with vulnerable client");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("HTTP client error: {0}", ex.Message);
            }
        }
        
        // Method using vulnerable System.Drawing.Common
        private static void TestVulnerableImageProcessing()
        {
            Logger.Info("Testing vulnerable image processing...");
            
            try
            {
                // Note: System.Drawing.Common might not work on non-Windows platforms in .NET 6+
                // but we're including it for vulnerability testing purposes
                if (OperatingSystem.IsWindows())
                {
                    // Create a simple bitmap (vulnerable version 4.5.0)
                    using (Bitmap bitmap = new Bitmap(100, 100))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.White);
                            g.DrawString("Test", new Font("Arial", 12), Brushes.Black, new PointF(10, 10));
                        }
                        
                        Logger.Debug("Created bitmap with vulnerable System.Drawing.Common");
                    }
                }
                else
                {
                    Logger.Info("Skipping image processing test on non-Windows platform");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Image processing error: {0}", ex.Message);
            }
        }
        
        // Method using vulnerable Microsoft.AspNetCore.Http
        private static void TestVulnerableHttpContext()
        {
            Logger.Info("Testing vulnerable HTTP context...");
            
            try
            {
                // Create a default HTTP context (vulnerable version 2.2.0)
                var context = new DefaultHttpContext();
                
                // Set some headers
                context.Request.Headers["Content-Type"] = "application/json";
                context.Request.Method = "GET";
                context.Request.Path = "/api/test";
                
                // Work with the request and response objects
                context.Request.ContentType = "application/json";
                context.Response.StatusCode = 200;
                
                // Test PathString operations (part of Microsoft.AspNetCore.Http)
                PathString path = new PathString("/api/v1/users");
                PathString basePath = new PathString("/api");
                PathString remainingPath;
                
                bool matched = path.StartsWithSegments(basePath, out remainingPath);
                Logger.Debug("Path matching result: {0}, remaining: {1}", matched, remainingPath);
                
                // Test HeaderDictionary
                var headers = new HeaderDictionary();
                headers["X-Custom-Header"] = "TestValue";
                headers["Authorization"] = "Bearer token123";
                
                Logger.Debug("Created HTTP context with path: {0}", context.Request.Path);
                Logger.Debug("Custom headers count: {0}", headers.Count);
            }
            catch (Exception ex)
            {
                Logger.Error("HTTP context error: {0}", ex.Message);
            }
        }
        
        public static void PrintElement(Library.DataStore<int, string> Store, int index)
        {
            Library.Pair<int, string>? element = Store.GetElementByIndex(index);
            if (element is Library.Pair<int, string> valueOfElment)
            {
                Logger.Trace("idx {0}: found element", index);
                Console.WriteLine(String.Format("idx {0}: key {1}, value {2}", index,
                    element.GetKey(), element.GetValue()));
            }
            else
            {
                Logger.Warn("idx {0}: no such element in DataStore", index);
                Console.WriteLine(String.Format("idx {0}: no such element in DataStore", index));
            }
        }
    } // class HelloWorld
} // namespace MyApp
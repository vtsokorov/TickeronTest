/*
 * Required version .NET Framework: .NET Framework 4.5
 * To launch, add to the project the following references:
 *        -> System.Net.Http
 *        -> System.Web
 *        -> System.Web.Extensions
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

//Add a reference to System.Net.Http in project
using System.Net.Http;
using System.Net.Http.Headers;

//Add a reference to System.Web in project
using System.Web.UI;

//Add a reference to System.Web.Extensions in project
using System.Web.Script.Serialization;


namespace TickeronTest
{
    class Program
    {
        private const String url = "https://min-api.cryptocompare.com/data/pricemulti";

        private static String getJson(String[] crypto, String currency)
        {
            String jsonData = null;
            try
            {
                String fsyms = "?fsyms=" + string.Join(",", crypto) + '&';
                String tsyms = "tsyms=" + currency;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(fsyms + tsyms).Result;
                if (response.IsSuccessStatusCode)
                    jsonData = response.Content.ReadAsStringAsync().Result;

                client.Dispose();
            }
            catch (System.Exception)
            {
                Console.WriteLine("Check Internet connection...");
            }

            return jsonData;
        }

        private static double getValueFromJson(String json, String crypto, String currency)
        {
            var serializer = new JavaScriptSerializer();
            Dictionary<String, object> data = (serializer.DeserializeObject(json) as Dictionary<String, object>);
            Dictionary<String, object> value = (data[crypto] as Dictionary<String, object>);
            return double.Parse(value[currency].ToString());
        }

        private static void showDataAccessError(string json)
        {
            var serializer = new JavaScriptSerializer();
            Dictionary<String, object> result = (serializer.DeserializeObject(json) as Dictionary<String, object>);
            String response = result["Response"].ToString();
            String message = result["Message"].ToString();
            Console.WriteLine(response + ": " + message);
        }

        public static void task1()
        {
            String data = getJson(new[] { "BTC", "ETH", "DASH" }, "USD");
            if (data != null)
            {
                Console.WriteLine("Today's price for Bitcoin (BTC): {0}", getValueFromJson(data, "BTC", "USD"));
            }
            else
                Console.WriteLine("No data available...");
        }

        public static void task2()
        {
            Console.Write("Input a cryptocurrency ticker: ");
            string ticker = Console.ReadLine();
            String data = getJson(new[] { ticker }, "USD");
            try
            {
                if (data != null)
                {
                    Console.WriteLine("Value cryptocurrency ticker: {0}", getValueFromJson(data, ticker, "USD"));
                }
                else
                    Console.WriteLine("No data available...");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                showDataAccessError(data);
            }
        }

        public static void task3()
        {
            String data = getJson(new[] { "BTC", "ETH", "DASH" }, "USD");
            if (data != null)
            {
                Console.WriteLine("Today's price for BTC: {0}", getValueFromJson(data, "BTC", "USD"));
                Console.WriteLine("Today's price for ETH: {0}", getValueFromJson(data, "ETH", "USD"));
                Console.WriteLine("Today's price for DASH: {0}", getValueFromJson(data, "DASH", "USD"));
            }
            else
                Console.WriteLine("No data available...");
        }

        public static void task4()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) + @"\JSON-DATA.txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
            {
                String data = getJson(new[] { "BTC", "ETH", "DASH" }, "USD");
                if (data != null)
                {
                    Console.WriteLine("Write file: {0}", filePath);
                    file.WriteLine(data);
                }
                else
                    Console.WriteLine("No data available...");

            }
        }

        public static void task5()
        {
            String[] tickers = { "BTC", "ETH", "DASH" };
            String data = getJson(tickers, "USD");
            if (data != null)
            {
                StringWriter stringWriter = new StringWriter();
                using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Html);
                    writer.RenderBeginTag(HtmlTextWriterTag.Head);
                    writer.RenderBeginTag(HtmlTextWriterTag.Title);
                    writer.Write("index");
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Body);
                    writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Cryptocurrency");
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Currency");
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Th);
                    writer.Write("Price");
                    writer.RenderEndTag();
                    writer.RenderEndTag();

                    for (int i = 0; i < tickers.Length; ++i)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(tickers[i]);
                        writer.RenderEndTag();
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write("USD");
                        writer.RenderEndTag();
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(getValueFromJson(data, tickers[i], "USD"));
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }

                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)) + @"\INDEX.html";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
                {
                    file.WriteLine(stringWriter.ToString());
                    Console.WriteLine("Write file: {0}", filePath);
                }
            }
            else
                Console.WriteLine("No data available...");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Task 1");
            task1();
            Console.WriteLine("Task 2");
            task2();
            Console.WriteLine("Task 3");
            task3();
            Console.WriteLine("Task 4");
            task4();
            Console.WriteLine("Task 5");
            task5();
            Console.ReadKey();
        }
    }
}

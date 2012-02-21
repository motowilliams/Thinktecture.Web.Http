using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using ContactManager.Formatters;
using ContactManager.Models;

namespace ConsoleClient
{
    class Program
    {
        private static string contactsUri = "http://renovator/cm/contacts";
        private static string contactUri = "http://renovator/cm/contact/1";
        private static string mediaType = "application/x-protobuf";
        //private static string mediaType = "application/json";

        static void Main(string[] args)
        {
            List<MediaTypeFormatter> formatters;
            HttpClient client;

            SetupClient(out formatters, out client);

            GetContacts(formatters, client);
            
            //PostContacts(formatters, client);
            //GetContacts(formatters, client);

            Console.ReadKey();
        }

        private static void PostContacts(List<MediaTypeFormatter> formatters, HttpClient client)
        {
            var contact = 
                new Contact
                {
                    Name = "Diana Weyer",
                    Address = "Wire Drive",
                    City = "Neustadt",
                    State = "Bayern",
                    Zip = "98765",
                    Email = "diana.weyer@thinktecture.com" 
                };
            
            var r = new HttpRequestMessage<Contact>(
                contact, MediaTypeHeaderValue.Parse(mediaType), formatters);

            var result = client.PostAsync(contactsUri, r.Content).Result;
        }

        private static void GetContact(List<MediaTypeFormatter> formatters, HttpClient client)
        {
            var sw = new Stopwatch();
            sw.Start();

            var response = client.GetAsync(contactUri).Result;
            var contact = response.Content.ReadAsAsync<Contact>(formatters).Result;

            sw.Stop();
            Console.WriteLine("Time elapsed in ms: {0}", sw.ElapsedMilliseconds);
        }

        private static void GetContacts(IEnumerable<MediaTypeFormatter> formatters, HttpClient client)
        {
            var sw = new Stopwatch();
            sw.Start();

            var response = client.GetAsync(contactsUri).Result;
            var contacts = response.Content.ReadAsAsync<List<Contact>>(formatters).Result;

            sw.Stop();

            Console.WriteLine("Time elapsed in ms: {0}", sw.ElapsedMilliseconds);
            Console.WriteLine("Number of contacts: {0}",contacts.Count);
        }

        private static void SetupClient(out List<MediaTypeFormatter> formatters, out HttpClient client)
        {
            MediaTypeFormatter.SkipStreamLimitChecks = true;

            formatters = new List<MediaTypeFormatter> 
            {
                new JsonMediaTypeFormatter(),
                new ProtoBufFormatter() 
            };

            // HttpClient is a Disposable!
            var config = new HttpClientHandler {  };
            client = new HttpClient(config);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(mediaType));
        }
    }
}

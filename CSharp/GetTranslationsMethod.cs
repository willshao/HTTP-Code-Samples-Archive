using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

namespace MicrosoftTranslatorSdk.HttpSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            AdmAuthentication admAuth = new AdmAuthentication("clientID", "client secret");
            try
            {
                admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                GetTranslationsMethod(headerValue);
            }
            catch (WebException e)
            {
                ProcessWebException(e);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }
        private static void GetTranslationsMethod(string authToken)
        {
            
            string text = "una importante contribución a la rentabilidad de la empresa";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetTranslations?text=" + text + "&from=" + "es" + "&to=" + "en" + "&maxTranslations=5";


            string requestBody = GenerateTranslateOptionsRequestBody("general", "text/plain", "", "", "", "TestUserId");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", authToken);
            request.ContentType = "text/xml";
            request.Method = "POST";
            using (System.IO.Stream stream = request.GetRequestStream())
            {
                byte[] arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(requestBody);
                stream.Write(arrBytes, 0, arrBytes.Length);
            }
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (Stream respStream = response.GetResponseStream())
                {
                    StreamReader rdr = new StreamReader(respStream, System.Text.Encoding.ASCII);
                    string strResponse = rdr.ReadToEnd();

                    Console.WriteLine(string.Format("Available translations for source text '{0}' are", text));
                    XDocument doc = XDocument.Parse(@strResponse);
                    XNamespace ns = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2";
                    int i = 1;
                    foreach (XElement xe in doc.Descendants(ns + "TranslationMatch"))
                    {
                        Console.WriteLine("{0}Result {1}", Environment.NewLine, i++);
                        foreach (var node in xe.Elements())
                        {
                            Console.WriteLine("{0} = {1}", node.Name.LocalName, node.Value);
                        }
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

        }
        // builds the outter xml body
        private static string GenerateTranslateOptionsRequestBody(string category, string contentType, string ReservedFlags, string State, string Uri, string user)
        {
            string body = "<TranslateOptions xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">" +
    "  <Category>{0}</Category>" +
    "  <ContentType>{1}</ContentType>" +
    "  <ReservedFlags>{2}</ReservedFlags>" +
    "  <State>{3}</State>" +
    "  <Uri>{4}</Uri>" +
    "  <User>{5}</User>" +
    "</TranslateOptions>";
            return string.Format(body, category, contentType, ReservedFlags, State, Uri, user);
        }
        private static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }
    }
}

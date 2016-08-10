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
                GetTranslationsArrayMethod(headerValue);
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
        private static void GetTranslationsArrayMethod(string authToken)
        {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetTranslationsArray";
            string requestBody = String.Format("<GetTranslationsArrayRequest>" +
           "  <AppId>{0}</AppId>" +
           "  <From>{1}</From>" +
           "  <Options>" +
           "  <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">general</Category>" +
           "  <ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">text/plain</ContentType>" +
           "  <ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\"/>" +
           "  <State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\"/>" +
           "  <Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{2}</Uri>" +
           "  <User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{3}</User>" +
           "  </Options>" +
           "  <Texts>{6}</Texts>" +
           "  <To>{4}</To>" +
           "  <MaxTranslations>{5}</MaxTranslations>" +
           "</GetTranslationsArrayRequest>", "", "es", "", "TestUserId", "en", "3", "{0}");
            string translationsCollection = String.Empty;
            string[] textTranslations = { "a veces los errores son divertidos", " una importante contribución a la rentabilidad de la empresa" };
            // build the Translations collection
            translationsCollection += String.Format("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", textTranslations[0]);
            translationsCollection += String.Format("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", textTranslations[1]);
            // update the body
            requestBody = string.Format(requestBody, translationsCollection);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", authToken);
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
                    XDocument doc = XDocument.Parse(@strResponse);
                    XNamespace ns = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2";
                    int i = 0;
                    foreach (XElement xe in doc.Descendants(ns + "GetTranslationsResponse"))
                    {
                        Console.WriteLine("\n\nSource Text: '{0}' Results", textTranslations[i++]);
                        int j = 1;
                        foreach (XElement xe2 in xe.Descendants(ns + "TranslationMatch"))
                        {
                            Console.WriteLine("\nCustom translation :{0} ", j++);
                            foreach (var node in xe2.Elements())
                            {
                                Console.WriteLine("{0} = {1}", node.Name.LocalName, node.Value);
                            }
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

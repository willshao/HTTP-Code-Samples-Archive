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
                AddTranslationArrayMethod(headerValue);

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

        private static void AddTranslationArrayMethod(string authToken)
        {
            string appId = "";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/AddTranslationArray";
            string originalText1 = "una importante contribución a la rentabilidad de la empresa";
            string translatedText1 = "a significant contribution tothe company profitability";
            string originalText2 = "a veces los errores son divertidos";
            string translatedText2 = "in some cases errors are fun";


            string body = GenerateAddtranslationRequestBody(appId, "es", "en", "general", "text/plain", "", "TestUserId");
            string translationsCollection = string.Format("{0}{1}",
                GenerateAddtranslationRequestElement(originalText1, 8, 0, translatedText1),
                GenerateAddtranslationRequestElement(originalText2, 6, 0, translatedText2));

            // update the body
            string requestBody = string.Format(body, translationsCollection);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "text/xml";
            request.Method = "POST";
            request.Headers.Add("Authorization", authToken);
            using (System.IO.Stream stream = request.GetRequestStream())
            {
                
                byte[] arrBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(requestBody);
                stream.Write(arrBytes, 0, arrBytes.Length);
            }

            // get the response
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (Stream respStream = response.GetResponseStream())
                {
                    Console.WriteLine(string.Format("Your translations for '{0}' and '{1}' has been added successfully.", originalText1, originalText2));
                }
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
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

        }

        private static string GenerateAddtranslationRequestBody(string appId, string from, string to, string category, string contentType, string uri, string user)
        {
            string body = "<AddtranslationsRequest>" +
                             "<AppId>{0}</AppId>" +
                             "<From>{1}</From>" +
                             "<Options>" +
                               "<Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{2}</Category>" +
                               "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{3}</ContentType>" +
                               "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{4}</User>" +
                               "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{5}</Uri>" +
                             "</Options>" +
                             "<To>{6}</To>" +
                             "<Translations>{7}</Translations>" +
                           "</AddtranslationsRequest>";
            return string.Format(body, appId, from, category, contentType, user, uri, to, "{0}");



        }

        private static string GenerateAddtranslationRequestElement(string originalText, int rating, int sequence, string translatedText)
        {
            string element = "<Translation xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">" +
                "<OriginalText>{0}</OriginalText>" +
                "<Rating>{1}</Rating>" +
                "<TranslatedText>{2}</TranslatedText>" +
                "<Sequence>{3}</Sequence>" +
                "</Translation>";
            return string.Format(element, originalText, rating.ToString(), translatedText, sequence.ToString());
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

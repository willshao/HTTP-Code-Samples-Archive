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
                DetectArrayMethod(headerValue);

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

        private static void DetectArrayMethod(string authToken)
        {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/DetectArray";
            // create the request
            string[] textArray = new string[2] { "les erreurs sont parfois amusants", "you can try to enter a longer phrase" };
            StringWriter swriter = new StringWriter();
            XmlTextWriter xwriter = new XmlTextWriter(swriter);
            xwriter.WriteStartElement("ArrayOfstring");
            xwriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
            foreach (string text in textArray)
            {
                xwriter.WriteStartElement("string");
                xwriter.WriteString(text);
                xwriter.WriteEndElement();
            }
            xwriter.WriteEndElement();
            xwriter.Close();
            swriter.Close();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", authToken);
            request.ContentType = "text/xml";
            request.Method = "POST";
            using (System.IO.Stream stream = request.GetRequestStream())
            {
                byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(swriter.ToString());
                stream.Write(arrBytes, 0, arrBytes.Length);
            }
            // get the response
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {

                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String[]"));
                    string[] detectArray = (string[])dcs.ReadObject(stream);
                    Console.WriteLine("The detected languages are: ");
                    for (int i = 0; i < detectArray.Length; i++)
                    {
                        Console.WriteLine(string.Format("Text '{0}' is from Language {1}", textArray[i], detectArray[i]));
                    }
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
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
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

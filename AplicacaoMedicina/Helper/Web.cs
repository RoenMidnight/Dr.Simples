using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace AplicacaoMedicina.Helper
{
    public static class Web
    {
        public static string GetHTML(string URL)
        {
            string connectionString = URL;

            try
            {
                System.Net.HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(connectionString);
                myRequest.Credentials = CredentialCache.DefaultCredentials;

                WebResponse webResponse = myRequest.GetResponse();
                Stream respStream = webResponse.GetResponseStream();

                StreamReader ioStream = new StreamReader(respStream);
                string pageContent = ioStream.ReadToEnd();

                ioStream.Close();
                respStream.Close();

                return pageContent;               

            } catch (Exception e) {
                return e.Message;
            }

          //  return URL;
        }

    }
}
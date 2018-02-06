using System.Security.Cryptography;
using System.Text;
using AplicacaoMedicina.Helper;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace AplicacaoMedicina.Helper
{
    public class FacebookHelper
    {        
        private const string Facebook_GraphAPI_Token = "https://graph.facebook.com/oauth/access_token?";
        private const string Facebook_GraphAPI_Me    = "https://graph.facebook.com/me?";
        private const string AppID                   = "1115640525247131";
        private const string facebookAuth            = "caf362eb4e6eadfd6ebdf4f4dec4dec0";

        public static string GetUserDate(string token, string accessCode)
        {
           /* string token = Web.GetHTML(Facebook_GraphAPI_Token +"client_id="+AppID+ "&redirect_uri="+ HttpUtility.HtmlEncode(redirectURI) + 
                "%3F__provider__%3Dfacebook" + "&client_secret=" + facebookAuth + "&code=" + accessCode);

            if (token == null || token == "")
            {
                return null;
            } */

            string data = Web.GetHTML(Facebook_GraphAPI_Me +"access_token=" + token + "&appsecret_proof=" + accessCode + "&fields=id,name,email,gender,locale");

            /* var userData = new Dictionary<string, string>();
             userData.Add("id", data.Substring("\"id\":\"", "\""));
             userData.Add("username", data.Substring("username\":\"", "\""));
             userData.Add("name", data.Substring("name\":\"", "\""));
             userData.Add("link", data.Substring("link\":\"", "\"").Replace("\\/", "/"));
             userData.Add("gender", data.Substring("gender\":\"", "\""));
             userData.Add("email", data.Substring("email\":\"", "\"").Replace("\\u0040", "@"));
             userData.Add("accesstoken", token.Substring("access_token=", "&")); */

            return data.Replace(@"\u0040", "@"); 

        }
        

        public static string FacebookSecretProof(string faceAccessToken)
        {
            byte[] keyByte     = Encoding.ASCII.GetBytes(facebookAuth);
            byte[] messageByte = Encoding.ASCII.GetBytes(faceAccessToken);
            byte[] tokenBytes  = new HMACSHA256(keyByte).ComputeHash(messageByte);

            StringBuilder token = new StringBuilder();
            foreach (byte b in tokenBytes)
            {
                token.AppendFormat("{0:x2}", b);
            }
            
            return GetUserDate(faceAccessToken,token.ToString());

        }


    }
}
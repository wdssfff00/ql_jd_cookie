﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JD_Get
{
    public class QLHelp
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }

        public QLHelp(string url, string clientID, string clientSecret)
        {
            Url = url;
            ClientID = clientID;
            ClientSecret = clientSecret;
            Token = "";
        }

      
        public string GetResponse(string url, out string statusCode)
        {
            

            var options = new RestClientOptions(Url);

            //if (!string.IsNullOrEmpty(Token))
            //    options.Authenticator = new HttpBasicAuthenticator(Token.Split(' ')[0], Token.Split(' ')[1]);

            var client = new RestClient(options);
            var request = new RestRequest(url);
            if (!string.IsNullOrEmpty(Token)) {
                //request.Authenticator = new HttpBasicAuthenticator(Token.Split(' ')[0], Token.Split(' ')[1]);
                request.AddOrUpdateHeader("Authorization", Token);
            }
             

            // The cancellation token comes from the caller. You can still make a call without it.
            var response = client.Get(request);
            statusCode = response.StatusCode.ToString();
            return response.Content;
        }
        public string PutResponse(string url,  string postData, out string statusCode)
        {
            var options = new RestClientOptions(Url);

            
            var client = new RestClient(options);
            var request = new RestRequest(url);
            request.AddStringBody(postData, ContentType.Json);
            if (!string.IsNullOrEmpty(Token))
            {
                //request.Authenticator = new HttpBasicAuthenticator(Token.Split(' ')[0], Token.Split(' ')[1]);
                request.AddOrUpdateHeader("Authorization", Token);
            }
            var response = client.Put(request);
            statusCode = response.StatusCode.ToString();
            return response.Content;
        }
        public string PostResponse(string url, string postData, out string statusCode)
        {
            var options = new RestClientOptions(Url); 

            var client = new RestClient(options);
            var request = new RestRequest(url);
            //request.AddStringBody(postData, ContentType.Json);
            request.AddStringBody(postData, ContentType.Json);
            if (!string.IsNullOrEmpty(Token))
            { 
                request.AddOrUpdateHeader("Authorization", Token);
            }
            try
            {
                var response = client.Post(request);
                statusCode = response.StatusCode.ToString();
                return response.Content;
            }
            catch(Exception e)
            {
                statusCode = "";
                return "";
            }
          
        }

        public  string Login()
        {
            string code = "";
            //var responseData = GetResponse($"{Url}/open/auth/token?client_id={ClientID}&client_secret={ClientSecret}",
            //    out code);
            //JObject jsonObj = JObject.Parse(responseData);
            
            //return this.Token;
            var responseData = GetResponse($"/open/auth/token?client_id={ClientID}&client_secret={ClientSecret}", out code);
            JObject jsonObj = JObject.Parse(responseData);
            this.Token = jsonObj["data"]["token_type"].ToString() + " " + jsonObj["data"]["token"].ToString();
            return this.Token;
        }

        /// <summary>
        /// 查找对应的环境变量
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public string GetEnvs(string searchValue)
        {
            string code = "";
            var responseData = GetResponse($"/open/envs?searchValue={searchValue}",
                out code);
            JObject jsonObj = JObject.Parse(responseData);
            if (jsonObj["data"].Count() == 0)
            {
                return "";
            }
            return jsonObj["data"][0]["id"].ToString();
        }  
        /// <summary>
        /// 更新环境变量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string UpdateEnvs(string id,string value)
        {
            string code = "";
            JObject patientinfo = new JObject();
            patientinfo["id"] = id;
            patientinfo["value"] = value;
            patientinfo["name"] = "JD_COOKIE";
            string postdata = JsonConvert.SerializeObject(patientinfo);

            var responseData = PutResponse($"/open/envs", postdata,
                out code);
           

            JObject jsonObj = JObject.Parse(responseData);
            //this.Token = jsonObj["code"]["token_type"].ToString() + " " + jsonObj["data"]["token"].ToString();
            return jsonObj["code"].ToString();
        }
        /// <summary>
        /// 添加环境变量
        /// </summary>
        /// <param name="remarks"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string AddEnvs(string remarks, string value)
        {
            string code = ""; 
            var requestData = new[]
            {
                new
                {
                    value = value,
                    name = "JD_COOKIE",
                    remarks=remarks
                }
            };
            string postdata = JsonConvert.SerializeObject(requestData);
            var responseData = PostResponse($"/open/envs", postdata,
                out code);


            JObject jsonObj = JObject.Parse(responseData);
            //this.Token = jsonObj["code"]["token_type"].ToString() + " " + jsonObj["data"]["token"].ToString();
            return jsonObj["code"].ToString();
        }

        /// <summary>
        /// 启用环境变量
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string EnableEnvs(List<string> ids)
        {
            string code = "";

            string postdata= JsonConvert.SerializeObject(ids);
            var responseData = PutResponse($"/open/envs/enable", postdata,
                out code);
            JObject jsonObj = JObject.Parse(responseData); 
            return jsonObj["code"].ToString();
        }

    }
}

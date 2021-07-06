using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MultiWPFApp.Model
{
    public class ServerIoT
    {
        private string ip;

        public ServerIoT(string _ip)
        {
            ip = _ip;
        }
        public string ScriptUrl
        {
            get => "http://" + ip + "/cgi-bin/led_display.py";
        }
        public async Task<string> PostControlRequest(List<KeyValuePair<string, string>> data)
        {
            string responseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var requestData = new FormUrlEncodedContent(data);
                    // Sent POST request
                    var result = await client.PostAsync(ScriptUrl, requestData);
                    // Read response content
                    responseText = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }
        private string GetFileUrl(string file)
        {
            return "http://" + ip + "/" + file + ".json";
        }

        private string GetScriptUrl(string script)
        {
            return "http://" + ip + "/" + script;
        }
        private string GetPixelsUrl()
        {
            return "http://" + ip + "/cgi-bin/get_pixels2.py";
        }

        private string GetMeasurementsUrl()
        {
            return "http://" + ip + "/get_measurements.php";
        }

        public async Task<string> POSTwithClient(string script, string file)
        {
            string responseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // POST request data
                    var requestDataCollection = new List<KeyValuePair<string, string>>();
                    requestDataCollection.Add(new KeyValuePair<string, string>("filename", file));
                    var requestData = new FormUrlEncodedContent(requestDataCollection);
                    // Sent POST request
                    var result = await client.PostAsync(GetScriptUrl(script), requestData);
                    // Read response content
                    responseText = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }

        public string POSTgetPixels()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetPixelsUrl());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string result = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return result;
        }

        public string POSTgetMeasurements()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetMeasurementsUrl());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string result = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return result;
        }
    }
}

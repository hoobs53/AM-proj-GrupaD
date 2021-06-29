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
        Random rand = new Random();

        public JArray getMeasurements()
        {
            string jsonText = "[";

            jsonText += "{\"Name\":\"Temperature\",\"Data\":" + (23.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"C\"},";
            jsonText += "{\"Name\":\"Pressure\",\"Data\":" + (1023.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"hPa\"},";
            jsonText += "{\"Name\":\"Humidity\",\"Data\":" + (43.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"%\"},";

            jsonText += "]";

            return JArray.Parse(jsonText);
        }

        public JArray getOrientation()
        {
            string jsonText = "[";

            jsonText += "{\"Name\":\"Roll\",\"Data\":" + (180.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"Deg\"},";
            jsonText += "{\"Name\":\"Pitch\",\"Data\":" + (0.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"Deg\"},";
            jsonText += "{\"Name\":\"Yaw\",\"Data\":" + (270.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"Deg\"}";

            jsonText += "]";

            return JArray.Parse(jsonText);
        }

        public JArray getJoystick()
        {
            string jsonText = "[";

            jsonText += "{\"Name\":\"x\",\"Data\":" + (180.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"-\"},";
            jsonText += "{\"Name\":\"y\",\"Data\":" + (0.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"-\"},";
            jsonText += "{\"Name\":\"mid_counter\",\"Data\":" + (270.0 + rand.NextDouble()).ToString(CultureInfo.InvariantCulture) + ",\"Unit\":\"-\"}";

            jsonText += "]";

            return JArray.Parse(jsonText);
        }

        private string ip;

        public ServerIoT(string _ip)
        {
            ip = _ip;
        }
        public ServerIoT()
        {

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

        private string GetScriptUrl()
        {
            return "http://" + ip + "/resource.php";
        }
        private string GetPixelsUrl()
        {
            return "http://" + ip + "/cgi-bin/get_pixels.py";
        }
        //public async Task<string> GETwithClient()
        //{
        //    string responseText = null;

        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            responseText = await client.GetStringAsync(GetFileUrl());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("NETWORK ERROR");
        //        Debug.WriteLine(e);
        //    }

        //    return responseText;
        //}

        public async Task<string> POSTwithClient(string file)
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
                    var result = await client.PostAsync(GetScriptUrl(), requestData);
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

        //public async Task<string> GETwithRequest()
        //{
        //    string responseText = null;

        //    try
        //    {
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetFileUrl());

        //        request.Method = "GET";

        //        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        //        using (Stream stream = response.GetResponseStream())
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            responseText = await reader.ReadToEndAsync();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("NETWORK ERROR");
        //        Debug.WriteLine(e);
        //    }

        //    return responseText;
        //}

        public async Task<string> POSTwithRequest()
        {
            string responseText = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetScriptUrl());

                // POST Request data 
                var requestData = "filename=chartdata";
                byte[] byteArray = Encoding.UTF8.GetBytes(requestData);
                // POST Request configuration
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                // Wrire data to request stream
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseText = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }
    }
}

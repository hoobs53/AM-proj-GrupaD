using System.IO;
using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MultiWPFApp.Model
{
    public class ConfigParams
    {
        private ConfigData configData;
        static readonly string ipAddressDefault = "192.168.1.104";
        public string Url;
        static readonly int sampleTimeDefault = 500;
        public int SampleTime;
        public readonly int MaxSampleNumber = 100;

        static readonly string apiDefault = "20";
        public string Api;

        static readonly string portDefault = "80";
        public string Port;
        private void LoadConfig()
        {
            using (StreamReader r = new StreamReader("config.json"))
            {
                string json = r.ReadToEnd();
                configData = JsonConvert.DeserializeObject<ConfigData>(json);
            }
        }
        public double XAxisMax
        {
            get
            {
                return MaxSampleNumber * SampleTime / 1000.0;
            }
            private set { }
        }
        public ConfigParams()
        {
            try
            {
                LoadConfig();
                if (configData != null)
                {
                    Url = configData.url;
                    SampleTime = configData.sample_time;
                    Api = configData.api;
                    Port = configData.port;
                }
                else
                {
                    Url = ipAddressDefault;
                    SampleTime = sampleTimeDefault;
                    Api = apiDefault;
                    Port = portDefault;
                }
            }
            catch (Exception e)
            {
                Url = ipAddressDefault;
                SampleTime = sampleTimeDefault;
                Api = apiDefault;
                Port = portDefault;
                Debug.WriteLine(e);
            }
        }

        public ConfigParams(string ip, int st)
        {
            try
            {
                LoadConfig();
                Port = configData.port;
                Api = configData.api;
            }
            catch (Exception e)
            {
                Api = apiDefault;
                Port = portDefault;
                Debug.WriteLine(e);
            }
            Url = ip;
            SampleTime = st;

        }
    }
}

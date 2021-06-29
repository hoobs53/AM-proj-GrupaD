using System.IO;
using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MultiWPFApp.Model
{
    public class ConfigParams
    {
        private ConfigData configData;
        public readonly string urlDefault = "192.168.1.101";
        public string Url;

        public readonly int sampleTimeDefault = 500;
        public int SampleTime;

        public readonly int maxSampleDefault = 100;
        public int MaxSample;

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
                return maxSampleDefault * SampleTime / 1000.0;
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
                    MaxSample = configData.sample_amount;
                }
                else
                {
                    Url = urlDefault;
                    SampleTime = sampleTimeDefault;
                    MaxSample = maxSampleDefault;
                }
            }
            catch (Exception e)
            {
                Url = urlDefault;
                SampleTime = sampleTimeDefault;
                MaxSample = maxSampleDefault;
                Debug.WriteLine(e);
            }
        }

        public ConfigParams(string ip, int st, int ms)
        {
            try
            {
                LoadConfig();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            Url = ip;
            SampleTime = st;
            MaxSample = ms;
        }
    }
}

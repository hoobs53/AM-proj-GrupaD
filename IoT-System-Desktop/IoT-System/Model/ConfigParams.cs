using System.IO;
using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MultiWPFApp.Model
{
    public class ConfigParams
    {
        private ConfigData configData;
        public readonly string ipDefault = "192.168.1.101";
        public string Ip;

        public readonly int sampleTimeDefault = 500;
        public int SampleTime;

        public readonly int maxSampleDefault = 100;
        public int MaxSample;
        
        private void LoadParams()
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
                return MaxSample * SampleTime / 1000.0;
            }
            private set { }
        }
        public ConfigParams()
        {
            try
            {
                LoadParams();
                if (configData != null)
                {
                    Ip = configData.Ip;
                    SampleTime = configData.Sample_time;
                    MaxSample = configData.Sample_amount;
                }
                else
                {
                    Ip = ipDefault;
                    SampleTime = sampleTimeDefault;
                    MaxSample = maxSampleDefault;
                }
            }
            catch (Exception e)
            {
                Ip = ipDefault;
                SampleTime = sampleTimeDefault;
                MaxSample = maxSampleDefault;
                Debug.WriteLine(e);
            }
        }
    }
}

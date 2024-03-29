﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Newtonsoft.Json;

namespace MultiWPFApp.ViewModel
{
    using Model;
    public class PHTViewModel : BaseViewModel
    {
        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        private int sampleTime;
        public string SampleTime
        {
            get { return sampleTime.ToString(); }
            set
            {
                if (Int32.TryParse(value, out int st))
                {
                    if (sampleTime != st)
                    {
                        sampleTime = st;
                        OnPropertyChanged("SampleTime");
                    }
                }
            }
        }

        //Initialize variables and models
        public PlotModel DataPlotModel { get; set; }
        public ButtonCommand StartButton { get; set; }
        public ButtonCommand StopButton { get; set; }

        private int timeStamp = 0;
        private ConfigParams config = new ConfigParams();
        private Timer RequestTimer;
        private ServerIoT server;


        public PHTViewModel()
        {
            DataPlotModel = new PlotModel { Title = "Measurements data" };

            //Adding X and Y axis
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 2000,
                Key = "VerticalPressure",
                Unit = "hPA",
                Title = "Pressure"
            });

            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Right,
                Minimum = -25,
                Maximum = 120,
                Key = "VerticalHumTemp",
                Unit = "%, *C",
                Title = "Humidity, Temp"
            });

            // Adding series for each orientation
            DataPlotModel.Series.Add(new LineSeries() { Title = "Pressure", Color = OxyColor.Parse("#FFFF0000"), YAxisKey = "VerticalPressure" });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Humidity", Color = OxyColor.Parse("#FF00FF00"), YAxisKey = "VerticalHumTemp" });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Temperature", Color = OxyColor.Parse("#FF0000FF"), YAxisKey = "VerticalHumTemp" });

            // Initializing buttons
            StartButton = new ButtonCommand(StartTimer);
            StopButton = new ButtonCommand(StopTimer);

            url = config.Ip;
            sampleTime = config.SampleTime;

            server = new ServerIoT(Url);
        }

        // Update plot with new data points
        private void UpdatePlot(double t, double press, double hum, double temp)
        {
            LineSeries pressureLineSeries = DataPlotModel.Series[0] as LineSeries;
            LineSeries humidityLineSeries = DataPlotModel.Series[1] as LineSeries;
            LineSeries temperatureLineSeries = DataPlotModel.Series[2] as LineSeries;

            pressureLineSeries.Points.Add(new DataPoint(t, press));
            humidityLineSeries.Points.Add(new DataPoint(t, hum));
            temperatureLineSeries.Points.Add(new DataPoint(t, temp));

            if (pressureLineSeries.Points.Count > config.maxSampleDefault)
            {
                pressureLineSeries.Points.RemoveAt(0);
                humidityLineSeries.Points.RemoveAt(0);
                temperatureLineSeries.Points.RemoveAt(0);
            }


            if (t >= config.XAxisMax)
            {
                DataPlotModel.Axes[0].Minimum = (t - config.XAxisMax);
                DataPlotModel.Axes[0].Maximum = t + config.SampleTime / 1000.0; ;
            }

            DataPlotModel.InvalidatePlot(true);
        }

        // Asynchrous chart update with parsing json data from server
        private async void UpdatePlot()
        {

            string responseText = await server.POSTwithClient("resource.php", "pht");

            try
            {
                PHTData responseJson = JsonConvert.DeserializeObject<PHTData>(responseText);
                if (responseJson != null)
                    UpdatePlot(timeStamp / 1000.0, responseJson.pressure, responseJson.humidity, responseJson.temperature);
            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(responseText);
                Debug.WriteLine(e);
            }

            timeStamp += config.SampleTime;
        }
        private void RequestTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdatePlot();
        }

        // starts timer interval
        private void StartTimer()
        {
            if (RequestTimer == null)
            {
                RequestTimer = new Timer(config.SampleTime);
                RequestTimer.Elapsed += new ElapsedEventHandler(RequestTimerElapsed);
                RequestTimer.Enabled = true;

                DataPlotModel.ResetAllAxes();
            }
        }
        // stops the timer
        private void StopTimer()
        {
            if (RequestTimer != null)
            {
                RequestTimer.Enabled = false;
                RequestTimer = null;
            }
        }

    }
}

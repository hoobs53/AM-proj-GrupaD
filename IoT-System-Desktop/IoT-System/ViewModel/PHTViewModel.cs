using System;
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
        public ButtonCommand UpdateConfigButton { get; set; }
        public ButtonCommand DefaultConfigButton { get; set; }

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
                Maximum = 360,
                Key = "Vertical",
                Unit = "*[deg]",
                Title = "Orientation"
            });

            // Adding series for each orientation
            DataPlotModel.Series.Add(new LineSeries() { Title = "Pressure", Color = OxyColor.Parse("#FFFF0000") });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Humidity", Color = OxyColor.Parse("#FF00FF00") });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Temperature", Color = OxyColor.Parse("#FF0000FF") });

            // Initializing buttons
            StartButton = new ButtonCommand(StartTimer);
            StopButton = new ButtonCommand(StopTimer);
            UpdateConfigButton = new ButtonCommand(UpdateConfig);
            DefaultConfigButton = new ButtonCommand(DefaultConfig);

            url = config.Url;
            sampleTime = config.SampleTime;

            server = new ServerIoT(Url);
        }

        // Update plot with new data points
        private void UpdatePlot(double t, double r, double p, double y)
        {
            LineSeries pressureLineSeries = DataPlotModel.Series[0] as LineSeries;
            LineSeries humidityLineSeries = DataPlotModel.Series[1] as LineSeries;
            LineSeries temperatureLineSeries = DataPlotModel.Series[2] as LineSeries;

            pressureLineSeries.Points.Add(new DataPoint(t, r));
            humidityLineSeries.Points.Add(new DataPoint(t, p));
            temperatureLineSeries.Points.Add(new DataPoint(t, y));

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

            string responseText = await server.POSTwithClient();

            try
            {
                ChartData responseJson = JsonConvert.DeserializeObject<ChartData>(responseText);
                UpdatePlot(timeStamp / 1000.0, responseJson.Roll, responseJson.Pitch, responseJson.Yaw);
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

        private void StopTimer()
        {
            if (RequestTimer != null)
            {
                RequestTimer.Enabled = false;
                RequestTimer = null;
            }
        }

        // Update parameters when Update Config button is clicked
        private void UpdateConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams(url, sampleTime, 100);
            server = new ServerIoT(Url);

            if (restartTimer)
                StartTimer();
        }

        // Loads default config
        private void DefaultConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams();
            Url = config.Url;
            SampleTime = config.SampleTime.ToString();
            server = new ServerIoT(Url);

            if (restartTimer)
                StartTimer();
        }

    }
}

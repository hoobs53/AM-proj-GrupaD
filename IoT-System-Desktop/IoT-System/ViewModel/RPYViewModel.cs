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
    public class RPYViewModel : BaseViewModel
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


        public RPYViewModel()
        {
            DataPlotModel = new PlotModel { Title = "Orientation data" };

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
            DataPlotModel.Series.Add(new LineSeries() { Title = "Roll", Color = OxyColor.Parse("#FFFF0000") });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Pitch", Color = OxyColor.Parse("#FF00FF00") });
            DataPlotModel.Series.Add(new LineSeries() { Title = "Yaw", Color = OxyColor.Parse("#FF0000FF") });

            // Initializing buttons
            StartButton = new ButtonCommand(StartTimer);
            StopButton = new ButtonCommand(StopTimer);

            url = config.Url;
            sampleTime = config.SampleTime;

            server = new ServerIoT(Url);
        }

        // Update plot with new data points
        private void UpdatePlot(double t, double r, double p, double y)
        {
            LineSeries rollLineSeries = DataPlotModel.Series[0] as LineSeries;
            LineSeries pitchLineSeries = DataPlotModel.Series[1] as LineSeries;
            LineSeries yawLineSeries = DataPlotModel.Series[2] as LineSeries;

            rollLineSeries.Points.Add(new DataPoint(t, r));
            pitchLineSeries.Points.Add(new DataPoint(t, p));
            yawLineSeries.Points.Add(new DataPoint(t, y));

            if (rollLineSeries.Points.Count > config.maxSampleDefault)
            {
                rollLineSeries.Points.RemoveAt(0);
                pitchLineSeries.Points.RemoveAt(0);
                yawLineSeries.Points.RemoveAt(0);
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

            string responseText = await server.POSTwithClient("resource.php", "rpy");

            try
            {
                RPYData responseJson = JsonConvert.DeserializeObject<RPYData>(responseText);
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;
using Newtonsoft.Json;
namespace MultiWPFApp.ViewModel
{
    using Model;
    public class DisplayViewModel : BaseViewModel
    {

        public SolidColorBrush[,] model = new SolidColorBrush[8, 8];

        private PixelsData pixels;
        public ButtonCommandWithParameter CommonButtonCommand { get; set; }

        public Action<string, Color> setColorHandler;

        public LedDisplay ledDisplay;  //!< LED display model
        private ServerIoT server;       //!< IoT server model
        private ConfigParams config;

        public int DisplaySizeX { get => ledDisplay.SizeX; }
        public int DisplaySizeY { get => ledDisplay.SizeY; }
        public Color DisplayOffColor { get => ledDisplay.OffColor; }

        public ButtonCommand SendRequestCommand { get; set; }
        public ButtonCommand SendClearCommand { get; set; }

        private byte _r;
        public int R
        {
            get
            {
                return _r;
            }
            set
            {
                if (_r != (byte)value)
                {
                    _r = (byte)value;
                    ledDisplay.ActiveColorR = _r;
                    SelectedColor = new SolidColorBrush(ledDisplay.ActiveColor);
                    OnPropertyChanged("R");
                }
            }
        }

        private byte _g;
        public int G
        {
            get
            {
                return _g;
            }
            set
            {
                if (_g != (byte)value)
                {
                    _g = (byte)value;
                    ledDisplay.ActiveColorG = _g;
                    SelectedColor = new SolidColorBrush(ledDisplay.ActiveColor);
                    OnPropertyChanged("G");
                }
            }
        }

        private byte _b;
        public int B
        {
            get
            {
                return _b;
            }
            set
            {
                if (_b != (byte)value)
                {
                    _b = (byte)value;
                    ledDisplay.ActiveColorB = _b;
                    SelectedColor = new SolidColorBrush(ledDisplay.ActiveColor);
                    OnPropertyChanged("B");
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if(status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        private SolidColorBrush _selectedColor;
        public SolidColorBrush SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged("SelectedColor");
                }
            }
        }

        public DisplayViewModel()
        {
            config = new ConfigParams();
            server = new ServerIoT(config.Ip);
            pixels = JsonConvert.DeserializeObject<PixelsData>(server.POSTgetPixels());
            ledDisplay = new LedDisplay(pixels.response);
            SelectedColor = new SolidColorBrush(ledDisplay.ActiveColor);
            CommonButtonCommand = new ButtonCommandWithParameter(ChangeColor);
            SendRequestCommand = new ButtonCommand(SendControlRequest);
            SendClearCommand = new ButtonCommand(ClearDisplay);
        }

        public (int, int) LedTagToIndex(string name)
        {
            return (int.Parse(name.Substring(3, 1)), int.Parse(name.Substring(4, 1)));
        }

        public string LedIndexToTag(int i, int j)
        {
            return "LED" + i.ToString() + j.ToString();
        }

        private void ChangeColor(string parameter)
        {
            // Set active color as background
            setColorHandler(parameter, SelectedColor.Color);
            // Find element x-y position
            (int x, int y) = LedTagToIndex(parameter);
            // Update LED display data model
            ledDisplay.UpdateModel(x, y);
            Status = ledDisplay.status;
        }

        private async void ClearDisplay()
        {
            // Clear LED display GUI
            for (int i = 0; i < ledDisplay.SizeX; i++)
                for (int j = 0; j < ledDisplay.SizeY; j++)
                    setColorHandler(LedIndexToTag(i, j), ledDisplay.OffColor);

            // Clear LED display data model
            ledDisplay.ClearModel();

            // Clear physical LED display
            await server.PostControlRequest(ledDisplay.getClearPostData());
        }

        private async void SendControlRequest()
        {
            await server.PostControlRequest(ledDisplay.getControlPostData());
            Status = "";
        }
    }
}

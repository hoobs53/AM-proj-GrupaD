using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MultiWPFApp.ViewModel
{
    using Model;
    public class DisplayViewModel : BaseViewModel
    {

        public SolidColorBrush[,] model = new SolidColorBrush[8, 8];

        public ButtonCommandWithParameter CommonButtonCommand { get; set; }

        public Action<string, Color> setColorHandler;

        private LedDisplay ledDisplay;  //!< LED display model
        private ServerIoT server;       //!< IoT server model
        private ConfigData config;

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
            ledDisplay = new LedDisplay(0x00000000);
            SelectedColor = new SolidColorBrush(ledDisplay.ActiveColor);
            CommonButtonCommand = new ButtonCommandWithParameter(SetButtonColor);
            SendRequestCommand = new ButtonCommand(SendControlRequest);
            SendClearCommand = new ButtonCommand(ClearDisplay);
            config = new ConfigData();

            server = new ServerIoT(config.url);
        }

        /**
         * @brief Conversion method: LED indicator Name to LED x-y position
         * @param name LED indicator Button Name propertie 
         * @return Tuple with LED x-y position (0=x, 1=y)
         */
        public (int, int) LedTagToIndex(string name)
        {
            return (int.Parse(name.Substring(3, 1)), int.Parse(name.Substring(4, 1)));
        }

        /**
         * @brief Conversion method: LED x-y position to LED indicator Name
         * @param x LED horizontal position in display
         * @param y LED vertical position in display
         * @return LED indicator Button Name property
         */
        public string LedIndexToTag(int i, int j)
        {
            return "LED" + i.ToString() + j.ToString();
        }

        /**
         * @brief LED indicator Click event handling procedure
         * @param parameter LED indicator Button Name property
         */
        private void SetButtonColor(string parameter)
        {
            // Set active color as background
            setColorHandler(parameter, SelectedColor.Color);
            // Find element x-y position
            (int x, int y) = LedTagToIndex(parameter);
            // Update LED display data model
            ledDisplay.UpdateModel(x, y);
        }

        /**
         * @brief Clear button Click event handling procedure
         */
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

        /**
         * @brief Send button Click event handling procedure
         */
        private async void SendControlRequest()
        {
            await server.PostControlRequest(ledDisplay.getControlPostData());
        }
    }
}

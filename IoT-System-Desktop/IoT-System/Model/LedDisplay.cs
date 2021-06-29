using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace MultiWPFApp.Model
{
    public class LedDisplay
    {
        public readonly int SizeX = 8;
        public readonly int SizeY = 8;
        public string status = "";
        public int[][] pixels;

        public byte ActiveColorA    //!< Active color Alpha components
        {
            get => 255;
        }

        private byte _activeColorR;
        public byte ActiveColorR    //!< Active color Red components
        {
            set => _activeColorR = value;
        }

        private byte _activeColorG;
        public byte ActiveColorG    //!< Active color Green components
        {
            set => _activeColorG = value;
        }

        private byte _activeColorB;
        public byte ActiveColorB    //!< Active color Blue components
        {
            set => _activeColorB = value;
        }

        public Color ActiveColor    //!< Active color in ARG format
        {
            get => Color.FromArgb(ActiveColorA, _activeColorR, _activeColorG, _activeColorB);
        }

        public readonly Color OffColor;   //!< 'LED-is-off' color in Int ARGB format

        public UInt16?[,,] model;
        private UInt16?[,,] currentModel;

        /**
         * @brief Default constructor
         */
        public LedDisplay(int [][]pixelsState)
        {
            model = new UInt16?[SizeX, SizeY, 3];
            currentModel = new UInt16?[SizeX, SizeY, 3];
            OffColor = Color.FromArgb(255, 0, 0, 0);
            _activeColorR = OffColor.R;
            _activeColorG = OffColor.G;
            _activeColorB = OffColor.B;

            int cnt = 0;
            for (int j = 0; j < SizeX; j++)
            {
                for (int i = 0; i < SizeY; i++)
                {

                    model[i, j, 0] = (byte)pixelsState[cnt][0];
                    model[i, j, 1] = (byte)pixelsState[cnt][1];
                    model[i, j, 2] = (byte)pixelsState[cnt][2];
                    cnt++;
                }
            }
            pixels = pixelsState;
            initDisplay();
        }

        /**
         * Conversion method: LED x-y position to position/color data in JSON format
         * @param x LED horizontal position in display
         * @param y LED vertical position in display
         * @return Position/color data in JSON format: [x,y,r,g,b] (x,y: 0-7; r,g,b: 0-255)
         */
        private JArray IndexToJsonArray(int x, int y)
        {
            JArray array = new JArray();
            try
            {
                array.Add(x);
                array.Add(y);
                array.Add(model[x, y, 0]);
                array.Add(model[x, y, 1]);
                array.Add(model[x, y, 2]);
            }
            catch (JsonException e)
            {
                Trace.TraceError(e.Message);
            }
            return array;
        }

        /**
         * @brief Null color check
         * @param x LED horizontal position in display
         * @param y LED vertical position in display
         * @return False if color is Null; True otherwise
         */
        private bool ColorNotNull(int x, int y)
        {
            return !((model[x, y, 0] == null) || (model[x, y, 1] == null) || (model[x, y, 2] == null));
        }

        /**
         * @brief Update display model with active color
         * @param x LED horizontal position in display
         * @param y LED vertical position in display
         */
        public void UpdateModel(int x, int y)
        {
            model[x, y, 0] = _activeColorR;
            model[x, y, 1] = _activeColorG;
            model[x, y, 2] = _activeColorB;

            checkIfChanged();
        }

        public void checkIfChanged()
        {
            status = "";
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    if (model[i, j, 0] != currentModel[i, j, 0] || model[i, j, 1] != currentModel[i, j, 1] || model[i, j, 2] != currentModel[i, j, 2])
                    {
                        status = "UNSAVED CHANGES*";
                    }
                }
            }
        }
        /**
         * @brief LED display data model clear - fill with all components with Null
         */
        public void ClearModel()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    model[i, j, 0] = 0;
                    model[i, j, 1] = 0;
                    model[i, j, 2] = 0;

                    currentModel[i, j, 0] = 0;
                    currentModel[i, j, 1] = 0;
                    currentModel[i, j, 2] = 0;

                }
            }
        }
        public void initDisplay()
        {
            ushort? r, g, b;
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {

                    r = model[i, j, 0];
                    g = model[i, j, 1];
                    b = model[i, j, 2];
                    currentModel[i, j, 0] = r;
                    currentModel[i, j, 1] = g;
                    currentModel[i, j, 2] = b;
                }
            }
        }
        /**
         * @brief Generate HTTP POST request parameters for LED display control via IoT server script
         * @return HTTP POST request parameters as Key-Value pairs
         */
        public List<KeyValuePair<string, string>> getControlPostData()
        {
            var postData = new List<KeyValuePair<string, string>>();
            ushort? r, g, b;
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    //if (ColorNotNull(i, j))
                        postData.Add(
                            new KeyValuePair<string, string>(
                                "LED" + i.ToString() + j.ToString(),
                                IndexToJsonArray(i, j).ToString()
                                ));
                    r = model[i, j, 0];
                    g = model[i, j, 1];
                    b = model[i, j, 2];
                    currentModel[i, j, 0] = r;
                    currentModel[i, j, 1] = g;
                    currentModel[i, j, 2] = b;
                }
            }
            return postData;
        }

        /**
         * @brief Generate HTTP POST request parameters for clearing LED display via IoT server script
         * @return HTTP POST request parameters as Key-Value pairs
         */
        List<KeyValuePair<string, string>> clearData;
        public List<KeyValuePair<string, string>> getClearPostData()
        {
            if (clearData == null)
            {
                clearData = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < SizeX; i++)
                {
                    for (int j = 0; j < SizeY; j++)
                    {
                        clearData.Add(
                            new KeyValuePair<string, string>(
                                "LED" + i.ToString() + j.ToString(),
                                "[" + i.ToString() + "," + j.ToString() + ",0,0,0]"
                                ));
                    }
                }
            }
            return clearData;
        }
    }
}

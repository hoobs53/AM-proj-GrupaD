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
        // returns json array for server request
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
        // updates model with current values
        public void UpdateModel(int x, int y)
        {
            model[x, y, 0] = _activeColorR;
            model[x, y, 1] = _activeColorG;
            model[x, y, 2] = _activeColorB;

            checkIfChanged();
        }
        // compare virtual model with physical matrix
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
        // clears all LEDs
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
        // initializes display with values on physical model
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
        public List<KeyValuePair<string, string>> getControlPostData()
        {
            var postData = new List<KeyValuePair<string, string>>();
            ushort? r, g, b;
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
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

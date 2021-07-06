using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MultiWPFApp.ViewModel
{
    using Model;
    public class ListViewModel : BaseViewModel
    {
        public ObservableCollection<MeasurementViewModel> Measurements { get; set; }

        public ObservableCollection<MeasurementViewModel> Orientation { get; set; }

        public ObservableCollection<MeasurementViewModel> Joystick { get; set; }
        public ButtonCommand Refresh_btn { get; set; }

        private ConfigParams config;
        private ServerIoT Server;

        public ListViewModel()
        {
            config = new ConfigParams();
            Server = new ServerIoT(config.Ip);
            // Create new collection for measurements data
            Measurements = new ObservableCollection<MeasurementViewModel>();
            Orientation = new ObservableCollection<MeasurementViewModel>();
            Joystick = new ObservableCollection<MeasurementViewModel>();

            // Bind button with action
            Refresh_btn = new ButtonCommand(Refresh);
        }

        void Refresh()
        {
            try
            {
                // Read data from server in JSON array format
                string response = Server.POSTgetMeasurements();
                MeasurementModel measurementObj = JsonConvert.DeserializeObject<MeasurementModel>(response);
                JArray measurementsJsonArray = measurementObj.measurements;
                // Convert generic JSON array container to list of specific type
                var measurementsList = measurementsJsonArray.ToObject<List<MeasurementListModel>>();

                JArray orientationJsonArray = measurementObj.orientation;

                var orientationList = orientationJsonArray.ToObject<List<MeasurementListModel>>();

                JArray joystickJsonArray = measurementObj.joystick;

                var joystickList = joystickJsonArray.ToObject<List<MeasurementListModel>>();
            
            // Add new elements to measurement collection
            if (Measurements.Count < measurementsList.Count)
            {
                foreach (var m in measurementsList)
                    Measurements.Add(new MeasurementViewModel(m));
            }
            // Update existing elements in measurement collection
            else
            {
                for (int i = 0; i < Measurements.Count; i++)
                    Measurements[i].UpdateWithModel(measurementsList[i]);
            }
            // Add new elements to orientation collection
            if (Orientation.Count < orientationList.Count)
            {
                foreach (var m in orientationList)
                    Orientation.Add(new MeasurementViewModel(m));
            }
            // Update existing elements in orientation collection
            else
            {
                for (int i = 0; i < Orientation.Count; i++)
                    Orientation[i].UpdateWithModel(orientationList[i]);
            }
            // Add new elements to joystick collection
            if (Joystick.Count < joystickList.Count)
            {
                foreach (var m in joystickList)
                    Joystick.Add(new MeasurementViewModel(m));
            }
            // Update existing elements in joystick collection
            else
            {
                for (int i = 0; i < Joystick.Count; i++)
                    Joystick[i].UpdateWithModel(joystickList[i]);
            }
            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(e);
            }
        }

    }
}

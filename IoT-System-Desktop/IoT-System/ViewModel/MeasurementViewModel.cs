using System;
using System.ComponentModel;
using System.Globalization;

namespace MultiWPFApp.ViewModel
{
    using Model;

    public class MeasurementViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private double _data;
        public string Data
        {
            get
            {
                return _data.ToString("0.0####", CultureInfo.InvariantCulture);
            }
            set
            {
                if (Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double tmp) && _data != tmp)
                {
                    _data = tmp;
                    OnPropertyChanged("Data");
                }
            }
        }

        private string _unit;
        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    OnPropertyChanged("Unit");
                }
            }
        }

        public MeasurementViewModel(MeasurementListModel model)
        {
            UpdateWithModel(model);
        }

        public void UpdateWithModel(MeasurementListModel model)
        {
            _name = model.Name;
            OnPropertyChanged("Name");
            _data = model.Value;
            OnPropertyChanged("Data");
            _unit = model.Unit;
            OnPropertyChanged("Unit");
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /**
         * @brief Simple function to trigger event handler
         * @params propertyName Name of ViewModel property as string
         */
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

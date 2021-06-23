using System;
namespace MultiWPFApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel _contentViewModel;

        public BaseViewModel ContentViewModel
        {
            get { return _contentViewModel; }
            set
            {
                if(_contentViewModel != value)
                {
                    _contentViewModel = value;
                    OnPropertyChanged("ContentViewModel");
                }
            }
        }

        public ButtonCommand MenuCommandView1 { get; set; }

        public ButtonCommand MenuCommandView2 { get; set; }

        public ButtonCommand MenuCommandView3 { get; set; }

        public MainWindowViewModel()
        {
            MenuCommandView1 = new ButtonCommand(MenuSetView1);

            MenuCommandView2 = new ButtonCommand(MenuSetView2);

            MenuCommandView3 = new ButtonCommand(MenuSetView3);

            ContentViewModel = new View1_ViewModel();
        }

        private void MenuSetView1()
        {
            ContentViewModel = new View1_ViewModel();
        }

        private void MenuSetView2()
        {
            ContentViewModel = new ListViewModel();
        }

        private void MenuSetView3()
        {
            ContentViewModel = new ChartViewModel();
        }
    }
}

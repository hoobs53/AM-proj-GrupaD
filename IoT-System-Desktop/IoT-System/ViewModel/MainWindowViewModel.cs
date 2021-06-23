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

        public ButtonCommand DisplayCommandView { get; set; }

        public ButtonCommand ListCommandView { get; set; }

        public ButtonCommand RPYCommandView { get; set; }

        public ButtonCommand PHTCommandView { get; set; }

        public ButtonCommand JoystickCommandView { get; set; }

        public MainWindowViewModel()
        {
            DisplayCommandView = new ButtonCommand(MenuSetDisplayView);

            ListCommandView = new ButtonCommand(MenuSetListView);

            RPYCommandView = new ButtonCommand(MenuSetRPYView);

            PHTCommandView = new ButtonCommand(MenuSetPHTView);

            JoystickCommandView = new ButtonCommand(MenuSetJoystickView);

            ContentViewModel = new DisplayViewModel();
        }

        private void MenuSetDisplayView()
        {
            ContentViewModel = new DisplayViewModel();
        }

        private void MenuSetListView()
        {
            ContentViewModel = new ListViewModel();
        }

        private void MenuSetRPYView()
        {
            ContentViewModel = new RPYViewModel();
        }

        private void MenuSetPHTView()
        {
            ContentViewModel = new PHTViewModel();
        }

        private void MenuSetJoystickView()
        {
            ContentViewModel = new JoystickViewModel();
        }
    }
}

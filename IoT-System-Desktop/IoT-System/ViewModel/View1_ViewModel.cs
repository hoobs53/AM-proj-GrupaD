using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MultiWPFApp.ViewModel
{
    public class View1_ViewModel : BaseViewModel
    {
        public Action<object> CodeBehindHandler{ get; set; }

        public SolidColorBrush[,] model = new SolidColorBrush[8, 8];

        public ButtonCommandWithParameter CommonButtonCommand { get; set; }

        public View1_ViewModel()
        {
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    model[i, j] = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
            }
            CommonButtonCommand = new ButtonCommandWithParameter(SetButtonColor);
        }
        private void SetButtonColor(string param)
        {
            CodeBehindHandler(param);
        }
    }
}

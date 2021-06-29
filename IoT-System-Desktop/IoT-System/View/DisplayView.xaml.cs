using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiWPFApp.View
{
    using ViewModel;
    /// <summary>
    /// Interaction logic for View1.xaml
    /// </summary>
    public partial class DisplayView : UserControl
    {
        private string responseText;
        private bool created = false;
        private DisplayViewModel viewmodel;
        public DisplayView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(DataContextChangedHandler);
        }
        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewmodel = (DataContext as DisplayViewModel);
            if (viewmodel != null)
            {
                viewmodel.setColorHandler = SetButtonColor;
                if (!created)
                    initButtons();
            }
        }

        private void SetButtonColor(string name, Color color)
        {
            (ButtonMatrixGrid.FindName(name) as Button).Background = new SolidColorBrush(color);
        }
        private void initButtons()
        {
            /* Button matrix grid definition */
            for (int i = 0; i < 8; i++)
            {
                ButtonMatrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
                ButtonMatrixGrid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
            }

            for (int i = 0; i < 8; i++)
            {
                ButtonMatrixGrid.RowDefinitions.Add(new RowDefinition());
                ButtonMatrixGrid.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
            }
            int[][] pixelsState = viewmodel.ledDisplay.pixels;
            byte r, g, b;
            int cnt = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    r = (byte)pixelsState[cnt][0];
                    g = (byte)pixelsState[cnt][1];
                    b = (byte)pixelsState[cnt][2];
                    // <Button
                    Button led = new Button()
                    {
                        // Name = "LEDij"
                        Name = "LED" + i.ToString() + j.ToString(),
                        // CommandParameter = "LEDij"
                        CommandParameter = "LED" + i.ToString() + j.ToString(),
                        // Style="{StaticResource LedButtonStyle}"
                        Style = (Style)FindResource("LedButtonStyle"),
                        // Bacground="{StaticResource ... }"

                        Background = new SolidColorBrush(Color.FromArgb(255, r, g, b)),
                        // BorderThicness="2"
                        BorderThickness = new Thickness(2),
                    };
                    // Command="{Binding CommonButtonCommand}" 
                    led.SetBinding(Button.CommandProperty, new Binding("CommonButtonCommand"));
                    // Grid.Column="i" 
                    Grid.SetColumn(led, i);
                    // Grid.Row="j"
                    Grid.SetRow(led, j);
                    // />
                    cnt++;
                    ButtonMatrixGrid.Children.Add(led);
                    ButtonMatrixGrid.RegisterName(led.Name, led);
                }
            }

            created = true;
        }
    }
}

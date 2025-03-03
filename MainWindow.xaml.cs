using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotionSeatTest_cSharp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.Loaded += MainWindow_Loaded;

    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (Button button in FindLogicalChildren<Button>(this))
        {
            button.Click += Button_Click;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        switch (((Button)sender).Tag)
        {
            case "roll":
                this.sld_roll.Value = 0;
                break;
            case "pitch":
                this.sld_pitch.Value = 0;
                break;
            case "yaw":
                this.sld_yaw.Value = 0;
                break;
        }
    }


    /// <summary>
    /// Function that return a list of UI element in a view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <returns></returns>
    private IEnumerable<T> FindLogicalChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        var children = LogicalTreeHelper.GetChildren(parent);
        foreach (object child in children)
        {
            if (child is DependencyObject dependencyObject)
            {
                if (child is T childAsT)
                {
                    yield return childAsT;
                }

                foreach (T descendant in FindLogicalChildren<T>(dependencyObject))
                {
                    yield return descendant;
                }
            }
        }

    }

    private async void btn_start_Click(object sender, RoutedEventArgs e)
    {
        GlobalVariables.cts = new CancellationTokenSource();

        await MotionSeatTest_cSharp.SimRacingStudio.SimRacingStudioAPI.StartMotionSystem(GlobalVariables.cts.Token);
    }

    private void btn_stop_Click(object sender, RoutedEventArgs e)
    {
        if(GlobalVariables.cts != null && !GlobalVariables.cts.IsCancellationRequested)
        {
            GlobalVariables.cts.Cancel();
            GlobalVariables.cts.Dispose();
        }
    }

    private void sld_pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GlobalVariables.pitch = (float)e.NewValue;
    }

    private void sld_roll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GlobalVariables.roll = (float)e.NewValue;
    }

    private void sld_yaw_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GlobalVariables.yaw = (float)e.NewValue;
    }
}
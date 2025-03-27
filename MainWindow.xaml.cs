using MotionSeatTest_cSharp.Manage;
using System.Windows;
using System.Windows.Controls;

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
        GlobalVariables.Cts = new CancellationTokenSource();
        GlobalVariables.CtsDeditec = new CancellationTokenSource();

        Task setupDeditecTask = Gpio.SetUpDeditec(GlobalVariables.CtsDeditec.Token);
        Task startMotionSystemTask = SimRacingStudio.SimRacingStudioAPI.StartMotionSystem(GlobalVariables.Cts.Token);

        await Task.WhenAll(setupDeditecTask, startMotionSystemTask);
    }

    private void btn_stop_Click(object sender, RoutedEventArgs e)
    {
        CancelAndDispose(ref GlobalVariables.Cts);
        CancelAndDispose(ref GlobalVariables.CtsDeditec);
    }

    private void CancelAndDispose(ref CancellationTokenSource cts)
    {
        if (cts != null)
        {
            cts.Cancel();

            cts.Dispose();
            cts = null;
        }
    }


    private void sld_pitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        //GlobalVariables.Pitch = (float)e.NewValue;
    }

    private void sld_roll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        //GlobalVariables.Roll = (float)e.NewValue;
    }

    private void sld_yaw_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        GlobalVariables.Yaw = (float)e.NewValue;
    }
}
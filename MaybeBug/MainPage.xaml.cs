using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace MaybeBug;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel
        {
            A = 0,
            B = 100
        };
        rba.IsChecked = true;
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            Debug.WriteLine("RadioButton Checked");
            if (sender == rba)
            {
                mycontrol.SetBinding(MyControl.ValueProperty, nameof(MainPageViewModel.A));
            }
            else if (sender == rbb)
            {
                mycontrol.SetBinding(MyControl.ValueProperty, nameof(MainPageViewModel.B));
            }
        }
    }

    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _a;

        [ObservableProperty]
        private int _b;
    }
}

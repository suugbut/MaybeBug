# Note (for me)
Don't delete this repo because it is used for a bug report.

# Description
This repo is intentionally made simple to illustrate the issue.
- A custom control `MyControl` has a bindable property `Value`, a `Label` for displaying `Value` and a `Button` for increasing `Value`.
  `MyControl` is also backed by a view model called `MyControlViewModel` nested in `MyControl`.
- `MainPage` uses `MyControl` and has two `RadioButton` to dynamically bind `MyControl` to one of two properties (`A` and `B`) in `MainPageViewModel`.

  ![image](https://github.com/user-attachments/assets/4d862425-6ef0-4481-b5bb-59fbe9dbb89b)
# Expected Behavior
- Selecting a radio button will bind `MyControl` to a property in `MainPageViewModel`.
- Pressing `Increase` button will increase the bound property.
- `MyControl`'s `Label` displays the bound property.
# Issue and How To Produce It
- From any checked radio button, switch to the other one.
- Press the `Increase` button at least once.
- Switch to the previous radio button but don't press the `Increase` button.
- Switch again to the other one. Here `MyControl`'s Label no longer displays the bound property.

Example by assuming that at the beginning the checked radio button is `A=0` and unchecked one is `B=100`.
```
A ---> B followed by pressing Increase button ---> A but not pressing Increase button ----> B
```
`MyControl`'s Label:
```
0 ---> 100 and then becomes 101 ---> 0 ----> 0 (thas should be 101).
```
# My Investigation
The property changed event of the bindable property `Value` is not triggered
in the last switch explained above (`A but not pressing Increase button ----> B`).
# Details for `MyControl`
```xml
<HorizontalStackLayout
    Spacing="20"
    BindingContext="{x:Reference This}"
    x:DataType="{x:Type local:MyControl}">
    <Button
        Text="Increase"
        Command="{Binding ViewModel.IncreaseCommand}" />
    <Label
        Text="{Binding ViewModel.Value}" />
</HorizontalStackLayout>
```
```cs
public partial class MyControl : ContentView
{
    public MyControlViewModel ViewModel { get; } = new();
    public MyControl()
    {
        InitializeComponent();
        ViewModel.PropertyChanged += (s, e) =>
        {
            Debug.WriteLine("\tViewModel Property Changed");
            if (e.PropertyName == nameof(MyControlViewModel.Value))
            {
                if (Value != ViewModel.Value)
                {
                    Value = ViewModel.Value;
                }
            }
        };
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        propertyName: nameof(Value),
        returnType: typeof(int),
        declaringType: typeof(MyControl),
        defaultValue: 0,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            Debug.WriteLine("\tValue Property Changed");
            if (b is MyControl me && o is int old && n is int @new)
            {
                if (me.ViewModel.Value != @new)
                {
                    me.ViewModel.Value = @new;
                }
            }
        });

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public partial class MyControlViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _value;

        [RelayCommand]
        private void Increase() => Value++;
    }
}
```
# Details for `MainPage`
```xml
<VerticalStackLayout
    Padding="30,0"
    Spacing="25">
    <RadioButton
        x:Name="rba"
        Content="{Binding A, StringFormat='A: {0}'}"
        CheckedChanged="RadioButton_CheckedChanged" />
    <RadioButton
        x:Name="rbb"
        Content="{Binding B, StringFormat='B: {0}'}"
        CheckedChanged="RadioButton_CheckedChanged" />
    <local:MyControl
        x:Name="mycontrol" />
</VerticalStackLayout>
```
```cs
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
```

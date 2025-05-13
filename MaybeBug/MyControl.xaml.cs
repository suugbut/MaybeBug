using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace MaybeBug;

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
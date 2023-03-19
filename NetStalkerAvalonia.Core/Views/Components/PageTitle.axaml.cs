using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetStalkerAvalonia.Core.Views.Components
{
    public partial class PageTitle : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly AttachedProperty<string> TextProperty =
                      AttachedProperty<string>.RegisterAttached<NavButton, string>(nameof(Text), typeof(NavButton));

        public PageTitle()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

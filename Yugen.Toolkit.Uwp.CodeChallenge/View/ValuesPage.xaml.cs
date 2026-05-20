using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Yugen.Toolkit.Uwp.CodeChallenge.ViewModel;

namespace Yugen.Toolkit.Uwp.CodeChallenge.View
{
    public sealed partial class ValuesPage : Page
    {
        public ValuesPage()
		{
			DataContext = App.Current.Services.GetService<ValuesViewModel>();
			InitializeComponent();
        }

        private ValuesViewModel ViewModel => (ValuesViewModel)DataContext;
    }
}

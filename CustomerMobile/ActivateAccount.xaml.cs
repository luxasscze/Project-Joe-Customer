using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static CustomerMobile.GlobalVar;

namespace CustomerMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivateAccount : ContentPage
    {
        public ActivateAccount()
        {
            InitializeComponent();
            bgImage.Source = _AppBackgroundImage;
        }
    }
}
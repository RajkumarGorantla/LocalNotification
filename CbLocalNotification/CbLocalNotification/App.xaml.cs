using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CbLocalNotification
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Get<INotificationManager>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

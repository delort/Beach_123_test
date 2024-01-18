using System;
using InterviewBle.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InterviewBle
{
    public partial class App : Application
    {
        public App(IBleAdapter adapter )
        {
            InitializeComponent();

            MainPage = new MainPage(adapter);
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

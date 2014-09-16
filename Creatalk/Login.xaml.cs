using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

namespace Creatalk
{
    public partial class Login : PhoneApplicationPage
    {
        //public static ProgressBar LoginBar;

        DispatcherTimer check = new DispatcherTimer();

        public Login()
        {
            InitializeComponent();

            //LoginBar = loader;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            e.Cancel = true;
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            //usern.Focus();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            if (MainPage.loggedin != -1)
            {
                if (MainPage.loggedin == 1)
                {
                    check.Stop();

                    NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
                }
                else
                    loader.Visibility = Visibility.Collapsed;
            }
        }

        public void uReset(object sender, UploadStringCompletedEventArgs e)
        {
            MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            //alanlarin uygunlugunu kontrol ettiriyoruz
            if (usern.Text.Replace(" ", "") != "" && passw.Password.Replace(" ", "") != "")
            {
                loader.Visibility = Visibility.Visible;

                //giris yapilip yapilmadigini kontrol etmek uzere timer ayarliyoruz
                check.Interval = TimeSpan.FromSeconds(1);
                check.Tick += OnTimerTick;
                check.Start();

                classes.LogIn.CheckFields(usern.Text, passw.Password, false);
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
        }

        private void register_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Register.xaml", UriKind.Relative));
        }

        private void reset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (usern.Text.Replace(" ", "") != "")
            {
                if (MessageBox.Show("Sayın " + usern.Text + ", şifrenizi sıfırlamak istiyor musunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //parametre listesini temizliyoruz
                    classes.General.general_list.Clear();
                    //listeseye api icin gerekli parametreleri yerlestiriyoruz (base64| -> xml tag ismini base64 olarak belirlemek ve icerigini sifrelemek icin bir belirtectir)
                    classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(usern.Text));
                    //apiyi cagirma komutunu kullaniyoruz
                    classes.Oauth.XmlRpcExecMethod("forget_password", uReset);
                }
            }
            else
            {
                MessageBox.Show("Lütfen kullanıcı adınızı girin ve tekrar deneyin.");
                usern.Focus();
            }
        }

        /*/private void loader_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoginBar.IsEnabled = true;
            loader.Visibility = Visibility.Collapsed;

            if (App.Session_LoadSession("xf_session").Replace(" ", "") != "" && General.username.Replace(" ", "") != "")
            {
                //check.Stop();
                NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
            }
        }/*/
    }
}
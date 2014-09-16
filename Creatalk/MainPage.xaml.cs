using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Configuration;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using Creatalk.Resources;


/* This application designed for tapatalk APIs (and tested only on xenforo)
 * Development is in progress for other forum bases (e.g. vBulletin)
 * You may need to change something in classes files.
 * Some components and snippet were used such as coding4fun, theme manager, wp toolkit, myrichtextbox (re-edited by me), telerik.
 * By the way, translating different languages (e.g. English, Spanish etc.) is in progress. You can give us some supports about this translation. Contact us.
 * Developed by Hüseyin ELMAS
 * http://www.huseyinelmas.net
 * For all social platforms use this keyword to find me "hsynlms" :]
 * Open source.
 * 09.16.2014 */

namespace Creatalk
{
    public partial class MainPage : PhoneApplicationPage
    {
        public static short loggedin = -1;

        DispatcherTimer check = new DispatcherTimer();
        //public static ProgressBar Loader;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            if (classes.General.forum_domain == "")
            {
                MessageBox.Show("Forum yolu belirtilmedi.");
                App.Current.Terminate();
            }
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            //eger xf_session bos degilse ayrıca general sinifindaki username bos degilse giris basariyla yapilmistir ve profil sayfasina yonlendir
            //if (App.Session_LoadSession("xf_session").Replace(" ", "") != "" && General.username.Replace(" ", "") != "")
            if (classes.General.DecodeBase64(App.Session_LoadSession("user_")).Replace(" ", "") != "" && classes.General.DecodeBase64(App.Session_LoadSession("pass_")).Replace(" ", "") != "")
            {
                if (loggedin != -1)
                {
                    check.Stop();

                    if (NavigationContext.QueryString.Count() > 0)
                    {
                        if (NavigationService.Source.OriginalString.IndexOf("sforum_id") >= 0)
                            NavigationService.Navigate(new Uri("/forum/ForumTopic.xaml?sforum_id=" + NavigationContext.QueryString["sforum_id"] + "&forum_id=" + NavigationContext.QueryString["forum_id"] + "&redirected", UriKind.Relative));
                        else if (NavigationService.Source.OriginalString.IndexOf("forum_id") >= 0)
                            NavigationService.Navigate(new Uri("/forum/ForumSub.xaml?forum_id=" + NavigationContext.QueryString["forum_id"] + "&redirected", UriKind.Relative));
                        else
                            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
                    }
                    else
                        NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
                }
            }
            else
            {
                check.Stop();
                NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //MessageBox.Show("Internet bağlantısı olmadığından giriş yapılamadı.");
                NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
            }
            else
            {
                if (classes.General.DecodeBase64(App.Session_LoadSession("user_")).Replace(" ", "") != "" && classes.General.DecodeBase64(App.Session_LoadSession("pass_")).Replace(" ", "") != "")
                {
                    check.Interval = TimeSpan.FromSeconds(1);
                    check.Tick += OnTimerTick;
                    check.Start();

                    classes.LogIn.CheckFields(classes.General.DecodeBase64(App.Session_LoadSession("user_")), classes.General.DecodeBase64(App.Session_LoadSession("pass_")), false);
                }
                else
                {
                    if (NavigationContext.QueryString.Count() > 0)
                    {
                        if (NavigationService.Source.OriginalString.IndexOf("sforum_id") >= 0)
                            NavigationService.Navigate(new Uri("/forum/ForumTopic.xaml?sforum_id=" + NavigationContext.QueryString["sforum_id"] + "&forum_id=" + NavigationContext.QueryString["forum_id"] + "&redirected", UriKind.Relative));
                        else if (NavigationService.Source.OriginalString.IndexOf("forum_id") >= 0)
                            NavigationService.Navigate(new Uri("/forum/ForumSub.xaml?forum_id=" + NavigationContext.QueryString["forum_id"] + "&redirected", UriKind.Relative));
                        else
                            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
                    }
                    else
                        NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
                }
            }

            /*/if (App.Session_LoadSession("xf_session") != "")
            {
                enterText.Text = "Giriş yapılıyor...";
                LogIn.AutomaticLogin("none");
            }
            else
            {
                enterText.Text = "Yönlendiriliyor...";
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            }/*/
        }

        /*/private void ProgressBar_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }/*/
    }
}
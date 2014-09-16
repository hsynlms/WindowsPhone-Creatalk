using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Creatalk.Resources;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
//using Microsoft.Phone.Scheduler;

namespace Creatalk
{
    public class FavoriData
    {
        public string sforumID { get; set; }
        public string topicID { get; set; }
        public string topicTitle { get; set; }
        public string topicAuthorInfo { get; set; }
        public string topicAuthorAvatar { get; set; }
        public string IsTopic { get; set; }
        public string IsForum { get; set; }
        public string IsOthers { get; set; }
    }

    public partial class EnterForum : PhoneApplicationPage
    {
        ObservableCollection<FavoriData> ds = new ObservableCollection<FavoriData>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Veriler alınıyor..." };

        /*/int total_participated = 0;
        int total_page = 0;
        int current_page = 0;/*/

        //PeriodicTask periodicTask;

        public EnterForum()
        {
            InitializeComponent();

            TiltEffect.TiltableItems.Add(typeof(HubTile));

            SystemTray.SetProgressIndicator(this, progress);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (pages.SelectedIndex == 0)
            {
                if (MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
            else
            {
                e.Cancel = true;
                pages.SelectedIndex = 0;
            }
        }

        /*/void CallTask()
        {
            periodicTask = ScheduledActionService.Find("NotifyMe") as PeriodicTask;

            if (periodicTask != null)
                ScheduledActionService.Remove("NotifyMe");

            periodicTask = new PeriodicTask("NotifyMe");
            periodicTask.Description = "Arkaplanda bildirim almanızı sağlar";

            try
            {
                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest("NotifyMe", TimeSpan.FromSeconds(60));
            }
            catch
            {
                //hicbir sey yapma
            }
        }/*/

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            classes.Notification.GetNotifications(notification, ncount);
            classes.Notification.GetConversationNotification(mnotification, mcount, ccount);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //gecmisteki tum geri gidilebilecek sayfalari siliyoruz
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            m1.DataContext = App.Session_LoadSession("menuC1");
            m2.DataContext = App.Session_LoadSession("menuC1");
            m3.DataContext = App.Session_LoadSession("menuC1");
            m4.DataContext = App.Session_LoadSession("menuC1");
            m5.DataContext = App.Session_LoadSession("menuC1");

            //ayarlari yukluyoruz
            classes.General.GetVariables();

            CheckUSer();
            //CallTask();

            if (pages.SelectedIndex == 1)
                GetFavori();

            edition.Text = "v" + new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version.ToString();

            //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            //eger onceden giris yapilmis ise secim sayfasina yonlendir
            //if (App.Session_LoadSession("xf_session").Replace(" ", "") != "")
            //LogIn.AutomaticLogin();
            //else
            //NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        /*void GetConversationNotification()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "")
                return;

            progress.IsVisible = true;

            General.general_list.Clear();

            Oauth.XmlRpcExecMethod("get_inbox_stat", cCompleted);
        }

        void cCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return;

                if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "")
                    return;

                if (Oauth.ParseXML("inbox_unread_count", e.Result) != "0" && Oauth.ParseXML("inbox_unread_count", e.Result).Replace(" ", "") != "")
                {
                    ccount.Content = Oauth.ParseXML("inbox_unread_count", e.Result);
                    ccount.Visibility = Visibility.Visible;
                }
                else
                    ccount.Visibility = Visibility.Collapsed;

                progress.IsVisible = false;
            }
            catch
            {
                ccount.Visibility = Visibility.Collapsed;
                progress.IsVisible = false;
            }
        }*/

        void CheckUSer()
        {
            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && classes.General.username.Replace(" ", "") == "")
            {
                logimage.Source = new BitmapImage(new Uri("/images/login.png", UriKind.Relative));
                logtext.Text = "giriş yap";
            }
            else
            {
                logimage.Source = new BitmapImage(new Uri("/images/logout.png", UriKind.Relative));
                logtext.Text = "çıkış yap";
            }
        }

        private void profile_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            NavigationService.Navigate(new Uri("/Profile.xaml", UriKind.Relative));
        }

        private void message_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            NavigationService.Navigate(new Uri("/conv/Conversation.xaml", UriKind.Relative));
        }

        private void settings_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            CheckUSer();
        }

        private void forum_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/forum/Forum.xaml", UriKind.Relative));
        }

        /*/private void online_Click(object sender, RoutedEventArgs e)
        {
            if (General.CheckLoggedIn() == "Fail")
                return;

            NavigationService.Navigate(new Uri("/Online.xaml", UriKind.Relative));
        }/*/

        /*/private void login_Click(object sender, RoutedEventArgs e)
        {
            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "")
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            else
            {
                if (General.CheckLoggedIn() == "Fail")
                    return;

                IsolatedStorageSettings.ApplicationSettings.Clear();

                General.general_list.Clear();
                General.username = "";

                Oauth.XmlRpcExecMethod("logout_user", uCompleted);
            }
        }/*/

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pages.SelectedIndex == 1)
                GetFavori();
        }

        void GetFavori()
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            try
            {
                ds.Clear();

                string already = "";

                for (int i = 0; i < classes.General.ReadFavori("fav_forum.txt").Split('#').Count() - 1; i++)
                {
                    already += classes.General.ReadFavori("fav_forum.txt").Split('#')[i].Split('|')[0] + ",";

                    ds.Add(new FavoriData()
                    {
                        sforumID = classes.General.ReadFavori("fav_forum.txt").Split('#')[i].Split('|')[0],
                        topicTitle = classes.General.ReadFavori("fav_forum.txt").Split('#')[i].Split('|')[1],
                        IsTopic = "Collapsed",
                        IsForum = "Visible",
                        IsOthers = "Collapsed"
                    });

                    for (int x = 0; x < classes.General.ReadFavori("fav_topic.txt").Split('#').Count() - 1; x++)
                    {
                        if (classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[0] != classes.General.ReadFavori("fav_forum.txt").Split('#')[i].Split('|')[0])
                            continue;

                        ds.Add(new FavoriData()
                        {
                            sforumID = classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[0],
                            topicID = classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[1],
                            topicTitle = classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[2],
                            topicAuthorInfo = classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[3],
                            topicAuthorAvatar = classes.General.ReadFavori("fav_topic.txt").Split('#')[x].Split('|')[4],
                            IsTopic = "Visible",
                            IsForum = "Collapsed",
                            IsOthers = "Collapsed"
                        });
                    }
                }

                if (classes.General.ReadFavori("fav_forum.txt") != "" && classes.General.ReadFavori("fav_topic.txt") != "")
                {
                    ds.Add(new FavoriData()
                    {
                        sforumID = "0",
                        IsTopic = "Collapsed",
                        IsForum = "Collapsed",
                        IsOthers = "Visible"
                    });
                }

                for (int i = 0; i < classes.General.ReadFavori("fav_topic.txt").Split('#').Count() - 1; i++)
                {
                    if (already.IndexOf(classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[0] + ",") >= 0)
                        continue;

                    ds.Add(new FavoriData()
                    {
                        sforumID = classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[0],
                        topicID = classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[1],
                        topicTitle = classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[2],
                        topicAuthorInfo = classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[3],
                        topicAuthorAvatar = classes.General.ReadFavori("fav_topic.txt").Split('#')[i].Split('|')[4],
                        IsTopic = "Visible",
                        IsForum = "Collapsed",
                        IsOthers = "Collapsed"
                    });
                }

                if (ds.Count() > 0)
                    nofav.Visibility = Visibility.Collapsed;
                else
                    nofav.Visibility = Visibility.Visible;

                favoriList.ItemsSource = ds;
                progress.IsVisible = false;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                progress.IsVisible = false;
            }
        }

        private void unfavori_Click(object sender, RoutedEventArgs e)
        {
            FavoriData drv = (sender as MenuItem).DataContext as FavoriData;

            if (drv != null)
            {
                if (drv.IsTopic == "Visible")
                {
                    string delete_this = classes.General.ReadFavori("fav_topic.txt").Replace(drv.sforumID + "|" + drv.topicID + "|" + drv.topicTitle + "|" + drv.topicAuthorInfo + "|" + drv.topicAuthorAvatar + "#", "");
                    classes.General.WriteFavori("fav_topic.txt", delete_this);
                }
                else if (drv.IsTopic == "Collapsed" && drv.sforumID != "0")
                {
                    string delete_this = classes.General.ReadFavori("fav_forum.txt").Replace(drv.sforumID + "|" + drv.topicTitle + "#", "");
                    classes.General.WriteFavori("fav_forum.txt", delete_this);
                }

                ds.Clear();
                GetFavori();
            }
        }

        private void favoriList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FavoriData drv = (FavoriData)favoriList.SelectedItem;

            if (drv != null)
            {
                if (drv.IsTopic == "Visible")
                    NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id= " + drv.sforumID + "&topic_id=" + drv.topicID, UriKind.Relative));
                else if (drv.IsTopic == "Collapsed" && drv.sforumID != "0")
                    NavigationService.Navigate(new Uri("/forum/ForumTopic.xaml?sforum_id=" + drv.sforumID, UriKind.Relative));
            }

            favoriList.SelectedIndex = -1;
        }

        private void search_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void login_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && classes.General.username.Replace(" ", "") == "")
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
            else
            {
                if (classes.General.CheckLoggedIn() == "Fail")
                    return;

                notification.Visibility = Visibility.Collapsed;

                App.SaveSettings("user_", "");
                App.SaveSettings("pass_", "");
                App.SaveSettings("xf_session", "");

                classes.General.general_list.Clear();
                classes.General.username = "";

                classes.Oauth.XmlRpcExecMethod("logout_user", uCompleted);
            }
        }

        private void mnotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/conv/Conversation.xaml", UriKind.Relative));
        }

        private void notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/forum/Forum.xaml?alerts", UriKind.Relative));
        }

        private void contactus_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = "developer@apps-dev.com";
            email.Subject = "Creatalk Uygulaması Hakkında"; //hdp.
            email.Show();
        }
    }
}
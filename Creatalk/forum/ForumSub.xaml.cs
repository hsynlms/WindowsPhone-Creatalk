using System;
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
using System.Reflection;

namespace Creatalk.forum
{
    public class ForumSubf
    {
        public string fName { get; set; }
        public string fDescription { get; set; }
        public string fID { get; set; }
        //public string fNewPost { get; set; }
        //public string fUnreadCount { get; set; }
        //public string fProtected { get; set; }
        //public string fCanSubscribe { get; set; }
        //public string fSubscribed { get; set; }
        public string fImage { get; set; }
    }

    public partial class ForumSub : PhoneApplicationPage
    {
        ObservableCollection<ForumSubf> ds = new ObservableCollection<ForumSubf>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Forumlar alınıyor..." };

        string forum_id = "-1";

        ApplicationBarIconButton m_button;

        //short reLoad = 0;
        short isLoaded = 0;

        public ForumSub()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);
            m_button = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        }

        /*/protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            reLoad = 1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (reLoad == 1)
            {
                //otomatik giris yapiyoruz
                LogIn.AutomaticLogin();
            }
        }/*/

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (NavigationService.Source.OriginalString.IndexOf("redirected") >= 0)
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/forum/Forum.xaml?redirected", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            classes.Notification.GetNotifications(notification, ncount);
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.Count() < 1)
            {
                m_button.IsEnabled = false;
                MessageBox.Show("Eksik parametre.");
                return;
            }

            if (isLoaded == 0)
            {
                isLoaded = 1;
                GetSubForums();
            }
        }

        void GetSubForums()
        {
            forum_id = NavigationContext.QueryString["forum_id"];

            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            ds.Clear();

            classes.General.general_list.Clear();
            classes.General.general_list.Add(false);
            classes.General.general_list.Add(forum_id);

            classes.Oauth.XmlRpcExecMethod("get_forum", uCompleted);
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                ds.Clear();

                subName.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML2(XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct").Elements("member"), "forum_name"));

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    if (classes.Oauth.ParseXML2(round.Elements("member"), "forum_id") == forum_id)
                        continue;

                    string image = (classes.Oauth.ParseXML2(round.Elements("member"), "unread_count") == "0") ? "/images/read.png" : "/images/unread.png";
                    ds.Add(new ForumSubf()
                    {
                        fName = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "forum_name")),
                        fDescription = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "description")),
                        fID = classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"),
                        fImage = image
                    });
                }
                subfList.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        classes.General.UpdateLayout(subfList, true);
                    });
                }

                m_button.IsEnabled = true;
                progress.IsVisible = false;
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
                progress.IsVisible = false;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : f0x002");
                //General.SendBugReport(e.Result, "f0x002", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/forum/Forum.xaml?alerts", UriKind.Relative));
        }

        private void mnotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/conv/Conversation.xaml", UriKind.Relative));
        }

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            GetSubForums();
            m_button.IsEnabled = false;
        }

        private void read_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumSubf drv = (sender as MenuItem).DataContext as ForumSubf;

            if (drv != null)
            {
                if (MessageBox.Show("Bu forumu okundu olarak işaretlemek istiyor musunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    classes.General.general_list.Clear();
                    classes.General.general_list.Add(drv.fID);
                    classes.Oauth.XmlRpcExecMethod("mark_all_as_read", xCompleted);
                }
            }
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                    GetSubForums();
                else if (classes.Oauth.ParseXML("result", e.Result) == "0" && classes.Oauth.ParseXML("result_text", e.Result) != "")
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : f0x005");
                //General.SendBugReport(e.Result, "f0x005", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void pin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ForumSubf drv = (sender as MenuItem).DataContext as ForumSubf;

                if (drv != null)
                {
                    FlipTileData tile = new FlipTileData
                    {
                        Title = drv.fName,
                        BackgroundImage = new Uri("/images/forum_background.png", UriKind.Relative),
                        WideBackBackgroundImage = new Uri("/images/forum_background_large.png", UriKind.Relative)
                    };

                    ShellTile.Create(new Uri("/MainPage.xaml?sforum_id=" + drv.fID + "&forum_id=" + forum_id, UriKind.Relative), tile, true);
                }
            }
            catch
            {
            }
        }

        private void favori_Click(object sender, RoutedEventArgs e)
        {
            ForumSubf drv = (sender as MenuItem).DataContext as ForumSubf;

            if (drv != null)
            {
                classes.General.WriteFavori("fav_forum.txt", classes.General.ReadFavori("fav_forum.txt") + drv.fID + "|" + drv.fName + "#");
                MessageBox.Show("Favorilere başarıyla eklendi.");
            }
        }

        private void subfList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForumSubf drv = (ForumSubf)subfList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumTopic.xaml?sforum_id=" + drv.fID, UriKind.Relative));
            }

            subfList.SelectedIndex = -1;
        }
    }
}
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
using System.Net.NetworkInformation;

namespace Creatalk.forum
{
    public class Notifications
    {
        public string NewAlert { get; set; }
        public string Message { get; set; }
        public string ShortContent { get; set; }
        public string PostTime { get; set; }
        public string PostID { get; set; }
        public string UserAvatar { get; set; }
        public string UserName { get; set; }
        public string TopicID { get; set; }
        public string backGround { get; set; }

    }
    public partial class Forum : PhoneApplicationPage
    {
        ObservableCollection<ForumSubf> ds = new ObservableCollection<ForumSubf>();
        ObservableCollection<UserTopic> cs = new ObservableCollection<UserTopic>();
        ObservableCollection<Notifications> ns = new ObservableCollection<Notifications>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Veriler alınıyor..." };

        short isLoaded = 0;
        short reLoad = 0;

        ApplicationBarIconButton m_button;

        public Forum()
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
                NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && classes.General.username.Replace(" ", "") == "") //hdp.
                if (pages.Items.Count() > 2)
                    pages.Items.RemoveAt(2);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService.Source.OriginalString.IndexOf("alerts") >= 0)
            {
                pages.SelectedIndex = 2;
            }

            if (pages.SelectedIndex == 1 && reLoad == 0)
            {
                reLoad = 1;
                recentList.ItemsSource = null;
                GetLists();
            }

            /*else
            {
                if (isLoaded == 0)
                {
                    isLoaded = 1;

                    ds.Clear();*/

            /*for (int i = 0; i < forums.Split(',').Count(); i++)
                ds.Add(new ForumSubf() { fName = forums.Split(',')[i], fID = forums_id.Split(',')[i] });

            allforums.ItemsSource = ds;*/

            /*GetForums();
        }
    }*/
        }

        void GetForums()
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            classes.General.general_list.Clear();
            classes.General.general_list.Add(false);
            classes.General.general_list.Add("0");

            classes.Oauth.XmlRpcExecMethod("get_forum", uCompleted);

            GetNotifications();
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                ds.Clear();

                if (e.Result == "")
                    return;

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    if (classes.Oauth.ParseXML2(round.Elements("member"), "parent_id") != "0")
                        continue;

                    string temp = classes.General.SetForumLogo(classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"));

                    //MessageBox.Show(General.DecodeBase64(Oauth.ParseXML2(round.Elements("member"), "description")));

                    ds.Add(new ForumSubf()
                    {
                        fName = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "forum_name")),
                        fID = classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"),
                        fImage = temp
                    });
                }

                allforums.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        do
                        {
                            if (allforums.Items.Count() == ds.Count())
                            {
                                classes.General.UpdateLayout(this.allforums, true);
                                return;
                            }
                        } while (allforums.Items.Count() != ds.Count());
                    });
                }

                progress.IsVisible = false;
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
                progress.IsVisible = false;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : f0x001");
                //General.SendBugReport(e.Result, "f0x001", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            //GetForums();
            GetNotifications();
            notification.Visibility = Visibility.Collapsed;
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }

        private void allforums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForumSubf drv = (ForumSubf)allforums.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumSub.xaml?forum_id=" + drv.fID, UriKind.Relative));
            }

            allforums.SelectedIndex = -1;
        }

        private void pin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ForumSubf drv = (sender as MenuItem).DataContext as ForumSubf;

                if (drv != null)
                {
                    FlipTileData ftd = new FlipTileData()
                    {
                        Title = drv.fName,
                        SmallBackgroundImage = new Uri("/images/forum_background.png", UriKind.Relative),
                        BackgroundImage = new Uri("/images/forum_background.png", UriKind.Relative),
                        WideBackgroundImage = new Uri("/images/forum_background_large.png", UriKind.Relative),
                    };

                    /*StandardTileData std = new StandardTileData();
                    std.Title = drv.fName;
                    std.BackgroundImage = new Uri("/images/forum_background.png", UriKind.Relative);*/

                    //ShellTile.Create(new Uri("/ForumSub.xaml?forum_id=" + drv.fID, UriKind.Relative), std);
                    ShellTile.Create(new Uri("/MainPage.xaml?forum_id=" + drv.fID, UriKind.Relative), ftd, true);
                }
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);
                //hicbir sey yapma
            }
        }

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        void GetLists()
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            classes.General.general_list.Clear();

            if (pages.SelectedIndex == 1)
            {
                classes.General.general_list.Add(0);
                classes.General.general_list.Add(classes.General.sayfabasiRecent - 1);
                classes.Oauth.XmlRpcExecMethod("get_latest_topic", xCompleted);
            }
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_button.IsEnabled = false;

            GetNotifications();
            classes.Notification.GetConversationNotification(mnotification, mcount, null);

            if (pages.SelectedIndex == 1)
            {
                if (reLoad == 0)
                {
                    reLoad = 1;
                    recentList.ItemsSource = null;
                    GetLists();
                }
            }
            else if (pages.SelectedIndex == 2)
            {
                m_button.IsEnabled = true;
                notification.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (isLoaded == 0)
                {
                    isLoaded = 1;

                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    ds.Clear();

                    GetForums();
                }
            }
        }

        private void recentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserTopic drv = (UserTopic)recentList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    reLoad = 0;

                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=" + drv.utForumID + "&topic_id=" + drv.utID, UriKind.Relative));
            }

            recentList.SelectedIndex = -1;
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                cs.Clear();

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", avatar_url = "", temp = "";

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "") != "")
                    {
                        string[] temp1 = classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "icon_url") != "")
                        avatar_url = classes.Oauth.ParseXML2(round.Elements("member"), "icon_url");
                    else
                        avatar_url = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png";

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "new_post") != "0") ? "OrangeRed" : "#FF454545";

                    cs.Add(new UserTopic()
                    {
                        utTitle = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "topic_title")),
                        utID = classes.Oauth.ParseXML2(round.Elements("member"), "topic_id"),
                        utForumID = classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"),
                        //utNewPost = Oauth.ParseXML2(round.Elements("member"), "new_post"),
                        utAuthorAvatar = avatar_url,
                        utLastReplyTime = sentDate,
                        utIsClosed = (classes.Oauth.ParseXML2(round.Elements("member"), "is_closed") == "0") ? "Collapsed" : "Visible",
                        utReplyNumber = classes.Oauth.ParseXML2(round.Elements("member"), "reply_number"),
                        utLastReplyAuthor = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) + ", " + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "user_type")) + " üye",
                        utUnread = temp
                    });
                }

                recentList.ItemsSource = cs;

                if (cs.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (recentList.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(recentList, true);
                                return;
                            }
                        } while (recentList.Items.Count() != ds.Count());/*/
                        classes.General.UpdateLayout(recentList, true);
                    });
                }

                progress.IsVisible = false;
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
                progress.IsVisible = false;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : m0x001");
                //General.SendBugReport(e.Result, "m0x001", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void favori_Click(object sender, RoutedEventArgs e)
        {
            UserTopic drv = (sender as MenuItem).DataContext as UserTopic;

            if (drv != null)
            {
                classes.General.WriteFavori("fav_topic.txt", classes.General.ReadFavori("fav_topic.txt") + drv.utForumID + "|" + drv.utID + "|" + drv.utTitle + "|" + drv.utLastReplyAuthor + "|" + drv.utAuthorAvatar + "#");
                MessageBox.Show("Favorilere başarıyla eklendi.");
            }
        }

        void GetNotifications()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && classes.General.username.Replace(" ", "") == "")
                return;

            //progress.IsVisible = true;

            //parametre listesini temizliyoruz
            classes.General.general_list.Clear();

            if (pages.SelectedIndex != 2)
                classes.General.general_list.Add(false);
            else
                classes.General.general_list.Add(true);

            //apiyi cagirma komutunu kullaniyoruz
            classes.Oauth.XmlRpcExecMethod("get_notification", nCompleted);
        }

        void nCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return;

                if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && classes.General.username.Replace(" ", "") == "")
                    return;

                if (classes.Oauth.ParseXML("new_alerts", e.Result) != "0" && classes.Oauth.ParseXML("new_alerts", e.Result).Replace(" ", "") != "")
                {
                    ncount.Text = classes.Oauth.ParseXML("new_alerts", e.Result); //+" bildirim";
                    notification.Visibility = Visibility.Visible;
                }
                else
                    notification.Visibility = Visibility.Collapsed;

                if (pages.SelectedIndex != 2)
                    return;

                progress.IsVisible = true;

                ns.Clear();

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", avatar_url = "", temp = "";

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "") != "")
                    {
                        string[] temp1 = classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "icon_url") != "")
                        avatar_url = classes.Oauth.ParseXML2(round.Elements("member"), "icon_url");
                    else
                        avatar_url = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png";

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "new_alert") != "0") ? "#FFc00202" : "#FF333333";

                    ns.Add(new Notifications()
                    {
                        NewAlert = classes.Oauth.ParseXML2(round.Elements("member"), "new_alert"),
                        Message = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "message")),
                        ShortContent = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "short_content")),
                        PostTime = sentDate,
                        PostID = classes.Oauth.ParseXML2(round.Elements("member"), "post_id"),
                        UserAvatar = avatar_url,
                        UserName = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "username")),
                        TopicID = classes.Oauth.ParseXML2(round.Elements("member"), "topic_id"),
                        backGround = temp
                    });
                }

                alertList.ItemsSource = ns;

                if (ns.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (alertList.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(this.alertList, true);
                                return;
                            }
                        } while (alertList.Items.Count() != ds.Count());/*/
                        classes.General.UpdateLayout(this.alertList, true);
                    });
                }

                progress.IsVisible = false;
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                //MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
                progress.IsVisible = false;
                notification.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                //MessageBox.Show("Bir hata oluştu : a0x003");
                //General.SendBugReport(e.Result, "a0x003", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
                notification.Visibility = Visibility.Collapsed;
            }
        }

        private void alertList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Notifications drv = (Notifications)alertList.SelectedItem;

            if (drv != null)
            {
                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=0&topic_id=" + drv.TopicID + "&post_id=" + drv.PostID + "&gbp", UriKind.Relative));
            }

            alertList.SelectedIndex = -1;
        }

        private void notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/forum/Forum.xaml?alerts", UriKind.Relative));
        }

        private void mnotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/conv/Conversation.xaml", UriKind.Relative));
        }
    }
}
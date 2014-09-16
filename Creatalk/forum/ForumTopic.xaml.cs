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
using System.Windows.Threading;
using System.Reflection;

namespace Creatalk.forum
{
    public class ForumTopics
    {
        public string delVisible { get; set; }
        public string editVisible { get; set; }
        public string lockVisible { get; set; }
        public string onayVisible { get; set; }
        public string tAuthorAvatar { get; set; }
        public string tAuthorInfo { get; set; }
        public string tID { get; set; }
        public string tIsClosed { get; set; }
        public string tLastReplyTime { get; set; }
        public string tReplyNumber { get; set; }
        public string tTitle { get; set; }
        public string tUnread { get; set; }
        public string unlockVisible { get; set; }
    }

    public partial class ForumTopic : PhoneApplicationPage
    {
        ObservableCollection<ForumTopics> ds = new ObservableCollection<ForumTopics>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Konular alınıyor..." };

        string sforum_id = "1";
        string get_mode = "";

        int total_topic = 0;
        int total_page = 0;
        int current_page = 1;

        short isLoaded = 0;

        ApplicationBarIconButton m_button;

        public ForumTopic()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);
        }

        void SetButton(short bid, bool isenabled)
        {
            m_button = ApplicationBar.Buttons[bid] as ApplicationBarIconButton;
            m_button.IsEnabled = isenabled;
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.Count() >= 1)
            {
                if (isLoaded == 0)
                {
                    isLoaded = 1;
                    GetTopics();
                }
                return;
            }

            SetButton(0, false);
            SetButton(1, false);
            SetButton(2, false);
            ApplicationBar.IsMenuEnabled = false;
            MessageBox.Show("Eksik parametre.");
        }

        private void GetTopics()
        {
            ListBox listBox = (get_mode != "TOP" ? ntopicList : stopicList);

            sforum_id = NavigationContext.QueryString["sforum_id"].ToString();

            SetButton(0, false);
            SetButton(2, false);

            ApplicationBar.IsMenuEnabled = false;

            get_mode = (topics.SelectedIndex == 1) ? "TOP" : "";

            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;
            listBox.ItemsSource = null;

            ds.Clear();

            if (current_page == 0)
                current_page = total_page;

            classes.General.general_list.Clear();
            classes.General.general_list.Add(this.sforum_id);

            if (current_page != 1)
                classes.General.general_list.Add(this.current_page * classes.General.sayfabasiTopic - (classes.General.sayfabasiTopic - 1));
            else
                classes.General.general_list.Add(this.current_page * classes.General.sayfabasiTopic - classes.General.sayfabasiTopic);

            classes.General.general_list.Add(current_page * classes.General.sayfabasiTopic);
            classes.General.general_list.Add(get_mode);

            classes.Oauth.XmlRpcExecMethod("get_topic", uCompleted);
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                ListBox item = (get_mode != "TOP") ? ntopicList : stopicList;

                ds.Clear();

                topicName.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("forum_name", e.Result));
                total_topic = Convert.ToInt32(classes.Oauth.ParseXML("total_topic_num", e.Result));

                if (total_topic % classes.General.sayfabasiTopic <= 0)
                    total_page = total_topic / classes.General.sayfabasiTopic;
                else
                    total_page = (total_topic - total_topic % classes.General.sayfabasiTopic) / classes.General.sayfabasiTopic + 1;

                vpage.Text = "Sayfa: " + current_page.ToString() + " / " + total_page.ToString();

                SetButton(2, (current_page >= total_page ? false : true));
                SetButton(0, (current_page < 2 ? false : true));

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", avatar_url = "", unread = "";

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "last_reply_time").Replace(" ", "") != "")
                    {
                        string[] temp1 = classes.Oauth.ParseXML2(round.Elements("member"), "last_reply_time").Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    avatar_url = (classes.Oauth.ParseXML2(round.Elements("member"), "icon_url") == "") ? classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png" : classes.Oauth.ParseXML2(round.Elements("member"), "icon_url");
                    unread = (classes.Oauth.ParseXML2(round.Elements("member"), "new_post") != "0" ? "OrangeRed" : "#FF454545");

                    ds.Add(new ForumTopics()
                    {
                        tTitle = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "topic_title")),
                        tID = classes.Oauth.ParseXML2(round.Elements("member"), "topic_id"),
                        tIsClosed = (classes.Oauth.ParseXML2(round.Elements("member"), "is_closed") == "0") ? "Collapsed" : "Visible",
                        tAuthorAvatar = avatar_url,
                        tLastReplyTime = sentDate,
                        tReplyNumber = classes.Oauth.ParseXML2(round.Elements("member"), "reply_number"),
                        tUnread = unread,
                        tAuthorInfo = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "topic_author_name")) + ", " + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "user_type")) + " üye",
                        delVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_delete") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_deleted") == "0") ? "Visible" : "Collapsed",
                        lockVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_close") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_closed") == "0") ? "Visible" : "Collapsed",
                        unlockVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_close") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_closed") == "1") ? "Visible" : "Collapsed",
                        editVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_rename") == "1") ? "Visible" : "Collapsed",
                        onayVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_approve") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_approved") == "1") ? "Visible" : "Collapsed"
                    });
                }
                item.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        classes.General.UpdateLayout(item, true);
                    });
                }

                SetButton(1, true);
                ApplicationBar.IsMenuEnabled = true;
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

                MessageBox.Show("Bir hata oluştu : f0x003");
                //General.SendBugReport(e.Result, "f0x003", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void favori_Click(object sender, RoutedEventArgs e)
        {
            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                classes.General.WriteFavori("fav_topic.txt", classes.General.ReadFavori("fav_topic.txt") + sforum_id + "|" + drv.tID + "|" + drv.tTitle + "|" + drv.tAuthorInfo + "|" + drv.tAuthorAvatar + "#");
                MessageBox.Show("Favorilere başarıyla eklendi.");
            }
        }

        private void lock_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(drv.tID);
                classes.General.general_list.Add(2);

                classes.Oauth.XmlRpcExecMethod("m_close_topic", mCompleted);
            }
        }

        public void mCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1")
                {
                    MessageBox.Show("İşlem başarılı.");
                    GetTopics();
                }
                else if (classes.Oauth.ParseXML("result", e.Result) == "0" && classes.Oauth.ParseXML("result_text", e.Result) != "")
                {
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
                }
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : ad0x001");
                //General.SendBugReport(e.Result, "ad0x001", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void unlock_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(drv.tID);
                classes.General.general_list.Add(1);

                classes.Oauth.XmlRpcExecMethod("m_close_topic", mCompleted);
            }
        }

        private void approve_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(drv.tID);
                classes.General.general_list.Add(2);

                classes.Oauth.XmlRpcExecMethod("m_approve_topic", mCompleted);
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(drv.tID);
                classes.General.general_list.Add(1);
                classes.General.general_list.Add("base64|");

                classes.Oauth.XmlRpcExecMethod("m_delete_topic", mCompleted);
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            ForumTopics drv = (sender as MenuItem).DataContext as ForumTopics;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                classes.General.topic_title = drv.tTitle;
                NavigationService.Navigate(new Uri("/topic/TopicEdit.xaml?topic_id=" + drv.tID, UriKind.Relative));
            }
        }

        private void prev_Click(object sender, EventArgs e)
        {
            current_page -= 1;
            GetTopics();
        }

        private void next_Click(object sender, EventArgs e)
        {
            current_page += 1;
            GetTopics();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            GetTopics();
            SetButton(1, false);
        }

        private void new_Click(object sender, EventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            if (App.Session_LoadSession("aRefresh") == "1")
                isLoaded = 0;

            NavigationService.Navigate(new Uri("/topic/TopicCreate.xaml?sforum_id=" + sforum_id, UriKind.Relative));
        }

        private void last_Click(object sender, EventArgs e)
        {
            current_page = 1;
            GetTopics();
        }

        private void first_Click(object sender, EventArgs e)
        {
            current_page = 0;
            GetTopics();
        }

        private void read_Click(object sender, EventArgs e)
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            classes.General.general_list.Clear();
            classes.General.general_list.Add(sforum_id);

            classes.Oauth.XmlRpcExecMethod("mark_all_as_read", xCompleted);
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                    GetTopics();
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

                MessageBox.Show("Bir hata oluştu : f0x004");
                //General.SendBugReport(e.Result, "f0x004", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void ntopicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForumTopics drv = (ForumTopics)ntopicList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    this.isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=" + sforum_id + "&topic_id=" + drv.tID, UriKind.Relative));
            }

            ntopicList.SelectedIndex = -1;
        }

        private void stopicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForumTopics drv = (ForumTopics)stopicList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=" + sforum_id + "&topic_id=" + drv.tID, UriKind.Relative));
            }

            stopicList.SelectedIndex = -1;
        }

        private void topics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current_page = 1;
            GetTopics();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (NavigationService.Source.OriginalString.IndexOf("redirected") >= 0 && NavigationService.Source.OriginalString.IndexOf("&forum_id") >= 0)
            {
                e.Cancel = true;
                NavigationService.Navigate(new Uri("/forum/ForumSub.xaml?forum_id=" + NavigationContext.QueryString["forum_id"].ToString() + "&redirected", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            classes.Notification.GetNotifications(notification, ncount);
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }
    }
}
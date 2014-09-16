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
using System.Windows.Media.Imaging;
using System.Reflection;

namespace Creatalk
{
    public class UserTopic
    {
        public string utID { get; set; }
        public string utForumID { get; set; }
        public string utTitle { get; set; }
        public string utAuthorAvatar { get; set; }
        public string utLastReplyAuthor { get; set; }
        public string utLastReplyTime { get; set; }
        public string utReplyNumber { get; set; }
        //public string utNewPost { get; set; }
        public string utUnread { get; set; }
        public string utIsClosed { get; set; }
    }

    public partial class Profile : PhoneApplicationPage
    {
        ObservableCollection<UserTopic> ds = new ObservableCollection<UserTopic>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Bilgiler alınıyor..." };

        short isLoaded = 1;
        //short reLoad = 0;

        int total_page = 0;
        int current_page = 1;
        int total_participated = 0;

        ApplicationBarIconButton m_button;

        public Profile()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);
        }

        void SetButton(short bid, bool isenabled)
        {
            m_button = ApplicationBar.Buttons[bid] as ApplicationBarIconButton;
            m_button.IsEnabled = isenabled;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (classes.General.username == "" || App.Session_LoadSession("xf_session") == "")
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (classes.General.username == "" || App.Session_LoadSession("xf_session") == "")
                NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded == 0)
            {
                isLoaded = 1;

                if (pages.SelectedIndex == 2)
                    ApplicationBar.IsVisible = true;
                else
                    ApplicationBar.IsVisible = false;

                GetProfile();
            }
        }

        void GetProfile()
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            ApplicationBar.IsMenuEnabled = false;

            progress.IsVisible = true;

            classes.General.general_list.Clear();

            if (NavigationContext.QueryString.Count() > 0)
            {
                if (NavigationContext.QueryString["user_name"] != "")
                    classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(NavigationContext.QueryString["user_name"]));
                else
                {
                    sendMessage.IsEnabled = false;
                    //reportUser.IsEnabled = false;
                    return;
                }
            }
            else
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(classes.General.username));

            if (pages.SelectedIndex == 0)
                classes.Oauth.XmlRpcExecMethod("get_user_info", uCompleted);
            else if (pages.SelectedIndex == 2)
            {
                if ((total_participated % classes.General.sayfabasiParticipated) > 0)
                    total_page = ((total_participated - (total_participated % classes.General.sayfabasiParticipated)) / classes.General.sayfabasiParticipated) + 1;
                else
                    total_page = (total_participated / classes.General.sayfabasiParticipated);

                if (current_page == 0)
                    current_page = total_page;

                if (current_page == 1)
                    classes.General.general_list.Add((current_page * classes.General.sayfabasiParticipated) - classes.General.sayfabasiParticipated);
                else
                    classes.General.general_list.Add((current_page * classes.General.sayfabasiParticipated) - (classes.General.sayfabasiParticipated - 1));

                classes.General.general_list.Add(current_page * classes.General.sayfabasiParticipated);

                classes.Oauth.XmlRpcExecMethod("get_participated_topic", zCompleted);
            }
            else
            {
                usertopicList.ItemsSource = null;
                classes.Oauth.XmlRpcExecMethod("get_user_topic", xCompleted);
            }
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "0")
                {
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
                    progress.IsVisible = false;
                    return;
                }

                if (classes.Oauth.ParseXML("icon_url", e.Result) != "")
                    avatar.Source = new BitmapImage(new Uri(classes.Oauth.ParseXML("icon_url", e.Result), UriKind.Absolute));
                else
                    avatar.Source = new BitmapImage(new Uri(classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png", UriKind.Absolute)); //hdp.

                if (classes.Oauth.ParseXML("is_ban", e.Result) == "1")
                    u_name.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("username", e.Result)) + " (Cezalı)";
                else
                    u_name.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("username", e.Result));

                u_posts.Text = classes.Oauth.ParseXML("post_count", e.Result) + " mesaj";

                if (NavigationContext.QueryString.Count() <= 0)
                    u_mail.Text = classes.General.DecodeBase64(App.Session_LoadSession("email"));
                else
                {
                    if (classes.General.DecodeBase64(classes.Oauth.ParseXML("username", e.Result)) != classes.General.username)
                    {
                        sendMessage.IsEnabled = true;
                        //reportUser.IsEnabled = true;
                    }
                }

                isOnline.Text = (classes.Oauth.ParseXML("is_online", e.Result) == "1") ? "Üye şuanda çevrimiçidir." : "Üye şuanda çevrimdışıdır.";
                LayoutRoot.DataContext = (classes.Oauth.ParseXML("is_online", e.Result) == "1") ? "/images/online.png" : "/images/offline.png";

                uType.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("display_text", e.Result));
                lastAct.Text = "-> " + classes.General.DecodeBase64(classes.Oauth.ParseXML("current_activity", e.Result));

                var xtemp = XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct");

                where1.Text = classes.General.DecodeBase64(xtemp.ElementAt(0).Elements("member").First().Element("value").Value);
                where2.Text = ": " + classes.General.DecodeBase64(xtemp.ElementAt(0).Elements("member").Last().Element("value").Value);

                job1.Text = classes.General.DecodeBase64(xtemp.ElementAt(1).Elements("member").First().Element("value").Value);
                job2.Text = ": " + classes.General.DecodeBase64(xtemp.ElementAt(1).Elements("member").Last().Element("value").Value);

                website1.Text = classes.General.DecodeBase64(xtemp.ElementAt(2).Elements("member").First().Element("value").Value);
                website2.Text = ": " + classes.General.DecodeBase64(xtemp.ElementAt(2).Elements("member").Last().Element("value").Value);

                /*/foreach (XElement round in xtemp)
                {
                    //MessageBox.Show(General.DecodeBase64(round.Elements("member").First().Element("value").Value));
                }/*/

                string[] temp = classes.Oauth.ParseXML("last_activity_time", e.Result).Replace(" ", "").Split('T');
                DateTime date = new DateTime();
                DateTime.TryParse(temp[0].Substring(temp[0].Length - 2, 2) + "/" + temp[0].Substring(temp[0].Length - 4, 2) + "/" + temp[0].Substring(0, 4) + " " + temp[1].Substring(0, 2) + ":" + temp[1].Substring(3, 2) + ":" + temp[1].Substring(6, 2), out date);
                u_last.Text = date.ToString();

                ApplicationBar.IsMenuEnabled = true;
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

                MessageBox.Show("Bir hata oluştu : a0x001");
                //General.SendBugReport(e.Result, "a0x001", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/conv/ConvCreate.xaml?user_name=" + u_name.Text, UriKind.Relative));
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                ds.Clear();

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
                        avatar_url = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png"; //hdp.

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "new_post") != "0") ? "OrangeRed" : "#FF454545";

                    ds.Add(new UserTopic()
                    {
                        utTitle = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "topic_title")),
                        utID = classes.Oauth.ParseXML2(round.Elements("member"), "topic_id"),
                        utForumID = classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"),
                        //utNewPost = Oauth.ParseXML2(round.Elements("member"), "new_post"),
                        utAuthorAvatar = avatar_url,
                        utLastReplyTime = sentDate,
                        utIsClosed = (classes.Oauth.ParseXML2(round.Elements("member"), "is_closed") == "0") ? "Collapsed" : "Visible",
                        utReplyNumber = classes.Oauth.ParseXML2(round.Elements("member"), "reply_number"),
                        utLastReplyAuthor = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "last_reply_author_name")) + ", " + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "user_type")) + " üye",
                        utUnread = temp
                    });
                }

                usertopicList.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (usertopicList.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(this.usertopicList, true);
                                return;
                            }
                        } while (usertopicList.Items.Count() != ds.Count());/*/
                        classes.General.UpdateLayout(this.usertopicList, true);
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

                MessageBox.Show("Bir hata oluştu : a0x002");
                //General.SendBugReport(e.Result, "a0x002", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        public void zCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                ds.Clear();

                //toplam yanit sayisini aliyoruz
                total_participated = Convert.ToInt32(classes.Oauth.ParseXML("total_topic_num", e.Result));

                if ((total_participated % classes.General.sayfabasiParticipated) > 0)
                    total_page = ((total_participated - (total_participated % classes.General.sayfabasiParticipated)) / classes.General.sayfabasiParticipated) + 1;
                else
                    total_page = (total_participated / classes.General.sayfabasiParticipated);

                vpage.Text = "Sayfa: " + current_page.ToString() + " / " + total_page.ToString();

                //eger suanki sayfa numarasi toplam sayfa sayisina esit ise sonraki sayfa butonunu kilitle
                SetButton(0, (current_page < 2) ? false : true);

                //eger suanki sayfa numarasi 2 den kucuk ise onceki sayfa butonunu kilitle
                SetButton(2, (current_page >= total_page) ? false : true);

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
                        avatar_url = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png"; //hdp.

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "new_post") != "0") ? "OrangeRed" : "#FF454545";

                    ds.Add(new UserTopic()
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

                participatedList.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (usertopicList.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(this.usertopicList, true);
                                return;
                            }
                        } while (usertopicList.Items.Count() != ds.Count());/*/
                        classes.General.UpdateLayout(this.participatedList, true);
                    });
                }

                SetButton(1, true);

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

                MessageBox.Show("Bir hata oluştu : a0x003");
                //General.SendBugReport(e.Result, "a0x003", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            if (pages.SelectedIndex == 2)
            {
                current_page = 1;
                ApplicationBar.IsVisible = true;
            }
            else
                ApplicationBar.IsVisible = false;

            GetProfile();
        }

        private void usertopicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserTopic drv = (UserTopic)usertopicList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=" + drv.utForumID + "&topic_id=" + drv.utID, UriKind.Relative));
            }

            usertopicList.SelectedIndex = -1;
        }

        private void participatedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserTopic drv = (UserTopic)participatedList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = 0;

                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=" + drv.utForumID + "&topic_id=" + drv.utID, UriKind.Relative));
            }

            participatedList.SelectedIndex = -1;
        }

        private void favori_Click(object sender, RoutedEventArgs e)
        {
            UserTopic drv = (sender as MenuItem).DataContext as UserTopic;

            if (drv != null)
            {
                classes.General.WriteFavori("fav_topic.txt", classes.General.ReadFavori("fav_topic.txt") + drv.utForumID + "|" + drv.utID + "|" + drv.utTitle + "|" + (u_name.Text + ", " + uType.Text) + "|" + (avatar.Source as BitmapImage).UriSource.OriginalString + "#");
                MessageBox.Show("Favorilere başarıyla eklendi.");
            }
        }

        private void prev_Click(object sender, EventArgs e)
        {
            current_page -= 1;
            GetProfile();
        }

        private void next_Click(object sender, EventArgs e)
        {
            current_page += 1;
            GetProfile();
        }

        private void first_Click(object sender, EventArgs e)
        {
            current_page = 1;
            GetProfile();
        }

        private void last_Click(object sender, EventArgs e)
        {
            current_page = 0;
            GetProfile();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            GetProfile();
            SetButton(1, false);
        }
    }
}
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
using Microsoft.Phone.Tasks;

namespace Creatalk.forum
{
    public class TopicPost
    {
        //public string pTitle { get; set; }
        public string pID { get; set; }
        public string pContent { get; set; }
        public string pAuthorName { get; set; }
        //public string pAuthorType { get; set; }
        //public string pIsOnline { get; set; }
        public string pAuthorAvatar { get; set; }
        public string pTime { get; set; }
        public string pCanLike { get; set; }
        //public string pIsLiked { get; set; }
        public string pLikeCount { get; set; }
        public string pAuthorInfo { get; set; }
        public string userstate { get; set; }
        public string quoteMessage { get; set; }
        public string quoteVisible { get; set; }
        public string editVisible { get; set; }
        public string likeVisible { get; set; }
        public string unlikeVisible { get; set; }
        public string fSize { get; set; }
        public string backGround { get; set; }
        public string quoteGround { get; set; }
        public string delVisible { get; set; }
        public string pCanDelete { get; set; }
        //public string reportVisible { get; set; }
        public string likesInfo { get; set; }
        public string readVisible { get; set; }
    }

    public partial class ForumPost : PhoneApplicationPage
    {
        ObservableCollection<TopicPost> ds = new ObservableCollection<TopicPost>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Cevaplar alınıyor..." };

        string sforum_id = "-1";
        string topic_id = "0";
        string like_postid = "";
        //string can_report = "0";
        string can_reply = "0";

        int total_post = 0;
        int total_page = 0;
        int current_page = 0;
        int last_unread = 0;

        public static short add_post = 0;

        short isLoaded = 0;
        short isLikeOrQuick = 0;
        short isMenuLoaded = 0;
        short isPostSended = 0;
        //short reLoad = 0;

        //Button sent_button;

        ApplicationBarIconButton m_button;

        public ForumPost()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            classes.Notification.GetNotifications(notification, ncount);
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            m_button = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

            if (sending.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                sending.Visibility = Visibility.Collapsed;
                m_button.Text = "hızlı yanıt";
                message.Text = "";
            }
            else
                e.Cancel = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*/if (NavigationService.Source.OriginalString.IndexOf("redirected") >= 0)
            {
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
            }/*/

            if (NavigationContext.QueryString.Count() < 2)
            {
                SetButton(0, false);
                SetButton(1, false);
                SetButton(2, false);
                SetButton(3, false);
                ApplicationBar.IsMenuEnabled = false;

                MessageBox.Show("Eksik parametre.");
                return;
            }

            topic_id = NavigationContext.QueryString["topic_id"];
            sforum_id = NavigationContext.QueryString["sforum_id"];
            //forum_id = NavigationContext.QueryString["forum_id"];

            if (isLoaded == 0)
            {
                isLoaded = 1;
                //GetPosts();
                GetUnread();
            }
        }

        void GetUnread()
        {
            SetButton(0, false);
            SetButton(1, false);
            SetButton(3, false);

            if (classes.General.CheckNetwork() == "Fail")
                return;

            classes.General.general_list.Clear();

            if (NavigationService.Source.OriginalString.IndexOf("gbp") >= 0)
                classes.General.general_list.Add(NavigationContext.QueryString["post_id"]);
            else
                classes.General.general_list.Add(topic_id);

            classes.General.general_list.Add(1);
            classes.General.general_list.Add(false);

            if (NavigationService.Source.OriginalString.IndexOf("gbp") >= 0)
                classes.Oauth.XmlRpcExecMethod("get_thread_by_post", kCompleted);
            else
                classes.Oauth.XmlRpcExecMethod("get_thread_by_unread", kCompleted);
        }

        public void kCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                last_unread = Convert.ToInt32(classes.Oauth.ParseXML("position", e.Result));
                GetPosts();
            }
            catch
            {
                last_unread = 0;
                GetPosts();
            }
        }

        void GetPosts()
        {
            SetButton(0, false);
            SetButton(1, false);
            SetButton(3, false);
            ApplicationBar.IsMenuEnabled = false;

            /*/if (NavigationContext.QueryString.Count() > 0)
            {
                
            }/*/

            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            //if (total_post == 0)
            //total_post = Convert.ToInt32(NavigationContext.QueryString["post_count"]); //+ 1;

            if (add_post == 1)
            {
                add_post = 0;
                total_post += 1;
                current_page = 0;
            }

            ds.Clear();

            classes.General.general_list.Clear();
            classes.General.general_list.Add(topic_id);

            if ((total_post % classes.General.sayfabasiPost) > 0)
                total_page = ((total_post - (total_post % classes.General.sayfabasiPost)) / classes.General.sayfabasiPost) + 1;
            else
                total_page = (total_post / classes.General.sayfabasiPost);

            if (last_unread <= 0)
            {
                if (current_page == 0)
                    current_page = total_page;
            }
            else
            {
                if (current_page == 0)
                {
                    if ((last_unread % classes.General.sayfabasiPost) > 0)
                        current_page = ((last_unread - (last_unread % classes.General.sayfabasiPost)) / classes.General.sayfabasiPost) + 1;
                    else
                        current_page = (last_unread / classes.General.sayfabasiPost);
                }

                if (last_unread > 0)
                    last_unread -= 1;
            }

            if (current_page == 1)
                classes.General.general_list.Add((current_page * classes.General.sayfabasiPost) - classes.General.sayfabasiPost);
            else
                classes.General.general_list.Add((current_page * classes.General.sayfabasiPost) - (classes.General.sayfabasiPost - 1));

            classes.General.general_list.Add(current_page * classes.General.sayfabasiPost);

            classes.Oauth.XmlRpcExecMethod("get_thread", uCompleted);
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                int set_this = 0;
                if (last_unread > 0)
                    if (current_page == 1)
                        set_this = last_unread - ((current_page - 1) * classes.General.sayfabasiPost);
                    else
                        set_this = last_unread - 1 - ((current_page - 1) * classes.General.sayfabasiPost);

                //MessageBox.Show("set_this: " + set_this.ToString() + "\nlast_unread: " + last_unread.ToString());

                /*if (last_unread > General.sayfabasiPost)
                    set_this = last_unread % General.sayfabasiPost;
                else
                    set_this = last_unread;*/

                last_unread = 0;

                ds.Clear();

                topicName.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("topic_title", e.Result));

                //toplam yanit sayisini aliyoruz
                total_post = Convert.ToInt32(classes.Oauth.ParseXML("total_post_num", e.Result)) - 1;

                if (total_post > 0)
                {
                    if ((total_post % classes.General.sayfabasiPost) > 0)
                        total_page = ((total_post - (total_post % classes.General.sayfabasiPost)) / classes.General.sayfabasiPost) + 1;
                    else
                        total_page = (total_post / classes.General.sayfabasiPost);
                }
                else
                    total_page = 1;

                //vtotal.Text = total_post.ToString() + " yanıt";
                vpage.Text = "Sayfa: " + current_page.ToString() + " / " + total_page.ToString();

                //can_report = Oauth.ParseXML("can_report", e.Result);
                can_reply = classes.Oauth.ParseXML("can_reply", e.Result);

                //eger suanki sayfa numarasi toplam sayfa sayisina esit ise sonraki sayfa butonunu kilitle
                SetButton(0, (current_page < 2) ? false : true);

                if (App.Session_LoadSession("xf_session").Replace(" ", "") != "" && classes.General.username.Replace(" ", "") != "")
                    SetButton(1, true);

                //eger suanki sayfa numarasi 2 den kucuk ise onceki sayfa butonunu kilitle
                SetButton(3, (current_page >= total_page) ? false : true);

                //if (App.Session_LoadSession("xf_session").Replace(" ", "") != "" && General.username.Replace(" ", "") != "")
                //ApplicationBar.IsMenuEnabled = true;

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", avatar_url = "", temp = "", quote = "", c_message = "", richtext = "", likes = "";

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

                    c_message = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_content"));
                    c_message = c_message.Replace("[url", " [url");
                    c_message = c_message.Replace("[URL", " [url");
                    c_message = c_message.Replace("[/URL]", "[/url]");
                    c_message = c_message.Replace("[/url]", "[/url] ");

                    c_message = c_message.Replace("[img]", " [img]");
                    c_message = c_message.Replace("[IMG]", " [img]");
                    c_message = c_message.Replace("[/IMG]", "[/img]");
                    c_message = c_message.Replace("[/img]", "[/img] ");

                    c_message = c_message.Replace("[QUOTE", "[quote");
                    c_message = c_message.Replace("[/QUOTE]", "[/quote]");
                    //c_message = c_message.Replace("\n\nCreatalk Windows Phone uygulamasıyla gönderildi.", "");

                    /*/if (c_message.IndexOf("[quote uid") >= 0 && c_message.IndexOf("[/quote]", c_message.IndexOf("[quote uid")) >= 0)
                    {
                        string quote_temp = c_message.Substring(c_message.IndexOf("[quote"), c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1 - c_message.IndexOf("[quote"));
                        quote = c_message.Substring(c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1, c_message.IndexOf("[/quote]") - (c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1));
                        c_message = c_message.Replace(quote_temp + quote + "[/quote]", "");
                        quote = quote_temp.Substring(quote_temp.IndexOf("name=") + 6, quote_temp.IndexOf("post=") - 8 - quote_temp.IndexOf("name=")) + " demiş ki:" + Environment.NewLine + quote;
                        
                        //if (c_message.Length > 0)
                            //c_message = c_message.Remove(0, 1);
                    }
                    else if (c_message.IndexOf("[quote]") >= 0 && c_message.IndexOf("[/quote]", c_message.IndexOf("[quote]")) >= 0)
                    {
                        quote = c_message.Substring(c_message.IndexOf("[quote]") + 7, c_message.IndexOf("[/quote]") - (c_message.IndexOf("[quote]") + 7));
                        c_message = c_message.Replace("[quote]" + quote + "[/quote]", "");
                    }/*/

                    while (c_message.IndexOf("[quote") >= 0)
                    {
                        if (c_message.IndexOf("[quote uid") >= 0 && c_message.IndexOf("[/quote]", c_message.IndexOf("[quote uid")) >= 0)
                        {
                            string quote_temp = c_message.Substring(c_message.IndexOf("[quote"), c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1 - c_message.IndexOf("[quote"));
                            //richtext = c_message.Substring(c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1, c_message.IndexOf("[/quote]") - (c_message.IndexOf("]", c_message.IndexOf("[quote") + 6) + 1));
                            richtext = c_message.Substring(c_message.IndexOf(quote_temp) + quote_temp.Length, c_message.IndexOf("[/quote]", c_message.IndexOf(quote_temp)) - (c_message.IndexOf(quote_temp) + quote_temp.Length));
                            c_message = c_message.Replace(quote_temp + richtext + "[/quote]", "");
                            quote += quote_temp.Substring(quote_temp.IndexOf("name=") + 6, quote_temp.IndexOf("post=") - 8 - quote_temp.IndexOf("name=")) + " demiş ki:" + Environment.NewLine + richtext + "\n\n------------------------------\n\n";

                            //if (c_message.Length > 0)
                            //c_message = c_message.Remove(0, 1);
                        }
                        else if (c_message.IndexOf("[quote]") >= 0 && c_message.IndexOf("[/quote]", c_message.IndexOf("[quote]")) >= 0)
                        {
                            if (c_message.IndexOf("[quote]") < 0 || c_message.IndexOf("[/quote]", c_message.IndexOf("[quote]")) < 0)
                                continue;

                            richtext = c_message.Substring(c_message.IndexOf("[quote]") + 7, c_message.IndexOf("[/quote]") - (c_message.IndexOf("[quote]") + 7));
                            quote += richtext + "\n\n------------------------------\n\n";
                            c_message = c_message.Replace("[quote]" + richtext + "[/quote]", "");
                        }
                    }

                    if (quote.Length > 34)
                        quote = quote.Remove(quote.Length - 34, 34);

                    //if (quote.Length > 2)
                    //quote = quote.Remove(quote.Length - 2, 2);

                    /*/if (c_message.IndexOf("[/img]") >= 0 || c_message.IndexOf("[/url]") >= 0)
                        c_message += " http://www.buhatayiboylefixliyorum.com/fixed.asp";

                    if (quote.IndexOf("[/img]") >= 0 || quote.IndexOf("[/url]") >= 0)
                        quote += " http://www.buhatayiboylefixliyorum.com/fixed.asp";/*/

                    if (App.Session_LoadSession("aImage") == "1")
                    {
                        c_message = classes.Smiley.GetSmiley(c_message);
                        quote = classes.Smiley.GetSmiley(quote);
                    }

                    if ((c_message.IndexOf("[url") < 0 || c_message.IndexOf("[img") < 0) && c_message.IndexOf("http://") >= 0)
                        c_message += " http://www.buhatayiboylefixliyorum.com/fixed.asp";
                    else if (c_message.IndexOf("[/img]") >= 0 || c_message.IndexOf("[/url]") >= 0)
                        c_message += " http://www.buhatayiboylefixliyorum.com/fixed.asp";

                    if ((quote.IndexOf("[url") < 0 || quote.IndexOf("[img") < 0) && quote.IndexOf("http://") >= 0)
                        quote += " http://www.buhatayiboylefixliyorum.com/fixed.asp";
                    else if (quote.IndexOf("[/img]") >= 0 || quote.IndexOf("[/url]") >= 0)
                        quote += " http://www.buhatayiboylefixliyorum.com/fixed.asp";

                    if (c_message.Length > 2)
                        if (c_message.Substring(0, 1) == "\n")
                            c_message = c_message.Remove(0, 1);

                    c_message = classes.General.ReturnCleanHTML(c_message);
                    quote = classes.General.ReturnCleanHTML(quote);

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "is_online") == "1" ? "/images/online.png" : "/images/offline.png");

                    //begeniler icin burayi bir gozden gecir
                    /*if (Oauth.ParseXML2(round.Elements("member"), "like_count") != "0")
                {
                    IEnumerable<XElement> enumerable = xElement.Elements("member");
                    if (ForumPost.CS$<>9__CachedAnonymousMethodDelegate7 == null)
                    {
                        ForumPost.CS$<>9__CachedAnonymousMethodDelegate7 = new Func<XElement, bool>(null, (XElement w) => w.Element("name").ToString().Contains("likes_info"));
                    }
                    foreach (XElement xElement1 in Extensions.Elements<XElement>(Extensions.Elements<XElement>(XDocument.Parse(Enumerable.First<XElement>(Enumerable.Where<XElement>(enumerable, ForumPost.CS$<>9__CachedAnonymousMethodDelegate7)).ToString()).Descendants("data"), "value"), "struct"))
                    {
                        likes += "@" + General.DecodeBase64(Oauth.ParseXML2(round.Elements("member"), "username")) + "#";
                    }
                }*/

                    ds.Add(new TopicPost
                    {
                        pID = classes.Oauth.ParseXML2(round.Elements("member"), "post_id"),
                        pContent = (c_message.Length > 2250 ? string.Concat(c_message.Substring(0, 2250), "...") : c_message),
                        pAuthorName = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")),
                        pAuthorAvatar = avatar_url,
                        pTime = sentDate,
                        pCanLike = classes.Oauth.ParseXML2(round.Elements("member"), "can_like"),
                        pLikeCount = classes.Oauth.ParseXML2(round.Elements("member"), "like_count") + " beğeni",
                        quoteMessage = quote,
                        quoteVisible = (quote != "" ? "Visible" : "Collapsed"),
                        fSize = classes.General.fontSize.ToString(),
                        editVisible = (classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) == classes.General.username || classes.Oauth.ParseXML2(round.Elements("member"), "can_edit") == "1") ? "Visible" : "Collapsed",
                        likeVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_like") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_liked") == "0") && (classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) != classes.General.username) ? "Visible" : "Collapsed",
                        unlikeVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "is_liked") == "1" && classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) != classes.General.username) ? "Visible" : "Collapsed",
                        backGround = (classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) == classes.General.username) ? App.Session_LoadSession("postC1") : App.Session_LoadSession("postOC1"),
                        quoteGround = (classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) == classes.General.username) ? App.Session_LoadSession("postC2") : App.Session_LoadSession("postOC2"),
                        delVisible = (classes.Oauth.ParseXML2(round.Elements("member"), "can_delete") == "1" && classes.Oauth.ParseXML2(round.Elements("member"), "is_deleted") == "0") ? "Visible" : "Collapsed",
                        pAuthorInfo = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) + ", " + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "user_type")) + " üye",
                        pCanDelete = classes.Oauth.ParseXML2(round.Elements("member"), "can_delete"),
                        readVisible = (c_message.Length > 2250) ? "Visible" : "Collapsed",
                        likesInfo = likes,
                        userstate = temp
                    });
                    postList.ItemsSource = ds;

                    if (ds.Count() > 0)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            try
                            {
                                if (isPostSended != 0)
                                    classes.General.UpdateLayout(postList, false);
                                else
                                    postList.ScrollIntoView(postList.Items[set_this]);
                            }
                            catch
                            {
                                if (isPostSended != 0)
                                    classes.General.UpdateLayout(postList, false);
                                else
                                    classes.General.UpdateLayout(postList, true);
                            }

                            isPostSended = 0;
                            postList.UpdateLayout();
                        });
                    }
                }


                if (isMenuLoaded == 0)
                {
                    isMenuLoaded = 1;
                    if (classes.Oauth.ParseXML("can_close", e.Result) == "1" && classes.Oauth.ParseXML("is_closed", e.Result) == "0")
                    {
                        ApplicationBarMenuItem abmb = new ApplicationBarMenuItem();
                        abmb.Text = "konuyu kilitle";
                        abmb.Click += (s, c) =>
                        {
                            if (classes.General.CheckNetwork() == "Fail")
                            {
                                return;
                            }

                            classes.General.general_list.Clear();
                            classes.General.general_list.Add(topic_id);
                            classes.General.general_list.Add(2);
                            classes.Oauth.XmlRpcExecMethod("m_close_topic", mCompleted);
                        };
                        ApplicationBar.MenuItems.Add(abmb);
                    }
                    else if (classes.Oauth.ParseXML("can_close", e.Result) == "1" && classes.Oauth.ParseXML("is_closed", e.Result) == "1")
                    {
                        ApplicationBarMenuItem abmb2 = new ApplicationBarMenuItem();
                        abmb2.Text = "konu kilidini kaldır";
                        abmb2.Click += (s, c) =>
                        {
                            if (classes.General.CheckNetwork() == "Fail")
                            {
                                return;
                            }

                            classes.General.general_list.Clear();
                            classes.General.general_list.Add(topic_id);
                            classes.General.general_list.Add(1);
                            classes.Oauth.XmlRpcExecMethod("m_close_topic", mCompleted);
                        };
                        ApplicationBar.MenuItems.Add(abmb2);
                    }
                    if (classes.Oauth.ParseXML("can_delete", e.Result) == "1" && classes.Oauth.ParseXML("is_deleted", e.Result) == "0")
                    {
                        ApplicationBarMenuItem abmb3 = new ApplicationBarMenuItem();
                        abmb3.Text = "konuyu sil";
                        abmb3.Click += (s, c) =>
                        {
                            if (classes.General.CheckNetwork() == "Fail")
                            {
                                return;
                            }

                            classes.General.general_list.Clear();
                            classes.General.general_list.Add(topic_id);
                            classes.General.general_list.Add(1);
                            classes.Oauth.XmlRpcExecMethod("m_delete_topic", mCompleted);
                        };
                        ApplicationBar.MenuItems.Add(abmb3);
                    }
                }
                SetButton(2, true);
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

                //General.SendBugReport(e.Result, "f0x006", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        public void mCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1")
                {
                    MessageBox.Show("İşlem başarılı.");
                    NavigationService.GoBack();
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

        private void like_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as Button).DataContext as TopicPost;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                Button sent_button = sender as Button;
                sent_button.Visibility = Visibility.Collapsed;

                isLikeOrQuick = 1;
                like_postid = drv.pID;

                classes.General.general_list.Clear();

                classes.General.general_list.Add(drv.pID);

                classes.Oauth.XmlRpcExecMethod("like_post", hCompleted);
            }
        }

        public void hCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            SetButton(1, true);
            message.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    if (isLikeOrQuick == 0)
                    {
                        sending.Visibility = Visibility.Collapsed;
                        m_button.Text = "hızlı yanıt";
                        message.Text = "";

                        total_post += 1;
                        GetPosts();
                    }
                    else
                    {
                        //sent_button.Content = (sent_button.Content.ToString() == "beğen") ? "beğ. vazgeç" : "beğen";

                        if (isLikeOrQuick == 1)
                        {
                            ds.Where(w => w.pID.Equals(like_postid)).First().pLikeCount = (Convert.ToInt32(ds.Where(w => w.pID.Equals(like_postid)).First().pLikeCount.Replace(" beğeni", "")) + 1).ToString() + " beğeni";
                            ds.Where(w => w.pID.Equals(like_postid)).First().likeVisible = "Collapsed";
                            ds.Where(w => w.pID.Equals(like_postid)).First().unlikeVisible = "Visible";
                        }
                        else
                        {
                            ds.Where(w => w.pID.Equals(like_postid)).First().pLikeCount = (Convert.ToInt32(ds.Where(w => w.pID.Equals(like_postid)).First().pLikeCount.Replace(" beğeni", "")) - 1).ToString() + " beğeni";
                            ds.Where(w => w.pID.Equals(like_postid)).First().likeVisible = "Visible";
                            ds.Where(w => w.pID.Equals(like_postid)).First().unlikeVisible = "Collapsed";
                        }

                        postList.ItemsSource = null;
                        postList.ItemsSource = ds;
                    }
                }
                else if (classes.Oauth.ParseXML("result", e.Result) == "0" && classes.Oauth.ParseXML("result_text", e.Result) != "")
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : f0x008");
                //General.SendBugReport(e.Result, "f0x008", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void unlike_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as Button).DataContext as TopicPost;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                Button sent_button = sender as Button;
                sent_button.Visibility = Visibility.Collapsed;

                isLikeOrQuick = 2;
                like_postid = drv.pID;

                classes.General.general_list.Clear();

                classes.General.general_list.Add(drv.pID);

                classes.Oauth.XmlRpcExecMethod("unlike_post", hCompleted);
            }
        }

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void quick_Click(object sender, EventArgs e)
        {
            m_button = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

            if (sending.Visibility == Visibility.Collapsed)
            {
                m_button.Text = "cevapla";
                sending.Visibility = Visibility.Visible;
            }
            else
            {
                if (message.Text.Replace(" ", "") != "")
                {
                    if (can_reply == "1")
                    {
                        //interneti kontrol ettiriyoruz
                        if (classes.General.CheckNetwork() == "Fail")
                            return;

                        //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                        m_button.IsEnabled = false;
                        message.IsEnabled = false;

                        current_page = 0;
                        isLikeOrQuick = 0;
                        isPostSended = 1;

                        message.Text = message.Text.Replace("\r", "\n");

                        //parametre listesini temizliyoruz
                        classes.General.general_list.Clear();

                        classes.General.general_list.Add(sforum_id);
                        classes.General.general_list.Add(topic_id);
                        classes.General.general_list.Add("base64|");
                        classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));
                        //General.general_list.Add("base64|" + General.EncodeBase64(message.Text + Environment.NewLine + Environment.NewLine + "[SIZE=2]" + App.Session_LoadSession("OwnSign") + "[/SIZE]"));

                        //apiyi cagirma komutunu kullaniyoruz
                        classes.Oauth.XmlRpcExecMethod("reply_post", hCompleted);
                    }
                    else
                        MessageBox.Show("Bu konuya yanıt yazma yetkiniz yok veya konu kilitli.");
                }
            }
        }

        private void first_Click(object sender, EventArgs e)
        {
            current_page = 1;
            GetPosts();
        }

        private void last_Click(object sender, EventArgs e)
        {
            current_page = 0;
            GetPosts();
        }

        private void message_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (message.SelectionStart == message.Text.Length)
            {
                sending.ScrollToVerticalOffset(message.ActualHeight);
                sending.UpdateLayout();
            }
        }

        private void notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/forum/Forum.xaml?alerts", UriKind.Relative));
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as MenuItem).DataContext as TopicPost;

            if (drv != null)
            {
                if (drv.pCanDelete == "1")
                {
                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    classes.General.general_list.Clear();
                    classes.General.general_list.Add(drv.pID);
                    classes.General.general_list.Add(1);
                    classes.General.general_list.Add("base64|");

                    classes.Oauth.XmlRpcExecMethod("m_delete_post", mCompleted);
                }
            }
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TopicPost drv = (sender as TextBlock).DataContext as TopicPost;

            if (drv != null)
            {
                if (drv.likesInfo != "")
                {
                    classes.General.post_likes = drv.likesInfo;
                    NavigationService.Navigate(new Uri("/post/PostLikes.xaml", UriKind.Relative));
                }
            }
        }

        private void read_Click(object sender, RoutedEventArgs e)
        {
            TopicPost drv = (sender as HyperlinkButton).DataContext as TopicPost;

            if (drv != null && topic_id != "0")
            {
                WebBrowserTask wbt = new WebBrowserTask();
                wbt.URL = classes.General.forum_domain + "konu/" + topic_id + "/#post-" + drv.pID; //hdp.
                wbt.Show();
            }
        }

        /*private void report_Click(object sender, RoutedEventArgs e)
        {
            if (General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as MenuItem).DataContext as TopicPost;

            if (drv != null)
            {
                if (can_report == "1")
                {
                    if (General.CheckNetwork() == "Fail")
                        return;

                    General.general_list.Clear();
                    General.general_list.Add(drv.pID);
                    General.general_list.Add("base64|" + General.EncodeBase64("Windows Phone uygulaması yanıt şikayet sistemi."));

                    Oauth.XmlRpcExecMethod("report_post", mCompleted);
                }
            }
        }*/

        private void mnotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/conv/Conversation.xaml", UriKind.Relative));
        }

        private void quote_Click(object sender, RoutedEventArgs e)
        {
            TopicPost drv = (sender as MenuItem).DataContext as TopicPost;

            if (drv != null)
            {
                isLoaded = 0;

                NavigationService.Navigate(new Uri("/post/PostCreate.xaml?sforum_id=" + sforum_id + "&topic_id=" + topic_id + "&post_id=" + drv.pID, UriKind.Relative));
            }
        }

        private void edit2_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as MenuItem).DataContext as TopicPost;

            if (drv != null)
            {
                isLoaded = 0;

                NavigationService.Navigate(new Uri("/post/PostEdit.xaml?post_id=" + drv.pID, UriKind.Relative));
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as Button).DataContext as TopicPost;

            if (drv != null)
            {
                isLoaded = 0;

                NavigationService.Navigate(new Uri("/post/PostEdit.xaml?post_id=" + drv.pID, UriKind.Relative));
            }
        }

        private void prev_Click(object sender, EventArgs e)
        {
            current_page -= 1;
            GetPosts();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            GetPosts();
            SetButton(2, false);
        }

        private void next_Click(object sender, EventArgs e)
        {
            current_page += 1;
            GetPosts();
        }

        private void reply_Click(object sender, EventArgs e)
        {

            if (this.can_reply == "1")
            {
                if (classes.General.CheckLoggedIn() == "Fail")
                    return;

                if (App.Session_LoadSession("aRefresh") == "1")
                    this.isLoaded = 0;

                isPostSended = 1;
                NavigationService.Navigate(new Uri("/post/PostCreate.xaml?sforum_id=" + sforum_id + "&topic_id=" + topic_id, UriKind.Relative));
            }
            else
                MessageBox.Show("Bu konuya yanıt yazma yetkiniz yok veya konu kilitli.");
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            TopicPost drv = (sender as Grid).DataContext as TopicPost;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    this.isLoaded = 0;

                NavigationService.Navigate(new Uri("/Profile.xaml?user_name=" + drv.pAuthorName, UriKind.Relative));
            }
        }
    }
}
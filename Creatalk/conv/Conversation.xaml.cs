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
using System.Text;
using Microsoft.Phone.Shell;
using System.Reflection;
using Telerik.Windows.Controls;

namespace Creatalk.conv
{
    public class Chats
    {
        public string chatID { get; set; }
        public string replyCount { get; set; }
        public string participants { get; set; }
        public string chatSubject { get; set; }
        //public string lastUser { get; set; }
        public string chatDate { get; set; }
        //public string startUSer { get; set; }
        public string userAvatar { get; set; }
        public string notification { get; set; }
        //public string prpcnwIDs { get; set; }
        public string usersInfo { get; set; }
        public string notifvisible { get; set; }
    }

    public partial class Conversation : PhoneApplicationPage
    {
        //olusturulan mesajlari kaydedecegimiz listemizi sinifimiza baglayarak olusturuyoruz
        ObservableCollection<Chats> ds = new ObservableCollection<Chats>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Konuşmalar alınıyor..." };

        int total_conversation = 0;
        int total_page = 0;
        int current_page = 1;

        bool isLoaded = false;
        bool reLoad = true;

        ApplicationBarIconButton m_button;

        public Conversation()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);

            chatList.DataVirtualizationMode = DataVirtualizationMode.OnDemandAutomatic;
            chatList.DataRequested += OnDataRequested;
            chatList.RefreshRequested += OnDataRequested;

            chatList.SetValue(InteractionEffectManager.IsInteractionEnabledProperty, true);
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
        }

        private void OnRefreshRequested(object sender, EventArgs args)
        {
            // Make async data request
            //current_page = 1;
            reLoad = false;
            ds.Clear();
            //GetConversations();
            //SetButton(1, false);
        }

        private void OnDataRequested(object sender, EventArgs args)
        {
            //TODO: download data items
            if (reLoad)
            {
                current_page += 1;
            }
            else
            {
                //chatList.ItemsSource = null;
                current_page = 1;
            }

            GetConversations();
            reLoad = true;
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

            classes.Notification.GetNotifications(notification, ncount);
            classes.Notification.GetConversationNotification(mnotification, mcount, null);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                GetConversations();
            }
        }

        void GetConversations()
        {
            //SetButton(0, false);
            //SetButton(2, false);
            //SetButton(3, false);
            ApplicationBar.IsMenuEnabled = false;

            //interneti kontrol ettiriyoruz
            if (classes.General.CheckNetwork() == "Fail")
                return;

            progress.IsVisible = true;

            //mesaj listesini temizliyoruz
            //ds.Clear();

            if (current_page == 0)
                current_page = total_page;

            //parametre listesini temizliyoruz
            classes.General.general_list.Clear();

            classes.General.general_list.Add((current_page * classes.General.sayfabasiChat) - (classes.General.sayfabasiChat - 1));
            classes.General.general_list.Add(current_page * classes.General.sayfabasiChat);

            //ilk parametre olarak sayfa numarasini ayarliyoruz
            //General.general_list.Add(current_page);
            //ikinci parametre olarak her bir sayfada olacak chat sayisini sayfa sayisiyla carpiyoruz ki o sayfaya ait kayitlari alabilelim
            //General.general_list.Add(General.sayfabasiChat * current_page);

            //apiyi cagirma komutunu kullaniyoruz
            classes.Oauth.XmlRpcExecMethod("get_conversations", uCompleted);
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                //mesaj listesini temizliyoruz
                //ds.Clear();

                //toplam konusma sayisini aliyoruz
                total_conversation = Convert.ToInt32(classes.Oauth.ParseXML("conversation_count", e.Result));

                //vtotal.Text = total_conversation.ToString() + " konuşma";

                if ((total_conversation % classes.General.sayfabasiChat) > 0)
                    total_page = ((total_conversation - (total_conversation % classes.General.sayfabasiChat)) / classes.General.sayfabasiChat) + 1;
                else
                    total_page = (total_conversation / classes.General.sayfabasiChat);

                vpage.Text = "Sayfa: " + current_page.ToString() + " / " + total_page.ToString();

                //eger suanki sayfa numarasi toplam sayfa sayisina esit ise sonraki sayfa butonunu kilitle
                //SetButton(2, (current_page >= total_page) ? false : true);
                //SetButton(1, true);
                //eger suanki sayfa numarasi 2 den kucuk ise onceki sayfa butonunu kilitle
                //SetButton(0, (current_page < 2) ? false : true);

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    //degiskenlerimizi tanimliyoruz
                    string add_prtcpnts = "", notification = "", user_avatar = "", sentDate = "", allInfo = "";

                    if (round.Elements("member").ElementAt(5).Element("value").Value.Replace(" ", "") != "")
                    {
                        string[] temp1 = round.Elements("member").ElementAt(5).Element("value").Value.Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    if (round.Elements("member").ElementAt(10).Element("value").Value != "0")
                        notification = "/images/notification.png";

                    foreach (XElement pavatar in round.Elements("member").Elements("value").Elements("struct").Elements("member"))
                    {
                        string temp = pavatar.Elements("value").Elements("struct").Elements("member").Last().Element("value").Value;

                        allInfo += pavatar.Element("name").Value + "|" + classes.General.DecodeBase64(pavatar.Elements("value").Elements("struct").Elements("member").First().Element("value").Value) + "|";

                        if (temp != "")
                            allInfo += temp + "#";
                        else
                            allInfo += classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png#"; //hdp.

                        //konusmadaki tum katilimcilari add_prtcpnts degiskenine ekliyoruz (append)
                        add_prtcpnts += "@" + classes.General.DecodeBase64(pavatar.Elements("value").Elements("struct").Elements("member").First().Element("value").Value) + ", ";

                        if (pavatar.Element("name").Value == round.Elements("member").ElementAt(3).Element("value").Value)
                        {
                            //uyenin resmi (avatar) var mı diye kontrol ediyoruz. eger yoksa sabit bir forum resmi veriyoruz
                            if (temp != "")
                                user_avatar = temp;
                            else
                                user_avatar = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png"; //hdp.
                        }
                    }

                    /*/foreach (XElement ppants in round.Elements("member").Elements("value").Elements("struct").Elements("member").Elements("value").Elements("struct"))
                    {
                        partipicants += round.Elements("member").Elements("value").Elements("struct").Elements("member").Elements("name").First().Value + "." + General.DecodeBase64(ppants.Elements("member").First().Element("value").Value) + "|";
                        add_prtcpnts += "@" + General.DecodeBase64(ppants.Elements("member").First().Element("value").Value) + ", ";
                    }/*/

                    ds.Add(new Chats()
                    {
                        chatID = round.Elements("member").ElementAt(0).Element("value").Value,
                        replyCount = "toplam " + (Convert.ToInt32(round.Elements("member").ElementAt(1).Element("value").Value) + 1).ToString() + " yanıt",
                        //startUSer = round.Elements("member").ElementAt(3).Element("value").Value,
                        //lastUser = round.Elements("member").ElementAt(4).Element("value").Value,
                        chatDate = sentDate,
                        chatSubject = classes.General.DecodeBase64(round.Elements("member").ElementAt(8).Element("value").Value),
                        participants = add_prtcpnts,
                        userAvatar = user_avatar,
                        notification = notification,
                        //prpcnwIDs = partipicants,
                        usersInfo = allInfo,
                        notifvisible = (notification != "") ? "Visible" : "Collapsed"
                    });
                }

                if (chatList.ItemsSource == null)
                    chatList.ItemsSource = ds;

                //if (ds.Count() > 0)
                //{
                ////Dispatcher.BeginInvoke(() =>
                ////{
                /*/do
                {
                    if (chatList.Items.Count() == ds.Count())
                    {
                        General.UpdateLayout(this.chatList, true);
                        return;
                    }
                } while (chatList.Items.Count() != ds.Count());/*/
                ////General.UpdateLayout(this.chatList, true);
                ////});
                //}

                chatList.StopPullToRefreshLoading(true);
                SetButton(1, true);
                ApplicationBar.IsMenuEnabled = true;
                progress.IsVisible = false;

                if (ds.Count() == total_conversation)
                    classes.General.DisableVirtualization(chatList);
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

                MessageBox.Show("Bir hata oluştu : c0x001");
                //General.SendBugReport(e.Result, "c0x001", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            Chats drv = (sender as MenuItem).DataContext as Chats;

            if (drv != null)
            {
                if (MessageBox.Show("Bu konuşmayı silmek istiyor musunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    classes.General.general_list.Clear();
                    classes.General.general_list.Add(drv.chatID);
                    classes.General.general_list.Add(classes.General.sohbetsilMode);

                    classes.Oauth.XmlRpcExecMethod("delete_conversation", zCompleted);
                }
            }
        }

        private void unread_Click(object sender, EventArgs e)
        {
            Chats drv = (sender as MenuItem).DataContext as Chats;

            if (drv != null)
            {
                if (MessageBox.Show("Bu konuşmayı okunmadı olarak işaretlemek istiyor musunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    classes.General.general_list.Clear();
                    classes.General.general_list.Add(drv.chatID);

                    classes.Oauth.XmlRpcExecMethod("mark_conversation_unread", zCompleted);
                }
            }
        }

        public void zCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    GetConversations();
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

                MessageBox.Show("Bir hata oluştu : c0x009");
                //General.SendBugReport(e.Result, "c0x009", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        /*/public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (Oauth.ParseXML("result", e.Result) == "1" && Oauth.ParseXML("result_text", e.Result) == "")
                {
                    MessageBox.Show("Konuşma başarıyla silindi.");
                    GetConversations();
                }
                else if (Oauth.ParseXML("result", e.Result) == "0" && Oauth.ParseXML("result_text", e.Result) != "")
                    MessageBox.Show(General.DecodeBase64(Oauth.ParseXML("result_text", e.Result)));
            }
            catch (TargetInvocationException ex)
            {
                BugSenseHandler.Instance.SendExceptionAsync(ex);
       
                MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                BugSenseHandler.Instance.SendExceptionAsync(ex);
                MessageBox.Show("Bir hata oluştu : c0x003");
                //General.SendBugReport(e.Result, "c0x003", ex.Message, ex.Source, ex.HelpLink);
            }
        }/*/

        private void chatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chats drv = (Chats)chatList.SelectedItem;

            if (drv != null)
            {
                if (App.Session_LoadSession("aRefresh") == "1")
                    isLoaded = false;

                classes.General.conv_participants = drv.usersInfo;
                NavigationService.Navigate(new Uri("/conv/ConvSend.xaml?box_id=" + drv.chatID + "&mcount=" + drv.replyCount.Replace("toplam ", "").Replace(" yanıt", ""), UriKind.Relative));
            }

            chatList.SelectedItem = null;
            ////chatList.SelectedIndex = -1;
        }

        /*private void prev_Click(object sender, EventArgs e)
        {
            current_page -= 1;
            GetConversations();
        }*/

        private void new_Click(object sender, EventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            if (App.Session_LoadSession("aRefresh") == "1")
                isLoaded = false;

            NavigationService.Navigate(new Uri("/conv/ConvCreate.xaml", UriKind.Relative));
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            //current_page = 1;
            reLoad = false;
            ds.Clear();
            //GetConversations();
            SetButton(1, false);
        }

        /*private void next_Click(object sender, EventArgs e)
        {
            current_page += 1;
            GetConversations();
        }*/

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        /*private void first_Click(object sender, EventArgs e)
        {
            current_page = 0;
            GetConversations();
        }

        private void last_Click(object sender, EventArgs e)
        {
            current_page = 1;
            GetConversations();
        }*/

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
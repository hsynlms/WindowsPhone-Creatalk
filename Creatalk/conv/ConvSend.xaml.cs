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
using System.Threading.Tasks;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Media;

namespace Creatalk.conv
{
    public class ConvItems
    {
        public string msgID { get; set; }
        public string Message { get; set; }
        //public string SenderID { get; set; }
        public string senderName { get; set; }
        //public string isUnread { get; set; }
        //public string isOnline { get; set; }
        public string sendDate { get; set; }
        //public string areYou { get; set; }
        //public string isHe { get; set; }
        public string backGround { get; set; }
        public string alignItem { get; set; }
        public string bubbleAlign { get; set; }
        public string quoteVisible { get; set; }
        public string quoteMessage { get; set; }
        public string quoteGround { get; set; }
        public string fSize { get; set; }
    }

    public class ConvUsers
    {
        public string cuAvatar { get; set; }
        public string cuName { get; set; }
        public string cuType { get; set; }
        //public string cuID { get; set; }

    }
    public partial class ConvSend : PhoneApplicationPage
    {
        ObservableCollection<ConvItems> ds = new ObservableCollection<ConvItems>();
        ObservableCollection<ConvUsers> cs = new ObservableCollection<ConvUsers>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Mesajlar alınıyor..." };

        string convID = "";

        int total_message = 0;
        int total_page = 1;
        int current_page = 0;

        public static short add_message = 0;

        //short reLoad = 0;
        short isLoaded = 0;

        ApplicationBarIconButton m_button;

        public ConvSend()
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
            if (NavigationContext.QueryString.Count() < 2)
            {
                message.IsEnabled = false;
                SetButton(0, false);
                SetButton(1, false);
                SetButton(2, false);
                SetButton(3, false);
                ApplicationBar.IsMenuEnabled = false;

                MessageBox.Show("Eksik parametre.");
                return;
            }

            //if (pages.SelectedIndex == 0)
            if (isLoaded == 0)
            {
                isLoaded = 1;
                GetConversation();
            }
        }

        void GetConversation()
        {
            SetButton(0, false);
            SetButton(1, false);
            SetButton(3, false);
            ApplicationBar.IsMenuEnabled = false;

            try
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                progress.IsVisible = true;

                cs.Clear();

                for (int i = 0; i < classes.General.conv_participants.Split('#').Count() - 1; i++)
                {
                    cs.Add(new ConvUsers()
                    {
                        cuAvatar = classes.General.conv_participants.Split('#')[i].Split('|')[2],
                        //cuID = General.conv_participants.Split('#')[i].Split('|')[0],
                        cuName = "@" + classes.General.conv_participants.Split('#')[i].Split('|')[1]
                    });
                }

                participants.ItemsSource = cs;

                //mesaj listesini temizliyoruz
                ds.Clear();

                classes.General.general_list.Clear();
                classes.General.general_list.Add(NavigationContext.QueryString["box_id"]);

                if (total_message == 0)
                    total_message = Convert.ToInt32(NavigationContext.QueryString["mcount"]);

                if (add_message == 1)
                {
                    add_message = 0;
                    total_message += 1;
                    current_page = 0;
                }

                if ((total_message % classes.General.sayfabasiMesaj) > 0)
                    total_page = ((total_message - (total_message % classes.General.sayfabasiMesaj)) / classes.General.sayfabasiMesaj) + 1;
                else
                    total_page = (total_message / classes.General.sayfabasiMesaj);

                if (current_page == 0)
                    current_page = total_page;

                classes.General.general_list.Add((current_page * classes.General.sayfabasiMesaj) - (classes.General.sayfabasiMesaj - 1));
                classes.General.general_list.Add(current_page * classes.General.sayfabasiMesaj);

                /*/if ((total_message % General.sayfabasiMesaj) > 0)
                {
                    //ilk parametre olarak toplam mesaja 1 ekliyoruz ve toplam mesaj sayisinin General.sayfabasiMesaj ile bolumunden kalani cikartiyoruz
                    General.general_list.Add(total_message + 1 - ((total_message % General.sayfabasiMesaj)));
                    //ikinci parametre olarak toplam mesaj sayisindan toplam mesaj sayisinin General.sayfabasiMesaj ile bolumunden kalani cikartiyor ve General.sayfabasiMesaj ekliyoruz o sayfaya kadar toplamda kac kayit olabilecegini hesapliyoruz
                    General.general_list.Add((total_message - ((total_message % General.sayfabasiMesaj))) + General.sayfabasiMesaj);
                }
                else
                {
                    //ilk parametre olarak toplam mesaj sayisi zaten General.sayfabasiMesaj in bir kati oldugu icin bu sayiya 1 ekliyoruz
                    General.general_list.Add(total_message + 1);
                    //ikinci parametre olarak toplam mesaj sayisi zaten General.sayfabasiMesaj in bir kati oldugu icin bu sayiya General.sayfabasiMesaj ekliyoruz (diger sayfaya kadar olan toplam mesaj sayisi)
                    General.general_list.Add((total_message + General.sayfabasiMesaj));
                }/*/

                classes.Oauth.XmlRpcExecMethod("get_conversation", uCompleted);
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : c0x004");
                //General.SendBugReport("Veri yok!", "c0x004", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                //mesaj listesini temizliyoruz
                ds.Clear();

                subText.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("conv_title", e.Result));

                convID = classes.Oauth.ParseXML("conv_id", e.Result);

                //toplam mesaj sayisini aliyoruz. api toplam mesaj sayisini 1 fazla goterdigi icin sayidan 1 cikartiyoruz
                total_message = Convert.ToInt32(classes.Oauth.ParseXML("total_message_num", e.Result)); //- 1

                if ((total_message % classes.General.sayfabasiMesaj) > 0)
                    total_page = ((total_message - (total_message % classes.General.sayfabasiMesaj)) / classes.General.sayfabasiMesaj) + 1;
                else
                    total_page = (total_message / classes.General.sayfabasiMesaj);

                vpage.Text = "Sayfa: " + current_page.ToString() + " / " + total_page.ToString();

                //eger suanki sayfa numarasi 2 den kucuk ise onceki sayfa butonunu kilitle
                SetButton(0, (current_page < 2) ? false : true);
                //eger suanki sayfa numarasi toplam sayfa sayisina esit ise sonraki sayfa butonunu kilitle
                SetButton(3, (current_page >= total_page) ? false : true);

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", temp = "", quote = "", c_message = "", richtext = "";

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "") != "")
                    {
                        string[] temp1 = classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    for (int i = 0; i < classes.General.conv_participants.Split('#').Count() - 1; i++)
                    {
                        if (classes.General.conv_participants.Split('#')[i].Split('|')[0] == classes.Oauth.ParseXML2(round.Elements("member"), "msg_author_id"))
                        {
                            temp = classes.General.conv_participants.Split('#')[i].Split('|')[1];
                            break;
                        }
                    }

                    c_message = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "msg_content"));
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
                    /*/if (c_message.IndexOf("[url") >= 0 && c_message.IndexOf("[/url]", c_message.IndexOf("[url")) >= 0)
                    {
                        while (c_message.IndexOf("[url") >= 0)
                        {
                            if (c_message.Substring(c_message.IndexOf("[url") + 4, 1) == "=")
                            {
                                richtext = c_message.Substring(c_message.IndexOf("[url"), c_message.IndexOf("[/url]", c_message.IndexOf("[url")) + 6 - c_message.IndexOf("[url"));
                                c_message = c_message.Replace(richtext, richtext.Substring(richtext.IndexOf('=') + 1, richtext.IndexOf(']', richtext.IndexOf('=')) - 1 - richtext.IndexOf('=')));
                            }
                            else if (c_message.Substring(c_message.IndexOf("[url") + 4, 1) == "]")
                            {
                                richtext = c_message.Substring(c_message.IndexOf("[url]"), c_message.IndexOf("[/url]", c_message.IndexOf("[url]")) + 6 - c_message.IndexOf("[url]"));
                                c_message = c_message.Replace(richtext, richtext.Replace("[url]", "").Replace("[/url]", ""));
                            }
                        }
                    }

                    if (c_message.IndexOf("[img]") >= 0 && c_message.IndexOf("[/img]") >= 0)
                        c_message = c_message.Replace("[img]", "").Replace("[/img]", "");/*/

                    while (c_message.IndexOf("[quote]") >= 0)
                    {
                        if (c_message.IndexOf("[quote]") >= 0 && c_message.IndexOf("[/quote]", c_message.IndexOf("[quote]")) >= 0)
                        {
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
                        quote = classes.Smiley.GetSmiley(quote); //hdp.
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

                    ds.Add(new ConvItems()
                    {
                        msgID = classes.Oauth.ParseXML2(round.Elements("member"), "msg_id"),
                        Message = c_message,
                        //SenderID = Oauth.ParseXML2(round.Elements("member"), "msg_author_id"),
                        senderName = temp,
                        //isUnread = Oauth.ParseXML2(round.Elements("member"), "is_unread"),
                        //isOnline = Oauth.ParseXML2(round.Elements("member"), "is_online"),
                        sendDate = sentDate,
                        //areYou = (App.Session_LoadSession("user_id") == Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "Visible" : "Collapsed",
                        //isHe = (App.Session_LoadSession("user_id") == Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "Collapsed" : "Visible",
                        //backGround = (App.Session_LoadSession("user_id") == Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "#FF2980b9" : "#FFf4ae1a",
                        backGround = (App.Session_LoadSession("user_id") == classes.Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? App.Session_LoadSession("youC1") : App.Session_LoadSession("heC1"),
                        alignItem = (App.Session_LoadSession("user_id") == classes.Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "Left" : "Right",
                        bubbleAlign = (App.Session_LoadSession("user_id") == classes.Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "LowerLeft" : "UpperRight",
                        quoteMessage = quote,
                        quoteVisible = (quote != "") ? "Visible" : "Collapsed",
                        //quoteGround = (App.Session_LoadSession("user_id") == Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? "#FF216694" : "#FFd19516",
                        quoteGround = (App.Session_LoadSession("user_id") == classes.Oauth.ParseXML2(round.Elements("member"), "msg_author_id")) ? App.Session_LoadSession("youC2") : App.Session_LoadSession("heC2"),
                        fSize = classes.General.fontSize.ToString()
                    });
                }

                chatHistory.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (chatHistory.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(this.chatHistory, false);
                                return;
                            }
                        } while (chatHistory.Items.Count() != ds.Count());/*/
                        if (current_page == 1)
                            classes.General.UpdateLayout(this.chatHistory, true);
                        else
                            classes.General.UpdateLayout(this.chatHistory, false);
                        //itemlist.ScrollToVerticalOffset(chatHistory.ActualHeight);
                        //itemlist.UpdateLayout();
                    });
                }

                SetButton(2, true);
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

                MessageBox.Show("Bir hata oluştu : c0x002");
                //General.SendBugReport(e.Result, "c0x002", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            if (message.Text.Replace(" ", "") != "")
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                SetButton(1, false);
                //m_button.IsEnabled = false;
                message.IsEnabled = false;

                current_page = 0;

                message.Text = message.Text.Replace("\r", "\n");

                classes.General.general_list.Clear();

                classes.General.general_list.Add(convID);
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));

                classes.Oauth.XmlRpcExecMethod("reply_conversation", xCompleted);
            }
            else
                MessageBox.Show("Lütfen mesaj alanını doldurunuz.");
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            //current_page = total_page;
            GetConversation();
            SetButton(2, false);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            if (MessageBox.Show("Bu konuşmayı silmek istiyor musunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(convID);
                classes.General.general_list.Add(classes.General.sohbetsilMode);

                classes.Oauth.XmlRpcExecMethod("delete_conversation", zCompleted);
            }
        }

        private void addUser_Click(object sender, RoutedEventArgs e)
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            string temp = @"<array><data>";

            if (add_prtcpn.Text.IndexOf(',') >= 0)
            {
                for (int i = 0; i < add_prtcpn.Text.Split(',').Count(); i++)
                {
                    temp += @"<value><base64>" + classes.General.EncodeBase64(add_prtcpn.Text.Split(',')[i].Replace(",", "").Replace(" ", "")) + "</base64></value>";
                }
            }
            else
                temp += @"<value><base64>" + classes.General.EncodeBase64(add_prtcpn.Text.Replace(",", "").Replace(" ", "")) + "</base64></value>";

            temp += @"</data></array>";

            classes.General.general_list.Clear();
            classes.General.general_list.Add(temp);
            classes.General.general_list.Add(convID);

            classes.Oauth.XmlRpcExecMethod("invite_participant", yCompleted);
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (classes.General.CheckLoggedIn() == "Fail")
                return;

            SetButton(0, false);
            SetButton(1, false);
            SetButton(2, false);
            SetButton(3, false);
            //m_button.IsEnabled = false;
            ApplicationBar.IsMenuEnabled = false;
            addparticipants.Visibility = Visibility.Visible;
        }

        private void add_prtcpn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (add_prtcpn.Text.Replace(" ", "") != "")
                addUser.IsEnabled = true;
            else
                addUser.IsEnabled = false;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (addparticipants.Visibility == Visibility.Collapsed)
                e.Cancel = false;
            else
            {
                e.Cancel = true;
                addparticipants.Visibility = Visibility.Collapsed;
                //m_button.IsEnabled = true;
                SetButton(0, (current_page < 2) ? false : true);
                SetButton(1, (message.Text.Length > 0) ? true : false);
                SetButton(2, true);
                SetButton(3, (current_page >= total_page) ? false : true);
                ApplicationBar.IsMenuEnabled = true;
            }
        }

        public void yCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    MessageBox.Show("Katılımcı(lar) başarıyla eklendi.");
                    addparticipants.Visibility = Visibility.Collapsed;

                    SetButton(0, (current_page < 2) ? false : true);
                    SetButton(1, (message.Text.Length > 0) ? true : false);
                    SetButton(2, true);
                    SetButton(3, (current_page >= total_page) ? false : true);

                    add_prtcpn.Text = "";
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

                MessageBox.Show("Bir hata oluştu : c0x007");
                //General.SendBugReport(e.Result, "c0x007", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        public void zCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    //MessageBox.Show("Konuşma başarıyla silindi.");
                    NavigationService.GoBack();
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

                MessageBox.Show("Bir hata oluştu : c0x006");
                //General.SendBugReport(e.Result, "c0x006", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            message.IsEnabled = true;
            SetButton(1, true);

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    /*/ds.Add(new ConvItems()
                        {
                            Message = message.Text,
                            //SenderID = App.Session_LoadSession("user_id"),
                            senderName = General.username,
                            //isUnread = "0",
                            sendDate = DateTime.Now.ToString(),
                            //areYou = "Visible",
                            //isHe = "Collapsed",
                            backGround = "#FF2980b9",
                            alignItem = "Left",
                            bubbleAlign = "LowerLeft"
                        });

                    chatHistory.ItemsSource = ds;

                    Dispatcher.BeginInvoke(() =>
                    {
                        chatHistory.ScrollIntoView(chatHistory.Items.Last());
                        chatHistory.UpdateLayout();
                    });/*/

                    current_page = 0;
                    total_message += 1;
                    GetConversation();
                    message.Text = "";
                }
                else if (classes.Oauth.ParseXML("result", e.Result) == "0" && classes.Oauth.ParseXML("result_text", e.Result) != "")
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));

                //m_button.IsEnabled = true;
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

                MessageBox.Show("Bir hata oluştu : c0x003");
                //General.SendBugReport(e.Result, "c0x003", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ApplicationBar.IsMenuEnabled = (pages.SelectedIndex == 1) ? false : true;
            //m_button = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            //m_button.IsEnabled = (pages.SelectedIndex == 1) ? false : true;
            //m_button = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (pages.SelectedIndex == 1)
            {
                SetButton(0, false);
                SetButton(1, false);
                SetButton(3, false);
            }
            //m_button.IsEnabled = false;
            else
            {
                SetButton(0, (current_page < 2) ? false : true);
                SetButton(1, (message.Text.Length > 0) ? true : false);
                SetButton(3, (current_page >= total_page) ? false : true);
            }
            //m_button.IsEnabled = (message.Text.Length > 0) ? true : false;

            //if (convID != "")
            //ApplicationBar.IsVisible = (ApplicationBar.IsVisible) ? false : true;
        }

        private void participants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConvUsers drv = (ConvUsers)participants.SelectedItem;

            if (drv != null)
            {
                NavigationService.Navigate(new Uri("/Profile.xaml?user_name=" + drv.cuName.Remove(0, 1), UriKind.Relative));
            }

            participants.SelectedIndex = -1;
        }

        private void message_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (message.SelectionStart == message.Text.Length)
            {
                sending.ScrollToVerticalOffset(message.ActualHeight);
                sending.UpdateLayout();
            }
        }

        private void message_TextChanged(object sender, TextChangedEventArgs e)
        {
            //General.UpdateLayout(this.chatHistory, false);
            SetButton(1, (message.Text.Length > 0) ? true : false);
            //message.SelectionStart = message.Text.Length;
            //message.Focus();
            //m_button.IsEnabled = (message.Text.Length > 0) ? true : false;
        }

        private void next_Click(object sender, EventArgs e)
        {
            current_page += 1;
            GetConversation();
        }

        private void prev_Click(object sender, EventArgs e)
        {
            current_page -= 1;
            GetConversation();
        }

        private void quote_Click(object sender, RoutedEventArgs e)
        {
            ConvItems drv = (sender as MenuItem).DataContext as ConvItems;

            if (drv != null)
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(convID);
                classes.General.general_list.Add(drv.msgID);

                classes.Oauth.XmlRpcExecMethod("get_quote_conversation", aCompleted);
            }
        }

        public void aCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                message.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("text_body", e.Result)) + Environment.NewLine;
                message.SelectionStart = message.Text.Length;
                message.Focus();
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

                MessageBox.Show("Bir hata oluştu : c0x010");
                //General.SendBugReport(e.Result, "c0x010", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void first_Click(object sender, EventArgs e)
        {
            current_page = 1;
            GetConversation();
        }

        private void last_Click(object sender, EventArgs e)
        {
            current_page = 0;
            GetConversation();
        }

        private void write_Click(object sender, EventArgs e)
        {
            isLoaded = 0;
            NavigationService.Navigate(new Uri("/MessageSend.xaml?conv_id=" + convID, UriKind.Relative));
        }
    }
}
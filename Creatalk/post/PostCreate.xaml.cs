using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;

namespace Creatalk.post
{
    public partial class PostCreate : PhoneApplicationPage
    {
        string sforum_id = "-1";
        string topic_id = "0";
        string post_id = "0";

        //int post_count = 0;

        //short reLoad = 0;

        ApplicationBarIconButton m_button;

        public PostCreate()
        {
            InitializeComponent();

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

        private void create_Click(object sender, EventArgs e)
        {
            if (message.Text.Replace(" ", "") != "")
            {
                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;

                message.Text = message.Text.Replace("\r", "\n");

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();

                classes.General.general_list.Add(sforum_id);
                classes.General.general_list.Add(topic_id);
                classes.General.general_list.Add("base64|");
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));
                //General.general_list.Add("base64|" + General.EncodeBase64(message.Text + Environment.NewLine + Environment.NewLine + "[SIZE=2]" + App.Session_LoadSession("OwnSign") + "[/SIZE]"));

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("reply_post", uCompleted);
            }
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            m_button.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    //MessageBox.Show("Yanıt başarıyla gönderildi.");
                    forum.ForumPost.add_post = 1;
                    NavigationService.GoBack();
                    //NavigationService.Navigate(new Uri("/ForumPost.xaml?forum_id=" + forum_id + "&topic_id=" + topic_id + "&post_count=" + (post_count + 1).ToString() + "&redirected", UriKind.Relative));
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

                MessageBox.Show("Bir hata oluştu : f0x011");
                //General.SendBugReport(e.Result, "f0x011", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.Count() < 2)
            {
                m_button.IsEnabled = false;
                message.IsEnabled = false;
                MessageBox.Show("Eksik parametre.");
                return;
            }

            sforum_id = NavigationContext.QueryString["sforum_id"];
            topic_id = NavigationContext.QueryString["topic_id"];
            //post_count = Convert.ToInt32(NavigationContext.QueryString["post_count"]);

            if (NavigationContext.QueryString.Count() > 2)
                post_id = NavigationContext.QueryString["post_id"];

            /*/if (NavigationContext.QueryString.Count() > 0)
            {
                
            }
            else (Exception ex)
            {
                BugSenseHandler.Instance.SendExceptionAsync(ex);
            
                MessageBox.Show("Bir hata oluştu : p0x001");
                //General.SendBugReport(e.Result, "p0x001", ex.Message, ex.Source, ex.HelpLink);
                return;
            }/*/

            if (post_id != "0")
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();

                classes.General.general_list.Add(post_id);

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("get_quote_post", xCompleted);
            }
        }

        public void xCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            m_button.IsEnabled = true;

            try
            {
                //message.Text = General.DecodeBase64(Oauth.ParseXML("post_content", e.Result)).Replace("\n\n[SIZE=2]Creatalk Windows Phone uygulamasıyla gönderildi.[/SIZE]", "") + Environment.NewLine;
                message.Text = classes.General.DecodeBase64(classes.Oauth.ParseXML("post_content", e.Result)) + Environment.NewLine;
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

                MessageBox.Show("Bir hata oluştu : p0x001");
                //General.SendBugReport(e.Result, "p0x001", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void menu_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Cevap yazmaktan vazgeçiyorsunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void message_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (message.SelectionStart == message.Text.Length)
            {
                itemlist.ScrollToVerticalOffset(content.ActualHeight);
                itemlist.UpdateLayout();
            }
        }

        private void wUrl_Click(object sender, RoutedEventArgs e)
        {
            message.Text += "[url][/url]";
            message.Select(message.Text.Length - 6, 0);
            //message.Focus();
        }

        private void wImg_Click(object sender, RoutedEventArgs e)
        {
            message.Text += "[img][/img]";
            message.Select(message.Text.Length - 6, 0);
            //message.Focus();
        }
    }
}
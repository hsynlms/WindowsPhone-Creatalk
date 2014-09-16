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

namespace Creatalk
{
    public partial class MessageSend : PhoneApplicationPage
    {
        string convID = "0";

        ApplicationBarIconButton m_button;

        public MessageSend()
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.Count() < 1)
            {
                m_button.IsEnabled = false;
                message.IsEnabled = false;
                MessageBox.Show("Eksik parametre.");
                return;
            }

            convID = NavigationContext.QueryString["conv_id"];
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

        private void send_Click(object sender, EventArgs e)
        {
            if (message.Text.Replace(" ", "") != "")
            {
                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;

                message.Text = message.Text.Replace("\r", "\n");

                classes.General.general_list.Clear();

                classes.General.general_list.Add(convID);
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));

                classes.Oauth.XmlRpcExecMethod("reply_conversation", uCompleted);
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
                    //MessageBox.Show("Mesaj başarıyla gönderildi.");
                    conv.ConvSend.add_message = 1;
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

                MessageBox.Show("Bir hata oluştu : c0x011");
                //General.SendBugReport(e.Result, "c0x011", ex.Message, ex.Source, ex.HelpLink);
            }
        }
    }
}
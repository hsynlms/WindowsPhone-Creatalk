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

namespace Creatalk.topic
{
    public partial class TopicCreate : PhoneApplicationPage
    {
        string sforum_id = "0";

        ApplicationBarIconButton m_button;

        //short reLoad = 0;

        public TopicCreate()
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

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            m_button.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    MessageBox.Show("Konu başarıyla oluşturuldu.");
                    NavigationService.GoBack();
                    //NavigationService.Navigate(new Uri("/ForumTopic.xaml?forum_id=" + forum_id + "&sforum_id=" + sforum_id + "&redirected", UriKind.Relative));
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

                MessageBox.Show("Bir hata oluştu : f0x007");
                //General.SendBugReport(e.Result, "f0x007", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void create_Click(object sender, EventArgs e)
        {
            if (subject.Text.Replace(" ", "") != "" && message.Text.Replace(" ", "") != "")
            {
                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                if (NavigationContext.QueryString.Count() < 1) //2
                {
                    MessageBox.Show("Eksik parametre.");
                    return;
                }

                //forum_id = NavigationContext.QueryString["forum_id"];
                sforum_id = NavigationContext.QueryString["sforum_id"];

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;

                message.Text = message.Text.Replace("\r", "\n");

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();

                classes.General.general_list.Add(sforum_id);
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(subject.Text));
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));
                //General.general_list.Add("base64|" + General.EncodeBase64(message.Text + Environment.NewLine + Environment.NewLine + "[SIZE=2]" + App.Session_LoadSession("OwnSign") + "[/SIZE]"));

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("new_topic", uCompleted);
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
        }

        private void menu_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Konu açmaktan vazgeçiyorsunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void message_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            itemlist.ScrollToVerticalOffset(content.ActualHeight);
            itemlist.UpdateLayout();
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
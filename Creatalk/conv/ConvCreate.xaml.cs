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

namespace Creatalk.conv
{
    public partial class ConvCreate : PhoneApplicationPage
    {
        ApplicationBarIconButton m_button;

        //short reLoad = 0;

        public ConvCreate()
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
            if (NavigationContext.QueryString.Count() > 0)
                participants.Text = NavigationContext.QueryString["user_name"] + ",";
        }

        private void create_Click(object sender, EventArgs e)
        {
            if (participants.Text.Replace(" ", "") != "" && subject.Text.Replace(" ", "") != "" && message.Text.Replace(" ", "") != "")
            {
                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;

                message.Text = message.Text.Replace("\r", "\n");

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();

                string temp = @"<array><data>";

                if (participants.Text.IndexOf(',') >= 0)
                {
                    for (int i = 0; i < participants.Text.Split(',').Count(); i++)
                    {
                        temp += @"<value><base64>" + classes.General.EncodeBase64(participants.Text.Split(',')[i].Replace(",", "").Replace(" ", "")) + "</base64></value>";
                    }
                }
                else
                    temp += @"<value><base64>" + classes.General.EncodeBase64(participants.Text.Replace(",", "").Replace(" ", "")) + "</base64></value>";

                temp += @"</data></array>";

                classes.General.general_list.Add(temp);
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(subject.Text));
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(message.Text));

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("new_conversation", uCompleted);
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
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
                    MessageBox.Show("Sohbet başarıyla oluşturuldu.");
                    NavigationService.GoBack();
                    //NavigationService.Navigate(new Uri("/Conversation.xaml", UriKind.Relative));
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

                MessageBox.Show("Bir hata oluştu : c0x005");
                //General.SendBugReport(e.Result, "c0x005", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void menu_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Sohbet başlatmaktan vazgeçiyorsunuz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
    }
}
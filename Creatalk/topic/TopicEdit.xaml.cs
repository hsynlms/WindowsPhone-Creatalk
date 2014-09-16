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
    public partial class TopicEdit : PhoneApplicationPage
    {
        string topic_id = "0";

        ApplicationBarIconButton m_button;

        public TopicEdit()
        {
            InitializeComponent();

            m_button = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.Count() < 1)
            {
                m_button.IsEnabled = false;
                subject.IsEnabled = false;
                MessageBox.Show("Eksik parametre.");
                return;
            }

            topic_id = NavigationContext.QueryString["topic_id"];
            subject.Text = classes.General.topic_title;

            //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
            //m_button.IsEnabled = false;
        }

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

        private void menu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (subject.Text.Replace(" ", "") != "")
            {
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                m_button.IsEnabled = false;
                subject.IsEnabled = false;

                classes.General.general_list.Clear();
                classes.General.general_list.Add(topic_id);
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(subject.Text));

                classes.Oauth.XmlRpcExecMethod("m_rename_topic", mCompleted);
            }
        }

        public void mCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            m_button.IsEnabled = true;
            subject.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1")
                {
                    MessageBox.Show("İşlem başarılı.");
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

                MessageBox.Show("Bir hata oluştu : ad0x001");
                //General.SendBugReport(e.Result, "ad0x001", ex.Message, ex.Source, ex.HelpLink);
            }
        }
    }
}
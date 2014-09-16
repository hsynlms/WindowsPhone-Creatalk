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
    public partial class ChangeInfo : PhoneApplicationPage
    {
        //short reLoad = 0;

        public ChangeInfo()
        {
            InitializeComponent();
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

        private void save1_Click(object sender, RoutedEventArgs e)
        {
            if (msifre1.Password.Replace(" ", "") != "" && new_pass1.Password.Replace(" ", "") != "" && new_pass2.Password.Replace(" ", "") != "")
            {
                if (new_pass1.Password == new_pass2.Password)
                {
                    //interneti kontrol ettiriyoruz
                    if (classes.General.CheckNetwork() == "Fail")
                        return;

                    //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                    save1.IsEnabled = false;

                    //parametre listesini temizliyoruz
                    classes.General.general_list.Clear();
                    classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(msifre1.Password));
                    classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(new_pass1.Password));

                    //apiyi cagirma komutunu kullaniyoruz
                    classes.Oauth.XmlRpcExecMethod("update_password", uCompleted);
                }
                else
                {
                    MessageBox.Show("Girilen şifreler birbiriyle uyuşmuyor.");
                    new_pass1.Focus();
                }
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            save1.IsEnabled = true;
            save2.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1" && classes.Oauth.ParseXML("result_text", e.Result) == "")
                {
                    MessageBox.Show("İşlem başarılı.");
                    NavigationService.Navigate(new Uri("/EnterForum.xaml", UriKind.Relative));
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

                MessageBox.Show("Bir hata oluştu : s0x001");
                //General.SendBugReport(e.Result, "s0x001", ex.Message, ex.Source, ex.HelpLink);
            }
        }

        private void save2_Click(object sender, RoutedEventArgs e)
        {
            if (msifre2.Password.Replace(" ", "") != "" && email.Text.Replace(" ", "") != "")
            {
                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                save2.IsEnabled = false;

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(msifre2.Password));
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(email.Text));

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("update_email", uCompleted);
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService.Source.OriginalString.IndexOf("email") >= 0)
                pages.SelectedIndex = 1;
            else
                pages.SelectedIndex = 0;
        }
    }
}
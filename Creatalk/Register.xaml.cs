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
    public partial class Register : PhoneApplicationPage
    {
        public Register()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            username.Focus();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text.Replace(" ", "") != "" && password.Password.Replace(" ", "") != "" && email.Text.Replace(" ", "") != "")
            {
                if (username.Text.Length < 3)
                {
                    MessageBox.Show("Kullanıcı adı en az 3 harfli olmalıdır.");
                    username.Focus();
                    return;
                }

                //interneti kontrol ettiriyoruz
                if (classes.General.CheckNetwork() == "Fail")
                    return;

                //butonu deaktif ediyoruz tekrar tekrar basilmamasi icin
                register.IsEnabled = false;

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(username.Text));
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(password.Password));
                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(email.Text));

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("register", uCompleted);
            }
            else
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            //butonu aktif ediyoruz
            register.IsEnabled = true;

            try
            {
                if (classes.Oauth.ParseXML("result", e.Result) == "1")
                {
                    MessageBox.Show(classes.General.DecodeBase64(classes.Oauth.ParseXML("result_text", e.Result)));
                    //NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
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

                MessageBox.Show("Bir hata oluştu : r0x001");
                //General.SendBugReport(e.Result, "r0x001", ex.Message, ex.Source, ex.HelpLink);
            }
        }
    }
}
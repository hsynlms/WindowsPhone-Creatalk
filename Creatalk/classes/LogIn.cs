using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Net;

namespace Creatalk.classes
{
    public class LogIn
    {
        private static bool wokedup = false;
        private static string user_name = "";
        private static string user_pass = "";

        public static void AutomaticLogin()
        {
            if (General.DecodeBase64(App.Session_LoadSession("user_")).Replace(" ", "") != "" && General.DecodeBase64(App.Session_LoadSession("pass_")).Replace(" ", "") != "")
            {
                CheckFields(General.DecodeBase64(App.Session_LoadSession("user_")), General.DecodeBase64(App.Session_LoadSession("pass_")), true);
            }
        }

        public static void CheckFields(string u_name, string u_pass, bool wokeup)
        {
            if (u_name.Replace(" ", "") != "" && u_pass.Replace(" ", "") != "")
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return;
                }

                MainPage.loggedin = -1;
                wokedup = wokeup;
                user_name = u_name;
                user_pass = u_pass;

                General.general_list.Clear();
                General.general_list.Add("base64|" + General.EncodeBase64(u_name));
                General.general_list.Add("base64|" + General.EncodeBase64(u_pass));

                Oauth.XmlRpcExecMethod("login", uCompleted);
            }
        }

        public static void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            WebClient webClient = (WebClient)sender;
            string str = "";
            try
            {
                str = webClient.ResponseHeaders["Set-cookie"].ToString();
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);
            }

            try
            {
                string str1 = Oauth.ParseXML("result", e.Result);
                string str2 = General.DecodeBase64(Oauth.ParseXML("result_text", e.Result));
                string str3 = Oauth.ParseXML("status", e.Result);
                if (str1 == "1")
                {
                    App.SaveSettings("xf_session", ""); //hdp.
                    App.SaveSettings("user_", General.EncodeBase64(user_name));
                    App.SaveSettings("pass_", General.EncodeBase64(user_pass));
                    App.SaveSettings("email", Oauth.ParseXML("email", e.Result));
                    App.SaveSettings("user_id", Oauth.ParseXML("user_id", e.Result));
                    App.SaveSettings("icon_url", Oauth.ParseXML("icon_url", e.Result));
                    App.SaveSettings("can_pm", Oauth.ParseXML("can_pm", e.Result));
                    App.SaveSettings("can_send_pm", Oauth.ParseXML("can_send_pm", e.Result));
                    App.SaveSettings("can_search", Oauth.ParseXML("can_search", e.Result));

                    General.username = General.DecodeBase64(Oauth.ParseXML("username", e.Result));
                    App.SaveSettings("xf_session", str.Substring(str.IndexOf("=") + 1, str.IndexOf(";") - str.IndexOf("="))); //hdp.
                    MainPage.loggedin = 1;
                }
                else if (str3 != "2")
                {
                    if (!wokedup)
                    {
                        if (str2 == "")
                        {
                            MessageBox.Show("Girilen hesap bilgilerini kontrol ediniz.");
                        }
                        else
                        {
                            MessageBox.Show(str2);
                        }
                    }

                    App.SaveSettings("user_", "");
                    App.SaveSettings("pass_", "");
                    App.SaveSettings("xf_session", ""); //hdp.
                    General.username = "";
                    MainPage.loggedin = 0;
                }
                else
                {
                    if (!LogIn.wokedup)
                    {
                        MessageBox.Show("Böyle bir hesap mevcut değil.");
                    }

                    App.SaveSettings("user_", "");
                    App.SaveSettings("pass_", "");
                    App.SaveSettings("xf_session", ""); //hdp.
                    General.username = "";
                    MainPage.loggedin = 0;
                }
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                if (!wokedup)
                {
                    MessageBox.Show("Lütfen yeniden giriş yapın.");
                }

                App.SaveSettings("user_", "");
                App.SaveSettings("pass_", "");
                App.SaveSettings("xf_session", ""); //hdp.
                General.username = "";
                MainPage.loggedin = 0;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                if (!wokedup)
                {
                    //General.SendBugReport(e.Result, "result-error", ex.Message, ex.Source, ex.HelpLink);
                }

                App.SaveSettings("user_", "");
                App.SaveSettings("pass_", "");
                App.SaveSettings("xf_session", ""); //hdp.
                General.username = "";
                MainPage.loggedin = 0;
            }
        }
    }
}

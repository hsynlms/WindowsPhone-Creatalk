using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;

namespace Creatalk.classes
{
    public class General
    {
        public static string forum_domain = ""; //Buraya tapatalk istemcisini yapacağınız forumun adresini yazmalısınız. Örneğin "http://www.huseyinelmas.net/forum/"
        public static string username = "";
        public static string conv_participants = "";
        public static string topic_title = "";
        public static string post_likes = "";
        public static List<Object> general_list = new List<Object>();

        //public static short sayfabasiOnline = 20;

        //sayfa basina dusen sohbete ait mesaj sayisi
        public static short sayfabasiMesaj = 12;
        //sayfa basina dusen sohbet sayisi
        public static short sayfabasiChat = 20;
        //sayfa basina dusen konu sayisi
        public static short sayfabasiTopic = 10;
        //sayfa basina dusen yanit sayisi
        public static short sayfabasiPost = 8;
        //sayfa basina dusen yeni/sicak/taze konu sayisi
        public static short sayfabasiRecent = 15;
        //sayfa basina dusen takip edilen konu sayisi
        public static short sayfabasiParticipated = 10;
        //mesaj silme modu (2 = cevap gelse bile haber verme)
        public static short sohbetsilMode = 2;
        public static short fontSize = 20;
        public static short sayfabasiSearch = 15;

        public static string CheckNetwork()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Internet bağlantınızı kontrol ediniz.");
                return "Fail";
            }
            else
                return "";
        }

        public static void WriteFavori(string wfile, string wtext)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(wfile, FileMode.Create, FileAccess.Write, isf)))
                {
                    sw.WriteLine(wtext.Replace("\n", "").Replace("\r", ""));
                    sw.Close();
                }
            }
        }

        public static string ReadFavori(string rfile)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(rfile))
                {
                    using (StreamReader sw = new StreamReader(new IsolatedStorageFileStream(rfile, FileMode.Open, FileAccess.Read, isf)))
                    {
                        return sw.ReadToEnd().Replace("\n", "").Replace("\r", "");
                    }
                }
                else
                    return "";
            }
        }

        public static void DisableVirtualization(RadDataBoundListBox listbox)
        {
            listbox.DataVirtualizationMode = DataVirtualizationMode.None;
        }

        public static string SetForumLogo(string topic_id) //hdp.
        {
            return "/images/forum.png";
            /*string temp = "";

            switch (topic_id)
            {
                case "3":
                    temp = "/images/forums/mobile.png";
                    break;
                case "4":
                    temp = "/images/forums/computer.png";
                    break;
                case "5":
                    temp = "/images/forums/sell.png";
                    break;
                case "6":
                    temp = "/images/forums/smile.png";
                    break;
                case "7":
                    temp = "/images/forums/admin.png";
                    break;
                case "8":
                    temp = "/images/forums/home.png";
                    break;
                case "10":
                    temp = "/images/forums/cd.png";
                    break;
                case "30":
                    temp = "/images/forums/spor.png";
                    break;
                case "33":
                    temp = "/images/forums/telekom.png";
                    break;
                case "40":
                    temp = "/images/forums/nokia.png";
                    break;
                case "144":
                    temp = "/images/forums/multimedya.png";
                    break;
                case "159":
                    temp = "/images/forums/education.png";
                    break;
                case "180":
                    temp = "/images/forums/mobile.png";
                    break;
                case "285":
                    temp = "/images/forums/ios.png";
                    break;
                case "381":
                    temp = "/images/forums/woman.png";
                    break;
                case "448":
                    temp = "/images/forums/xbox.png";
                    break;
                case "543":
                    temp = "/images/forums/android.png";
                    break;
                case "605":
                    temp = "/images/forums/windowsphone.png";
                    break;
                case "655":
                    temp = "/images/forums/meego.png";
                    break;
                case "752":
                    temp = "/images/forums/symbian.png";
                    break;
                case "757":
                    temp = "/images/forums/tablet.png";
                    break;
                default:
                    temp = "/images/sticky.png";
                    break;
            }

            return temp;*/ //set ok

            /*if (topic_id == "3")
                temp = "/images/forums/mobile.png";
            else if (topic_id == "4")
                temp = "/images/forums/computer.png";
            else if (topic_id == "5")
                temp = "/images/forums/sell.png";
            else if (topic_id == "6")
                temp = "/images/forums/smile.png";
            else if (topic_id == "10")
                temp = "/images/forums/cd.png";
            else if (topic_id == "30")
                temp = "/images/forums/spor.png";
            else if (topic_id == "33")
                temp = "/images/forums/telekom.png";
            else if (topic_id == "40")
                temp = "/images/forums/nokia.png";
            else if (topic_id == "144")
                temp = "/images/forums/multimedya.png";
            else if (topic_id == "159")
                temp = "/images/forums/education.png";
            else if (topic_id == "180")
                temp = "/images/forums/mobile.png";
            else if (topic_id == "285")
                temp = "/images/forums/ios.png";
            else if (topic_id == "381")
                temp = "/images/forums/woman.png";
            else if (topic_id == "448")
                temp = "/images/forums/xbox.png";
            else if (topic_id == "543")
                temp = "/images/forums/android.png";
            else if (topic_id == "605")
                temp = "/images/forums/windowsphone.png";
            else if (topic_id == "655")
                temp = "/images/forums/meego.png";
            else if (topic_id == "752")
                temp = "/images/forums/symbian.png";
            else if (topic_id == "757")
                temp = "/images/forums/tablet.png";
            else
                temp = "/images/sticky.png";*/
        }

        public static void GetVariables()
        {
            fontSize = Convert.ToInt16(App.Session_LoadSession("fSize"));
            sayfabasiMesaj = Convert.ToInt16(App.Session_LoadSession("sbMesaj"));
            sayfabasiChat = Convert.ToInt16(App.Session_LoadSession("sbChat"));
            sayfabasiTopic = Convert.ToInt16(App.Session_LoadSession("sbTopic"));
            sayfabasiPost = Convert.ToInt16(App.Session_LoadSession("sbPost"));
            sayfabasiRecent = Convert.ToInt16(App.Session_LoadSession("sbNew"));
            sayfabasiSearch = Convert.ToInt16(App.Session_LoadSession("sbSearch"));
            sayfabasiParticipated = Convert.ToInt16(App.Session_LoadSession("sbParticipate"));
        }

        /*public static void SendBugReport(string received, string error_code, string error_message, string error_source, string error_link)
        {
            string tbody = "";

            EmailComposeTask task = new EmailComposeTask();
            task.To = "developer@apps-dev.com";
            task.Subject = "Creatalk uygulaması bug bildirme";

            if (received.Length > 2500)
                tbody = "Alınan data:\n" + received.Substring(1, 2500) + "\n\n\nHata kodu:\n" + error_code + "\n\nHata mesajı:\n" + error_message + "\n\nSorun kaynağı:\n" + error_source + "\n\nÇözüm linki:\n" + error_link;
            else
                tbody = "Alınan data:\n" + received + "\n\n\nHata kodu:\n" + error_code + "\n\nHata mesajı:\n" + error_message + "\n\nSorun kaynağı:\n" + error_source + "\n\nÇözüm linki:\n" + error_link;

            if (General.DecodeBase64(App.Session_LoadSession("user_")).Replace(" ", "") != "")
                tbody += "\n\nKullanıcı adı belirlendi: Evet";
            else
                tbody += "\n\nKullanıcı adı belirlendi: Hayır";

            if (General.DecodeBase64(App.Session_LoadSession("pass_")).Replace(" ", "") != "")
                tbody += "\n\nŞifre belirlendi: Evet";
            else
                tbody += "\n\nŞifre belirlendi: Hayır";

            tbody += "\n\nÇerez: " + App.Session_LoadSession("xf_session");

            task.Body = tbody;
            task.Show();
        }*/

        public static string CheckLoggedIn()
        {
            //eger xf_session bos ise ayrıca general sinifindaki username de bos ise giris basarisizdir
            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "") //hdp.
            {
                MessageBox.Show("Bu işlemi yapabilmek için önce giriş yapmalısınız.");
                return "Fail";
            }
            else
                return "";
        }

        public static string ReturnCleanHTML(string sData)
        {
            string richtext = "", c_message = sData;

            /*/c_message = c_message.Replace("[url", " -> [url");
            c_message = c_message.Replace("[URL", " -> [url");
            c_message = c_message.Replace("[/URL]", "[/url]");
            c_message = c_message.Replace("[/url]", "[/url] <-");

            c_message = c_message.Replace("[img]", " -> [img]");
            c_message = c_message.Replace("[IMG]", " -> [img]");
            c_message = c_message.Replace("[/IMG]", "[/img]");
            c_message = c_message.Replace("[/img]", "[/img] <-");

            c_message = c_message.Replace("[QUOTE", "[quote");
            c_message = c_message.Replace("[/QUOTE]", "[/quote]");/*/

            c_message = c_message.Replace("[DOUBLEPOST", "[doublepost");
            c_message = c_message.Replace("[/DOUBLEPOST]", "[/doublepost]");

            try
            {
                if (c_message.IndexOf("[url") >= 0 && c_message.IndexOf("[/url]", c_message.IndexOf("[url")) >= 0)
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
            }
            catch
            {
                //hicbir sey yapma
            }

            if (c_message.IndexOf("[img]") >= 0 && c_message.IndexOf("[/img]") >= 0)
                c_message = c_message.Replace("[img]", "").Replace("[/img]", ".maxiresim ");

            if (c_message.IndexOf("[doublepost") >= 0 && c_message.IndexOf("[/doublepost]", c_message.IndexOf("[doublepost")) >= 0)
            {
                while (c_message.IndexOf("[doublepost") >= 0 && c_message.IndexOf("[/doublepost]", c_message.IndexOf("[doublepost")) >= 0)
                {
                    c_message = c_message.Replace(c_message.Substring(c_message.IndexOf("[doublepost"), c_message.IndexOf("[/doublepost]", c_message.IndexOf("[doublepost")) + 13 - c_message.IndexOf("[doublepost")), Environment.NewLine + "--- Mesajlar birleştirildi ---" + Environment.NewLine);
                }
            }

            c_message = c_message.Replace("[/url]", "");
            c_message = c_message.Replace("[img]", "");
            c_message = c_message.Replace("[/img]", "");
            c_message = c_message.Replace("[/quote]", "");

            return c_message;
        }

        public static void UpdateLayout(ListBox item, bool isfirst)
        {
            try
            {
                item.UpdateLayout();

                if (isfirst)
                    item.ScrollIntoView(item.Items.First());
                else
                    item.ScrollIntoView(item.Items.Last());

                item.UpdateLayout();
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                //MessageBox.Show(ex.Message);
            }
        }

        public static string EncodeBase64(string str)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Hata oluştu : u0x001");
            }

            return "";
        }

        public static string DecodeBase64(string str)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(str), 0, Convert.FromBase64String(str).Length);
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Hata oluştu : u0x002");
            }

            return "";
        }

        private static void AnimateMe(UIElement item, double topFrom, double topTo, double opacityFrom, double opacityTo, short hv)
        {
            // setup
            TranslateTransform _Translate;
            if (hv == 0)
                _Translate = new TranslateTransform { X = 0, Y = topFrom };
            else
                _Translate = new TranslateTransform { X = topFrom, Y = 0 };

            item.RenderTransform = _Translate;
            item.Visibility = Visibility.Visible;
            item.Opacity = opacityFrom;

            // animate
            Storyboard _Storyboard = new Storyboard();
            TimeSpan _Duration = TimeSpan.FromSeconds(1.5);

            // opacity
            DoubleAnimation _OpacityAnimate = new DoubleAnimation
            {
                To = opacityTo,
                Duration = _Duration,
            };

            _Storyboard.Children.Add(_OpacityAnimate);
            Storyboard.SetTarget(_OpacityAnimate, item);
            Storyboard.SetTargetProperty(_OpacityAnimate, new PropertyPath(UIElement.OpacityProperty));

            // translate (location)
            DoubleAnimation _TranslateAnimate = new DoubleAnimation
            {
                To = topTo,
                Duration = _Duration,
                EasingFunction = new SineEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            _Storyboard.Children.Add(_TranslateAnimate);
            Storyboard.SetTarget(_TranslateAnimate, _Translate);
            Storyboard.SetTargetProperty(_TranslateAnimate, new PropertyPath(TranslateTransform.YProperty));

            // finalize
            _Storyboard.Begin();
        }
    }
}

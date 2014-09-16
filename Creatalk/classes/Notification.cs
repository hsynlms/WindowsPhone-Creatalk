using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Windows;
using Coding4Fun.Toolkit.Controls;

namespace Creatalk.classes
{
    public class Notification
    {
        private static Grid notification = new Grid();
        private static TextBlock ncount = new TextBlock();
        private static Grid mnotification = new Grid();
        private static TextBlock mcount = new TextBlock();
        private static TileNotification ccount = new TileNotification();

        public static void GetNotifications(Grid p1, TextBlock p2)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "") //hdp.
            {
                p1.Visibility = Visibility.Collapsed;
                return;
            }

            if (App.Session_LoadSession("aNotify") == "0")
            {
                p1.Visibility = Visibility.Collapsed;
                return;
            }

            //progress.IsVisible = true;
            notification = p1;
            ncount = p2;

            //parametre listesini temizliyoruz
            General.general_list.Clear();
            General.general_list.Add(false);

            //apiyi cagirma komutunu kullaniyoruz
            Oauth.XmlRpcExecMethod("get_notification", nCompleted);
        }

        private static void nCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return;

                if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "") //hdp.
                    return;

                if (Oauth.ParseXML("new_alerts", e.Result) != "0" && Oauth.ParseXML("new_alerts", e.Result).Replace(" ", "") != "")
                {
                    ncount.Text = Oauth.ParseXML("new_alerts", e.Result); //+" bildirim";
                    notification.Visibility = Visibility.Visible;
                }
                else
                    notification.Visibility = Visibility.Collapsed;
            }
            catch
            {
                notification.Visibility = Visibility.Collapsed;
            }
        }

        public static void GetConversationNotification(Grid p1, TextBlock p2, TileNotification p3)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "") //hdp.
            {
                p1.Visibility = Visibility.Collapsed;
                return;
            }

            if (App.Session_LoadSession("aNotify") == "0")
            {
                p1.Visibility = Visibility.Collapsed;
                return;
            }

            //progress.IsVisible = true;
            mnotification = p1;
            mcount = p2;
            ccount = p3;

            //parametre listesini temizliyoruz
            General.general_list.Clear();

            //apiyi cagirma komutunu kullaniyoruz
            Oauth.XmlRpcExecMethod("get_inbox_stat", cCompleted);
        }

        private static void cCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return;

                if (App.Session_LoadSession("xf_session").Replace(" ", "") == "" && General.username.Replace(" ", "") == "") //hdp.
                    return;

                if (Oauth.ParseXML("inbox_unread_count", e.Result) != "0" && Oauth.ParseXML("inbox_unread_count", e.Result).Replace(" ", "") != "")
                {
                    mcount.Text = Oauth.ParseXML("inbox_unread_count", e.Result);

                    if (ccount != null)
                    {
                        ccount.Content = Oauth.ParseXML("inbox_unread_count", e.Result);
                        ccount.Visibility = Visibility.Visible;
                    }

                    mnotification.Visibility = Visibility.Visible;
                }
                else
                {
                    if (ccount != null)
                        ccount.Visibility = Visibility.Collapsed;

                    mnotification.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                if (ccount != null)
                    ccount.Visibility = Visibility.Collapsed;

                mnotification.Visibility = Visibility.Collapsed;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;

namespace Creatalk
{
    public class SearchItem
    {
        public string sForumID { get; set; }
        public string sForumName { get; set; }
        public string sTopicID { get; set; }
        public string sTopicTitle { get; set; }
        public string sPostID { get; set; }
        //public string sPostAuthorID { get; set; }
        //public string sPostAuthorType { get; set; }
        public string sPostAuthorInfo { get; set; }
        public string sPostAuthor { get; set; }
        public string sAuthorAvatar { get; set; }
        public string sPostTime { get; set; }
        public string sReplyNumber { get; set; }
        //public string sNewPost { get; set; }
        public string sShortContent { get; set; }
        //public string sViewNumber { get; set; }
        public string sUnread { get; set; }
    }

    public partial class Search : PhoneApplicationPage
    {
        ObservableCollection<SearchItem> ds = new ObservableCollection<SearchItem>();

        ProgressIndicator progress = new ProgressIndicator { IsVisible = false, IsIndeterminate = true, Text = "Arama yapılıyor..." };

        short isLoaded = 0;

        public Search()
        {
            InitializeComponent();

            SystemTray.SetProgressIndicator(this, progress);
        }

        void DoSearch()
        {
            if (classes.General.CheckNetwork() == "Fail")
                return;

            if (search.Text.Replace(" ", "") == "")
                MessageBox.Show("Lütfen arama yapmak için birkaç şey yazın.");
            else
            {
                progress.IsVisible = true;

                //parametre listesini temizliyoruz
                classes.General.general_list.Clear();

                classes.General.general_list.Add("base64|" + classes.General.EncodeBase64(search.Text));
                classes.General.general_list.Add(1);
                classes.General.general_list.Add(classes.General.sayfabasiSearch);

                //apiyi cagirma komutunu kullaniyoruz
                classes.Oauth.XmlRpcExecMethod("search_post", uCompleted);
            }
        }

        private void find_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DoSearch();
        }

        public void uCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result); //debug mode

            try
            {
                ds.Clear();

                foreach (XElement round in XDocument.Parse(e.Result).Descendants("data").Elements("value").Elements("struct"))
                {
                    string sentDate = "", avatar_url = "", temp = "";

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "") != "")
                    {
                        string[] temp1 = classes.Oauth.ParseXML2(round.Elements("member"), "post_time").Replace(" ", "").Split('T');
                        DateTime date = new DateTime();
                        DateTime.TryParse(temp1[0].Substring(temp1[0].Length - 2, 2) + "/" + temp1[0].Substring(temp1[0].Length - 4, 2) + "/" + temp1[0].Substring(0, 4) + " " + temp1[1].Substring(0, 2) + ":" + temp1[1].Substring(3, 2) + ":" + temp1[1].Substring(6, 2), out date);
                        sentDate = date.ToString();
                    }
                    else
                        continue;

                    if (classes.Oauth.ParseXML2(round.Elements("member"), "icon_url") != "")
                        avatar_url = classes.Oauth.ParseXML2(round.Elements("member"), "icon_url");
                    else
                        avatar_url = classes.General.forum_domain + "styles/default/xenforo/avatars/avatar_l.png"; //hdp.

                    temp = (classes.Oauth.ParseXML2(round.Elements("member"), "new_post") != "0") ? "OrangeRed" : "#FF454545";

                    ds.Add(new SearchItem()
                    {
                        sForumID = classes.Oauth.ParseXML2(round.Elements("member"), "forum_id"),
                        sPostID = classes.Oauth.ParseXML2(round.Elements("member"), "post_id"),
                        sReplyNumber = classes.Oauth.ParseXML2(round.Elements("member"), "reply_number"),
                        sForumName = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "forum_name")),
                        sTopicID = classes.Oauth.ParseXML2(round.Elements("member"), "topic_id"),
                        sTopicTitle = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "topic_title")),
                        //sViewNumber = Oauth.ParseXML2(round.Elements("member"), "view_number"),
                        //sNewPost = Oauth.ParseXML2(round.Elements("member"), "new_post"),
                        //sPostAuthorType = General.DecodeBase64(Oauth.ParseXML2(round.Elements("member"), "user_type")),
                        //sPostAuthorID = Oauth.ParseXML2(round.Elements("member"), "post_author_id"),
                        sShortContent = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "short_content")),
                        sPostTime = sentDate,
                        sAuthorAvatar = avatar_url,
                        sPostAuthor = classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")),
                        sPostAuthorInfo = "@" + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "post_author_name")) + ", " + classes.General.DecodeBase64(classes.Oauth.ParseXML2(round.Elements("member"), "user_type")) + " üye",
                        sUnread = temp
                    });
                }

                searchList.ItemsSource = ds;

                if (ds.Count() > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*/do
                        {
                            if (item.Items.Count() == ds.Count())
                            {
                                General.UpdateLayout(item, true);
                                return;
                            }
                        } while (item.Items.Count() != ds.Count());/*/
                        classes.General.UpdateLayout(this.searchList, true);
                    });
                }

                progress.IsVisible = false;
            }
            catch (TargetInvocationException ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Lütfen yeniden deneyin.");
                //MessageBox.Show(ex.Message);
                progress.IsVisible = false;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Bir hata oluştu : s0x002");
                //General.SendBugReport(e.Result, "s0x002", ex.Message, ex.Source, ex.HelpLink);
                progress.IsVisible = false;
            }
        }

        private void favori_Click(object sender, RoutedEventArgs e)
        {
            SearchItem drv = (sender as MenuItem).DataContext as SearchItem;

            if (drv != null)
            {
                classes.General.WriteFavori("fav_topic.txt", classes.General.ReadFavori("fav_topic.txt") + drv.sForumID + "|" + drv.sTopicID + "|" + drv.sTopicTitle + "|" + drv.sPostAuthorInfo + "|" + drv.sAuthorAvatar + "#");
                MessageBox.Show("Favorilere başarıyla eklendi.");
            }
        }

        private void search_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                DoSearch();
        }

        private void searchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchItem drv = (SearchItem)searchList.SelectedItem;

            if (drv != null)
            {
                NavigationService.Navigate(new Uri("/forum/ForumPost.xaml?sforum_id=0&topic_id=" + drv.sTopicID + "&post_id=" + drv.sPostID + "&gbp", UriKind.Relative));
            }

            searchList.SelectedIndex = -1;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded == 0)
            {
                isLoaded = 1;
                search.Focus();
            }
        }

        /*/private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (General.CheckLoggedIn() == "Fail")
                return;

            SearchItem drv = (sender as Grid).DataContext as SearchItem;

            if (drv != null)
            {
                NavigationService.Navigate(new Uri("/Profile.xaml?user_name=" + drv.sPostAuthor, UriKind.Relative));
            }
        }/*/
    }
}
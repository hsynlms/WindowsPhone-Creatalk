using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Creatalk.post
{
    public partial class PostLikes : PhoneApplicationPage
    {
        short isLoaded = 0;

        public PostLikes()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded == 0)
            {
                isLoaded = 1;

                if (classes.General.post_likes != "")
                {
                    for (int i = 0; i < classes.General.post_likes.Split('#').Count() - 1; i++)
                    {
                        postliker.Items.Add(classes.General.post_likes.Split('#')[i]);
                    }
                }
            }
        }

        private void postliker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/Profile.xaml?user_name=" + postliker.SelectedItem.ToString().Replace("@", ""), UriKind.Relative));
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Kullanıcı profiline gidilemiyor!");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace Creatalk
{
    public class ColorList
    {
        public string cRenk { get; set; }
        public string cText { get; set; }
        public string Quote { get; set; }
    }

    public partial class Settings : PhoneApplicationPage
    {
        ObservableCollection<ColorList> ds = new ObservableCollection<ColorList>();

        short isLoaded = 0;

        public Settings()
        {
            InitializeComponent();

            //mysign.Text = App.Session_LoadSession("OwnSign");
        }

        private void showimage_Click(object sender, RoutedEventArgs e)
        {
            App.SaveSettings("aImage", Convert.ToInt16(showimage.IsChecked).ToString());
        }

        private void cpass_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChangeInfo.xaml", UriKind.Relative));
        }

        private void cmail_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChangeInfo.xaml?email", UriKind.Relative));
        }

        private void youColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                ColorList drv = (ColorList)youColor.SelectedItem;

                if (drv != null)
                {
                    App.SaveSettings("youC1", drv.cRenk);
                    App.SaveSettings("youC2", drv.Quote);
                }
            }
        }

        private void heColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                ColorList drv = (ColorList)heColor.SelectedItem;

                if (drv != null)
                {
                    App.SaveSettings("heC1", drv.cRenk);
                    App.SaveSettings("heC2", drv.Quote);
                }
            }
        }

        private void postColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                ColorList drv = (ColorList)postColor.SelectedItem;

                if (drv != null)
                {
                    App.SaveSettings("postC1", drv.cRenk);
                    App.SaveSettings("postC2", drv.Quote);
                }
            }
        }

        private void postColorOthers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                ColorList drv = (ColorList)postColorOthers.SelectedItem;

                if (drv != null)
                {
                    App.SaveSettings("postOC1", drv.cRenk);
                    App.SaveSettings("postOC2", drv.Quote);
                }
            }
        }

        private void menuTiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                ColorList drv = (ColorList)menuTiles.SelectedItem;

                if (drv != null)
                {
                    App.SaveSettings("menuC1", drv.cRenk);
                    //App.SaveSettings("menuC2", drv.Quote);
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded == 0)
            {
                autoRefresh.IsChecked = (App.Session_LoadSession("aRefresh") == "0") ? false : true;
                notifyme.IsChecked = (App.Session_LoadSession("aNotify") == "0") ? false : true;
                showimage.IsChecked = (App.Session_LoadSession("aImage") == "0") ? false : true;

                fsize.Value = (double)classes.General.fontSize;

                for (int i = 8; i < 21; i++)
                {
                    sbMesaj.Items.Add(i);
                    sbChat.Items.Add(i);
                    sbTopic.Items.Add(i);
                    sbPost.Items.Add(i);
                    sbParticipate.Items.Add(i);
                    sbNew.Items.Add(i);
                    sbSearch.Items.Add(i);

                    if (classes.General.sayfabasiMesaj == i)
                        sbMesaj.SelectedItem = sbMesaj.Items[i - 8];

                    if (classes.General.sayfabasiChat == i)
                        sbChat.SelectedItem = sbChat.Items[i - 8];

                    if (classes.General.sayfabasiTopic == i)
                        sbTopic.SelectedItem = sbTopic.Items[i - 8];

                    if (classes.General.sayfabasiPost == i)
                        sbPost.SelectedItem = sbPost.Items[i - 8];

                    if (classes.General.sayfabasiParticipated == i)
                        sbParticipate.SelectedItem = sbParticipate.Items[i - 8];

                    if (classes.General.sayfabasiRecent == i)
                        sbNew.SelectedItem = sbNew.Items[i - 8];

                    if (classes.General.sayfabasiSearch == i)
                        sbSearch.SelectedItem = sbSearch.Items[i - 8];
                }

                ds.Add(new ColorList() { cText = "Creatalk Mavisi", cRenk = Color.FromArgb(255, 41, 128, 185).ToString(), Quote = Color.FromArgb(255, 33, 102, 148).ToString() });
                ds.Add(new ColorList() { cText = "Creatalk Turuncusu", cRenk = Color.FromArgb(255, 244, 174, 26).ToString(), Quote = Color.FromArgb(255, 209, 149, 22).ToString() });
                ds.Add(new ColorList() { cText = "Menü Tile", cRenk = Color.FromArgb(255, 240, 197, 20).ToString(), Quote = Color.FromArgb(255, 174, 143, 15).ToString() });
                ds.Add(new ColorList() { cText = "Kırmızı", cRenk = Color.FromArgb(255, 235, 55, 55).ToString(), Quote = Color.FromArgb(255, 156, 36, 36).ToString() });
                ds.Add(new ColorList() { cText = "Pembe", cRenk = Color.FromArgb(255, 237, 51, 104).ToString(), Quote = Color.FromArgb(255, 168, 34, 72).ToString() });
                ds.Add(new ColorList() { cText = "Fesrengi", cRenk = Color.FromArgb(255, 220, 20, 60).ToString(), Quote = Color.FromArgb(255, 165, 15, 44).ToString() });
                ds.Add(new ColorList() { cText = "Mor", cRenk = Color.FromArgb(255, 156, 51, 237).ToString(), Quote = Color.FromArgb(255, 103, 34, 156).ToString() });
                ds.Add(new ColorList() { cText = "Mavi", cRenk = Color.FromArgb(255, 51, 112, 237).ToString(), Quote = Color.FromArgb(255, 35, 79, 168).ToString() });
                ds.Add(new ColorList() { cText = "Yeşil", cRenk = Color.FromArgb(255, 131, 192, 35).ToString(), Quote = Color.FromArgb(255, 106, 153, 31).ToString() });
                ds.Add(new ColorList() { cText = "Turuncu", cRenk = Color.FromArgb(255, 231, 192, 54).ToString(), Quote = Color.FromArgb(255, 181, 150, 41).ToString() });

                youColor.ItemsSource = ds;
                heColor.ItemsSource = ds;
                postColor.ItemsSource = ds;
                postColorOthers.ItemsSource = ds;
                menuTiles.ItemsSource = ds;

                for (int i = 0; i < ds.Count(); i++)
                {
                    if (ds.ElementAt(i).cRenk == App.Session_LoadSession("youC1"))
                        youColor.SelectedIndex = i;

                    if (ds.ElementAt(i).cRenk == App.Session_LoadSession("heC1"))
                        heColor.SelectedIndex = i;

                    if (ds.ElementAt(i).cRenk == App.Session_LoadSession("postC1"))
                        postColor.SelectedIndex = i;

                    if (ds.ElementAt(i).cRenk == App.Session_LoadSession("postOC1"))
                        postColorOthers.SelectedIndex = i;

                    if (ds.ElementAt(i).cRenk == App.Session_LoadSession("menuC1"))
                        menuTiles.SelectedIndex = i;
                }

                isLoaded = 1;
            }
        }

        private void toggle_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch button = sender as ToggleSwitch;
            button.Content = "Açık";
        }

        private void toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch button = sender as ToggleSwitch;
            button.Content = "Kapalı";
        }

        /*private void mysign_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.SaveSettings("OwnSign", mysign.Text);
        }*/

        private void fsize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("fSize", e.NewValue.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbMesaj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbMesaj", sbMesaj.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbChat", sbChat.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbTopic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbTopic", sbTopic.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbPost", sbPost.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbParticipate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbParticipate", sbParticipate.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbNew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbNew", sbNew.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void sbSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == 1)
            {
                App.SaveSettings("sbSearch", sbSearch.SelectedItem.ToString());
                classes.General.GetVariables();
            }
        }

        private void delFav_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tüm favorileri silmek istediğinize emin misiniz?", "Onaylama", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                classes.General.WriteFavori("fav_topic.txt", "");
                classes.General.WriteFavori("fav_forum.txt", "");
            }
        }

        private void notification_Click(object sender, RoutedEventArgs e)
        {
            App.SaveSettings("aNotify", Convert.ToInt16(notifyme.IsChecked).ToString());
        }

        private void autoRefresh_Click(object sender, RoutedEventArgs e)
        {
            App.SaveSettings("aRefresh", Convert.ToInt16(autoRefresh.IsChecked).ToString());
        }
    }
}
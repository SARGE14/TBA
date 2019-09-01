using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Threading;


using System.Diagnostics;

using System.Windows;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace TBA
{
    public sealed partial class MainPage : Page
    {
       
        public MainPage()
        {
            this.InitializeComponent();
         //   contentFrame.Navigate(typeof(HomePage));

        }

        private void navMenu_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                  contentFrame.Navigate(typeof(SettingsPage));
                //   splitView.IsPaneOpen = true;
            }
            else
            {

                if (args.InvokedItem is string ItemContent)
                {
                    switch (ItemContent)
                    {
                        case "Главная":
                            contentFrame.Navigate(typeof(HomePage));
                            break;

                        case "Портфель":
                            contentFrame.Navigate(typeof(PortfolioPage));
                            break;

                    }
                }
               /* TextBlock ItemContent = args.InvokedItem as TextBlock;
                if (ItemContent != null)
                { Contains.Tag
                    switch (ItemContent.Tag.ToString())
                    {
                        case "HomePage":
                            contentFrame.Navigate(typeof(HomePage));
                            break;

                        case "PortfolioPage":
                            contentFrame.Navigate(typeof(PortfolioPage));
                            break;
                    }
                    
                }*/

            }
        }
        private void NavMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in navMenu.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "HomePage")
                {
                    navMenu.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(HomePage));
        }
    }
}

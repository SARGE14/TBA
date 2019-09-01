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
        DispatcherTimer timer;
        public int timereload;
        private const String APP_ID = "Microsoft.Samples.DesktopToastsSample";
        public string priceBuy;
        public string priceSell;
        public class RootObject
        {
            public double last { get; set; }
            public double high { get; set; }
            public double low { get; set; }
            public double volume { get; set; }
            public double vwap { get; set; }
            public double max_bid { get; set; }
            public double min_ask { get; set; }
            public double best_bid { get; set; }
            public double best_ask { get; set; }
            //public string ask { get; set; }
            //public string bid { get; set; }
            public double ask { get; set; }
            public double bid { get; set; }
            public Ticker ticker { get; set; }
        }
        public class Ticker
        {
            public double high { get; set; }
            public double low { get; set; }
            public double avg { get; set; }
            public double vol { get; set; }
            public int vol_cur { get; set; }
            public double last { get; set; }
            public double buy { get; set; }
            public double sell { get; set; }
            public int updated { get; set; }
            public int server_time { get; set; }
        }
        public MainPage()
        {
            this.InitializeComponent();
           // ApplicationView.PreferredLaunchViewSize = new Size { Height = 300, Width = 770 };
           // ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Price_OldGet();
            Price_Get();
        }
        private void Timer_Tick(object sender, object e)
        {
            Price_Get();
        }

        private void Price_OldGet()
        {
            timereload = Convert.ToInt32(timeload.Text);
        }
        private void Price_Get()
        {
            /*Livecoin*/
            string apipricebtc = new WebClient().DownloadString(@"https://api.livecoin.net/exchange/ticker?currencyPair=BTC/USD");
            RootObject btcp = JsonConvert.DeserializeObject<RootObject>(apipricebtc);
            pricebtc.Text = btcp.best_ask.ToString("F");

            string apipriceeth = new WebClient().DownloadString(@"https://api.livecoin.net/exchange/ticker?currencyPair=ETH/USD");
            RootObject ethp = JsonConvert.DeserializeObject<RootObject>(apipriceeth);
            priceeth.Text = ethp.best_ask.ToString("F");

            string apibtc = new WebClient().DownloadString(@"https://api.livecoin.net/exchange/ticker?currencyPair=PLBT/BTC");
            RootObject btc = JsonConvert.DeserializeObject<RootObject>(apibtc);
            pricebtcsell.Text = btc.best_ask.ToString("F8");
            pricebtcbuy.Text = btc.best_bid.ToString("F8");

            string apieth = new WebClient().DownloadString(@"https://api.livecoin.net/exchange/ticker?currencyPair=PLBT/ETH");
            RootObject eth = JsonConvert.DeserializeObject<RootObject>(apieth);
            priceethsell.Text = eth.best_ask.ToString("F8");
            priceethbuy.Text = eth.best_bid.ToString("F8");

            string apiusd = new WebClient().DownloadString(@"https://api.livecoin.net/exchange/ticker?currencyPair=PLBT/USD");
            RootObject usd = JsonConvert.DeserializeObject<RootObject>(apiusd);
            priceusdsell.Text = usd.best_ask.ToString("F");
            priceusdbuy.Text = usd.best_bid.ToString("F");

                /*HitBTC*/
            string apibtchit = new WebClient().DownloadString(@"https://api.hitbtc.com/api/2/public/ticker/PLBTBTC");
            RootObject btchit= JsonConvert.DeserializeObject<RootObject>(apibtchit);
            // double doublebtchitask = double.Parse(btchit.ask.Replace(".", ","));
            //  double doublebtchitbit = double.Parse(btchit.bid.Replace(".", ","));
            //pricebtcsell_hitbtc.Text = doublebtchitask.ToString("F6");
            //pricebtcbuy_hitbtc.Text = doublebtchitbit.ToString("F6");
            pricebtcsell_hitbtc.Text = btchit.ask.ToString("F8");
            pricebtcbuy_hitbtc.Text = btchit.bid.ToString("F8");

                 /*YoBit*/
            string apibtcyob = new WebClient().DownloadString(@"https://yobit.net/api/2/plbt_btc/ticker");
            RootObject btcyob = JsonConvert.DeserializeObject<RootObject>(apibtcyob);
            pricebtcsell_yobit.Text = btcyob.ticker.sell.ToString("F8");
            pricebtcbuy_yobit.Text = btcyob.ticker.buy.ToString("F8");
            Price_Alert();
            Price_Count();
        }
        private void Price_Count()
        { 
            double pbs = double.Parse(pricebtcsell.Text) * double.Parse(pricebtc.Text);
            labelbtcusdsell.Text = pbs.ToString("F") + " USD";
		    double pes = double.Parse(priceethsell.Text) * double.Parse(priceeth.Text);
            labelethusdsell.Text = pes.ToString("F") + " USD";
		    double pbb = double.Parse(pricebtcbuy.Text) * double.Parse(pricebtc.Text);
            labelbtcusdbuy.Text = pbb.ToString("F") + " USD";
		    double peb = double.Parse(priceethbuy.Text) * double.Parse(priceeth.Text);
            labelethusdbuy.Text = peb.ToString("F") + " USD";
            /*HitBTC*/
            double pbbhit = double.Parse(pricebtcbuy_hitbtc.Text) * double.Parse(pricebtc.Text);
            labelbtcusdbuy_hitbtc.Text = pbbhit.ToString("F") + " USD";
            double pbshit = double.Parse(pricebtcsell_hitbtc.Text) * double.Parse(pricebtc.Text);
            labelbtcusdsell_hitbtc.Text = pbshit.ToString("F") + " USD";
            /*YoBit*/
            double pbbyob = double.Parse(pricebtcbuy_yobit.Text) * double.Parse(pricebtc.Text);
            labelbtcusdbuy_yobit.Text = pbbyob.ToString("F") + " USD";
            double pbsyob= double.Parse(pricebtcsell_yobit.Text) * double.Parse(pricebtc.Text);
            labelbtcusdsell_yobit.Text = pbsyob.ToString("F") + " USD";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Price_OldGet();
            Price_Get();
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Price_Count();
            Price_Alert();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(10);
            timer.Interval = new TimeSpan(0, 0, timereload);
            timer.Tick += new EventHandler<object>(Timer_Tick);
            timer.Start();
        }
        private void Price_Alert()
        {

            double priceAlertBuyLive = double.Parse(pricebtcbuy.Text);
            double priceAlertSellHit = double.Parse(pricebtcsell_hitbtc.Text);
            double priceAlertSellLive = double.Parse(pricebtcsell.Text);
            double priceAlertBuyHit = double.Parse(pricebtcbuy_hitbtc.Text);
            margintext.Text = "";

            if (priceAlertBuyLive > priceAlertSellHit)
            {
                double result = (priceAlertBuyLive / priceAlertSellHit - 1d) * 100d;
                margintext.Text = "На HitBTC дешевле на " + result.ToString("F") + "%";
                priceSell = "На HitBTC цена продажи " + pricebtcsell_hitbtc.Text;
                priceBuy = "На Livecoin цена покупки " + pricebtcbuy.Text;
                Toast_Price();
            }
            if (priceAlertBuyHit > priceAlertSellLive)
            {
                double result = (priceAlertBuyHit / priceAlertSellLive - 1d) * 100d;
                margintext.Text = "На Livecoin дешевле на " + result.ToString("F") + "%";
                priceSell = "На Livecoin цена продажи " + pricebtcsell.Text;
                priceBuy = "На HitBTC цена покупки " + pricebtcbuy_hitbtc.Text;
                Toast_Price();
            }
            
        }
        private void Toast_Price()
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                if (i == 0)
                {
                    stringElements[i].AppendChild(toastXml.CreateTextNode(margintext.Text));
                }
                else
                    if (i == 1)
                    {
                    stringElements[i].AppendChild(toastXml.CreateTextNode(priceSell));
                    }
                else
                    if (i == 2)
                    stringElements[i].AppendChild(toastXml.CreateTextNode(priceBuy));
            }

            // Specify the absolute path to an image
            String imagePath = "file:///" + Path.GetFullPath("livecoin.png");
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }

        private void ToastActivated(ToastNotification sender, object e)
        {
 
            /*   Dispatcher.(() =>
            {
                Activate();
                Output.Text = "The user activated the toast.";
            });*/
        }

        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
         /*     String outputText = "";
               switch (e.Reason)
               {
                   case ToastDismissalReason.ApplicationHidden:
                       outputText = "The app hid the toast using ToastNotifier.Hide";
                       break;
                   case ToastDismissalReason.UserCanceled:
                       outputText = "The user dismissed the toast";
                       break;
                   case ToastDismissalReason.TimedOut:
                       outputText = "The toast has timed out";
                       break;
               }

            this.Dispatcher.BeginInvoke ((Action) =>
            {
                Output.Text = outputText;
            });*/

        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
          /*  Dispatcher.Invoke(() =>
            {
                Output.Text = "The toast encountered an error.";
            });*/
        }

       
    }
}

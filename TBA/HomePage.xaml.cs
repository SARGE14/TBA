﻿using System;
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
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace TBA
{
    public sealed partial class HomePage : Page
    {
        readonly WebClient webClient;
        public string url;
        public string exchange;
        public string imageName;
        public string currencyPair;
        public string priceBidAsk;
        public bool errorLivecoin;
        public bool errorHitBtc;
        public bool errorYobit;
        public bool firstUpDown;
        public double oldPrice;
        public double newPrice;
        public double percentUpDown = 1.05;
        public int timerload = 30;
        public string toastMesLine1;
        public string toastMesLine2;
        RootObject json;
        public class RootObject
        {
            /*  public double last { get; set; }
              public double high { get; set; }
              public double low { get; set; }
              public double volume { get; set; }
              public double vwap { get; set; }
              public double max_bid { get; set; }
              public double min_ask { get; set; }*/
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
            /*   public double high { get; set; }
               public double low { get; set; }
               public double avg { get; set; }
               public double vol { get; set; }
               public double vol_cur { get; set; }
               public double last { get; set; }*/
            public double Buy { get; set; }
            public double Sell { get; set; }
            // public int updated { get; set; }
            //  public int server_time { get; set; }
        }
        public HomePage()
        {
            this.InitializeComponent();
            webClient = new WebClient();
            messageText.Text = "";
            firstUpDown = true;
            errorText.Text = "Соединение...";
            // ApplicationView.PreferredLaunchViewSize = new Size { Height = 300, Width = 770 };
            // ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PriceGet();
            AdventureTimer().Start();
            errorText.Text = "";
        }
        public DispatcherTimer AdventureTimer()
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, timerload) };
            timer.Tick += new EventHandler<object>(TimerTick);

            return timer;
        }
        private void WebTest()
        {
            using (WebClient wc = webClient)
            {
                string webPage = null;

                try
                {
                    webPage = wc.DownloadString(@url);
                    if (exchange == "Livecoin")
                    {
                        errorLivecoin = false;
                        errorText.Text = "";
                    }
                    if (exchange == "HitBTC")
                    {
                        errorHitBtc = false;
                        errorText.Text = "";

                    }
                    if (exchange == "YoBit")
                    {
                        errorYobit = false;
                        errorText.Text = "";
                    }
                    json = null;
                    json = JsonConvert.DeserializeObject<RootObject>(webPage);


                }
                catch (WebException)
                {
                    if (exchange == "Livecoin")
                    {
                        errorLivecoin = true;
                        errorText.Text = "API Livecoin недоступно";
                    }
                    if (exchange == "HitBTC")
                    {
                        errorHitBtc = true;
                        errorText.Text = "API HitBTC недоступно";
                    }
                    if (exchange == "YoBit")
                    {
                        errorYobit = true;
                        errorText.Text = "API YoBit недоступно";
                    }

                    url = null;
                }
            }
        }
        private void TimerTick(object sender, object e)
        {
            PriceGet();
        }

        private void PriceOldGet()
        {
            AdventureTimer().Stop();
            timerload = Convert.ToInt32(timeUpdate.Text);
            AdventureTimer().Start();

        }
        private void PriceGet()
        {

            string urlExchange;
            messageText.Text = "";
            /*Livecoin*/
            exchange = "Livecoin";
            urlExchange = "https://api.livecoin.net/exchange/ticker?currencyPair=";
            currencyPair = "BTC/USD";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorLivecoin != true)
            {
                oldPrice = double.Parse(priceBtc.Text);
                newPrice = json.best_ask;

                PriceUpDown();
                priceBtc.Text = newPrice.ToString("F");


                currencyPair = "ETH/USD";
                url = urlExchange + currencyPair;
                WebTest();
                oldPrice = double.Parse(priceEth.Text);
                newPrice = json.best_ask;

                PriceUpDown();
                priceEth.Text = newPrice.ToString("F");

                currencyPair = "PLBT/BTC";
                url = urlExchange + currencyPair;
                WebTest();
                oldPrice = double.Parse(priceBtcSell.Text);
                newPrice = json.best_ask;

                priceBidAsk = "продажи";
                PriceUpDown();
                priceBtcSell.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceBtcBuy.Text);
                newPrice = json.best_bid;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceBtcBuy.Text = newPrice.ToString("F8");

                currencyPair = "PLBT/ETH";
                url = urlExchange + currencyPair;
                WebTest();
                oldPrice = double.Parse(priceEthSell.Text);
                newPrice = json.best_ask;

                priceBidAsk = "продажи";
                PriceUpDown();
                priceEthSell.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceEthBuy.Text);
                newPrice = json.best_bid;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceEthBuy.Text = newPrice.ToString("F8");

                currencyPair = "PLBT/USD";
                url = urlExchange + currencyPair;
                WebTest();
                oldPrice = double.Parse(priceUsdSell.Text);
                newPrice = json.best_ask;

                priceBidAsk = "продажи";
                PriceUpDown();
                priceUsdSell.Text = newPrice.ToString("F");
                oldPrice = double.Parse(priceUsdBuy.Text);
                newPrice = json.best_bid;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceUsdBuy.Text = newPrice.ToString("F");
            }
            /*HitBTC*/
            exchange = "HitBTC";
            urlExchange = "https://api.hitbtc.com/api/2/public/ticker/";
            currencyPair = "PLBTBTC";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorHitBtc != true)
            {

                oldPrice = double.Parse(priceBtcSell_hitbtc.Text);
                newPrice = json.ask;

                priceBidAsk = "продажи";
                PriceUpDown();
                priceBtcSell_hitbtc.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceBtcBuy_hitbtc.Text);
                newPrice = json.bid;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceBtcBuy_hitbtc.Text = newPrice.ToString("F8");
            }

            /*YoBit*/
            exchange = "YoBit";
            urlExchange = "https://yobit.net/api/2/";
            currencyPair = "plbt_btc/ticker";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorYobit != true)
            {
                oldPrice = double.Parse(priceBtcSell_yobit.Text);
                newPrice = json.ticker.Sell;

                priceBidAsk = "продажи";
                PriceUpDown();
                priceBtcSell_yobit.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceBtcBuy_yobit.Text);
                newPrice = json.ticker.Buy;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceBtcBuy_yobit.Text = newPrice.ToString("F8");
            }
            PriceAlert();
            PriceCount();
            firstUpDown = false;
        }
        private void PriceUpDown()
        {
            double calcPriceUp;
            double calcPriceDown;
            calcPriceUp = newPrice / oldPrice;
            calcPriceDown = oldPrice / newPrice;
            if (!firstUpDown)
            {
                if (percentUpDown < calcPriceUp)
                {
                    calcPriceUp = (calcPriceUp - 1d) * 100d;
                    messageText.Text = "Цена " + priceBidAsk + " " + currencyPair + " на " + exchange + " вверх на " + calcPriceUp.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + newPrice.ToString("F8");
                    imageName = "Resources/greenup.png";
                    ToastPrice();
                    /// цена вверх
                }
                if (percentUpDown < calcPriceDown)
                {
                    /// цена вниз
                    calcPriceDown = (calcPriceDown - 1d) * 100d;
                    messageText.Text = "Цена " + priceBidAsk + " " + currencyPair + " на " + exchange + " вниз на " + calcPriceDown.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + newPrice.ToString("F8");
                    imageName = "Resources/reddown.png";
                    ToastPrice();
                }
            }
        }
        private void PriceCount()
        {
            /*Livecoin*/
            double pbs = double.Parse(priceBtcSell.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell.Text = pbs.ToString("F") + " USD";
            double pes = double.Parse(priceEthSell.Text) * double.Parse(priceEth.Text);
            labelEthUsdSell.Text = pes.ToString("F") + " USD";
            double pbb = double.Parse(priceBtcBuy.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy.Text = pbb.ToString("F") + " USD";
            double peb = double.Parse(priceEthBuy.Text) * double.Parse(priceEth.Text);
            labelEthUsdBuy.Text = peb.ToString("F") + " USD";
            /*HitBTC*/
            double pbbhit = double.Parse(priceBtcBuy_hitbtc.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy_hitbtc.Text = pbbhit.ToString("F") + " USD";
            double pbshit = double.Parse(priceBtcSell_hitbtc.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell_hitbtc.Text = pbshit.ToString("F") + " USD";
            /*YoBit*/
            double pbbyob = double.Parse(priceBtcBuy_yobit.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy_yobit.Text = pbbyob.ToString("F") + " USD";
            double pbsyob = double.Parse(priceBtcSell_yobit.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell_yobit.Text = pbsyob.ToString("F") + " USD";
        }
        private void PriceAlert()
        {
            double priceAlertBuyLive = double.Parse(priceBtcBuy.Text);
            double priceAlertSellHit = double.Parse(priceBtcSell_hitbtc.Text);
            double priceAlertSellLive = double.Parse(priceBtcSell.Text);
            double priceAlertBuyHit = double.Parse(priceBtcBuy_hitbtc.Text);
            if (priceAlertBuyLive > priceAlertSellHit)
            {
                double result = (priceAlertBuyLive / priceAlertSellHit - 1d) * 100d;
                messageText.Text = "На HitBTC дешевле на " + result.ToString("F") + "%";
                toastMesLine1 = "На HitBTC цена продажи " + priceBtcSell_hitbtc.Text;
                toastMesLine2 = "На Livecoin цена покупки " + priceBtcBuy.Text;
                if (toastCheck.IsChecked == true && result > 3d)
                {
                    toastCheck.IsChecked = false;
                }
                imageName = "Resources/hitbtc.png";
                ToastPrice();
            }
            if (priceAlertBuyHit > priceAlertSellLive)
            {
                double result = (priceAlertBuyHit / priceAlertSellLive - 1d) * 100d;
                messageText.Text = "На Livecoin дешевле на " + result.ToString("F") + "%";
                toastMesLine1 = "На Livecoin цена продажи " + priceBtcSell.Text;
                toastMesLine2 = "На HitBTC цена покупки " + priceBtcBuy_hitbtc.Text;
                if (toastCheck.IsChecked == true && result > 3d)
                {
                    toastCheck.IsChecked = false;
                }
                imageName = "Resources/livecoin.png";
                ToastPrice();
            }
        }
        private void ToastPrice()
        {
            if (toastCheck.IsChecked == false)
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

                for (int i = 0; i < stringElements.Length; i++)
                {
                    if (i == 0)
                    {
                        stringElements[i].AppendChild(toastXml.CreateTextNode(messageText.Text));
                    }
                    else
                        if (i == 1)
                    {
                        stringElements[i].AppendChild(toastXml.CreateTextNode(toastMesLine1));
                    }
                    else
                        if (i == 2)
                        stringElements[i].AppendChild(toastXml.CreateTextNode(toastMesLine2));
                }
                String imagePath = "file:///" + Path.GetFullPath(imageName);
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

                XmlElement audio = toastXml.CreateElement("audio");
                audio.SetAttribute("src", "ms-winsoundevent:Notification.Looping.Call2");
                toastXml.DocumentElement.AppendChild(audio);

                XmlElement actions = toastXml.CreateElement("actions");
                toastXml.DocumentElement.AppendChild(actions);

                XmlElement action = toastXml.CreateElement("action");
                actions.AppendChild(action);
                action.SetAttribute("content", "Показать детали");
                action.SetAttribute("arguments", "viewdetails");

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            PriceOldGet();
            PriceGet();
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            PriceCount();
            PriceAlert();
        }
    }
}

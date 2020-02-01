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
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace TBA
{
    public sealed partial class HomePage : Page
    {
        readonly WebClient webClient;
        public string url;
        public string coin;
        public string exchange;
        public string imageName;
        public string currencyPair;
        public string priceBidAsk;
        public string errorExh;
        public string alertPair;
        public bool firstUpDown;
        public bool secondUp;
        public double oldPrice;
        public double newPrice;
        public double percentUpDown = 1.05f;
        public int timerload = 15;
        public string toastMesLine1;
        public string toastMesLine2;
        public float priceAlertBuyLive;
        public float priceAlertSellHit;
        public float priceAlertSellLive;
        public float priceAlertBuyHit;
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
                  
                        errorExh = "";
                        errorText.Text = "";
                        json = null;
                    try
                    {
                        json = JsonConvert.DeserializeObject<RootObject>(webPage);
                    }
                    catch
                    {
                        errorText.Text = "API "+ exchange + " недоступно";
                        url = null;
                        json = null;
                    }
                }
                catch (WebException)
                {
                    errorExh = exchange;
                    errorText.Text = "API " + exchange + " недоступно";
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
            timerload = int.Parse(timeUpdate.Text);
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
            if (errorExh != "Livecoin" && json != null)
            {
                if (!secondUp)
                {
                    currencyPair = "BTC/USD";
                    url = urlExchange + currencyPair;
                    WebTest();

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
                }

                currencyPair = "PLBT/BTC";
                url = urlExchange + currencyPair;
                WebTest();
                
                priceBidAsk = "продажи";
                alertPair = currencyPair;
                oldPrice = double.Parse(priceBtcSell.Text);
                newPrice = json.best_ask;
                PriceUpDown();
                priceBtcSell.Text = newPrice.ToString("F8");
                
                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy.Text);
                newPrice = json.best_bid;
                PriceUpDown();
                priceBtcBuy.Text = newPrice.ToString("F8");

                if (!secondUp)
                {
                    currencyPair = "PLBT/ETH";
                    url = urlExchange + currencyPair;
                    WebTest();

                    priceBidAsk = "продажи";
                    alertPair = currencyPair;
                    oldPrice = double.Parse(priceEthSell.Text);
                    newPrice = json.best_ask;
                    PriceUpDown();
                    priceEthSell.Text = newPrice.ToString("F8");

                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceEthBuy.Text);
                    newPrice = json.best_bid;
                    PriceUpDown();
                    priceEthBuy.Text = newPrice.ToString("F8");

                    currencyPair = "PLBT/USD";
                    url = urlExchange + currencyPair;
                    WebTest();

                    priceBidAsk = "продажи";
                    alertPair = currencyPair;
                    oldPrice = double.Parse(priceUsdSell.Text);
                    newPrice = json.best_ask;
                    PriceUpDown();
                    priceUsdSell.Text = newPrice.ToString("F");


                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceUsdBuy.Text);
                    newPrice = json.best_bid;
                    PriceUpDown();
                    priceUsdBuy.Text = newPrice.ToString("F");
                }

                currencyPair = "EMC/BTC";
                url = urlExchange + currencyPair;
                WebTest();

                priceBidAsk = "продажи";
                alertPair = currencyPair;
                oldPrice = double.Parse(priceBtcSell_Emc.Text);
                newPrice = json.best_ask;
                PriceUpDown();
                priceBtcSell_Emc.Text = newPrice.ToString("F8");

                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy_Emc.Text);
                newPrice = json.best_bid;
                PriceUpDown();
                priceBtcBuy_Emc.Text = newPrice.ToString("F8");
            }
            /*HitBTC*/
            exchange = "HitBTC";
            urlExchange = "https://api.hitbtc.com/api/2/public/ticker/";
            currencyPair = "PLBTBTC";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "HitBTC" && json != null)
            {
                alertPair = "PLBT/BTC";
                priceBidAsk = "продажи";
                oldPrice = double.Parse(priceBtcSell_hitbtc.Text);
                newPrice = json.ask;
                PriceUpDown();
                priceBtcSell_hitbtc.Text = newPrice.ToString("F8");
               
                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy_hitbtc.Text);
                newPrice = json.bid;
                PriceUpDown();
                priceBtcBuy_hitbtc.Text = newPrice.ToString("F8");

                /*HitBTC ETH*/
                if (!secondUp)
                {
                    currencyPair = "PLBTETH";
                    url = urlExchange + currencyPair;
                    WebTest();

                    alertPair = "PLBT/ETH";
                    priceBidAsk = "продажи";
                    oldPrice = double.Parse(priceEthSell_hitbtc.Text);
                    newPrice = json.ask;
                    PriceUpDown();
                    priceEthSell_hitbtc.Text = newPrice.ToString("F8");

                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceEthBuy_hitbtc.Text);
                    newPrice = json.bid;
                    PriceUpDown();
                    priceEthBuy_hitbtc.Text = newPrice.ToString("F8");
                }

                /*HitBTC EMC*/
                currencyPair = "EMCBTC";
                url = urlExchange + currencyPair;
                WebTest();

                priceBidAsk = "продажи";
                alertPair = "EMC/BTC";
                oldPrice = double.Parse(priceBtcSell_hitbtc_Emc.Text);
                newPrice = json.ask;
                PriceUpDown();
                priceBtcSell_hitbtc_Emc.Text = newPrice.ToString("F8");

                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy_hitbtc_Emc.Text);
                newPrice = json.bid;
                PriceUpDown();
                priceBtcBuy_hitbtc_Emc.Text = newPrice.ToString("F8");
            }

            /*YoBit*/
            exchange = "YoBit";
            urlExchange = "https://yobit.net/api/2/";
            currencyPair = "plbt_btc/ticker";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "YoBit" && json != null)
            {
                oldPrice = double.Parse(priceBtcSell_yobit.Text);
                newPrice = json.ticker.Sell;

                alertPair = "PLBT/BTC";
                priceBidAsk = "продажи";
                PriceUpDown();
                priceBtcSell_yobit.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceBtcBuy_yobit.Text);
                newPrice = json.ticker.Buy;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceBtcBuy_yobit.Text = newPrice.ToString("F8");
                if (!secondUp)
                {
                    currencyPair = "plbt_eth/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    alertPair = "PLBT/ETH";
                    priceBidAsk = "продажи";
                    oldPrice = double.Parse(priceEthSell_yobit.Text);
                    newPrice = json.ticker.Sell;
                    PriceUpDown();
                    priceEthSell_yobit.Text = newPrice.ToString("F8");

                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceEthBuy_yobit.Text);
                    newPrice = json.ticker.Buy;
                    PriceUpDown();
                    priceEthBuy_yobit.Text = newPrice.ToString("F8");

                    currencyPair = "plbt_usd/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    alertPair = "PLBT/USD";
                    priceBidAsk = "продажи";
                    oldPrice = double.Parse(priceUsdSell_yobit.Text);
                    newPrice = json.ticker.Sell;
                    PriceUpDown();
                    priceUsdSell_yobit.Text = newPrice.ToString("F");


                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceUsdBuy_yobit.Text);
                    newPrice = json.ticker.Buy;
                    PriceUpDown();
                    priceUsdBuy_yobit.Text = newPrice.ToString("F");

                    currencyPair = "plbt_rur/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    alertPair = "PLBT/RUR";
                    priceBidAsk = "продажи";
                    oldPrice = double.Parse(priceRurSell_yobit.Text);
                    newPrice = json.ticker.Sell;
                    PriceUpDown();
                    priceRurSell_yobit.Text = newPrice.ToString("F");


                    priceBidAsk = "покупки";
                    oldPrice = double.Parse(priceRurBuy_yobit.Text);
                    newPrice = json.ticker.Buy;
                    PriceUpDown();
                    priceRurBuy_yobit.Text = newPrice.ToString("F");
                    secondUp = true;
                }
                else
                {
                    secondUp = false;
                }
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
                    /// цена вверх
                    calcPriceUp = (calcPriceUp - 1f) * 100f;
                    messageText.Text = "Цена " + priceBidAsk + " " + alertPair + " на " + exchange + " вверх на " + calcPriceUp.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + newPrice.ToString("F8");
                    imageName = "Resources/greenup.png";
                    ToastPrice();
                }
                if (percentUpDown < calcPriceDown)
                {
                    /// цена вниз
                    calcPriceDown = (calcPriceDown - 1f) * 100f;
                    messageText.Text = "Цена " + priceBidAsk + " " + alertPair + " на " + exchange + " вниз на " + calcPriceDown.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + newPrice.ToString("F8");
                    imageName = "Resources/reddown.png";
                    ToastPrice();
                }
            }
        }
        private void PriceCount()
        {
            double result;
            /*Livecoin*/
            result = double.Parse(priceBtcSell.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthSell.Text) * double.Parse(priceEth.Text);
            labelEthUsdSell.Text = result.ToString("F") + " USD";
            result = double.Parse(priceBtcBuy.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthBuy.Text) * double.Parse(priceEth.Text);
            labelEthUsdBuy.Text = result.ToString("F") + " USD";
            /*HitBTC*/
            result = double.Parse(priceBtcBuy_hitbtc.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy_hitbtc.Text = result.ToString("F") + " USD";
            result = double.Parse(priceBtcSell_hitbtc.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell_hitbtc.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthBuy_hitbtc.Text) * double.Parse(priceEth.Text);
            labelEthUsdBuy_hitbtc.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthSell_hitbtc.Text) * double.Parse(priceEth.Text);
            labelEthUsdSell_hitbtc.Text = result.ToString("F") + " USD";
            /*YoBit*/
            result = double.Parse(priceBtcBuy_yobit.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdBuy_yobit.Text = result.ToString("F") + " USD";
            result = double.Parse(priceBtcSell_yobit.Text) * double.Parse(priceBtc.Text);
            labelBtcUsdSell_yobit.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthBuy_yobit.Text) * double.Parse(priceEth.Text);
            labelEthUsdBuy_yobit.Text = result.ToString("F") + " USD";
            result = double.Parse(priceEthSell_yobit.Text) * double.Parse(priceEth.Text);
            labelEthUsdSell_yobit.Text = result.ToString("F") + " USD";
        }
        private void PriceAlert()
        {
            float result;
            coin = "PLBT";
            priceAlertBuyLive = float.Parse(priceBtcBuy.Text);
            priceAlertSellLive = float.Parse(priceBtcSell.Text);
            priceAlertBuyHit = float.Parse(priceBtcBuy_hitbtc.Text);
            priceAlertSellHit = float.Parse(priceBtcSell_hitbtc.Text);

            if (priceAlertBuyLive > priceAlertSellHit)
            {
                result = (priceAlertBuyLive / priceAlertSellHit - 1f) * 100f;
                if (toastCheck.IsChecked == true && result > 3f)
                    toastCheck.IsChecked = false;
                if (toastCheck.IsChecked == false)
                    PriceAlertsHit(result);
            }
            if (priceAlertBuyHit > priceAlertSellLive)
            {
                result = (priceAlertBuyHit / priceAlertSellLive - 1f) * 100f;
                if (toastCheck.IsChecked == true && result > 3f)
                    toastCheck.IsChecked = false;
                if (toastCheck.IsChecked == false)
                    PriceAlertsLive(result);
            }

            coin = "EMC";
            priceAlertBuyLive = float.Parse(priceBtcBuy_Emc.Text);
            priceAlertSellLive = float.Parse(priceBtcSell_Emc.Text);
            priceAlertBuyHit = float.Parse(priceBtcBuy_hitbtc_Emc.Text);
            priceAlertSellHit = float.Parse(priceBtcSell_hitbtc_Emc.Text);

            if (priceAlertBuyLive > priceAlertSellHit)
            {
                result = (priceAlertBuyLive / priceAlertSellHit - 1f) * 100f;
                if (toastCheck_EMC.IsChecked == true && result > 9f)
                    toastCheck_EMC.IsChecked = false;
                if (toastCheck_EMC.IsChecked == false)
                    PriceAlertsHit(result);
            }
            if (priceAlertBuyHit > priceAlertSellLive)
            {
                result = (priceAlertBuyHit / priceAlertSellLive - 1f) * 100f;
                if (toastCheck_EMC.IsChecked == true && result > 9f)
                    toastCheck_EMC.IsChecked = false;
                if (toastCheck_EMC.IsChecked == false)
                    PriceAlertsLive(result);
            }
        }
        private void PriceAlertsHit(double result)
        {
            messageText.Text = coin + " на HitBTC дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "На HitBTC цена продажи:   " + priceAlertSellHit.ToString("F8");
            toastMesLine2 = "На Livecoin цена покупки: " + priceAlertBuyLive.ToString("F8");
            imageName = "Resources/hitbtc.png";
            ToastPrice();
        }
        private void PriceAlertsLive(double result)
        {
            messageText.Text = coin + " на Livecoin дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "На Livecoin цена продажи: " + priceAlertSellLive.ToString("F8");
            toastMesLine2 = "На HitBTC цена покупки:   " + priceAlertBuyHit.ToString("F8");
            imageName = "Resources/livecoin.png";
            ToastPrice();
        }
        private void ToastPrice()
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

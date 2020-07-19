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
    public sealed partial class MainPage : Page
    {
        readonly WebClient webClient;
        public string url;
        public string coin;
        public string exchange;
        public string exchange2;
        public string imageName;
        public string currencyPair;
        public string priceBidAsk;
        public string errorExh;
        public string alertPair;
        public string textImage;
        public bool firstUpDown;
        public bool secondUp;
        public double oldPrice;
        public double newPrice;
        public double percentUpDown = 1.05f;
        public int timerload = 15;
        public string toastMesLine1;
        public string toastMesLine2;
        public float priceAlertBuyExc;
        public float priceAlertSellExc2;
        public float priceAlertSellExc;
        public float priceAlertBuyExc2;
        public TextBox textBox;
        public bool usd;
        RootObject json;
        public class RootObject
        {
            /*Binance*/
            public double bidPrice { get; set; }
            public double askPrice { get; set; }
            public double best_bid { get; set; }
            public double best_ask { get; set; }
            public double ask { get; set; }
            public double bid { get; set; }
            public Ticker ticker { get; set; }
        }
        public class Ticker
        {
            public double Buy { get; set; }
            public double Sell { get; set; }
        }
        public MainPage()
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
                        errorText.Text = "API " + exchange + " недоступно";
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
        private void PriceSell()
        {
            priceBidAsk = "продажи";
            PriceBuySell();
        }
        private void PriceBuy()
        {
            priceBidAsk = "покупки";
            PriceBuySell();

        }
        private void PriceBuySell()
        {
            oldPrice = double.Parse(textBox.Text);
            if (oldPrice != newPrice)
            {
                switch (currencyPair)
                {
                    case "PLBTBTC":
                        alertPair = "PLBT/BTC";
                        break;
                    case "PLBTETH":
                        alertPair = "PLBT/ETH";
                        break;
                    case "EMCBTC":
                        alertPair = "EMC/BTC";
                        break;
                    case "plbt_btc/ticker":
                        alertPair = "PLBT/BTC";
                        break;
                    case "plbt_eth/ticker":
                        alertPair = "PLBT/ETH";
                        break;
                    case "plbt_usd/ticker":
                        alertPair = "PLBT/USD";
                        break;
                    case "plbt_rur/ticker":
                        alertPair = "PLBT/RUR";
                        break;
                    default:
                        alertPair = currencyPair;
                        break;
                }
                PriceUpDown();
                if (!usd)
                    textBox.Text = newPrice.ToString("F8");
                else
                    textBox.Text = newPrice.ToString("F");
            }
            usd = false;
        }
        private void PriceGet()
        {
            string urlExchange;
            messageText.Text = "";
            /*Binance*/
            exchange = "Binance";
            urlExchange = "https://api.binance.com/api/v3/ticker/bookTicker?symbol=";
            currencyPair = "BTCUSDT";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "Binance" && json != null)
            {
                if (!secondUp)
                {
                    currencyPair = "BTCUSDT";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceBtcSell_binance;
                    newPrice = json.askPrice;
                    PriceSell();

                    usd = true;
                    textBox = priceBtcBuy_binance;
                    newPrice = json.bidPrice;
                    PriceBuy();
                }
            }
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

                    usd = true;
                    textBox = priceBtc;
                    newPrice = json.best_ask;
                    PriceSell();

                    currencyPair = "BTC/USDT";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceBtcSell_livecoin;
                    newPrice = json.best_ask;
                    PriceSell();

                    usd = true;
                    textBox = priceBtcBuy_livecoin;
                    newPrice = json.best_bid;
                    PriceBuy();

                    currencyPair = "ETH/USD";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceEth;
                    newPrice = json.best_ask;
                    PriceSell();
                }

                currencyPair = "PLBT/BTC";
                url = urlExchange + currencyPair;
                WebTest();

                textBox = priceBtcSell;
                newPrice = json.best_ask;
                PriceSell();

                textBox = priceBtcBuy;
                newPrice = json.best_bid;
                PriceBuy();

                if (!secondUp)
                {
                    currencyPair = "PLBT/ETH";
                    url = urlExchange + currencyPair;
                    WebTest();

                    textBox = priceEthSell;
                    newPrice = json.best_ask;
                    PriceSell();
                    //    priceEthSell.Text = newPrice.ToString("F8");

                    textBox = priceEthBuy;
                    newPrice = json.best_bid;
                    PriceBuy();

                    currencyPair = "PLBT/USD";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceUsdSell;
                    newPrice = Math.Round(json.best_ask, 2);
                    PriceSell();

                    usd = true;
                    textBox = priceUsdBuy;
                    newPrice = json.best_bid;
                    PriceBuy();
                }

                currencyPair = "EMC/BTC";
                url = urlExchange + currencyPair;
                WebTest();

                textBox = priceBtcSell_Emc;
                newPrice = json.best_ask;
                PriceSell();

                textBox = priceBtcBuy_Emc;
                newPrice = json.best_bid;
                PriceBuy();
            }
            /*HitBTC*/
            exchange = "HitBTC";
            urlExchange = "https://api.hitbtc.com/api/2/public/ticker/";
            currencyPair = "PLBTBTC";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "HitBTC" && json != null)
            {
                textBox = priceBtcSell_hitbtc;
                newPrice = json.ask;
                PriceSell();

                textBox = priceBtcBuy_hitbtc;
                newPrice = json.bid;
                PriceBuy();

                /*HitBTC ETH*/
                if (!secondUp)
                {
                    currencyPair = "PLBTETH";
                    url = urlExchange + currencyPair;
                    WebTest();

                    textBox = priceEthSell_hitbtc;
                    newPrice = json.ask;
                    PriceSell();

                    textBox = priceEthBuy_hitbtc;
                    newPrice = json.bid;
                    PriceBuy();
                }

                /*HitBTC EMC*/
                currencyPair = "EMCBTC";
                url = urlExchange + currencyPair;
                WebTest();

                textBox = priceBtcSell_hitbtc_Emc;
                newPrice = json.ask;
                PriceSell();

                textBox = priceBtcBuy_hitbtc_Emc;
                newPrice = json.bid;
                PriceBuy();
            }

            /*YoBit*/
            exchange = "YoBit";
            urlExchange = "https://yobit.net/api/2/";
            currencyPair = "plbt_btc/ticker";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "YoBit" && json != null)
            {
                textBox = priceBtcSell_yobit;
                newPrice = json.ticker.Sell;
                PriceSell();

                textBox = priceBtcBuy_yobit;
                newPrice = json.ticker.Buy;
                PriceBuy();

                if (!secondUp)
                {
                    currencyPair = "plbt_eth/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    textBox = priceEthSell_yobit;
                    newPrice = json.ticker.Sell;
                    PriceSell();

                    textBox = priceEthBuy_yobit;
                    newPrice = json.ticker.Buy;
                    PriceBuy();

                    currencyPair = "plbt_usd/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceUsdSell_yobit;
                    newPrice = json.ticker.Sell;
                    PriceSell();


                    usd = true;
                    textBox = priceUsdBuy_yobit;
                    newPrice = json.ticker.Buy;
                    PriceBuy();

                    currencyPair = "plbt_rur/ticker";
                    url = urlExchange + currencyPair;
                    WebTest();

                    usd = true;
                    textBox = priceRurSell_yobit;
                    newPrice = json.ticker.Sell;
                    PriceSell();

                    usd = true;
                    textBox = priceRurBuy_yobit;
                    newPrice = json.ticker.Buy;
                    PriceBuy();
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
            string tempNew;
            string tempOld;
            if (!firstUpDown)
            {
                calcPriceUp = newPrice / oldPrice;
                calcPriceDown = oldPrice / newPrice;
                if (usd)
                {
                    tempOld = oldPrice.ToString("F2")+ "USD";
                    tempNew = newPrice.ToString("F2")+ "USD";
                }
                else
                {
                    tempOld = oldPrice.ToString("F8");
                    tempNew = newPrice.ToString("F8");
                }

                if (percentUpDown < calcPriceUp)
                {
                    /// цена вверх
                    calcPriceUp = (calcPriceUp - 1f) * 100f;
                    messageText.Text = "Цена " + priceBidAsk + " " + alertPair + " на " + exchange + " вверх на " + calcPriceUp.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + tempOld;
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + tempNew;
                    imageName = "Resources/greenup.png";
                    ToastPrice();
                }
                if (percentUpDown < calcPriceDown)
                {
                    /// цена вниз
                    calcPriceDown = (calcPriceDown - 1f) * 100f;
                    messageText.Text = "Цена " + priceBidAsk + " " + alertPair + " на " + exchange + " вниз на " + calcPriceDown.ToString("F") + "%";
                    toastMesLine1 = "Старая цена на " + exchange + ": " + tempOld;
                    toastMesLine2 = "Новая цена на  " + exchange + ": " + tempNew;
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
            priceAlertBuyExc = float.Parse(priceBtcBuy.Text);
            priceAlertSellExc = float.Parse(priceBtcSell.Text);
            priceAlertBuyExc2 = float.Parse(priceBtcBuy_hitbtc.Text);
            priceAlertSellExc2 = float.Parse(priceBtcSell_hitbtc.Text);

            if (priceAlertBuyExc > priceAlertSellExc2)
            {
                result = (priceAlertBuyExc / priceAlertSellExc2 - 1f) * 100f;
                if (toastCheck.IsChecked == true && result > float.Parse(textPLBT.Text))
                    toastCheck.IsChecked = false;
                if (toastCheck.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "HitBTC";
                    PriceAlertsHit(result);
                }
            }
            if (priceAlertBuyExc2 > priceAlertSellExc)
            {
                result = (priceAlertBuyExc2 / priceAlertSellExc - 1f) * 100f;
                if (toastCheck.IsChecked == true && result > float.Parse(textPLBT.Text))
                    toastCheck.IsChecked = false;
                if (toastCheck.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "HitBTC";
                    PriceAlertsLive(result);
                }
            }

            coin = "EMC";
            priceAlertBuyExc = float.Parse(priceBtcBuy_Emc.Text);
            priceAlertSellExc = float.Parse(priceBtcSell_Emc.Text);
            priceAlertBuyExc2 = float.Parse(priceBtcBuy_hitbtc_Emc.Text);
            priceAlertSellExc2 = float.Parse(priceBtcSell_hitbtc_Emc.Text);

            if (priceAlertBuyExc > priceAlertSellExc2)
            {
                result = (priceAlertBuyExc / priceAlertSellExc2 - 1f) * 100f;
                if (toastCheck_EMC.IsChecked == true && result > float.Parse(textEMC.Text))
                    toastCheck_EMC.IsChecked = false;
                if (toastCheck_EMC.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "HitBTC";
                    PriceAlertsHit(result);
                }
            }
            if (priceAlertBuyExc2 > priceAlertSellExc)
            {
                result = (priceAlertBuyExc2 / priceAlertSellExc - 1f) * 100f;
                if (toastCheck_EMC.IsChecked == true && result > float.Parse(textEMC.Text))
                    toastCheck_EMC.IsChecked = false;
                if (toastCheck_EMC.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "HitBTC";
                    PriceAlertsLive(result);
                }
            }

            coin = "BTC";
            priceAlertBuyExc = float.Parse(priceBtcBuy_livecoin.Text);
            priceAlertSellExc = float.Parse(priceBtcSell_livecoin.Text);
            priceAlertBuyExc2 = float.Parse(priceBtcBuy_binance.Text);
            priceAlertSellExc2 = float.Parse(priceBtcSell_binance.Text);
            if (priceAlertBuyExc > priceAlertSellExc2)
            {
                result = (priceAlertBuyExc / priceAlertSellExc2 - 1f) * 100f;
                if (toastCheck_BTC.IsChecked == true && result > float.Parse(textBTC.Text))
                    toastCheck_BTC.IsChecked = false;
                if (toastCheck_BTC.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "Binance";
                    PriceAlertsHit(result);
                }
            }
            if (priceAlertBuyExc2 > priceAlertSellExc)
            {
                result = (priceAlertBuyExc2 / priceAlertSellExc - 1f) * 100f;
                if (toastCheck_BTC.IsChecked == true && result > float.Parse(textBTC.Text))
                    toastCheck_BTC.IsChecked = false;
                if (toastCheck_BTC.IsChecked == false)
                {
                    exchange = "Livecoin";
                    exchange2 = "HitBTC";
                    PriceAlertsLive(result);
                }
            }
        }

        private void PriceAlertsHit(double result)
        {
            messageText.Text = coin + " на " + exchange2 + " дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "На " + exchange2 + " цена продажи: " + priceAlertSellExc2.ToString("F8");
            toastMesLine2 = "На " + exchange + " цена покупки: " + priceAlertBuyExc.ToString("F8");
            PriceAlertsImage(exchange2);
            ToastPrice();
        }
        private void PriceAlertsLive(double result)
        {
            messageText.Text = coin + " на " +exchange+ " дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "На " + exchange + " цена продажи: " + priceAlertSellExc.ToString("F8");
            toastMesLine2 = "На " + exchange2 + " цена покупки: " + priceAlertBuyExc2.ToString("F8");
            PriceAlertsImage(exchange);
            ToastPrice();
        }
        private void PriceAlertsImage(string textImage) 
        {
            switch (textImage)
            {
                case "HitBTC":
                    imageName = "Resources/hitbtc.png";
                    break;
                case "Livecoin":
                    imageName = "Resources/livecoin.png";
                    break;
                case "Binance":
                    imageName = "Resources/binance.png";
                    break;
                default:
                    break;
            }
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
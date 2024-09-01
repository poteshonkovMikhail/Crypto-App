using Plugin.LocalNotification;
using System.Net.Security;
using System.Timers;

using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using static CryptoMonitoring.MainPage;

//using AndroidX.Work;
using System.Threading.Tasks;
using System.Threading;
//using Java.Util.Concurrent;
//using Android.Content;
//using AndroidX.Core.App;


namespace CryptoMonitoring
{
    public partial class MainPage : ContentPage
    {
        //public class BackgroundWorker : Worker
        //{
        //    public BackgroundWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        //    {
        //    }

        //    public override Result DoWork()
        //    {
        //        MainPage mainPage = new MainPage();
        //        mainPage.CheckCryptocurrencyPrices();
        //        return Result.InvokeSuccess();
        //    }

           
        //}

        //public class BackgroundServicE : BackgroundService
        //{
        //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //    {
        //        while (!stoppingToken.IsCancellationRequested)
        //        {

        //            MainPage mainPage = new MainPage();
        //            mainPage.CheckCryptocurrencyPrices();

        //            // Ждать одну минуту прежде чем снова проверить цены
        //            await Task.Delay(60000, stoppingToken);
        //        }
        //    }


        //}

        public readonly CryptoService _cryptoService;

        private List<CryptoService.Cryptocurrency> cryptocurrenciesList;

        public MainPage()
        {
            InitializeComponent();
            _cryptoService = new CryptoService(new HttpClient());
            LoadCryptocurrencies();
            StartTimer();
        }

        private async void LoadCryptocurrencies()
        {
            var cryptocurrencies = await _cryptoService.GetCryptocurrenciesAsync();
            SetPreviousPrices(cryptocurrencies);
            CryptoList.ItemsSource = cryptocurrencies;
        }


        public async void CheckCryptocurrencyPrices()
        {

            var cryptocurrencies = await _cryptoService.GetCryptocurrenciesAsync();
            // Логика проверки изменений в цене и отправки уведомлений
            foreach (var cryptocurrency in cryptocurrencies)
            {
                foreach (var item in GetPreviousPrices())
                {
                    if (cryptocurrency.Name == item.Name)
                    {
                        double Y = cryptocurrency.Price / item.Price * 100.000;

                        if (Y >= 100.300)
                        {
                            var notification = new NotificationRequest
                            {
                                Title = "Повышение цены",
                                Description = $"Цена {cryptocurrency.Name} возросла на {Y}%",
                                ReturningData = "Dummy data",
                                NotificationId = 1337
                            };
                            await LocalNotificationCenter.Current.Show(notification);
                        }
                        if (Y <= 99.700)
                        {

                            var notification = new NotificationRequest
                            {
                                Title = "Падение цены",
                                Description = $"Цена {cryptocurrency.Name} понизилась на {Y}%",
                                ReturningData = "Dummy data",
                                NotificationId = 1337
                            };
                            await LocalNotificationCenter.Current.Show(notification);
                        }
                    }
                }

            }
            //// Пример отправки уведомления
            //var notification = new NotificationRequest
            //{
            //    Title = "Изменение цены",
            //    Description = $"Цена  изменилась на более чем 1%",
            //    ReturningData = "Dummy data",
            //    NotificationId = 1337
            //};
            //await LocalNotificationCenter.Current.Show(notification);

        }

        private void SetPreviousPrices(List<CryptoService.Cryptocurrency> cryptocurrencies)
        {
            cryptocurrenciesList = cryptocurrencies;
        }

        public List<CryptoService.Cryptocurrency> GetPreviousPrices()
        {
            return cryptocurrenciesList;
        }

        private void StartTimer()
        {
            //Создаем таймер с интервалом в 60000 миллисекунд(1 минута)
            var _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true; // чтобы таймер повторялся
            _timer.Enabled = true;    // запускаем таймер
        }

        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            // Вызываем метод в основном потоке
            await Device.InvokeOnMainThreadAsync(() =>
            {
                CheckCryptocurrencyPrices();
                LoadCryptocurrencies();
            });
        }

        private async void OnContactSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is CryptoService.Cryptocurrency cryptocurrency)
            {
                CryptoList.SelectedItem = null;
            }
        }

        

    }
}
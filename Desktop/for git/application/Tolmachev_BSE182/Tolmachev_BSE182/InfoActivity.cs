using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using EDMTDialog;
using Newtonsoft.Json;
using Tolmachev_BSE182.Models;
using Xamarin.Essentials;

namespace Tolmachev_BSE182
{
    [Activity(Label = "InfoActivity")]
    public class InfoActivity : Activity
    {
        //поля класса
        static string rest;
        static string id = "", uri = "", city_id = "";
        static DetailsResponse details_response;

        static ImageView image;
        static Button btn_back, btn_menu;
        static TextView text;

        static string apiKey = "42d2cabd12ce312359ab22752d9aaf97";
        static string BasePath = "https://developers.zomato.com/api/v2.1";
        //метод для получения Bitmap
        static private Bitmap GetBitMapFromURL(string url)
        {
            Bitmap imageBitmap = null;
            using(var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
        //метод для обращения к API
        async void GetInfoAsync()
        {
            HttpClient client = new HttpClient();
            HttpClient client_translate = new HttpClient();

            client.DefaultRequestHeaders.Add("user-key", apiKey);
            client_translate.DefaultRequestHeaders.Add("X-RapidAPI-Host", "microsoft-azure-translation-v1.p.rapidapi.com");
            client_translate.DefaultRequestHeaders.Add("X-RapidAPI-Key", "44aa189e74mshaa49add707f3528p1d96acjsn216ddd330fe5");
            try
            {
                
                Android.Support.V7.App.AlertDialog dialog =
                        new EDMTDialogBuilder().SetContext(this).SetMessage("Please wait..").Build();
                if (!dialog.IsShowing)
                    dialog.Show();
                //обращение к API
                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(BasePath + "/restaurant?res_id=" + id);
                }
                catch (HttpRequestException)
                {
                    return;
                }
                //десериализация
                details_response = JsonConvert.DeserializeObject<DetailsResponse>(await response.Content.ReadAsStringAsync());
                var bitmap = GetBitMapFromURL(details_response.featured_image);
                image.SetImageBitmap(bitmap);

                response = null;
                //обращение к API переводчика
                try
                {
                    response = await client_translate.GetAsync("https://microsoft-azure-translation-v1.p.rapidapi.com/" +
                        "translate?from=en&to=ru&text=" + details_response.cuisines);

                }
                catch (HttpRequestException)
                {
                    return;
                }
                try
                {
                    //обработка ответа
                    details_response.cuisines = await response.Content.ReadAsStringAsync();
                    details_response.cuisines = details_response.cuisines.Split('>')[1];
                    details_response.cuisines = details_response.cuisines.Split('<')[0];
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }

                //формирование строки вывода

                string info = $"{details_response.name}\nРейтинг : {details_response.user_rating.aggregate_rating} ; Всего отзывов" +
                    $" : {details_response.user_rating.votes}" +
                    $"\nКухня : {details_response.cuisines}\nАдрес : {details_response.location.address}\nСредний чек :" +
                    $" {details_response.average_cost_for_two} {details_response.currency}\nБронь стола : ";
                if (details_response.has_table_booking == "0")
                    info += $"нет";
                else info += $"есть";
                if (details_response.has_online_delivery == "0")
                    info += $"\nОнлайн доставка : нет";
                else info += $"\nОнлайн доставка : есть";
                if (details_response.phone_numbers != "")
                    info += $"\nНомер телефона : {details_response.phone_numbers}";
                else info += $"\nНомер телефона : нет";
                text.SetText(info, TextView.BufferType.Normal);
                btn_back.Visibility = ViewStates.Visible;
                btn_menu.Visibility = ViewStates.Visible;
                uri = details_response.menu_url;
                if (dialog.IsShowing)
                    dialog.Dismiss();
            }
            catch (Exception ex)
            {

            }
        }
        

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.info_layout);

            id = Intent.GetStringExtra("res_id" ?? "Not recv");
            rest = Intent.GetStringExtra("restaurants" ?? "Not recv");
            city_id = Intent.GetStringExtra("city_id" ?? "Not recv");
            //инициализация
            btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_menu = FindViewById<Button>(Resource.Id.btn_menu);
            text = FindViewById<TextView>(Resource.Id.text_info);
            image = FindViewById<ImageView>(Resource.Id.image);

            
            text.SetTextColor(Color.Black);

            btn_back.Visibility = ViewStates.Invisible;
            btn_menu.Visibility = ViewStates.Invisible;

            Toast.MakeText(this, city_id, ToastLength.Long).Show();

            //открытие браузера
            btn_menu.Click += async delegate
            {
                try
                {
                    if (uri != "")
                        await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                    else Toast.MakeText(this, "Меню не доступно для этого заведения", ToastLength.Long);
                }
                catch (HttpRequestException)
                {
                    Toast.MakeText(this, "Интернет отсутсвует!", ToastLength.Long);
                }
            };
            //назад
            btn_back.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(RestSaveActivity));
                nextActivity.PutExtra("restaurants", rest);
                nextActivity.PutExtra("city_id", city_id);
                StartActivity(nextActivity);
                
            };
            GetInfoAsync();
        }
    }
}
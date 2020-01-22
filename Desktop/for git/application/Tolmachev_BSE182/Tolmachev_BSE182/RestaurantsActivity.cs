using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EDMTDialog;
using Newtonsoft.Json;
using Tolmachev_BSE182.Models;

namespace Tolmachev_BSE182
{
    [Activity(Label = "Restaurants")]

    public class RestaurantsActivity : Activity
    {
        //поля класса
        static string apiKey = "42d2cabd12ce312359ab22752d9aaf97";
        static string BasePath = "https://developers.zomato.com/api/v2.1";
        static int count = 15;
        static string city_id = "", cuisine_id = "", collection_id = "", category_id = "", sort = "", sort_dir = "", res_id = "";
        static RestaurantsResponce restaurant_collection;
        static List<Restaurant> restaurants = new List<Restaurant>();
        static List<string> restaurants_names = new List<string>();
        static ListView list_restaurants;
        Button btn_back;
        
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.restaurants_layout);
            //инициализация
            city_id = Intent.GetStringExtra("city_id" ?? "Not recv");
            cuisine_id = Intent.GetStringExtra("cuisine_id" ?? "Not recv");
            count = Intent.GetIntExtra("count", 15);
            collection_id = Intent.GetStringExtra("collection_id" ?? "Not recv");
            category_id = Intent.GetStringExtra("category_id" ?? "Not recv");
            sort = Intent.GetStringExtra("sort" ?? "Not recv");
            sort_dir = Intent.GetStringExtra("sort_dir" ?? "Not recv");

            list_restaurants = FindViewById<ListView>(Resource.Id.list_restaurants);
            btn_back = FindViewById<Button>(Resource.Id.btn_back);
            
            //событие для получения индекса выбранного элемента
            list_restaurants.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(GetIndex);

            // метод для обращения к API
            GetListAsync();

            //переход назад
            btn_back.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(FiltersActivity));
                nextActivity.PutExtra("city_id", city_id);
                StartActivity(nextActivity);
            };

        }
        //метод для обращения к API
        async void GetListAsync()
        {
            string path = BasePath + "/search?entity_id=" + city_id + "&entity_type=city&count=" + count.ToString();
            if (cuisine_id != "-1")
                path += $"&cuisines={cuisine_id}";
            if (collection_id != "-1")
                path += $"$collection_id={collection_id}";

            if (category_id != "-1")
                path += $"$category_id={category_id}";

            if (sort != "-1")
            {
                path += $"&sort={sort}";
                path += $"&order={sort_dir}";
            }
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("user-key", apiKey);
                Android.Support.V7.App.AlertDialog dialog =
                        new EDMTDialogBuilder().SetContext(this).SetMessage("Please wait..").Build();
                if (!dialog.IsShowing)
                    dialog.Show();


                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(path);
                }
                catch (HttpRequestException)
                {
                    Toast.MakeText(this, text: "Отсутсвует подключение к интернету!", duration: ToastLength.Long).Show();
                    return;
                }
                restaurant_collection = JsonConvert.DeserializeObject<RestaurantsResponce>(await response.Content.ReadAsStringAsync());
                restaurants = restaurant_collection.restaurants;
                foreach (var restaurant in restaurants)
                    restaurants_names.Add(restaurant.restaurant.name);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, restaurants_names);
                list_restaurants.Adapter = adapter;
                if (dialog.IsShowing)
                    dialog.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "" + ex.Message, ToastLength.Long).Show();
            }
        }
        //метод получения индекса и переход в новое меню
        private void GetIndex(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lsview = sender as ListView;
            res_id = restaurant_collection.restaurants[e.Position].restaurant.id;

            Intent nextActivity = new Intent(this, typeof(InfoActivity));
            nextActivity.PutExtra("restaurants", JsonConvert.SerializeObject(restaurant_collection));
            nextActivity.PutExtra("city_id", city_id);
            nextActivity.PutExtra("res_id", res_id);
            StartActivity(nextActivity);
        }
    }
}
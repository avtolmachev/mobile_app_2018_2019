using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Tolmachev_BSE182.Models;

namespace Tolmachev_BSE182
{
    [Activity(Label = "RestSaveActivity")]
    public class RestSaveActivity : Activity
    {
        //поля класса
        static string res_id = "", city_id = "";
        Button btn_main;
        ListView list;
        static RestaurantsResponce restaurant_collection;
        static List<Restaurant> restaurants;
        //вывод на ээкран
        void Run()
        {
            List<string> restaurants_names = new List<string>();
            restaurants = restaurant_collection.restaurants;
            foreach (var restaurant in restaurants)
                restaurants_names.Add(restaurant.restaurant.name);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, restaurants_names);
            list.Adapter = adapter;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.rest_save_layout);
            //инициализация
            btn_main = FindViewById<Button>(Resource.Id.btn_goto_main);
            list = FindViewById<ListView>(Resource.Id.listView_text);

            restaurant_collection = JsonConvert.DeserializeObject<RestaurantsResponce>(Intent.GetStringExtra("restaurants"));
            city_id = Intent.GetStringExtra("city_id" ?? "Not recv");


            list.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(GetIndex);

            Run();
            //к фильтрам
            btn_main.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(FiltersActivity));
                nextActivity.PutExtra("city_id", city_id);
                StartActivity(nextActivity);
            };
        }
        //получение индекса и переход к подробной информации
        private void GetIndex(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lsview = sender as ListView;
            res_id = restaurant_collection.restaurants[e.Position].restaurant.id;
            if (res_id != "")
            {
                Intent nextActivity = new Intent(this, typeof(InfoActivity));
                nextActivity.PutExtra("restaurants", JsonConvert.SerializeObject(restaurant_collection));
                nextActivity.PutExtra("res_id", res_id);
                nextActivity.PutExtra("city_id", city_id);

                StartActivity(nextActivity);
            }
        }
    }
}
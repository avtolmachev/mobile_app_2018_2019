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

namespace Tolmachev_BSE182
{
    [Activity(Label = "FiltersActivity")]  
    public class FiltersActivity : Activity
    {
        // поля класса
        Button btn_get_data, btn_main_menu;
        string city_id = "";
        Spinner spinner_cuisines, spinner_collections, spinner_categories, spinner_sort, spinner_sort_dir;
        string[] cuisines_ids_Array = new string[] {"-1", "152", "1", "3", "227", "132", "168", "161", "45", "55", "60",
        "73", "83", "141", "308"},
            collections_ids_Array = new string[] {"-1", "1", "20", "15", "125", "6", "30", "79", "47"},
            categories_ids_Array = new string[] {"-1", "1", "3", "4", "6", "7", "9", "10", "11", "14"};
        static string cuisine_id = "-1", collection_id = "-1", category_id = "-1", sort = "-1", sort_dir = "desc";
        int count = 15;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //инициализация
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.filters_layout);
            city_id = Intent.GetStringExtra("city_id" ?? "Not recv");
            btn_get_data = FindViewById<Button>(Resource.Id.btn_get_data);
            btn_main_menu = FindViewById<Button>(Resource.Id.btn_main_menu);
            //возврат назад
            btn_main_menu.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(MainActivity));
            };
            //переход к новому меню
            btn_get_data.Click += delegate
            {
                Intent nextActivity = new Intent(this, typeof(RestaurantsActivity));
                nextActivity.PutExtra("city_id", city_id);
                nextActivity.PutExtra("cuisine_id", cuisine_id);
                nextActivity.PutExtra("count", count);
                nextActivity.PutExtra("collection_id", collection_id);
                nextActivity.PutExtra("category_id", category_id);
                nextActivity.PutExtra("sort", sort);
                nextActivity.PutExtra("sort_dir", sort_dir);

                StartActivity(nextActivity);
            };

            // фильтр кухни
            spinner_cuisines = FindViewById<Spinner>(Resource.Id.spinner_cuisines);
            spinner_cuisines.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_cuisines_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Cuisines, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_cuisines.Adapter = adapter;

            //фильтр коллекций
            spinner_collections = FindViewById<Spinner>(Resource.Id.spinner_collections);
            spinner_collections.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_collections_ItemSelected);
            var adapter_col = ArrayAdapter.CreateFromResource(this, Resource.Array.Collections, Android.Resource.Layout.SimpleSpinnerItem);
            adapter_col.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_collections.Adapter = adapter_col;

            // фильтр категорий
            spinner_categories = FindViewById<Spinner>(Resource.Id.spinner_categories);
            spinner_categories.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_categories_ItemSelected);
            var adapter_cat = ArrayAdapter.CreateFromResource(this, Resource.Array.Categories, Android.Resource.Layout.SimpleSpinnerItem);
            adapter_cat.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_categories.Adapter = adapter_cat;

            //поле сортировки
            spinner_sort = FindViewById<Spinner>(Resource.Id.spinner_sort);
            spinner_sort.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_sort_ItemSelected);
            var adapter_sort = ArrayAdapter.CreateFromResource(this, Resource.Array.Sort, Android.Resource.Layout.SimpleSpinnerItem);
            adapter_sort.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_sort.Adapter = adapter_sort;

            //направление сортировки
            spinner_sort_dir = FindViewById<Spinner>(Resource.Id.spinner_sort_dir);
            spinner_sort_dir.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_sort_dir_ItemSelected);
            var adapter_sort_dir = ArrayAdapter.CreateFromResource(this, Resource.Array.Sort_dir, Android.Resource.Layout.SimpleSpinnerItem);
            adapter_sort_dir.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_sort_dir.Adapter = adapter_sort_dir;


        }
        //методы инициализации полей фильтров
        private void spinner_sort_dir_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var Spinner = sender as Spinner;
            if (e.Position == 0)
                sort_dir = "desc";
            else sort_dir = "asc";
        }

        private void spinner_sort_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var Spinner = sender as Spinner;
            if (e.Position == 0)
                sort = "-1";
            else if (e.Position == 1)
                sort = "cost";
            else sort = "rating";
        }

        private void spinner_categories_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var Spinner = sender as Spinner;
            category_id = categories_ids_Array[e.Position];
        }

        private void spinner_collections_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var Spinner = sender as Spinner;
            collection_id = collections_ids_Array[e.Position];
        }

        private void spinner_cuisines_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var Spinner = sender as Spinner;
            cuisine_id = cuisines_ids_Array[e.Position];
        }
    }
}
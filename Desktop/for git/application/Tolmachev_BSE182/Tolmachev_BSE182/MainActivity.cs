using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Refit;
using EDMTDialog;
using Tolmachev_BSE182.Models;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Android.Content;

namespace Tolmachev_BSE182
{
    [Activity(Label = "Restaurant Finder", Theme = "@style/AppTheme", MainLauncher = true)]

    public class MainActivity : AppCompatActivity
    {
        // описание полей
        Button btn_get_cities;
        ListView list_cities;
        AutoCompleteTextView search_text;
        CitiesResponse responce_cities;
        string apiKey = "42d2cabd12ce312359ab22752d9aaf97";
        string BasePath = "https://developers.zomato.com/api/v2.1";
        string city_id = "";

        /// <summary>
        /// Метод старта
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            string query = "";
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            btn_get_cities = FindViewById<Button>(Resource.Id.btn_get_cities);
            search_text = FindViewById<AutoCompleteTextView>(Resource.Id.search_text);
            list_cities = FindViewById<ListView>(Resource.Id.list_cities);


            //событие для получения индекса выбранного элемента
            list_cities.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(GetIndex);
            //событие для обращения к API
            btn_get_cities.Click += async delegate
            {
                if (search_text.Text != "")
                {
                    //чтение запроса пользователя
                    query = search_text.Text;
                    query = query.Replace(' ', '+');
                    query = query.Replace(",", "%2C");
                    //обращение к API
                    HttpClient client = new HttpClient();
                    HttpClient client_translate = new HttpClient();
                    client.DefaultRequestHeaders.Add("user-key", apiKey);
                    client_translate.DefaultRequestHeaders.Add("X-RapidAPI-Host", "microsoft-azure-translation-v1.p.rapidapi.com");
                    client_translate.DefaultRequestHeaders.Add("X-RapidAPI-Key", "44aa189e74mshaa49add707f3528p1d96acjsn216ddd330fe5");
                    try
                    {
                        HttpResponseMessage response = null;
                        //открытие диалога загрузки
                        Android.Support.V7.App.AlertDialog dialog =
                        new EDMTDialogBuilder().SetContext(this).SetMessage("Please wait..").Build();
                        if (!dialog.IsShowing)
                            dialog.Show();
                        //обращение к API переводчика
                        try
                        {
                            response = await client_translate.GetAsync("https://microsoft-azure-translation-v1.p.rapidapi.com/" +
                                "translate?from=ru&to=en&text=" + query);
                        }
                        catch
                        {
                            Toast.MakeText(this, "Отсутсвует подключение к интернету!", ToastLength.Long).Show();
                            return;
                        }
                        try
                        {
                            //обработка запроса
                            query = await response.Content.ReadAsStringAsync();
                            query = query.Split('>')[1];
                            query = query.Split('<')[0];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Toast.MakeText(this, "Некорректный запрос", ToastLength.Long).Show();
                            return;
                        }
                        response = null;
                        try
                        {
                            response = await client.GetAsync(BasePath + "/cities?q=" + query);
                        }
                        catch 
                        {
                            Toast.MakeText(this, "Отсутсвует подключение к интернету!", ToastLength.Long).Show();
                            return;
                        }
                        //десериализация
                        responce_cities = JsonConvert.DeserializeObject<CitiesResponse>(await response.Content.ReadAsStringAsync());

                        
                        List<string> city_names = new List<string>();
                        response = null;
                        //перевод ответа
                        for (int i = 0; i < responce_cities.location_suggestions.Count; i++)
                        {
                            string name = responce_cities.location_suggestions[i].name;
                            name = name.Replace(' ', '+');
                            name = name.Replace(",", "%2C");
                            response = await client_translate.GetAsync("https://microsoft-azure-translation-v1.p.rapidapi.com/" +
                                "translate?from=en&to=ru&text=" + name);
                            try
                            {
                                name = await response.Content.ReadAsStringAsync();
                                name = name.Split('>')[1];
                                name = name.Split('<')[0];
                            }
                            catch (IndexOutOfRangeException)
                            {
                                return;
                            }
                            responce_cities.location_suggestions[i].name = name;
                            city_names.Add(name);
                        }
                        //инициализация списка для вывода на экран
                        if (city_names.Count == 0)
                        {
                            Toast.MakeText(this, "Ничего не найдено!", ToastLength.Long).Show();
                        }
                        else
                        {
                            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, city_names);
                            list_cities.Adapter = adapter;
                        }

                        if (dialog.IsShowing)
                            dialog.Dismiss();
                    }
                    catch
                    {
                        Toast.MakeText(this, "Отсутсвует подключение к интернету!", ToastLength.Long).Show();
                    }
                }
                else
                    Toast.MakeText(this, "Введите город", ToastLength.Long).Show();
            };

        }
        /// <summary>
        /// обработчик события для получения индекса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetIndex(object sender, AdapterView.ItemClickEventArgs e)
        {
            var lsview = sender as ListView;
            city_id = responce_cities.location_suggestions[e.Position].id.ToString();
            Intent nextActivity = new Intent(this, typeof(FiltersActivity));
            nextActivity.PutExtra("city_id", city_id);
            StartActivity(nextActivity);
        }
    }
}
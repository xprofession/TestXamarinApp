using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

using System.Diagnostics;

namespace XamarinApp
{
    class VkApi
    {
        //Поля
        public static int countryId;
        public static int cityId;

        //Списки
        public static List<string> countriesList = new List<string>();
        public static List<string> citiesList = new List<string>();        
        public static List<string> universitiesList = new List<string>();

        //Классы для десериализации
        private Countries countries;
        private Cities cities;
        private Universities universities;

        //События для объявления завершения работы асинхронных методов
        public event EventHandler SetCountryId;
        public event EventHandler CitiesListUpdated;
        public event EventHandler UniversitiesListUpdated;
        public event EventHandler SetCityId;
        
        public void GetVkCountries()
        {            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/api.php?oauth=1&method=database.getCountries&v=5.5&need_all=1&count=250");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd().Replace("]}}", "]}").Replace("{\"response\":", "").Replace("items", "countriesList");
            Debug.WriteLine(result);

            countries = JsonConvert.DeserializeObject<Countries>(result);
            for (int i = 0; i < countries.count; i++)
            {
                countriesList.Add(countries.countriesList[i].title);
            }                
        }

        public void GetVkCities(int countryId, string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/api.php?oauth=1&method=database.getCities&v=5.5&country_id=" + countryId + "&need_all=3&count=1000&q=" + query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder sb = new StringBuilder(reader.ReadToEnd());
            string result = sb.Replace("]}}", "]}").Replace("{\"response\":", "").Replace("items", "citiesList").ToString();
            cities = JsonConvert.DeserializeObject<Cities>(result);

            citiesList.Clear();
            for (int i = 0; i < cities.count; i++)
            {
                citiesList.Add(cities.citiesList[i].title);// +" (" + cities.citiesList[i].region + ")");
            }

            Debug.WriteLine(result);
        }

        public void GetCitiesCallback(IAsyncResult asyncResult)
        {
            CitiesListUpdated(this, EventArgs.Empty);
        }

        public void GetVkUniversities(int countryId, int cityId, string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.vk.com/api.php?oauth=1&method=database.getUniversities&v=5.5&country_id=" + countryId + "&city_id=" + cityId + "&count=1000&q=" + query);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder sb = new StringBuilder(reader.ReadToEnd());
            string result = sb.Replace("]}}", "]}").Replace("{\"response\":", "").Replace("items", "universitiesList").ToString();
            universities = JsonConvert.DeserializeObject<Universities>(result);

            universitiesList.Clear();
            for (int i = 0; i < universities.count; i++)
            {
                universitiesList.Add(universities.universitiesList[i].title);
            }
        }

        internal void GetUniversitiesCallback(IAsyncResult asyncResult)
        {
            UniversitiesListUpdated(this, EventArgs.Empty);
        }

        internal void GetCountryId(string country)
        {
            int countryId = 0;
            GetVkCountries();
            countriesList.Clear();
            for (int i = 0; i < countries.count; i++)
            {
                if (countries.countriesList[i].title == country)
                {
                    countryId = countries.countriesList[i].id;
                    break;
                }
            }
            VkApi.countryId = countryId;
        }

        internal void GetCountryIdCallback(IAsyncResult asyncResult)
        {
            SetCountryId(this, EventArgs.Empty);
        }

        internal void GetCityId(int countryId, string city)
        {
            int cityId = 0;
            GetVkCities(countryId, city);
            citiesList.Clear();
            for (int i = 0; i < cities.count; i++)
            {
                if (cities.citiesList[i].title == city)
                {
                    cityId = cities.citiesList[i].id;
                    break;
                }
            }
            VkApi.cityId = cityId;
        }

        internal void GetCityIdCallback(IAsyncResult asyncResult)
        {
            SetCityId(this, EventArgs.Empty);
        }

        internal int GetUniversityId(int countryId, int cityId, string universityName)
        {
            int universityId = 0;
            GetVkUniversities(countryId, cityId, universityName);

            universitiesList.Clear();
            for (int i = 0; i < universities.count; i++)
            {
                if (universities.universitiesList[i].title == universityName)
                {
                    universityId = universities.universitiesList[i].id;
                    break;
                }
            }
            return universityId;
        }
    }
}

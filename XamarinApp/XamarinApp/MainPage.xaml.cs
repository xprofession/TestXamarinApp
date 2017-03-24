using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using Plugin.Connectivity;

namespace XamarinApp
{
	public partial class MainPage : ContentPage
	{
        //Данные пользователя
        private string userName;
        private string userSurname;
        private string userCountry;
        private int userCountryId;
        private string userCity;
        private int userCityId;
        private string userUniversity;

        VkApi vkApi = new VkApi();
        Android.App.Activity activity = new Android.App.Activity();
        ConnectivityImplementation connection = new ConnectivityImplementation();

        //Делегаты для получения данных асинхронно
        delegate void UpdateCities(int countryId, string query);
        delegate void UpdateUniversities(int countryId, int CityId, string query);
        delegate void GetCountryId(string countryName);
        delegate void GetCityId(int countryId, string cityName);
        delegate int GetUniversityId(int countryId, int cityId, string universityName);


        bool cityIdChanged = true;
        
        public MainPage()
		{
			InitializeComponent();
            CheckConnection();
            GetCountriesList();

            //
            entrSurname.IsEnabled = false;
            pckCountry.IsEnabled = false;
            entrCity.IsEnabled = false;
            entrUniversity.IsEnabled = false;
            btnSubmit.IsEnabled = false;

            vkApi.CitiesListUpdated += VkApi_citiesListUpdated;
            vkApi.UniversitiesListUpdated += VkApi_universitiesListUpdated;
            vkApi.SetCountryId += VkApi_SetCountryId;
            vkApi.SetCityId += VkApi_SetCityId;
        }

#region События, вызываемые после Callback
        private void VkApi_SetCityId(object sender, EventArgs e)
        {
            userCityId = VkApi.cityId;
        }

        private void VkApi_SetCountryId(object sender, EventArgs e)
        {
            userCountryId = VkApi.countryId;
        }

        private void VkApi_citiesListUpdated(object sender, EventArgs e)
        {
            activity.RunOnUiThread(() =>
            {
                lstViewCities.ItemsSource = null;
                lstViewCities.ItemsSource = VkApi.citiesList;
            });
        }

        private void VkApi_universitiesListUpdated(object sender, EventArgs e)
        {
            activity.RunOnUiThread(() =>
            {
                lstViewUniversities.ItemsSource = null;
                lstViewUniversities.ItemsSource = VkApi.universitiesList;
            });
        }
#endregion

        //Загрузка стран в список
        public void GetCountriesList()
        {
            vkApi.GetVkCountries();
            pckCountry.Items.Clear();
            for (int i = 0; i < VkApi.countriesList.Count; i++)
                pckCountry.Items.Add(VkApi.countriesList[i]);
        }

        private void CheckConnection()
        {
            if (!connection.IsConnected)
            {
                while (!connection.IsConnected)
                {
                    //Нет соединения
                }
            }
        }

        private void entrName_Unfocused(object sender, FocusEventArgs e)
        {
            entrSurname.IsEnabled = true;

            if (entrName.Text != string.Empty && entrName.Text != null)
            {
                char c;
                int lenght = 0;
                for (int i = 0; i < entrName.Text.Length; i++)
                {
                    c = entrName.Text[i];
                    if (Char.IsLetter(c) || Char.IsWhiteSpace(c))
                        lenght++;
                }
                
                if(entrName.Text.Length == lenght)
                    userName = entrName.Text;
                else
                    DisplayAlert("Предупреждение", "Введены недопустимые символы", "OK");

            }
            else
                entrName.PlaceholderColor = Color.Red;
        }

        private void entrName_Completed(object sender, EventArgs e)
        {
            entrSurname.Focus();
        }

        private void entrSurname_Unfocused(object sender, FocusEventArgs e)
        {
            pckCountry.IsEnabled = true;

            if (entrSurname.Text != string.Empty && entrSurname.Text != null)
            {
                char c;
                int lenght = 0;
                for (int i = 0; i < entrSurname.Text.Length; i++)
                {
                    c = entrSurname.Text[i];
                    if (Char.IsLetter(c) || Char.IsWhiteSpace(c))
                        lenght++;
                }

                if (entrSurname.Text.Length == lenght)
                    userSurname = entrSurname.Text;
                else
                    DisplayAlert("Предупреждение", "Введены недопустимые символы", "OK");
            }
            else
                entrSurname.PlaceholderColor = Color.Red;
        }

        private void entrSurname_Completed(object sender, EventArgs e)
        {
            pckCountry.Focus();
        }

        private void pckCountry_Unfocused(object sender, FocusEventArgs e)
        {
                if (pckCountry.SelectedIndex != -1)
                {
                    userCountry = pckCountry.Items[pckCountry.SelectedIndex];

                    GetCountryId getCountryId = new GetCountryId(vkApi.GetCountryId);
                    getCountryId.BeginInvoke(userCountry, vkApi.GetCountryIdCallback, null);

                    entrCity.IsEnabled = true;
                    entrCity.Focus();
                }
        }

        private void entrCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (userCountryId != 0)
            {
                grdCities.IsVisible = true;
                UpdateCities updateCities = new UpdateCities(vkApi.GetVkCities);
                updateCities.BeginInvoke(userCountryId, entrCity.Text, vkApi.GetCitiesCallback, null);
            }            
        }

        private void entrCity_Unfocused(object sender, FocusEventArgs e)
        {
            entrUniversity.IsEnabled = true;

            if (entrCity.Text != string.Empty && entrCity.Text != null)
                userCity = entrCity.Text;
            else
                entrCity.PlaceholderColor = Color.Red;

            cityIdChanged = true;
        }

        private void entrCity_Completed(object sender, EventArgs e)
        {
            if (grdCities.IsVisible)
                grdCities.IsVisible = false;

            entrUniversity.Focus();
        }

        private void lstViewCities_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            entrUniversity.IsEnabled = true;

            entrCity.Text = lstViewCities.SelectedItem.ToString();
            grdCities.IsVisible = false;
            userCity = entrCity.Text;

            entrUniversity.Focus();
        }

        private void lstViewCities_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            entrCity.Text = lstViewCities.SelectedItem.ToString();
            grdCities.IsVisible = false;
            userCity = entrCity.Text;

            entrUniversity.Focus();
        }

        private void entrUniversity_Focused(object sender, FocusEventArgs e)
        {
            if (cityIdChanged == true)
            {
                cityIdChanged = false;
                GetCityId getCityId = new GetCityId(vkApi.GetCityId);
                getCityId.BeginInvoke(userCountryId, userCity, vkApi.GetCityIdCallback, null);
            }
        }

        private void entrUniversity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (userCountryId != 0 && userCityId != 0)
            {
                grdUniversities.IsVisible = true;

                UpdateUniversities updateUniversities = new UpdateUniversities(vkApi.GetVkUniversities);
                updateUniversities.BeginInvoke(userCountryId, userCityId, entrUniversity.Text, vkApi.GetUniversitiesCallback, null);
            }
        }

        private void entrUniversity_Unfocused(object sender, FocusEventArgs e)
        {
            btnSubmit.IsEnabled = true;

            if (entrCity.Text != string.Empty && entrUniversity.Text != null)
                userUniversity = entrUniversity.Text;
            else
                entrUniversity.PlaceholderColor = Color.Red;
        }

        private void entrUniversity_Completed(object sender, EventArgs e)
        {
            if (grdUniversities.IsVisible)
                grdUniversities.IsVisible = false;
        }

        private void lstViewUniversities_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            entrUniversity.Text = lstViewUniversities.SelectedItem.ToString();
            grdUniversities.IsVisible = false;
            userUniversity = entrUniversity.Text;
        }

        private void lstViewUniversities_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            entrUniversity.Text = lstViewUniversities.SelectedItem.ToString();
            grdUniversities.IsVisible = false;
            userUniversity = entrUniversity.Text;
        }

        private void btnSubmit_Clicked(object sender, EventArgs e)
        {
            if (userName != string.Empty && userName != null)
            {
                if (userSurname != string.Empty && userSurname != null)
                {
                    if (userCountryId != -1)
                    {
                        if (userCityId != 0)
                        {
                            //Проверка, есть ли такой университет
                            GetUniversityId getUniversityId = new GetUniversityId(vkApi.GetUniversityId);
                            IAsyncResult asyncResult = getUniversityId.BeginInvoke(userCountryId, userCityId, userUniversity, null, null);

                            while (!asyncResult.IsCompleted)
                            {
                                ActivityIndicator act = new ActivityIndicator();
                                act.IsRunning = true;
                            }

                            try
                            {
                                int universityId = getUniversityId.EndInvoke(asyncResult);

                                if (universityId != 0)
                                {
                                //Отображение пользовательских данных на новой странице
                                StackLayout layout = new StackLayout();
                                Label lblTitle = new Label() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(5, 25, 5, 15), Text = "Данные пользователя", TextColor = Color.Gray, FontSize = 21 };
                                Label lblName = new Label() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(50, 0, 5, 0), Text = "Имя: " + userName, TextColor = Color.Gray, FontSize = 18 };
                                Label lblSurname = new Label() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(50, 0, 5, 0), Text = "Фамилия: " + userSurname, TextColor = Color.Gray, FontSize = 18 };
                                Label lblCountry = new Label() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(50, 0, 5, 0), Text = "Страна: " + userCountry, TextColor = Color.Gray, FontSize = 18 };
                                Label lblCity = new Label() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(50, 0, 5, 0), Text = "Город: " + userCity, TextColor = Color.Gray, FontSize = 18 };
                                Label lblUniversity = new Label() { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(50, 0, 5, 0), Text = "Университет: " + userUniversity, TextColor = Color.Gray, FontSize = 18 };

                                layout.Children.Add(lblTitle);
                                layout.Children.Add(lblName);
                                layout.Children.Add(lblSurname);
                                layout.Children.Add(lblCountry);
                                layout.Children.Add(lblCity);
                                layout.Children.Add(lblUniversity);

                                NavigationPage userData = new NavigationPage(new DisplayUserData() { Content = layout });

                                Navigation.PushModalAsync(userData);
                                }
                                else
                                {
                                //Униврситет
                                DisplayAlert("Предупреждение", "Неверно указан университет", "ОК");
                                }
                            }
                            catch (Exception ex)
                            {
                                DisplayAlert("", ex.ToString(), "OK");
                            }
                        }
                        else
                        {
                            //Город
                            DisplayAlert("Предупреждение", "Неверно указан город", "ОК");
                        }
                    }
                    else
                    {
                        //Страна
                        DisplayAlert("Предупреждение", "Неверно указана страна", "ОК");
                    }
                }
                else
                {
                    //Фамилия
                    DisplayAlert("Предупреждение", "Поле \"Фамилия\" не должно быть пустым", "ОК");
                }
            }
            else
            {
                //Имя
                DisplayAlert("Предупреждение", "Поле \"Имя\" не должно быть пустым", "ОК");
            }
        }
    }
}

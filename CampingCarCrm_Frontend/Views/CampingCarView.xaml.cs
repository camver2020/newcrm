using CampingCarCrm_Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CampingCarCrm_Frontend.Views
{
    public partial class CampingCarView : UserControl
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";

        public CampingCarView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ = LoadCampingCarsAsync();
        }

        private async Task LoadCampingCarsAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{backendUrl}/api/CampingCar");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var cars = JsonConvert.DeserializeObject<List<CampingCar>>(responseBody);
                CampingCarDataGrid.ItemsSource = cars;
            }
            catch (HttpRequestException ex) { MessageBox.Show($"서버에 연결할 수 없습니다: {ex.Message}"); }
        }

        private void CampingCarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CampingCarDataGrid.SelectedItem is CampingCar selectedCar)
            {
                CarNumberTextBox.Text = selectedCar.CarNumber;
                CarModelTextBox.Text = selectedCar.CarModel;
                CarNicknameTextBox.Text = selectedCar.CarNickname;
                CarStatusTextBox.Text = selectedCar.CarStatus;
            }
        }

        private async void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            var newCar = new CampingCar { CarNumber = CarNumberTextBox.Text, CarModel = CarModelTextBox.Text, CarNickname = CarNicknameTextBox.Text, CarStatus = CarStatusTextBox.Text };
            var json = JsonConvert.SerializeObject(newCar);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync($"{backendUrl}/api/CampingCar", content);
                MessageBox.Show("새로운 차량을 성공적으로 추가했습니다.");
                await LoadCampingCarsAsync();
            }
            catch (Exception ex) { MessageBox.Show($"추가 중 오류 발생: {ex.Message}"); }
        }

        private async void UpdateCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampingCarDataGrid.SelectedItem is not CampingCar selectedCar) { MessageBox.Show("수정할 차량을 목록에서 먼저 선택하세요."); return; }
            var updatedCar = new CampingCar { CarNumber = CarNumberTextBox.Text, CarModel = CarModelTextBox.Text, CarNickname = CarNicknameTextBox.Text, CarStatus = CarStatusTextBox.Text };
            var json = JsonConvert.SerializeObject(updatedCar);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                await client.PutAsync($"{backendUrl}/api/CampingCar/{selectedCar.CarID}", content);
                MessageBox.Show("차량 정보를 성공적으로 수정했습니다.");
                await LoadCampingCarsAsync();
            }
            catch (Exception ex) { MessageBox.Show($"수정 중 오류 발생: {ex.Message}"); }
        }

        private async void DeleteCarButton_Click(object sender, RoutedEventArgs e)
        {
            if (CampingCarDataGrid.SelectedItem is not CampingCar selectedCar) { MessageBox.Show("삭제할 차량을 목록에서 먼저 선택하세요."); return; }
            MessageBoxResult confirm = MessageBox.Show($"정말로 '{selectedCar.CarNickname}' 차량을 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    await client.DeleteAsync($"{backendUrl}/api/CampingCar/{selectedCar.CarID}");
                    MessageBox.Show("차량을 성공적으로 삭제했습니다.");
                    await LoadCampingCarsAsync();
                }
                catch (Exception ex) { MessageBox.Show($"삭제 중 오류 발생: {ex.Message}"); }
            }
        }
    }
}
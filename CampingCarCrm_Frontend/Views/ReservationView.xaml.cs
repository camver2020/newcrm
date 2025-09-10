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
    public partial class ReservationView : UserControl
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";

        public ReservationView()
        {
            InitializeComponent();
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReservationCalendar.SelectedDate = DateTime.Today;
            _ = LoadReservationsForDateAsync(DateTime.Today);
        }

        private void ReservationCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationCalendar.SelectedDate.HasValue)
            {
                _ = LoadReservationsForDateAsync(ReservationCalendar.SelectedDate.Value);
            }
        }

        private void NewReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReservationCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("달력에서 예약할 날짜를 먼저 선택하세요.");
                return;
            }

            ReservationTypeWindow typeWindow = new ReservationTypeWindow();
            if (typeWindow.ShowDialog() == true)
            {
                bool? finalResult = null;
                if (typeWindow.SelectedType == ReservationType.NewMember)
                {
                    ReservationWindow reservationWindow = new ReservationWindow(ReservationCalendar.SelectedDate.Value);
                    finalResult = reservationWindow.ShowDialog();
                }
                else if (typeWindow.SelectedType == ReservationType.ExistingMember)
                {
                    MemberSearchWindow searchWindow = new MemberSearchWindow();
                    if (searchWindow.ShowDialog() == true)
                    {
                        Member selectedMember = searchWindow.SelectedMember;
                        ReservationWindow reservationWindow = new ReservationWindow(ReservationCalendar.SelectedDate.Value, selectedMember);
                        finalResult = reservationWindow.ShowDialog();
                    }
                }

                if (finalResult == true)
                {
                    _ = LoadReservationsForDateAsync(ReservationCalendar.SelectedDate.Value);
                }
            }
        }

        private void EditReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationDataGrid.SelectedItem is not Reservation selectedReservation)
            {
                MessageBox.Show("수정할 예약을 목록에서 먼저 선택하세요.");
                return;
            }

            ReservationWindow reservationWindow = new ReservationWindow(selectedReservation);
            if (reservationWindow.ShowDialog() == true)
            {
                _ = LoadReservationsForDateAsync(selectedReservation.StartDateTime.Value);
            }
        }

        private async void DeleteReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReservationDataGrid.SelectedItem is not Reservation selectedReservation)
            {
                MessageBox.Show("삭제할 예약을 목록에서 먼저 선택하세요.");
                return;
            }

            if (MessageBox.Show($"'{selectedReservation.MemberName}'님의 예약을 정말로 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{backendUrl}/api/Reservation/{selectedReservation.ReservationID}");
                response.EnsureSuccessStatusCode();
                MessageBox.Show("예약이 성공적으로 삭제되었습니다.");
                _ = LoadReservationsForDateAsync(selectedReservation.StartDateTime.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예약 삭제 중 오류 발생: {ex.Message}");
            }
        }

        private async Task LoadReservationsForDateAsync(DateTime date)
        {
            try
            {
                string dateString = date.ToString("yyyy-MM-dd");
                HttpResponseMessage response = await client.GetAsync($"{backendUrl}/api/Reservation/bydate/{dateString}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var reservations = JsonConvert.DeserializeObject<List<Reservation>>(responseBody);
                ReservationDataGrid.ItemsSource = reservations;
            }
            catch (HttpRequestException)
            {
                ReservationDataGrid.ItemsSource = null;
            }
        }
    }
}
using CampingCarCrm_Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CampingCarCrm_Frontend
{
    public partial class ReservationWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";

        private DateTime _selectedDate;
        private Member _existingMember = null;
        private Reservation _reservationToUpdate = null;

        public ReservationWindow(DateTime selectedDate)
        {
            InitializeComponent();
            _selectedDate = selectedDate;
            this.Title = $"{selectedDate:yyyy-MM-dd} 신규 예약 접수";
        }

        public ReservationWindow(DateTime selectedDate, Member existingMember)
        {
            InitializeComponent();
            _selectedDate = selectedDate;
            _existingMember = existingMember;
            this.Title = $"{selectedDate:yyyy-MM-dd} - {_existingMember.MemberName}님 예약";
        }

        public ReservationWindow(Reservation reservationToUpdate)
        {
            InitializeComponent();
            _reservationToUpdate = reservationToUpdate;
            this.Title = $"{_reservationToUpdate.StartDateTime:yyyy-MM-dd} - {_reservationToUpdate.MemberName}님 예약 수정";
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(
                LoadCampingCarsAsync(),
                LoadCompaniesAsync(),
                LoadBranchesAsync(),
                LoadStatusesAsync(),
                LoadManagersAsync()
            );

            if (_existingMember != null) // 기존 회원 신규 예약 모드
            {
                NameTextBox.Text = _existingMember.MemberName;
                ContactTextBox.Text = _existingMember.Contact;
                CompanyComboBox.Text = _existingMember.CompanyName;
                BranchComboBox.Text = _existingMember.BranchName;
                NameTextBox.IsEnabled = false;
                ContactTextBox.IsEnabled = false;
                CompanyComboBox.IsEnabled = false;
                BranchComboBox.IsEnabled = false;
            }
            else if (_reservationToUpdate != null) // 예약 수정 모드
            {
                NameTextBox.Text = _reservationToUpdate.MemberName;
                NameTextBox.IsEnabled = false;
                ContactTextBox.IsEnabled = false;
                CompanyComboBox.IsEnabled = false;
                BranchComboBox.IsEnabled = false;

                // 기존 예약 정보로 드롭다운 메뉴 선택
                CampingCarComboBox.SelectedValue = _reservationToUpdate.CarID;
                StatusComboBox.SelectedValue = _reservationToUpdate.ReservationStatus;
                ManagerComboBox.SelectedValue = _reservationToUpdate.ManagerName;
            }
        }

        private async Task LoadCampingCarsAsync() { try { var response = await client.GetStringAsync($"{backendUrl}/api/CampingCar"); CampingCarComboBox.ItemsSource = JsonConvert.DeserializeObject<List<CampingCar>>(response); CampingCarComboBox.DisplayMemberPath = "CarNickname"; CampingCarComboBox.SelectedValuePath = "CarID"; } catch (Exception ex) { MessageBox.Show($"차량 목록 로딩 실패: {ex.Message}"); } }
        private async Task LoadCompaniesAsync() { try { var response = await client.GetStringAsync($"{backendUrl}/api/Options/companies"); CompanyComboBox.ItemsSource = JsonConvert.DeserializeObject<List<Company>>(response); CompanyComboBox.DisplayMemberPath = "CompanyName"; } catch (Exception ex) { MessageBox.Show($"회사 목록 로딩 실패: {ex.Message}"); } }
        private async Task LoadBranchesAsync() { try { var response = await client.GetStringAsync($"{backendUrl}/api/Options/branches"); BranchComboBox.ItemsSource = JsonConvert.DeserializeObject<List<Branch>>(response); BranchComboBox.DisplayMemberPath = "BranchName"; } catch (Exception ex) { MessageBox.Show($"지점 목록 로딩 실패: {ex.Message}"); } }
        private async Task LoadStatusesAsync() { try { var response = await client.GetStringAsync($"{backendUrl}/api/Options/statuses"); StatusComboBox.ItemsSource = JsonConvert.DeserializeObject<List<Status>>(response); StatusComboBox.DisplayMemberPath = "StatusName"; } catch (Exception ex) { MessageBox.Show($"상태 목록 로딩 실패: {ex.Message}"); } }
        private async Task LoadManagersAsync() { try { var response = await client.GetStringAsync($"{backendUrl}/api/Options/managers"); ManagerComboBox.ItemsSource = JsonConvert.DeserializeObject<List<Manager>>(response); ManagerComboBox.DisplayMemberPath = "ManagerName"; } catch (Exception ex) { MessageBox.Show($"담당자 목록 로딩 실패: {ex.Message}"); } }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reservationToUpdate != null) // 수정 모드
            {
                var selectedCar = (CampingCar)CampingCarComboBox.SelectedItem;
                _reservationToUpdate.CarID = selectedCar.CarID;
                _reservationToUpdate.ReservationStatus = (StatusComboBox.SelectedItem as Status)?.StatusName;
                _reservationToUpdate.ManagerName = (ManagerComboBox.SelectedItem as Manager)?.ManagerName;
                var json = JsonConvert.SerializeObject(_reservationToUpdate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    await client.PutAsync($"{backendUrl}/api/Reservation/{_reservationToUpdate.ReservationID}", content);
                    MessageBox.Show("예약이 성공적으로 수정되었습니다.");
                    this.DialogResult = true; this.Close();
                }
                catch (Exception ex) { MessageBox.Show($"예약 수정 실패: {ex.Message}"); }
            }
            else // 신규 등록 모드
            {
                int memberIdToUse;
                if (_existingMember != null) { memberIdToUse = _existingMember.MemberID; }
                else
                {
                    var newMember = new Member { MemberName = NameTextBox.Text, Contact = ContactTextBox.Text, CompanyName = (CompanyComboBox.SelectedItem as Company)?.CompanyName, BranchName = (BranchComboBox.SelectedItem as Branch)?.BranchName };
                    var memberJson = JsonConvert.SerializeObject(newMember);
                    var memberContent = new StringContent(memberJson, Encoding.UTF8, "application/json");
                    try
                    {
                        var memberResponse = await client.PostAsync($"{backendUrl}/api/Member", memberContent);
                        memberResponse.EnsureSuccessStatusCode();
                        var result = JsonConvert.DeserializeObject<dynamic>(await memberResponse.Content.ReadAsStringAsync());
                        memberIdToUse = result.memberId;
                    }
                    catch (Exception ex) { MessageBox.Show($"회원 생성 실패: {ex.Message}"); return; }
                }

                if (CampingCarComboBox.SelectedItem == null) { MessageBox.Show("차량을 선택하세요."); return; }
                var selectedCar = (CampingCar)CampingCarComboBox.SelectedItem;
                var newReservation = new Reservation { MemberID = memberIdToUse, CarID = selectedCar.CarID, StartDateTime = _selectedDate, ReservationStatus = (StatusComboBox.SelectedItem as Status)?.StatusName, ManagerName = (ManagerComboBox.SelectedItem as Manager)?.ManagerName };
                var reservationJson = JsonConvert.SerializeObject(newReservation);
                var reservationContent = new StringContent(reservationJson, Encoding.UTF8, "application/json");
                try
                {
                    await client.PostAsync($"{backendUrl}/api/Reservation", reservationContent);
                    MessageBox.Show("예약이 성공적으로 등록되었습니다.");
                    this.DialogResult = true; this.Close();
                }
                catch (Exception ex) { MessageBox.Show($"예약 생성 실패: {ex.Message}"); }
            }
        }
    }
}
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
    public partial class MemberView : UserControl
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";

        public MemberView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ = LoadMembersAsync();
        }

        private async void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            var newMember = new Member { MemberName = NameTextBox.Text, Contact = ContactTextBox.Text, EmergencyContact = EmergencyContactTextBox.Text, CompanyName = CompanyTextBox.Text, BranchName = BranchTextBox.Text, MemberMemo = MemoTextBox.Text };
            var json = JsonConvert.SerializeObject(newMember);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PostAsync($"{backendUrl}/api/Member", content);
                response.EnsureSuccessStatusCode();
                MessageBox.Show("새로운 회원을 성공적으로 추가했습니다.");
                NameTextBox.Clear(); ContactTextBox.Clear(); EmergencyContactTextBox.Clear(); CompanyTextBox.Clear(); BranchTextBox.Clear(); MemoTextBox.Clear();
                await LoadMembersAsync();
            }
            catch (HttpRequestException ex) { MessageBox.Show($"서버에 데이터를 보내는 중 오류가 발생했습니다: {ex.Message}"); }
        }

        private async void UpdateMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem is not Member selectedMember) { MessageBox.Show("수정할 회원을 목록에서 먼저 선택하세요."); return; }
            var updatedMemberData = new Member { MemberName = NameTextBox.Text, Contact = ContactTextBox.Text, EmergencyContact = EmergencyContactTextBox.Text, CompanyName = CompanyTextBox.Text, BranchName = BranchTextBox.Text, MemberMemo = MemoTextBox.Text };
            var json = JsonConvert.SerializeObject(updatedMemberData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PutAsync($"{backendUrl}/api/Member/{selectedMember.MemberID}", content);
                response.EnsureSuccessStatusCode();
                MessageBox.Show("회원 정보를 성공적으로 수정했습니다.");
                await LoadMembersAsync();
            }
            catch (Exception ex) { MessageBox.Show($"수정 중 오류 발생: {ex.Message}"); }
        }

        private async void DeleteMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem is not Member selectedMember) { MessageBox.Show("삭제할 회원을 목록에서 먼저 선택하세요."); return; }
            MessageBoxResult confirm = MessageBox.Show($"정말로 '{selectedMember.MemberName}' 회원을 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"{backendUrl}/api/Member/{selectedMember.MemberID}");
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("회원을 성공적으로 삭제했습니다.");
                    await LoadMembersAsync();
                }
                catch (Exception ex) { MessageBox.Show($"삭제 중 오류 발생: {ex.Message}"); }
            }
        }

        private void MemberDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem is Member selectedMember)
            {
                NameTextBox.Text = selectedMember.MemberName; ContactTextBox.Text = selectedMember.Contact; EmergencyContactTextBox.Text = selectedMember.EmergencyContact; CompanyTextBox.Text = selectedMember.CompanyName; BranchTextBox.Text = selectedMember.BranchName; MemoTextBox.Text = selectedMember.MemberMemo;
            }
        }

        private async Task LoadMembersAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{backendUrl}/api/Member");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var members = JsonConvert.DeserializeObject<List<Member>>(responseBody);
                MemberDataGrid.ItemsSource = members;
            }
            catch (HttpRequestException ex) { MessageBox.Show($"서버에 연결할 수 없습니다: {ex.Message}"); }
        }
    }
}
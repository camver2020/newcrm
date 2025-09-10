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
    public partial class SettingsView : UserControl
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";

        public SettingsView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _ = Task.WhenAll(
                LoadCompaniesAsync(),
                LoadBranchesAsync(),
                LoadManagersAsync()
            );
        }

        // --- 회사 관리 ---
        private async Task LoadCompaniesAsync()
        {
            try
            {
                var response = await client.GetStringAsync($"{backendUrl}/api/Options/companies");
                CompanyListBox.ItemsSource = JsonConvert.DeserializeObject<List<Company>>(response);
            }
            catch (Exception ex) { MessageBox.Show($"회사 목록 로딩 실패: {ex.Message}"); }
        }
        private async void AddCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewCompanyTextBox.Text)) { MessageBox.Show("회사 이름을 입력하세요."); return; }
            var newCompany = new { CompanyName = NewCompanyTextBox.Text };
            var content = new StringContent(JsonConvert.SerializeObject(newCompany), Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync($"{backendUrl}/api/Options/companies", content);
                NewCompanyTextBox.Clear();
                await LoadCompaniesAsync();
            }
            catch (Exception ex) { MessageBox.Show($"회사 추가 실패: {ex.Message}"); }
        }
        private async void DeleteCompanyButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompanyListBox.SelectedItem is not Company selectedCompany) { MessageBox.Show("삭제할 회사를 목록에서 선택하세요."); return; }
            if (MessageBox.Show($"'{selectedCompany.CompanyName}' 회사를 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            try
            {
                await client.DeleteAsync($"{backendUrl}/api/Options/companies/{selectedCompany.CompanyID}");
                await LoadCompaniesAsync();
            }
            catch (Exception ex) { MessageBox.Show($"회사 삭제 실패: {ex.Message}"); }
        }

        // --- 지점 관리 ---
        private async Task LoadBranchesAsync()
        {
            try
            {
                var response = await client.GetStringAsync($"{backendUrl}/api/Options/branches");
                BranchListBox.ItemsSource = JsonConvert.DeserializeObject<List<Branch>>(response);
            }
            catch (Exception ex) { MessageBox.Show($"지점 목록 로딩 실패: {ex.Message}"); }
        }
        private async void AddBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewBranchTextBox.Text)) { MessageBox.Show("지점 이름을 입력하세요."); return; }
            var newBranch = new { BranchName = NewBranchTextBox.Text };
            var content = new StringContent(JsonConvert.SerializeObject(newBranch), Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync($"{backendUrl}/api/Options/branches", content);
                NewBranchTextBox.Clear();
                await LoadBranchesAsync();
            }
            catch (Exception ex) { MessageBox.Show($"지점 추가 실패: {ex.Message}"); }
        }
        private async void DeleteBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (BranchListBox.SelectedItem is not Branch selectedBranch) { MessageBox.Show("삭제할 지점을 선택하세요."); return; }
            if (MessageBox.Show($"'{selectedBranch.BranchName}' 지점을 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            try
            {
                await client.DeleteAsync($"{backendUrl}/api/Options/branches/{selectedBranch.BranchID}");
                await LoadBranchesAsync();
            }
            catch (Exception ex) { MessageBox.Show($"지점 삭제 실패: {ex.Message}"); }
        }

        // --- 담당자 관리 ---
        private async Task LoadManagersAsync()
        {
            try
            {
                var response = await client.GetStringAsync($"{backendUrl}/api/Options/managers");
                ManagerListBox.ItemsSource = JsonConvert.DeserializeObject<List<Manager>>(response);
            }
            catch (Exception ex) { MessageBox.Show($"담당자 목록 로딩 실패: {ex.Message}"); }
        }
        private async void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewManagerTextBox.Text)) { MessageBox.Show("담당자 이름을 입력하세요."); return; }
            var newManager = new { ManagerName = NewManagerTextBox.Text };
            var content = new StringContent(JsonConvert.SerializeObject(newManager), Encoding.UTF8, "application/json");
            try
            {
                await client.PostAsync($"{backendUrl}/api/Options/managers", content);
                NewManagerTextBox.Clear();
                await LoadManagersAsync();
            }
            catch (Exception ex) { MessageBox.Show($"담당자 추가 실패: {ex.Message}"); }
        }
        private async void DeleteManagerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManagerListBox.SelectedItem is not Manager selectedManager) { MessageBox.Show("삭제할 담당자를 선택하세요."); return; }
            if (MessageBox.Show($"'{selectedManager.ManagerName}' 담당자를 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            try
            {
                await client.DeleteAsync($"{backendUrl}/api/Options/managers/{selectedManager.ManagerID}");
                await LoadManagersAsync();
            }
            catch (Exception ex) { MessageBox.Show($"담당자 삭제 실패: {ex.Message}"); }
        }
    }
}
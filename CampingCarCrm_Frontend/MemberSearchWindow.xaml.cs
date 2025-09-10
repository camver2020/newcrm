using CampingCarCrm_Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace CampingCarCrm_Frontend
{
    public partial class MemberSearchWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string backendUrl = "http://localhost:5222";
        private List<Member> _allMembers; // 전체 회원 목록을 담아둘 변수

        // 선택된 회원을 부모 창으로 전달하기 위한 속성
        public Member SelectedMember { get; private set; }

        public MemberSearchWindow()
        {
            InitializeComponent();
            _allMembers = new List<Member>();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllMembersAsync();
        }

        // 창이 열릴 때, 백엔드에서 모든 회원 목록을 미리 한번만 불러옴
        private async Task LoadAllMembersAsync()
        {
            try
            {
                var response = await client.GetStringAsync($"{backendUrl}/api/Member");
                _allMembers = JsonConvert.DeserializeObject<List<Member>>(response);
                MemberDataGrid.ItemsSource = _allMembers; // 처음에는 모든 회원 보여주기
            }
            catch (Exception ex)
            {
                MessageBox.Show($"전체 회원 목록 로딩 실패: {ex.Message}");
            }
        }

        // '검색' 버튼 클릭
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                MemberDataGrid.ItemsSource = _allMembers; // 검색어가 없으면 전체 목록 보여주기
            }
            else
            {
                // 이름 또는 연락처에 검색어가 포함된 회원만 필터링해서 보여주기
                var filteredMembers = _allMembers.Where(m =>
                    m.MemberName.ToLower().Contains(searchTerm) ||
                    (m.Contact != null && m.Contact.Contains(searchTerm))
                ).ToList();
                MemberDataGrid.ItemsSource = filteredMembers;
            }
        }

        // '선택' 버튼 클릭
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem is not Member selectedMember)
            {
                MessageBox.Show("목록에서 회원을 선택하세요.");
                return;
            }
            SelectedMember = selectedMember; // 선택된 회원 정보를 저장
            this.DialogResult = true; // 성공 신호 보내기
            this.Close();
        }
    }
}
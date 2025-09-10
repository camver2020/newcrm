using CampingCarCrm_Frontend.Views;
using System.Windows;

namespace CampingCarCrm_Frontend
{
    // Models 폴더를 사용하도록 using 구문과 데이터 클래스 정의는 그대로 둡니다.
    // ...

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new ReservationView();
        }

        private void ShowReservationView_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReservationView();
        }
        private void ShowMemberView_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MemberView();
        }
        private void ShowCampingCarView_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CampingCarView();
        }
        private void ShowSettingsView_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }
    }
}
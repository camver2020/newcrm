using System.Windows;

namespace CampingCarCrm_Frontend
{
    public enum ReservationType
    {
        None,
        ExistingMember,
        NewMember
    }

    public partial class ReservationTypeWindow : Window
    {
        public ReservationType SelectedType { get; private set; } = ReservationType.None;

        public ReservationTypeWindow()
        {
            InitializeComponent();
        }

        private void ExistingMemberButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = ReservationType.ExistingMember;
            this.DialogResult = true;
            this.Close();
        }

        private void NewMemberButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = ReservationType.NewMember;
            this.DialogResult = true;
            this.Close();
        }
    }
}
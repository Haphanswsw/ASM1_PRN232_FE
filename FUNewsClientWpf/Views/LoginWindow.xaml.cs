using FUNewsClientWpf.Models;
using FUNewsClientWpf.Services;
using System.Windows;

namespace FUNewsClientWpf.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _auth;

        public LoginResult? Result { get; private set; }

        public LoginWindow(AuthService auth)
        {
            InitializeComponent();
            _auth = auth;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text.Trim();
            var pass = PasswordBox.Password;
            var res = await _auth.LoginAsync(email, pass);
            if (res == null)
            {
                MessageBox.Show("Invalid credentials.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Result = res;
            DialogResult = true;
            Close();
        }
    }
}
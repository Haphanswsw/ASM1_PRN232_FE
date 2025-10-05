using FUNewsClientWpf.Models;
using System;
using System.Windows;

namespace FUNewsClientWpf.Views.Dialogs
{
    public partial class AccountDialog : Window
    {
        public SystemAccount? Value { get; private set; }

        private readonly bool _isEdit;

        public AccountDialog(SystemAccount? source = null)
        {
            InitializeComponent();
            _isEdit = source != null;
            if (_isEdit)
            {
                IdBox.Text = source!.AccountId.ToString();
                IdBox.IsEnabled = false;
                NameBox.Text = source.AccountName ?? "";
                EmailBox.Text = source.AccountEmail ?? "";
                PassBox.Password = source.AccountPassword ?? "";
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = short.Parse(IdBox.Text);
                Value = new SystemAccount
                {
                    AccountId = id,
                    AccountName = NameBox.Text,
                    AccountEmail = EmailBox.Text,
                    AccountPassword = PassBox.Password
                };
                DialogResult = true;
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input.", "Account", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
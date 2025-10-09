using FUNewsClientWpf.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FUNewsClientWpf.Views.Dialogs
{
    public partial class AccountDialog : Window
    {
        public SystemAccount? Value { get; private set; }
        private readonly bool _isEdit;

        // Create (auto id)
        public AccountDialog(short newId)
        {
            InitializeComponent();
            _isEdit = false;
            IdBox.Text = newId.ToString();
            IdBox.IsEnabled = false;
            // hide ID fields if you prefer not to show them for creation
            // IdLabel.Visibility = Visibility.Collapsed;
            // IdBox.Visibility = Visibility.Collapsed;
            RoleBox.SelectedIndex = 1; // default Staff (index depends on order)
        }

        // Edit
        public AccountDialog(SystemAccount? source = null)
        {
            InitializeComponent();
            _isEdit = source != null;
            if (_isEdit && source != null)
            {
                IdBox.Text = source.AccountId.ToString();
                IdBox.IsEnabled = false;
                NameBox.Text = source.AccountName ?? "";
                EmailBox.Text = source.AccountEmail ?? "";
                PassBox.Password = source.AccountPassword ?? "";

                if (source.AccountRole.HasValue)
                {
                    foreach (var item in RoleBox.Items.OfType<ComboBoxItem>())
                    {
                        if (short.TryParse(item.Tag?.ToString(), out var tagVal) && tagVal == source.AccountRole.Value)
                        {
                            RoleBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                // If someone still calls parameterless constructor, we keep ID editable (fallback)
                RoleBox.SelectedIndex = 1; // default Staff
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = short.Parse(IdBox.Text);
                int? role = null;
                if (RoleBox.SelectedItem is ComboBoxItem ci && int.TryParse(ci.Tag?.ToString(), out var r))
                    role = r;

                Value = new SystemAccount
                {
                    AccountId = id,
                    AccountName = NameBox.Text,
                    AccountEmail = EmailBox.Text,
                    AccountPassword = string.IsNullOrWhiteSpace(PassBox.Password) ? null : PassBox.Password,
                    AccountRole = role
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
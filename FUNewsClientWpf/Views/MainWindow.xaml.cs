using FUNewsClientWpf.Models;
using FUNewsClientWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FUNewsClientWpf.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApiClient _api;
        private readonly AuthService _auth;
        private readonly AccountsService _accounts;
        private readonly CategoriesService _categories;
        private readonly NewsService _news;
        private readonly TagsService _tags;

        private List<SystemAccount> _accountsCache = new();
        private List<Category> _categoriesCache = new();
        private List<NewsArticle> _newsCache = new();
        private List<Tag> _tagsCache = new();

        public MainWindow()
        {
            InitializeComponent();
            _api = new ApiClient(App.ApiBaseUrl);
            _auth = new AuthService(_api);
            _accounts = new AccountsService(_api);
            _categories = new CategoriesService(_api);
            _news = new NewsService(_api);
            _tags = new TagsService(_api);

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Login
            var login = new LoginWindow(_auth);
            if (login.ShowDialog() != true || login.Result == null)
            {
                Close();
                return;
            }

            var me = await _auth.MeAsync();
            if (me == null)
            {
                MessageBox.Show("Failed to retrieve profile.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            Title = $"FU News Management - {me.name} ({me.role})";

            // Role-driven tabs
            var role = me.role ?? string.Empty;
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                AdminAccountsTab.Visibility = Visibility.Visible;
                CategoriesTab.Visibility = Visibility.Visible;
                NewsTab.Visibility = Visibility.Visible;
                ReportTab.Visibility = Visibility.Visible;

                await LoadAccountsAsync();
                await LoadCategoriesAsync();
                await LoadTagsAsync();
                await LoadNewsAsync();
            }
            else if (role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                CategoriesTab.Visibility = Visibility.Visible;
                NewsTab.Visibility = Visibility.Visible;
                HistoryTab.Visibility = Visibility.Visible;

                await LoadCategoriesAsync();
                await LoadTagsAsync();
                await LoadNewsAsync();
                await LoadHistoryAsync();
            }
            else
            {
                MessageBox.Show("Your role is not supported for this client.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async System.Threading.Tasks.Task LoadAccountsAsync()
        {
            var data = await _accounts.GetAllAsync() ?? new List<SystemAccount>();
            _accountsCache = data;
            ApplyAccountFilter();
        }

        private void ApplyAccountFilter()
        {
            var q = (AccountSearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtered = string.IsNullOrEmpty(q)
                ? _accountsCache
                : _accountsCache.Where(a =>
                    (a.AccountName ?? string.Empty).ToLowerInvariant().Contains(q) ||
                    (a.AccountEmail ?? string.Empty).ToLowerInvariant().Contains(q)).ToList();
            AccountsGrid.ItemsSource = filtered;
        }

        private async System.Threading.Tasks.Task LoadCategoriesAsync()
        {
            var data = await _categories.GetAllAsync() ?? new List<Category>();
            _categoriesCache = data;
            ApplyCategoryFilter();
        }

        private void ApplyCategoryFilter()
        {
            var q = (CategorySearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtered = string.IsNullOrEmpty(q)
                ? _categoriesCache
                : _categoriesCache.Where(c => (c.CategoryName ?? string.Empty).ToLowerInvariant().Contains(q)).ToList();
            CategoriesGrid.ItemsSource = filtered;
        }

        private async System.Threading.Tasks.Task LoadNewsAsync()
        {
            var data = await _news.GetAllAsync() ?? new List<NewsArticle>();
            _newsCache = data;
            ApplyNewsFilter();
        }

        private void ApplyNewsFilter()
        {
            var q = (NewsSearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtered = string.IsNullOrEmpty(q)
                ? _newsCache
                : _newsCache.Where(n =>
                    (n.NewsTitle ?? string.Empty).ToLowerInvariant().Contains(q) ||
                    (n.Headline ?? string.Empty).ToLowerInvariant().Contains(q)).ToList();
            NewsGrid.ItemsSource = filtered;
        }

        private async System.Threading.Tasks.Task LoadHistoryAsync()
        {
            var data = await _news.GetMineAsync() ?? new List<NewsArticle>();
            HistoryGrid.ItemsSource = data;
        }

        private async System.Threading.Tasks.Task LoadTagsAsync()
        {
            _tagsCache = await _tags.GetAllAsync() ?? new List<Tag>();
        }

        // Menu actions
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await _auth.LogoutAsync();
            System.Diagnostics.Process.Start(Environment.ProcessPath!);
            Application.Current.Shutdown();
        }

        private async void Profile_Click(object sender, RoutedEventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter new name (leave blank to skip):", "Profile");
            var pass = Microsoft.VisualBasic.Interaction.InputBox("Enter new password (leave blank to skip):", "Profile");
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(pass)) return;

            var ok = await _auth.UpdateMeAsync(string.IsNullOrWhiteSpace(name) ? null : name,
                                               string.IsNullOrWhiteSpace(pass) ? null : pass);
            MessageBox.Show(ok ? "Profile updated." : "Failed to update profile.", "Profile", MessageBoxButton.OK,
                ok ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        // Accounts events
        private async void AccountsRefresh_Click(object sender, RoutedEventArgs e) => await LoadAccountsAsync();

        private void AccountCreate_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.AccountDialog();
            if (dlg.ShowDialog() == true && dlg.Value != null)
            {
                _ = CreateAccount(dlg.Value);
            }
        }

        private async System.Threading.Tasks.Task CreateAccount(SystemAccount acc)
        {
            var created = await _accounts.CreateAsync(acc);
            if (created == null) MessageBox.Show("Failed to create account.", "Accounts", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadAccountsAsync();
        }

        private void AccountEdit_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsGrid.SelectedItem is not SystemAccount acc) return;
            var dlg = new Dialogs.AccountDialog(acc);
            if (dlg.ShowDialog() == true && dlg.Value != null)
            {
                _ = UpdateAccount(dlg.Value);
            }
        }

        private async System.Threading.Tasks.Task UpdateAccount(SystemAccount acc)
        {
            var ok = await _accounts.UpdateAsync(acc.AccountId, acc);
            if (!ok) MessageBox.Show("Failed to update account.", "Accounts", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadAccountsAsync();
        }

        private async void AccountDelete_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsGrid.SelectedItem is not SystemAccount acc) return;
            if (MessageBox.Show($"Delete account {acc.AccountEmail}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            var ok = await _accounts.DeleteAsync(acc.AccountId);
            if (!ok) MessageBox.Show("Delete failed (maybe it has created news).", "Accounts", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadAccountsAsync();
        }

        private void AccountSearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyAccountFilter();

        // Categories events
        private async void CategoriesRefresh_Click(object sender, RoutedEventArgs e) => await LoadCategoriesAsync();

        private void CategoryCreate_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.CategoryDialog();
            if (dlg.ShowDialog() == true && dlg.Value != null)
            {
                _ = CreateCategory(dlg.Value);
            }
        }

        private async System.Threading.Tasks.Task CreateCategory(Category c)
        {
            var created = await _categories.CreateAsync(c);
            if (created == null) MessageBox.Show("Failed to create category.", "Categories", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadCategoriesAsync();
        }

        private void CategoryEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category cat) return;
            var dlg = new Dialogs.CategoryDialog(cat);
            if (dlg.ShowDialog() == true && dlg.Value != null)
            {
                _ = UpdateCategory(dlg.Value);
            }
        }

        private async System.Threading.Tasks.Task UpdateCategory(Category c)
        {
            var ok = await _categories.UpdateAsync(c.CategoryId, c);
            if (!ok) MessageBox.Show("Failed to update category.", "Categories", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadCategoriesAsync();
        }

        private async void CategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is not Category cat) return;
            if (MessageBox.Show($"Delete category {cat.CategoryName}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            var ok = await _categories.DeleteAsync(cat.CategoryId);
            if (!ok) MessageBox.Show("Delete failed (maybe in use).", "Categories", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadCategoriesAsync();
        }

        private void CategorySearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyCategoryFilter();

        // News events
        private async void NewsRefresh_Click(object sender, RoutedEventArgs e) => await LoadNewsAsync();

        private void NewsCreate_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.NewsDialog(_categoriesCache, _tagsCache);
            if (dlg.ShowDialog() == true && dlg.Created != null)
            {
                _ = CreateNews(dlg.Created, dlg.SelectedTagIds);
            }
        }

        private async System.Threading.Tasks.Task CreateNews(CreateNewsArticleRequest req, List<int> tagIds)
        {
            var created = await _news.CreateAsync(req);
            if (created == null)
            {
                MessageBox.Show("Failed to create news.", "News", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (tagIds.Count > 0 && !string.IsNullOrEmpty(created.NewsArticleId))
            {
                await _news.ReplaceTagsAsync(created.NewsArticleId!, tagIds);
            }
            await LoadNewsAsync();
        }

        private void NewsEdit_Click(object sender, RoutedEventArgs e)
        {
            if (NewsGrid.SelectedItem is not NewsArticle item) return;
            var dlg = new Dialogs.NewsDialog(_categoriesCache, _tagsCache, item);
            if (dlg.ShowDialog() == true)
            {
                _ = UpdateNews(item.NewsArticleId!, dlg.Update, dlg.SelectedTagIds);
            }
        }

        private async System.Threading.Tasks.Task UpdateNews(string id, UpdateNewsArticleRequest req, List<int> tagIds)
        {
            var ok = await _news.UpdateAsync(id, req);
            if (!ok) MessageBox.Show("Failed to update news.", "News", MessageBoxButton.OK, MessageBoxImage.Error);
            await _news.ReplaceTagsAsync(id, tagIds);
            await LoadNewsAsync();
        }

        private async void NewsDelete_Click(object sender, RoutedEventArgs e)
        {
            if (NewsGrid.SelectedItem is not NewsArticle item) return;
            if (MessageBox.Show($"Delete news '{item.NewsTitle ?? item.Headline}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            var ok = await _news.DeleteAsync(item.NewsArticleId!);
            if (!ok) MessageBox.Show("Delete failed.", "News", MessageBoxButton.OK, MessageBoxImage.Error);
            await LoadNewsAsync();
        }

        private void NewsSearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyNewsFilter();

        // History
        private async void HistoryRefresh_Click(object sender, RoutedEventArgs e) => await LoadHistoryAsync();

        // Report
        private async void RunReport_Click(object sender, RoutedEventArgs e)
        {
            var s = StartDatePicker.SelectedDate ?? DateTime.Today;
            var d = EndDatePicker.SelectedDate ?? DateTime.Today;
            if (d < s)
            {
                MessageBox.Show("End date must be >= start date.", "Report", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var data = await _news.GetReportAsync(s, d);
            ReportGrid.ItemsSource = data ?? new List<NewsArticle>();
        }
    }
}
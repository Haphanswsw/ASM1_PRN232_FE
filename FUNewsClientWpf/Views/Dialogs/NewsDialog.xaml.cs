using FUNewsClientWpf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FUNewsClientWpf.Views.Dialogs
{
    public partial class NewsDialog : Window
    {
        public CreateNewsArticleRequest? Created { get; private set; }
        public UpdateNewsArticleRequest Update { get; private set; } = new();
        public List<int> SelectedTagIds { get; private set; } = new();

        private readonly bool _isEdit;

        public NewsDialog(List<Category> categories, List<Tag> tags, NewsArticle? existing = null)
        {
            InitializeComponent();
            CategoryBox.ItemsSource = categories;
            TagsList.ItemsSource = tags;
            _isEdit = existing != null;

            if (_isEdit)
            {
                Title = "Edit News";
                TitleBox.Text = existing!.NewsTitle ?? "";
                HeadlineBox.Text = existing.Headline ?? "";
                SourceBox.Text = existing.NewsSource ?? "";
                ContentBox.Text = existing.NewsContent ?? "";
                StatusBox.IsChecked = existing.NewsStatus ?? true;
                CategoryBox.SelectedValue = existing.CategoryId;
                var select = tags.Where(t => existing.Tags.Any(et => et.TagId == t.TagId)).ToList();
                foreach (var t in select) TagsList.SelectedItems.Add(t);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HeadlineBox.Text))
            {
                MessageBox.Show("Headline is required.", "News", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedTagIds = TagsList.SelectedItems.Cast<Tag>().Select(t => t.TagId).ToList();
            var catId = (short?)CategoryBox.SelectedValue;

            if (_isEdit)
            {
                Update.NewsTitle = TitleBox.Text;
                Update.Headline = HeadlineBox.Text;
                Update.NewsSource = SourceBox.Text;
                Update.NewsContent = ContentBox.Text;
                Update.NewsStatus = StatusBox.IsChecked ?? true;
                Update.CategoryId = catId;
                DialogResult = true;
                Close();
            }
            else
            {
                Created = new CreateNewsArticleRequest
                {
                    NewsTitle = TitleBox.Text,
                    Headline = HeadlineBox.Text,
                    NewsSource = SourceBox.Text,
                    NewsContent = ContentBox.Text,
                    NewsStatus = StatusBox.IsChecked ?? true,
                    CategoryId = catId
                };
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
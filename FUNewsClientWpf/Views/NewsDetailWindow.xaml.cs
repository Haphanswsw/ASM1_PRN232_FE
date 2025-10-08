using FUNewsClientWpf.Models;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FUNewsClientWpf.Views
{
    public partial class NewsDetailWindow : Window
    {
        public NewsDetailWindow(NewsArticle article, System.Collections.Generic.List<Category> categories)
        {
            InitializeComponent();

            var categoryName = categories.FirstOrDefault(c => c.CategoryId == article.CategoryId)?.CategoryName
                               ?? (article.CategoryId?.ToString() ?? string.Empty);

            TitleText.Text = article.NewsTitle ?? "(No title)";
            HeadlineText.Text = article.Headline ?? string.Empty;
            CategoryText.Text = categoryName;
            SourceText.Text = article.NewsSource ?? string.Empty;

            var isPublished = article.NewsStatus == true;
            StatusText.Text = isPublished ? "Published" : "Draft";
            var accent = (Brush)FindResource("AccentBrush");
            var neutral = (Brush)FindResource("BorderBrush");
            var textBrush = (Brush)FindResource("TextBrush");
            StatusBadgeBorder.Background = isPublished ? accent : neutral;
            StatusText.Foreground = isPublished ? Brushes.White : textBrush;

            CreatedText.Text = article.CreatedDate?.ToString("g") ?? string.Empty;
            ModifiedText.Text = article.ModifiedDate?.ToString("g") ?? string.Empty;
            CreatedByText.Text = article.CreatedById?.ToString() ?? string.Empty;
            UpdatedByText.Text = article.UpdatedById?.ToString() ?? string.Empty;
            TagsText.Text = (article.Tags != null && article.Tags.Count > 0)
                ? string.Join(", ", article.Tags.Select(t => t.TagName).Where(n => !string.IsNullOrWhiteSpace(n)))
                : "(none)";
            ContentText.Text = article.NewsContent ?? string.Empty;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
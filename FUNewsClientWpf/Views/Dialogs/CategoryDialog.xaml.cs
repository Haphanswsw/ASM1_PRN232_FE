using FUNewsClientWpf.Models;
using System;
using System.Windows;

namespace FUNewsClientWpf.Views.Dialogs
{
    public partial class CategoryDialog : Window
    {
        public Category? Value { get; private set; }

        public CategoryDialog(Category? src = null)
        {
            InitializeComponent();
            if (src != null)
            {
                Title = "Edit Category";
                NameBox.Text = src.CategoryName ?? "";
                DescBox.Text = src.CategoryDesciption ?? "";
                ParentBox.Text = src.ParentCategoryId?.ToString() ?? "";
                ActiveBox.IsChecked = src.IsActive ?? true;
                Value = new Category { CategoryId = src.CategoryId };
            }
            else
            {
                ActiveBox.IsChecked = true;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            short? parent = null;
            if (short.TryParse(ParentBox.Text, out var val)) parent = val;

            if (Value == null) Value = new Category();
            Value.CategoryName = NameBox.Text;
            Value.CategoryDesciption = DescBox.Text;
            Value.ParentCategoryId = parent;
            Value.IsActive = ActiveBox.IsChecked ?? true;

            if (string.IsNullOrWhiteSpace(Value.CategoryName))
            {
                MessageBox.Show("Name is required.", "Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
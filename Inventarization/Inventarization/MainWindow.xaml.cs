using Inventarization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inventarization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();
            using (InventarizationContext db = new InventarizationContext())
            {
                statusUser.Text = $"{user.RoleNavigation.Name}: {user.Surname} {user.Name} {user.Patronymic}";
                productlistView.ItemsSource = db.Products.ToList();

                List<string> sortList = new List<string>() { "По возрастанию цены", "По убыванию цены" }; 
                List<string> filtertList = db.Products.Select(u => u.Manufacturer).Distinct().ToList();
                sortProductsComboBox.ItemsSource = sortList.ToList();
                filtertList.Insert(0, "Все производители");
                filterProductsComboBox.ItemsSource = filtertList.ToList();
                countTextBlock.Text = $"Количество: {db.Products.Count()}";
            }
            
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void UpdateProducts()
        {
            using (InventarizationContext db = new InventarizationContext())
            {

                var currentProducts = db.Products.ToList();
                productlistView.ItemsSource = currentProducts;

                //Сортировка
                if (sortProductsComboBox.SelectedIndex != -1)
                {
                    if (sortProductsComboBox.SelectedValue == "По убыванию цены")
                    {
                        currentProducts = currentProducts.OrderByDescending(u => u.Cost).ToList();

                    }

                    if (sortProductsComboBox.SelectedValue == "По возрастанию цены")
                    {
                        currentProducts = currentProducts.OrderBy(u => u.Cost).ToList();

                    }
                }


                // Фильтрация
                if (filterProductsComboBox.SelectedIndex != -1)
                {
                    if (db.Products.Select(u => u.Manufacturer).Distinct().ToList().Contains(filterProductsComboBox.SelectedValue))
                    {
                        currentProducts = currentProducts.Where(u => u.Manufacturer == filterProductsComboBox.SelectedValue.ToString()).ToList();
                    }
                    else
                    {
                        currentProducts = currentProducts.ToList();
                    }
                }

                // Поиск

                if (searchTextBox.Text.Length > 0)
                {

                    currentProducts = currentProducts.Where(u => u.Name.Contains(searchTextBox.Text) || u.Description.Contains(searchTextBox.Text)).ToList();

                }

                productlistView.ItemsSource = currentProducts;
                countTextBlock.Text = $"Количество: {currentProducts.Count} из {db.Products.ToList().Count}";
            }
        }

        private void sortProductsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (InventarizationContext db = new InventarizationContext())
            {
                if (sortProductsComboBox.SelectedValue == "По убыванию цены")
                {
                    productlistView.ItemsSource = db.Products.OrderByDescending(u => u.Cost).ToList();
                }

                if (sortProductsComboBox.SelectedValue == "По возрастанию цены")
                {
                    productlistView.ItemsSource = db.Products.OrderBy(u => u.Cost).ToList();
                }
            }
            UpdateProducts();

        }

        private void filterProductsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (InventarizationContext db = new InventarizationContext())
            {
                if (db.Products.Select(u => u.Manufacturer).Distinct().ToList().Contains(filterProductsComboBox.SelectedValue))
                {
                    productlistView.ItemsSource = db.Products.Where(u => u.Manufacturer == filterProductsComboBox.SelectedValue).ToList();
                }
                else
                {
                    productlistView.ItemsSource = db.Products.ToList();
                }
            }
            UpdateProducts();
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (InventarizationContext db = new InventarizationContext())
            {
                if (searchTextBox.Text.Length > 0)
                {
                    productlistView.ItemsSource = db.Products.Where(u => u.Name.Contains(searchTextBox.Text) || u.Description.Contains(searchTextBox.Text)).ToList();
                }
            }
            UpdateProducts();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            sortProductsComboBox.SelectedIndex = -1;
            filterProductsComboBox.SelectedIndex = -1;
        }
    }
}

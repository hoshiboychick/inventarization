using Inventarization.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inventarization
{
    /// <summary>
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        Product? currentProduct;
        string? oldImage;
        string? newImage;
        string? newImagePath;

        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void saveProductButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(articleNumberTextBox.Text))
                errors.AppendLine("Укажите артикул");
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                errors.AppendLine("Укажите название");
            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                errors.AppendLine("Укажите описание");
            if (string.IsNullOrWhiteSpace(manufacturerTextBox.Text))
                errors.AppendLine("Укажите производителя");
            if (string.IsNullOrWhiteSpace(costTextBox.Text))
                errors.AppendLine("Укажите цену");
            if (string.IsNullOrWhiteSpace(discountAmountTextBox.Text))
                errors.AppendLine("Укажите размер скидки");
            if (string.IsNullOrWhiteSpace(quantityInStockTextBox.Text))
                errors.AppendLine("Укажите количество на складе");

            //
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            using (InventarizationContext db = new InventarizationContext())
            {
                try
                {
                    Product product = new Product()
                    {

                        ArticleNumber = articleNumberTextBox.Text,
                        Name = nameTextBox.Text,
                        Description = descriptionTextBox.Text,
                        Photo = photoTextBox.Text, // "picture.png",
                        Manufacturer = manufacturerTextBox.Text,
                        Cost = Convert.ToDecimal(costTextBox.Text),
                        DiscountAmount = Convert.ToInt32(discountAmountTextBox.Text),
                        QuantityInStock = Convert.ToInt32(quantityInStockTextBox.Text),
                    };

                    if (product.Cost < 0)
                    {
                        MessageBox.Show("Цена должна быть положительной!");
                        return;
                    }

                    if (product.QuantityInStock < 0)
                    {
                        MessageBox.Show("Количество товаров на складе не может быть меньше нуля!");
                        return;
                    }

                    db.Products.Add(product);


                    if (String.IsNullOrEmpty(newImage))
                    {
                        product.Photo = "picture.png";
                        BitmapImage image = new BitmapImage(new Uri(product.ImagePath));
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        imageBoxPath.Source = image;
                    }
                    else
                    {
                        string newRelativePath = $"{System.DateTime.Now.ToString("HHmmss")}_{newImage}";
                        product.Photo = newRelativePath;

                        File.Copy(newImagePath, System.IO.Path.Combine(Environment.CurrentDirectory, $"images/{newRelativePath}"));

                        BitmapImage image = new BitmapImage(new Uri(product.ImagePath));
                        image.CacheOption = BitmapCacheOption.OnLoad;

                        imageBoxPath.Source = image;
                    }


                    db.SaveChanges();

                    MessageBox.Show("Продукт успешно добавлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                }
            }
        }

        private void addPhotoButton_Click(object sender, RoutedEventArgs e)
        {

            Stream myStream;

            if (currentProduct != null)
            {
                oldImage = System.IO.Path.Combine(Environment.CurrentDirectory, $"images/{currentProduct.Photo}");
            }
            else
            {
                oldImage = null;
            }

            // проверяем, есть ли изображение у товара и запоминаем путь к изображению сейчас
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {
                if ((myStream = dlg.OpenFile()) != null)
                {
                    dlg.DefaultExt = ".png";
                    dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                    dlg.Title = "Open Image";
                    dlg.InitialDirectory = "./";

                    // Предпросмотр изображения
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri(dlg.FileName);

                    image.DecodePixelWidth = 200;
                    image.DecodePixelHeight = 300;
                    imageBoxPath.Source = image;
                    image.EndInit();

                    try
                    {
                        newImage = dlg.SafeFileName;
                        newImagePath = dlg.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                myStream.Dispose();
            }

        }
    }
}

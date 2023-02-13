using Lopushok.DataBase;
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
using Lopushok.DataBase;
using Microsoft.Win32;
using System.IO;

namespace Lopushok.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddProductPage.xaml
    /// </summary>
    public partial class AddProductPage : Page
    {
        public static Product selectProduct { get; set; }
        public List<Workshop> workshops { get; set; }
        public List<Material> materials { get; set; }
        public List<ProductType> types { get; set; }
        public AddProductPage(Product product)
        {
            InitializeComponent();
            selectProduct = product;

            //workshops = Connection.connection.Workshops.ToList();
            //cbWork.ItemsSource = workshops;
            //cbWork.DisplayMemberPath = "Name";

            types = Connection.connection.ProductTypes.ToList();
            cbType.ItemsSource = types;
            cbType.DisplayMemberPath = "Name";

            if (product != null)
            {
                //cbWork.SelectedItem = product.Workshop;
                cbType.SelectedItem = product.ProductType;
                btnDelete.Visibility = Visibility.Visible;
            }

            materials = Connection.connection.Materials.ToList();
            cbMaterial.ItemsSource = materials;
            cbMaterial.DisplayMemberPath = "Name";

            DataContext = this;
        }

        private void btnBackClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductListPage());
        }

        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            if (IsArticleUnique())
            {
                MessageBox.Show("Артикул не уникален", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DataAccess.SaveProduct(selectProduct);
            }
            catch
            {
                MessageBox.Show("Введены некорректные значения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataAccess.SaveProduct(selectProduct);
            NavigationService.GoBack();
        }

        public bool IsArticleUnique()
        {
            return DataAccess.GetProducts().Any(product => product == selectProduct && product.ArticleNumber != selectProduct.ArticleNumber);
        }

        private void btnDeleteClick(object sender, RoutedEventArgs e)
        {
            var messageDelete = MessageBox.Show("Вы хотите удалить продукт?", "Внимание", MessageBoxButton.YesNoCancel);
            if (messageDelete == MessageBoxResult.Yes)
            {
                Connection.connection.Products.Remove(selectProduct);
                NavigationService.Navigate(new ProductListPage());
            }
        }

        private void cbMaterialSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMaterial.SelectedItem != null)
            {
                var selectMaterial = cbMaterial.SelectedItem as Material;
                var uniqMaterial = Connection.connection.ProductMaterials.FirstOrDefault(x => x.Material.ID == selectMaterial.ID && x.Product.ID == selectProduct.ID);
                if (uniqMaterial == null)
                {
                    ProductMaterial productMaterial = new ProductMaterial()
                    {
                        Product = selectProduct,
                        Material = selectMaterial
                    };
                    selectProduct.ProductMaterials.Add(productMaterial);
                }

                lvListMaterial.ItemsSource = selectProduct.ProductMaterials;
                lvListMaterial.Items.Refresh();
            }
        }

        private void lvListMaterialSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvListMaterial.SelectedItem != null)
            {
                var selectMaterial = lvListMaterial.SelectedItem as ProductMaterial;
                var messageDelete = MessageBox.Show("Вы хотите удалить материал?", "Внимание", MessageBoxButton.YesNoCancel);
                if (messageDelete == MessageBoxResult.Yes)
                {
                    selectProduct.ProductMaterials.Remove(selectMaterial);
                }
            }

            lvListMaterial.ItemsSource = selectProduct.ProductMaterials;
            lvListMaterial.Items.Refresh();
        }

        private void btnAddPhotoClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (fileDialog.ShowDialog().Value)
            {
                var image = File.ReadAllBytes(fileDialog.FileName);
                selectProduct.Image = image;

                imgPhoto.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void btnDelPhotoClick(object sender, RoutedEventArgs e)
        {
            var messageDelete = MessageBox.Show("Вы хотите удалить фото?", "Внимание", MessageBoxButton.YesNoCancel);
            if (messageDelete == MessageBoxResult.Yes)
            {
                selectProduct.Image = null;
                imgPhoto.Source = null;
            }
        }
    }
}
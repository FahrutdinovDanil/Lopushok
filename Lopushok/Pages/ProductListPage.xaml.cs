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

namespace Lopushok.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductListPage.xaml
    /// </summary>
    public partial class ProductListPage : Page
    {
        public List<Product> products { get; set; }
        public List<ProductType> types { get; set; }
        public List<Sorting> sortings { get; set; }
        public ProductListPage()
        {
            InitializeComponent();
            products = Connection.connection.Products.ToList();

            types = Connection.connection.ProductTypes.ToList();
            types.Insert(0, new ProductType { ID = 0, Title = "Все типы" });
            cbFiltr.ItemsSource = types;
            cbFiltr.DisplayMemberPath = "Name";

            sortings = new List<Sorting>()
            {
                new Sorting() {Id=0, Name ="Пусто"},
                new Sorting() {Id=1, Name ="Минимальная стоимость по убыванию"},
                new Sorting() {Id=2, Name ="Минимальная стоимость по возрастанию"},
                new Sorting() {Id=3, Name ="Наименование по убыванию"},
                new Sorting() {Id=4, Name ="Наименование по возрастанию"},
                new Sorting() {Id=5, Name ="Номер цеха по убыванию"},
                new Sorting() {Id=6, Name ="Номер цеха по возрастанию"}
            };
            cbSort.ItemsSource = sortings;
            cbSort.DisplayMemberPath = "Name";
            this.DataContext = this;
        }

        public void Filter(bool filtersChanged = true)
        {
            List<Product> filterProduct = products;

            if (tbSearch.Text.Trim().Length != 0)
            {
                filterProduct = filterProduct.Where(x => x.Title.Contains(tbSearch.Text.Trim())).ToList();
            }

            if (cbFiltr.SelectedItem != null)
            {
                var selectFiltr = cbFiltr.SelectedItem as ProductType;
                if (selectFiltr.ID != 0)
                {
                    filterProduct = filterProduct.Where(x => x.ProductType.ID == selectFiltr.ID).ToList();
                }
            }

            if (cbSort.SelectedItem != null)
            {
                var selectSort = cbSort.SelectedItem as Sorting;

                if (selectSort.Id == 1)
                {
                    filterProduct = filterProduct.OrderByDescending(x => x.MinCostForAgent).ToList();
                }
                else if (selectSort.Id == 2)
                {
                    filterProduct = filterProduct.OrderBy(x => x.MinCostForAgent).ToList();
                }
                else if (selectSort.Id == 3)
                {
                    filterProduct = filterProduct.OrderByDescending(x => x.Title).ToList();
                }
                else if (selectSort.Id == 4)
                {
                    filterProduct = filterProduct.OrderBy(x => x.MinCostForAgent).ToList();
                }
                else if (selectSort.Id == 5)
                {
                    filterProduct = filterProduct.OrderBy(x => x.ProductionWorkshopNumber).ToList();
                }
                else if (selectSort.Id == 6)
                {
                    filterProduct = filterProduct.OrderByDescending(x => x.ProductionWorkshopNumber).ToList();
                }
            }

            lvProducts.ItemsSource = filterProduct;
            lvProducts.Items.Refresh();
        }

        public class Sorting
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void tbSearchSelectionChanged(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void cbFiltrSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cbSortSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage(null));
        }


        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditPrice.Visibility = lvProducts.SelectedItems == null ? Visibility.Hidden : Visibility.Visible;
        }
        private void lvProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvProducts.SelectedItem != null)
            {
                var selectProd = lvProducts.SelectedItem as Product;
                NavigationService.Navigate(new AddProductPage(selectProd));
            }

        }
        private void btnEditPrice_Click(object sender, RoutedEventArgs e)
        {
            var products = lvProducts.SelectedItems.Cast<Product>().ToList();
            var result = new Windows.EditPriceWindow(products).ShowDialog();

            if (result.Value)
                MessageBox.Show("Цены успешно обновлены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

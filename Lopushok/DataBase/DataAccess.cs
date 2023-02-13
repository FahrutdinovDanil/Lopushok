using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lopushok.DataBase
{
    public class DataAccess
    {
        public static ObservableCollection<Product> GetProducts()
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>(Connection.connection.Products);
            return products;
        }
        public static void SaveProduct(Product product)
        {
            if (!GetProducts().Any(x => x == product))
                Connection.connection.Products.Add(product);

            Connection.connection.SaveChanges();
        }
    }
}

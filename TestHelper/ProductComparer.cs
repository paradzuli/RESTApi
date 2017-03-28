using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace TestHelper
{
    public class ProductComparer: IComparer, IComparer<Product>
    {
        public int Compare(object expected, object actuals)
        {
            var lhs = expected as Product;
            var rhs = actuals as Product;

            if(lhs==null || rhs==null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }

        public int Compare(Product expected, Product actual)
        {
            
            int temp=0;
            temp = expected.ProductId.CompareTo(actual.ProductId);
            return (temp != 0 ? temp: String.Compare(expected.ProductName, actual.ProductName, StringComparison.Ordinal));
        }
    }
}

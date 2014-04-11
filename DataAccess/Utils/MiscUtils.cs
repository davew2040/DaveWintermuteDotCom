using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Utils
{
    public static class MiscUtils
    {
        public static IEnumerable<T> OrderByIndexList<T>(this IEnumerable<T> collection, IEnumerable<int> orderList) 
        {
            var collectionTypeGenerics = collection.GetType().GetGenericArguments();
            if (collectionTypeGenerics.Length != 1)
            {
                throw new Exception("Parameter to OrderByIndexList must contain integer property named 'Id'.");
            }
            var typeInfo = collectionTypeGenerics.First();
            var idProperty = typeInfo.GetProperty("Id");
            if (null == idProperty || idProperty.PropertyType.FullName != "System.Int32")
            {
                throw new Exception("Parameter to OrderByIndexList must contain integer property named 'Id'.");
            }

            var pullList = collection.Select(x => x).ToList();
            List<T> returnList = new List<T>();

            foreach (var index in orderList)
            {
                var pullItem = pullList.Where(x => index == (int)idProperty.GetValue(x)).FirstOrDefault();
                if (pullItem != null)
                {
                    pullList.Remove(pullItem);
                    returnList.Add(pullItem);
                }
            }

            // push all remaining items
            foreach (var item in pullList)
            {
                returnList.Add(item);
            }

            return returnList;
        }
    }
}

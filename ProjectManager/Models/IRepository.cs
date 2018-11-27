using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Models
{
    interface IRepository<T> where T:class
    {
        T Find(Guid? guid);
        T Find(int? id);
        IEnumerable<T> GetCollections();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

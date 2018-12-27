using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ProjectManagementEntities db = null;
        protected DbSet<T> dbset = null;

        public Repository()
        {
            db = new ProjectManagementEntities();
            dbset = db.Set<T>();
        }

        public void Add(T entity)
        {
            dbset.Add(entity);
            db.SaveChanges();
        }

        public void Delete(T entity)
        {
            dbset.Remove(entity);
            db.SaveChanges();
        }

        public T Find(Guid? guid)
        {
            return dbset.Find(guid);
        }

        public T Find(int? id)
        {
            return dbset.Find(id);
        }

        public IEnumerable<T> GetCollections()
        {
            return dbset.ToList();
        }

        public void Update(T entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public T Find(Guid? id, Guid? pid)
        {
            return dbset.Find(id, pid);
        }

        public void AddList(IEnumerable<T> entityList)
        {
            foreach (var i in entityList)
            {
                dbset.Add(i);
            }

            db.SaveChanges();
        }

        public void UpdateList(IEnumerable<T> entityList)
        {
            foreach (var i in entityList)
            {
                db.Entry(i).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        public void DeleteList(IEnumerable<T> entityList)
        {
            foreach (var i in entityList)
            {
                dbset.Remove(i);
            }

            db.SaveChanges();
        }
    }
}
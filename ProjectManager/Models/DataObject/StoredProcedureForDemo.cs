using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace ProjectManager.Models
{
    public class StoredProcedureForDemo
    {
        public StoredProcedureForDemo()
        {

        }
        public StoredProcedureForDemo(string storedProcedureName)
        {
            this.StoredProcedureName = storedProcedureName;
        }

        private ProjectManagementEntities db = new ProjectManagementEntities();
        public string StoredProcedureName { get; set; }
        public enum StoredProcedures
        {
            InsertProjectForDemo = 1, InsertTasksForDemo
        }

        public void Execute()
        {
            StoredProcedures _storedProcedures;
            if (Enum.TryParse(this.StoredProcedureName, out _storedProcedures))
            {
                try
                {
                    switch ((int)_storedProcedures)
                    {
                        case 1:
                            db.InsertProjectForDemo();
                            break;
                        case 2:
                            db.InsertTasksForDemo();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
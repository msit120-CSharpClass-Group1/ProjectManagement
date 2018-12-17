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

        public void Execute()
        {            
            if (StoredProcedureName == "InsertProjectForDemo")
            {                
                try
                {                    
                    db.InsertProjectForDemo();
                }
                catch (Exception)
                {

                }
                
            }
            else if(StoredProcedureName == "InsertTasksForDemo")
            {                
                try
                {                    
                    db.InsertTasksForDemo();
                }
                catch (Exception)
                {

                }
                
            }
           
        }
    }
}
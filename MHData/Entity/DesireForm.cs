using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace MHData.Entity
{
    public class DesireForm
    {


        [Key]
        public int ID { get; set; }
        public int SNo { get; set; }
        [Required(ErrorMessage = "Please enter Employee Name.")]
        public string NameOfEmployee { get; set; }
        
        public string NameOfEmployeeHindi {get;set;}
        [Required(ErrorMessage = "Please enter employee post.")]
        public String Post { get; set; }
        public String Department { get; set; }
        [Required(ErrorMessage = "Please enter employee current Location.")]
        public String CurrentLocation { get; set; }
        [Required(ErrorMessage = "Please enter employee current district.")]
        public String CurrentDistrict { get; set; }
        [Required(ErrorMessage = "Please enter employee Desire Location.")]
        public String DesireLocation { get; set; }
        [Required(ErrorMessage = "Please enter employee desire district.")]
        public String DesireDistrict { get; set; }
        [Required(ErrorMessage = "Please enter Desire type")]
        public String DesireType { get; set; }
        public List<ReferencesForDesire> Referencees { get; set; }
        public String MinisterDirections { get; set; }
        public String Comments { get; set; }
        public String Sepciality { get; set; }


    }
    public class ReferencesForDesire
    {
        public int ID { get; set; }
        public String ReferenceName { get; set; }
        public String ReferencePost { get; set; }
        public int DesireID { get; set; }


    }

    public class QueryToRun
    {
        public String Query { get; set; }
        public int QueryType { get; set; }
    }

    public enum QueryType
    {
        ExecuteNonQuery = 1,
        ExecuteScalar = 2,
        ExecuteDataTable = 3
    } 
    //public class DesireFormList
    //{
    //    public List<DesireForm> DesireFormItemList{get;set;}  
    //}
    //public class MAndHEntitesContext : DbContext 
    //{

    //    public MAndHEntitesContext()
    //        : base("DefaultConnection")
    //    {
    //    }
    //    public DbSet<DesireForm> DesireForm { get; set; }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<DesireForm>();
    //        base.OnModelCreating(modelBuilder);
    //    }
        
    //} 
}

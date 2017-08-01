using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace MandHData
{
    public class DesireForm
    {


        [Key]
        public int SNo {get;set;}
        [Required]
        public string NameOfEmployee {get;set;}
        [Required]
        public String Post {get;set;}
        public String Department {get;set;}
        [Required]
        public String CurrentLocation {get;set;}
        [Required]
        public String CurrentDistrict{get;set;}
        [Required]
        public String DesireLocation {get;set;}
        [Required]
        public String DesireDistrict {get;set;}
        [Required]
        public String DesireType {get;set;}
        public String ReferenceName {get;set;}
        public String ReferencePost {get;set;}
        public String MinisterDirections {get;set;}
        public String Comments {get;set;}

    }
    public class DesireFormList
    {
        public List<DesireForm> DesireFormItemList{get;set;}  
    }
    public class MAndHEntitesContext : DbContext 
    {

        public MAndHEntitesContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<DesireForm> DesireForm { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DesireForm>();
            base.OnModelCreating(modelBuilder);
        }
        
    } 
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedicalAndHealthWeb.Models
{
    public class FilterModel
    {
        public String SelectedReferenceName { get; set; }
        public String SelectedDepartment { get; set; }
        public String SelectedPost{ get; set; }
        public String SelectedEmployee { get; set; }
        public String Speciality { get; set; }
        public String DispatchNumber { get; set; }
        public String DispatchTo { get; set; }
        public Boolean DispatchedDesire { get; set; }
        public int FilterFor { get; set; }
    }
}
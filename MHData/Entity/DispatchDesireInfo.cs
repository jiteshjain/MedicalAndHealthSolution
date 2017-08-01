using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHData.Entity
{
    public class DispatchDesireInfo
    {
        public int ID { get; set; }
        public string DispatchNumber { get; set; }
        public string DispatchDate { get; set; }
        public string DispatchTo { get; set; }
        public List<DispatchedDesires> DesiresToDispatch { get; set; }
    }
    public class DispatchedDesires
    { 
        public int DispatchID { get; set; }      
        public int DesireId { get; set; }
    }
}

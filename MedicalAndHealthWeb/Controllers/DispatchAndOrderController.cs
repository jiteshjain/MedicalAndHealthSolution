using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MHData;
using MHData.Entity;
using MedicalAndHealthWeb.Models;
 

namespace MedicalAndHealthWeb.Controllers
{
    public class DispatchAndOrderController : Controller
    {
        DesireForms context = new DesireForms();

        public void BindViewBags()
        {
             
           ((dynamic)ViewBag).ReferenceList = context.GetReferenceLlist();
           ((dynamic)ViewBag).Departments = context.GetDesireDept();
           ((dynamic)ViewBag).Posts = context.GetDesirePost();
           ((dynamic)ViewBag).Employees = context.GetDesireEmp();
           ((dynamic)ViewBag).SpecialityList = context.GetDesireSpeciality();
 
        }
        public ActionResult Index()
        {
            FilterModel filterValues = null;
            if (Session["FilterValues"] == null)
            {
                filterValues = new FilterModel()
                {
                    DispatchTo = String.Empty,
                    DispatchNumber = String.Empty, 
                };
            }
            else
            {
                filterValues = (FilterModel)Session["FilterValues"]; 
            }
            ((dynamic)ViewBag).DispatchToList = context.GetDispatchToList();
            filterValues.FilterFor = 1;
            Session["FilterValues"] = filterValues;
            return View(context.GetFilteredDispatchList(filterValues.DispatchTo.Trim(), filterValues.DispatchNumber));
              
        }
         
        public ActionResult GetFilteredDesires(String referenceName, string department, string post, string empName,String speciality)
        {
            FilterModel filterValues = new FilterModel()
            {
                SelectedReferenceName = referenceName,
                SelectedDepartment = department,
                SelectedEmployee = empName,
                SelectedPost = post,
                Speciality = speciality
            };

            List<DesireForm> filterDesire = context.GetFilterDesire(referenceName.Trim(), department.Trim(), post.Trim(), empName.Trim(), speciality);
            Session["FilterValues"] = filterValues;
            return PartialView("_ListView", filterDesire);

        }
        /// <summary>
        /// Method to Dispatch Desire (Calls on click of Add To Desire Button in Grid view)
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DispatchDate"></param>
        /// <param name="DispatchNumber"></param>
        /// <param name="DispatchTo"></param>
        /// <returns></returns>
        public ActionResult AddDesireToDispatch(int ID, string DispatchDate, string DispatchNumber, string DispatchTo)
        {
            BindViewBags();
            DispatchDesireInfo dispatchDesire = (DispatchDesireInfo)Session["DesireToDispatch"];
            if (dispatchDesire == null)
                dispatchDesire = new DispatchDesireInfo();

            if(dispatchDesire.DesiresToDispatch==null)
            {
                List<DispatchedDesires> dDesires = new List<DispatchedDesires>();
                dDesires.Add(new DispatchedDesires() { DesireId = ID, DispatchID = dispatchDesire.ID });
                dispatchDesire.DesiresToDispatch = dDesires;
            }
            else
                dispatchDesire.DesiresToDispatch.Add(new DispatchedDesires() {DesireId=ID,DispatchID=dispatchDesire.ID});// += ID + ",";
            dispatchDesire.DispatchDate = DispatchDate;
            dispatchDesire.DispatchNumber = DispatchNumber;
            dispatchDesire.DispatchTo = DispatchTo;
            Session["DesireToDispatch"] = dispatchDesire;

            FilterModel filterValues = (FilterModel)Session["FilterValues"];
            if (filterValues == null)
            {
                filterValues = new FilterModel();

                filterValues.SelectedReferenceName = "";
                filterValues.SelectedDepartment = "";
                filterValues.SelectedEmployee = "";
                filterValues.SelectedPost = "";
                filterValues.Speciality = "";
            }
            else
            {
                if (filterValues.FilterFor == 1)
                {
                    filterValues.SelectedReferenceName = "";
                    filterValues.SelectedDepartment = "";
                    filterValues.SelectedEmployee = "";
                    filterValues.SelectedPost = "";
                    filterValues.Speciality = "";
                }
            }
            return Json("Done");// View("DispatchDesire", context.GetFilterDesire(filterValues.SelectedReferenceName.Trim(), filterValues.SelectedDepartment.Trim(), filterValues.SelectedPost.Trim(), filterValues.SelectedEmployee.Trim(),filterValues.SelectedReferenceName));
        }

        public JsonResult DeleteDispatchDesireFromSession(int DesireId)
        {
            DispatchDesireInfo dispatchDesire = (DispatchDesireInfo)Session["DesireToDispatch"];
            List<DesireForm> dispatchDesireList = new List<DesireForm>();
            if (dispatchDesire.DesiresToDispatch != null)
            {
                dispatchDesire.DesiresToDispatch.RemoveAll(x => x.DesireId == DesireId);
                Session["DesireReferences"] = dispatchDesire;
                return Json("Done");
            }
            return Json("error");
        }

        /// <summary>
        /// Used to edit Dispatch Information (calls on Edit button in Dispatch List Grid View)
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DispatchDate"></param>
        /// <param name="DispatchNumber"></param>
        /// <param name="DispatchTo"></param>
        /// <returns></returns>
        public ActionResult AddEditDispatchInfo(int ID=0)
        {
            BindViewBags();

             DispatchDesireInfo dispatchInfoToAddEdit =null;
            if (ID > 0)
            {
                dispatchInfoToAddEdit = context.GetDispatchDetailToEdit(ID.ToString());
            }
            else
            {
               
                dispatchInfoToAddEdit = new DispatchDesireInfo();
            }
            if (dispatchInfoToAddEdit != null)
            {

                Session["DesireToDispatch"] = dispatchInfoToAddEdit;

                FilterModel filterValues = (FilterModel)Session["FilterValues"];
                if (filterValues == null)
                        filterValues = new FilterModel();

                    filterValues.SelectedReferenceName = "";
                    filterValues.SelectedDepartment = "";
                    filterValues.SelectedEmployee = "";
                    filterValues.SelectedPost = "";
                    filterValues.Speciality= ""; 

            }
            return Json("Done");
          }

        public ActionResult GetCurrentDispatchDesire()
        {
            DispatchDesireInfo dispatchDesire = (DispatchDesireInfo)Session["DesireToDispatch"];
            List<DesireForm> dispatchDesireList =new List<DesireForm> ();
            if (dispatchDesire != null)
            {
                if (dispatchDesire.DesiresToDispatch != null)
                {
                    string diesireIds = string.Join(",", dispatchDesire.DesiresToDispatch.Select(i => i.DesireId).ToArray());
                    if (!String.IsNullOrEmpty(diesireIds))
                        dispatchDesireList = context.GetDispatchDesireList(diesireIds);
                }
            }
             
            return Json(dispatchDesireList,JsonRequestBehavior.AllowGet);
        }
        public ActionResult SubmitDispatchInfo(DispatchDesireInfo dispatchInfo)
        {
            DispatchDesireInfo dispatchDesire = (DispatchDesireInfo)Session["DesireToDispatch"];
            dispatchDesire.DispatchDate = dispatchInfo.DispatchDate;
            dispatchDesire.DispatchNumber = dispatchInfo.DispatchNumber;
            dispatchDesire.DispatchTo= dispatchInfo.DispatchTo;
            
            int  result = context.SaveAndUpdateDesireDispatch(dispatchDesire);
            return (result==0)?Json("error"):Json("saved");
        }
       
        public ActionResult DispatchedDesire()
        {
            BindViewBags();
            FilterModel filterValues = (FilterModel)Session["FilterValues"];
            if (filterValues == null)
                filterValues = new FilterModel()
                {
                    SelectedReferenceName = "",
                    SelectedDepartment = "",
                    SelectedEmployee = "",
                    SelectedPost = "",
                    Speciality="",
                    FilterFor = 0
                };

            if (filterValues.FilterFor == 1)
            {
                filterValues.SelectedReferenceName = "";
                filterValues.SelectedDepartment = "";
                filterValues.SelectedEmployee = "";
                filterValues.SelectedPost = ""; 
                filterValues.Speciality = "";
            }

            return View("DispatchDesire", context.GetFilterDesire(filterValues.SelectedReferenceName.Trim(), filterValues.SelectedDepartment.Trim(), filterValues.SelectedPost.Trim(), filterValues.SelectedEmployee.Trim(), filterValues.Speciality.Trim()));

        }
        public ActionResult GetFilteredDispatchList(String dispatchTo, string dispatchNo)
        {
              FilterModel filterValues = null;
            if (Session["FilterValues"] == null)
            {
                filterValues = new FilterModel()
                {
                    DispatchNumber = dispatchNo,
                    DispatchTo = dispatchTo
                };
            }
            else
            {
                filterValues = (FilterModel)Session["FilterValues"];
                filterValues.DispatchTo = dispatchTo;
                filterValues.DispatchNumber = dispatchNo;
            }
             
            List<DispatchDesireInfo> filterDispatchList = context.GetFilteredDispatchList(dispatchTo, dispatchNo);
            filterValues.FilterFor = 0;
            Session["FilterValues"] = filterValues;
            
            return PartialView("_DispatchedListView", filterDispatchList);
            

        }
        public JsonResult DeleteDispatchedDesire(int ID)
        { 
            int result = context.DeleteDesireDispatch(ID);
            return (result == 0) ? Json("error") : Json("delete");
             
        }
    }
}

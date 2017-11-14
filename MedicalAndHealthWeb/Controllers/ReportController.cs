using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using MHData.Entity;
using MHData;
using MedicalAndHealthWeb.Models;
namespace MedicalAndHealthWeb.Controllers
{
    public class ReportController : Controller
    {
        DesireForms context = new DesireForms();
        // GET: /Report/
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
            BindViewBags();
            return View(context.GetFilterDesire("".Trim(), "".Trim(), "".Trim(), "".Trim(), ""));
        }
        public ActionResult GetFilteredDesires(String referenceName, string department, string post, string empName,Boolean isDispatchedDesires)
        {
            FilterModel filterValues = new FilterModel()
            {
                SelectedReferenceName = referenceName,
                SelectedDepartment = department,
                SelectedEmployee = empName,
                SelectedPost = post
            //    Speciality = speciality
            };

            List<DesireForm> filterDesire = context.GetFilterDesire(referenceName.Trim(), department.Trim(), post.Trim(), empName.Trim(),"");
            Session["FilterValues"] = filterValues;
            return PartialView("_ListView", filterDesire);

        } 

        public void ExportData(string selectedIds)
        {
            selectedIds = selectedIds.Substring(0, selectedIds.LastIndexOf(","));
            context.ExportToExcel(selectedIds);
        }
    }
}

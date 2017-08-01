using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MHData.Entity;
using MHData;
 

namespace MedicalAndHealthWeb.Controllers
{
    public class DesireController : Controller
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
            BindViewBags();
            Session["DesireReferences"] = new List<ReferencesForDesire>();
            return View(context.DesireFormsList());
        }

        public ActionResult AddDesire()
        {
            int nextSNo = context.GetLastDesire().SNo + 1;
            Session["DesireReferences"] = new List<ReferencesForDesire>();
            return View("AddEditDesire", new DesireForm() { SNo = nextSNo, Referencees=new List<ReferencesForDesire>() });
        }
        public ActionResult EditDesire(int ID)
        {
            DesireForm desireFormToEdit= context.GetRecordByID(ID);
            desireFormToEdit.Referencees = context.DeisreReferenceeList(ID);
            Session["DesireReferences"] = desireFormToEdit.Referencees;
            return View("AddEditDesire", desireFormToEdit);
        }
        public ActionResult SaveDesire(MHData.Entity.DesireForm desireForm)
        {
            if (ModelState.IsValid)
            {
                desireForm.Referencees = (List<ReferencesForDesire>)Session["DesireReferences"];
                //string empnamehindi = "";// desireform.nameofemployeehindi;
                //empnamehindi = empnamehindi.replace("'", "\'");
                desireForm.NameOfEmployeeHindi = "";
                int nextSNo = context.SaveAndUpdate(desireForm);
                return RedirectToAction("Index");
            }
            desireForm.Referencees = (List<ReferencesForDesire>)Session["DesireReferences"];
            return View("AddEditDesire", desireForm);


        }
        public JsonResult AddDesireRefrence(ReferencesForDesire refForDesire)
        {
           List<ReferencesForDesire> desireRef = (List<ReferencesForDesire>)Session["DesireReferences"];
            if(String.IsNullOrEmpty(refForDesire.ReferenceName) || String.IsNullOrEmpty(refForDesire.ReferencePost))
                return Json("InvalidInput");
            if (desireRef.Exists(x => x.ReferenceName.Trim() == refForDesire.ReferenceName.Trim() && x.ReferencePost.Trim() == refForDesire.ReferencePost.Trim()))
                return Json("AlreadyExist");
            desireRef.Add(refForDesire);
            return Json(desireRef);
        }
        public JsonResult DelteDesireRefrence(ReferencesForDesire refForDesire)
        {
            List<ReferencesForDesire> desireRef = (List<ReferencesForDesire>)Session["DesireReferences"];

            ReferencesForDesire referenceObj = desireRef.Find(x => x.ReferenceName.Trim() == refForDesire.ReferenceName.Trim() && x.ReferencePost.Trim() == refForDesire.ReferencePost.Trim());
            if(referenceObj!=null)
                desireRef.Remove(referenceObj);
            return Json(desireRef);
        }
        public ActionResult GetFilteredDesires(String referenceName, string department,string post,string empName,string speciality)
        {
            List<DesireForm> filterDesire = context.GetFilterDesire(referenceName.Trim(), department.Trim(), post.Trim(), empName.Trim(), speciality.Trim());
            return PartialView("_ListView", filterDesire);
             
        }


    }
}

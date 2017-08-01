﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.OleDb;
using System.Reflection;
using System.Data;

using MHData.Entity;

namespace MHData
{
    public class DesireForms
    {
        DataTable dtReport = null;
        public int SaveAndUpdate(DesireForm obj)
        {
            int retVal = 0;
            SqlQuery ab = new SqlQuery();
             
                 OleDbConnection conn = new OleDbConnection(OleDbHelper.ACCESS_CONNECTIONSTRING);
                 conn.Open();

                 OleDbTransaction trans = conn.BeginTransaction();
                 try
                {
                    int result =0;
                    if (obj.ID == 0)
                        retVal = OleDbHelper.ExecuteNonQuery(conn, trans, ab.InsertDesire(obj));
                    else
                        retVal = OleDbHelper.ExecuteNonQuery(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.UpdateDesire(obj));
                    Int32 identity = 0;
                    if (retVal > 0)
                    {
                        if (obj.ID == 0)
                            identity = Convert.ToInt32(OleDbHelper.ExecuteScalar(conn, trans, "Select @@Identity"));
                        else
                            identity = obj.ID;

                        retVal = OleDbHelper.ExecuteNonQuery(conn, trans, "Delete from ReferencesForDesire where DesireId = " + identity);
                        foreach(ReferencesForDesire refForDesire in obj.Referencees)
                        {
                            string refQuery = ab.InsertDesireReferences(refForDesire, identity);
                            retVal = OleDbHelper.ExecuteNonQuery(conn, trans, refQuery);
                        }
                       
                        if (retVal > 0)
                        {
                            trans.Commit();
                            conn.Close();
                        }

                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    conn.Close();
                    return retVal = 0;
                } 
            return retVal;

        }
        public List<DesireForm> DesireFormsList()
        {
            SqlQuery ab = new SqlQuery(); 
            DataTable dt  = OleDbHelper.ExecuteDataTable (OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DesireForm"));
            List<DesireForm> desireForms = new List<DesireForm>();
            DesireForm desireFrm = null;
            if (dt != null)
            { 
                foreach (DataRow dr in dt.Rows)
                {
                    desireFrm = new DesireForm();
                    PropertyInfo[] propertiesInfo = desireFrm.GetType().GetProperties();
                    foreach (PropertyInfo prop in propertiesInfo)
                    {
                        if (prop.PropertyType == typeof(List<ReferencesForDesire>))
                        {
                            prop.SetValue(desireFrm, DeisreReferenceeList(desireFrm.ID));
                        }
                        else
                        {
                           if (dr[prop.Name]  is System.DBNull)
                                prop.SetValue(desireFrm, String.Empty);
                           else
                               prop.SetValue(desireFrm, dr[prop.Name]);
                        }

                       
                    }  desireForms.Add(desireFrm);
                }
                dt.Dispose();
            } 
              
            return desireForms;
        }
        public List<ReferencesForDesire> DeisreReferenceeList(Int32 desireId)
        {
            SqlQuery ab = new SqlQuery();
            OleDbParameter cmdParam = new OleDbParameter();
            cmdParam.ParameterName = "DesireId";
            cmdParam.Value=  desireId;
            DataTable dataReader = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("RefAndDesire"),cmdParam);
            List<ReferencesForDesire> referenceeList = new List<ReferencesForDesire>();
            foreach(DataRow dr in dataReader.Rows)
            {
                ReferencesForDesire referencee = new ReferencesForDesire();

                PropertyInfo[] propertiesInfo = referencee.GetType().GetProperties();
                foreach (PropertyInfo prop in propertiesInfo)
                {
                    prop.SetValue(referencee, (dr[prop.Name]==DBNull.Value)?String.Empty:dr[prop.Name]);
                }
                referenceeList.Add(referencee);

            }
            return referenceeList;
        }
        public DesireForm GetLastDesire()
        {
            SqlQuery ab = new SqlQuery();
            OleDbDataReader dataReader = OleDbHelper.ExecuteReader(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetLastRecord("DesireForm"));
            DesireForm desireFrm = new DesireForm();
            while (dataReader.Read())
            { 
                PropertyInfo[] propertiesInfo = desireFrm.GetType().GetProperties();
                foreach (PropertyInfo prop in propertiesInfo)
                {
                    if (prop.PropertyType == typeof(List<ReferencesForDesire>))
                        prop.SetValue(desireFrm, new List<ReferencesForDesire>());
                    else
                    {
                        if(dataReader[prop.Name] is System.DBNull)
                            prop.SetValue(desireFrm, String.Empty);
                        else
                            prop.SetValue(desireFrm, dataReader[prop.Name]);
                    }
                } 
            }
            return desireFrm;
        }
        public DesireForm GetRecordByID(int id)
        {
            SqlQuery ab = new SqlQuery();
            OleDbDataReader dataReader = OleDbHelper.ExecuteReader(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecordById("DesireForm",id));
            DesireForm desireFrm =null;
            while (dataReader.Read())
            {
                desireFrm = new DesireForm();
                PropertyInfo[] propertiesInfo = desireFrm.GetType().GetProperties();
                foreach (PropertyInfo prop in propertiesInfo)
                {
                    if (prop.PropertyType == typeof(List<ReferencesForDesire>))
                    {
                        prop.SetValue(desireFrm, DeisreReferenceeList(id));
                    }
                    else
                    {
                        if (dataReader[prop.Name] is System.DBNull)
                            prop.SetValue(desireFrm, String.Empty);
                        else
                            prop.SetValue(desireFrm, dataReader[prop.Name]);
                    }
                }
            }
            return desireFrm;
        }

        public string  GetDesireIdsByRefName(String referenceName)
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetDesireByReference("ReferencesForDesire", referenceName));
            string resultStr = String.Empty;
            if (dt != null)
            {
                foreach (DataRow strID in dt.Rows)
                {
                    resultStr += strID["DesireId"].ToString()+",";
                }
                resultStr += "#";
            }
            resultStr = resultStr.Replace(",#", String.Empty);
            return resultStr;
        }
        public List<String> GetReferenceLlist()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("ReferencesForDesire"));
            
            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["ReferenceName"].ToString()))
                        continue;
                    dataList.Add(dr["ReferenceName"].ToString().ToUpper());
                }
            }
            return dataList;
        }
        public List<String> GetDesireDept()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DesireForm"));

            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["Department"].ToString()))
                        continue;
                    dataList.Add(dr["Department"].ToString().ToUpper());
                }
            }
            return dataList;
        }
        public List<String> GetDesirePost()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DesireForm"));

            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["Post"].ToString()))
                        continue;
                    dataList.Add(dr["Post"].ToString().ToUpper());
                }
            }
            return dataList;
        } 
        public List<String> GetDesireEmp()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DesireForm"));

            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["NameOfEmployee"].ToString()))
                        continue;
                    dataList.Add(dr["NameOfEmployee"].ToString().ToUpper());
                }
            }
            return dataList;
        }
        public List<String> GetDesireSpeciality()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DesireForm"));

            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["Sepciality"].ToString()))
                        continue;
                    dataList.Add(dr["Sepciality"].ToString().ToUpper());
                }
            }
            return dataList;
        }
         public List<String> GetDispatchToList()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DispatchDesireInfo"));

            List<String> dataList = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.IsNullOrEmpty(dr["DispatchTo"].ToString()))
                        continue;
                    dataList.Add(dr["DispatchTo"].ToString().ToUpper());
                }
            }
            return dataList;
        }

         public List<DesireForm> GetFilterDesire(string ReferenceName, string Department, string Post, string NameOfEmployee, string Speciality)
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetFilterDesireByParam("DesireForm", ReferenceName, Department, Post, NameOfEmployee,Speciality));
  List<DesireForm> desireForms = new List<DesireForm>();
            if (dt != null)
            {
              
                DesireForm desireFrm = null;
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        desireFrm = new DesireForm();
                        PropertyInfo[] propertiesInfo = desireFrm.GetType().GetProperties();
                        foreach (PropertyInfo prop in propertiesInfo)
                        {
                            if (prop.PropertyType == typeof(List<ReferencesForDesire>))
                                continue;

                            if (dr[prop.Name] is System.DBNull)
                                prop.SetValue(desireFrm, String.Empty);
                            else 
                                prop.SetValue(desireFrm, dr[prop.Name]);


                        } 
                        desireForms.Add(desireFrm);
                    }

                    dt.Dispose();
                }

                
            }
            return desireForms;
        }   
        
        public List<DesireForm> GetDispatchDesireList(string dispatchIds)
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetDispatchDesireList("DesireForm", dispatchIds));
            List<DesireForm> desireForms = new List<DesireForm>();
            if (dt != null)
            {
              
                DesireForm desireFrm = null;
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        desireFrm = new DesireForm();
                        PropertyInfo[] propertiesInfo = desireFrm.GetType().GetProperties();
                        foreach (PropertyInfo prop in propertiesInfo)
                        {
                            if (prop.PropertyType == typeof(List<ReferencesForDesire>))
                                continue;

                            if (dr[prop.Name] is System.DBNull)
                                prop.SetValue(desireFrm, String.Empty);
                            else
                                prop.SetValue(desireFrm, dr[prop.Name]);


                        } 
                        desireForms.Add(desireFrm);
                    }
                    dt.Dispose();
                }
                 
            }
            return desireForms;
        }


        public int SaveAndUpdateDesireDispatch(DispatchDesireInfo obj)
        {
            int retVal = 0;
            SqlQuery ab = new SqlQuery();

            OleDbConnection conn = new OleDbConnection(OleDbHelper.ACCESS_CONNECTIONSTRING);
            conn.Open();

            OleDbTransaction trans = conn.BeginTransaction();
            try
            {
                
                if (obj.ID == 0)
                    retVal = OleDbHelper.ExecuteNonQuery(conn, trans, ab.InsertDispatch(obj));
                else
                    retVal = OleDbHelper.ExecuteNonQuery(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.UpdateDispatch(obj));
                Int32 identity = 0;
                if (retVal > 0)
                {
                    if (obj.ID == 0)
                        identity = Convert.ToInt32(OleDbHelper.ExecuteScalar(conn, trans, "Select @@Identity"));
                    else
                        identity = obj.ID;

                    retVal = OleDbHelper.ExecuteNonQuery(conn, trans, "Delete from DispatchedDesires where DispatchID = " + identity);
                    foreach(DispatchedDesires dd in obj.DesiresToDispatch)
                    {
                        string refQuery = ab.InsertDesireToDispatch(dd, identity);
                        retVal = OleDbHelper.ExecuteNonQuery(conn, trans, refQuery);
                    }

                    if (retVal > 0)
                    {
                        trans.Commit();
                        conn.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                conn.Close();
                return retVal = 0;
            }
            return retVal;



        }

        public DispatchDesireInfo GetDispatchDetailToEdit(string dispatchId)
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetDispatchDetailToEdit("DispatchDesireInfo", dispatchId));
            DispatchDesireInfo dispatchInfo = null;
            if (dt != null)
            { 
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatchInfo = new DispatchDesireInfo();
                        PropertyInfo[] propertiesInfo = dispatchInfo.GetType().GetProperties();
                        foreach (PropertyInfo prop in propertiesInfo)
                        {
                            if (prop.PropertyType == typeof(List<DispatchedDesires>))
                            {
                                prop.SetValue(dispatchInfo, DeisreToDispatchList(dispatchInfo.ID));
                            }
                            else
                            {
                                if (dr[prop.Name].GetType() == typeof(DateTime))
                                    prop.SetValue(dispatchInfo, ((DateTime)dr[prop.Name]).ToShortDateString());
                                else
                                    prop.SetValue(dispatchInfo, dr[prop.Name]);
                            }
                             
                        } 
                    }
                    dt.Dispose();
                } 
            }
            return dispatchInfo;
        }
        public List<DispatchedDesires> DeisreToDispatchList(Int32 dispathId)
        {
            SqlQuery ab = new SqlQuery();
            OleDbParameter cmdParam = new OleDbParameter();
            cmdParam.ParameterName = "DispatchID";
            cmdParam.Value = dispathId;
            DataTable dataReader = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ""ab.GetRecord(""), cmdParam);
            List<DispatchedDesires> dispatchedDesireList = new List<DispatchedDesires>();
            foreach (DataRow dr in dataReader.Rows)
            {
                DispatchedDesires dispatchedDesire = new DispatchedDesires();

                PropertyInfo[] propertiesInfo = dispatchedDesire.GetType().GetProperties();
                foreach (PropertyInfo prop in propertiesInfo)
                {
                    prop.SetValue(dispatchedDesire, (dr[prop.Name] == DBNull.Value) ? String.Empty : dr[prop.Name]);
                }
                dispatchedDesireList.Add(dispatchedDesire);

            }
            return dispatchedDesireList;
        }
        public List<DispatchDesireInfo> GetDispatchedList()
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetRecord("DispatchDesireInfo"));
            List<DispatchDesireInfo> dispatchedList = new List<DispatchDesireInfo>();
            DispatchDesireInfo dispatchedDesire = null;
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dispatchedDesire = new DispatchDesireInfo();
                    PropertyInfo[] propertiesInfo = dispatchedDesire.GetType().GetProperties();
                    foreach (PropertyInfo prop in propertiesInfo)
                    {
                        prop.SetValue(dispatchedDesire, dr[prop.Name]); 
                    }
                    dispatchedList.Add(dispatchedDesire);
                }
                dt.Dispose();
            }

            return dispatchedList;
        }
        public List<DispatchDesireInfo> GetFilteredDispatchList(string dispatchTo, string dispatchNo)
        {
            SqlQuery ab = new SqlQuery();
            DataTable dt = OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, ab.GetFilterDispatchByParam("DispatchDesireInfo", dispatchTo, dispatchNo));
            List<DispatchDesireInfo> dispatchList = new List<DispatchDesireInfo>();
            if (dt != null)
            {

                DispatchDesireInfo dispatchInfo= null;
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dispatchInfo = new DispatchDesireInfo();
                        PropertyInfo[] propertiesInfo = dispatchInfo.GetType().GetProperties();
                        foreach (PropertyInfo prop in propertiesInfo)
                        {
                            //new DateTime(Convert.ToInt32(prop.Name.Split("/".ToCharArray())[2]), Convert.ToInt32(prop.Name.Split("/".ToCharArray())[1]), Convert.ToInt32(prop.Name.Split("/".ToCharArray())[0])))
                            if (prop.PropertyType == typeof(List<DispatchedDesires>))
                                continue;
                            else if (dr[prop.Name].GetType() == typeof(DateTime))
                                prop.SetValue(dispatchInfo, ((DateTime)dr[prop.Name]).ToShortDateString());
                            else
                                prop.SetValue(dispatchInfo, dr[prop.Name]); 
                        }
                        dispatchList.Add(dispatchInfo);
                    }
                    dt.Dispose();
                } 
            }
            return dispatchList;
        }

        public void ExportToExcel()
        {
            

           OleDbHelper.ExportToExcel( GetDataToExport("4,11,"), "D:\\DesireReport.xlsx");
          

        }

        public DataTable  GetDataToExport(string dispatchIds)
        {
            string [] _dispatchIds = dispatchIds.Split(",".ToCharArray());
            
            String selectdispatchDetail = "Select * from DispatchDesireInfo";
            DataTable dtDispatchInfo =OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, selectdispatchDetail);

            String selectDesires = "Select * from DesireFormAndReferencesQry";
            DataTable dtdesires =OleDbHelper.ExecuteDataTable(OleDbHelper.ACCESS_CONNECTIONSTRING, selectDesires);
            dtReport = GetDesireTableStructure();
            AddDesireToTable(dtdesires);
            AddDispatchDetailToTable(dtDispatchInfo);

            return dtReport;
             
        }
        public void AddDesireToTable(DataTable dt )
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dtReport.NewRow();
                    DataRow sourceDR=dt.Rows[i];
                    dr["DesireId"] =sourceDR["DesireForm_ID"];
                     dr["SNo"]=sourceDR["SNo"];
                     dr["NameOfEmployee"]=sourceDR["NameOfEmployee"];
                    dr["Post"]=sourceDR["Post"];
                    dr["Department"]=sourceDR["Department"];
                    dr["CurrentLocation"]=sourceDR["CurrentLocation"];
                  dr["CurrentDistrict"]=sourceDR["CurrentDistrict"];
                    dr["DesireLocation"]=sourceDR["DesireLocation"];
                     dr["DesireDistrict"]=sourceDR["DesireDistrict"];

                    dr["DesireType"]=sourceDR["DesireType"];
                    dr["MinisterDirections"]=sourceDR["MinisterDirections"];
                    dr["Comments"]=sourceDR["Comments"];
                    dr["Sepciality"]=sourceDR["Sepciality"]; 

                    dtReport.Rows.Add(dr);
                }

            }
        }
         public void AddDispatchDetailToTable(DataTable dt )
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                     string [] _dispatchDesiresIds = dt.Rows[1]["DesireIds"].ToString().Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                     //for (int j = 0; j < _dispatchDesiresIds.Length; j++)
                     //{
                     //    if (!String.IsNullOrEmpty(_dispatchDesiresIds[j]))
                     //    {
                     //        DataRow findDr = dtReport.AsEnumerable().Where(x=>x["DesireId"].ToString().Trim()== _dispatchDesiresIds[j])
                     //        if (findDr != null)
                     //        {
                     //            findDr["DispatchDate"] = dt.Rows[i]["DispatchDate"];
                     //            findDr["DispatchNumber"] = dt.Rows[i]["DispatchNumber"];
                     //            findDr["DispatchTo"] = dt.Rows[i]["DispatchTo"];
                     //        }
                     //    }
                     //}
                    dtReport.AcceptChanges();
                }

            }
        }
        public DataTable GetDesireTableStructure()
        {
            DataTable dt =new DataTable ();
           
            //dt.PrimaryKey
            DataColumn dtDesireId = new DataColumn("DesireId", Type.GetType("System.Int32"));
            dt.Columns.Add(dtDesireId);

            //DataColumn [] dtKeys = {dtDesireId };
            //dt.PrimaryKey=dtKeys;

            dt.Columns.Add(new DataColumn("SNo", Type.GetType("System.Int32")));
            
            dt.Columns.Add(new DataColumn("NameOfEmployee", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Post", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Department", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("CurrentLocation", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("CurrentDistrict", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("DesireLocation", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("DesireDistrict", Type.GetType("System.String")));

            dt.Columns.Add(new DataColumn("DesireType", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("MinisterDirections", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Comments", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Sepciality", Type.GetType("System.String")));

            dt.Columns.Add(new DataColumn("DispatchDate", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("DispatchNumber", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("DispatchTo", Type.GetType("System.String")));
           
            dt.Columns.Add(new DataColumn("ReferenceName", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("ReferenceDepartment", Type.GetType("System.String")));
             
            return dt ;
        }

    }
}

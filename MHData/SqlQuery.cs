using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;

using MHData.Entity;
namespace MHData
{
    public class SqlQuery
    {

        public String GetLastRecord(String tableName)
        {
            return "Select * from " + tableName + " where ID= (Select max(ID) from " + tableName + ") ";
        }
        public String GetRecord(string tableName, Int32 id = 0)
        {
            if (id == 0)
                return "Select * from " + tableName;
            else
                return "Select * from " + tableName + " where = " + id;
        }
        public String GetDesireByReference(string tableName, string fieldValue)
        {
            return "Select distinct* from " + tableName + " where ReferenceName= '" + fieldValue + "'";
        }
        public String GetDispatchedDesireIds()
        {
            return "Select distinct DesireID from DispatchedDesires";
        }
        public String GetFilterDesireByParam(string tableName, params string[] param)
        {
            string strquery = "Select distinct* from " + tableName;
            string strVal = String.Empty;
            bool isAndReq = false;
            string disireIdByRef = string.Empty;
            DesireForms db = new DesireForms();
            strVal += " ID not in ( " + db.GetDispatchedDesireIds() + " )";
            if (param[0] != "")
            {
                disireIdByRef = db.GetDesireIdsByRefName(param[0]);
                strVal += " ID in ( " + disireIdByRef + " )";
                isAndReq = true;
            }
            if (param[1] != "")
            {
                if (isAndReq)
                {
                    strVal += " and ";
                }
                else
                    isAndReq = true;
                strVal += " Department = '" + param[1] + "'";

            }
            if (param[2] != "")
            {
                if (isAndReq)
                {
                    strVal += " and ";
                }
                else
                    isAndReq = true;
                strVal += " Post = '" + param[2] + "'";
            }
            if (param[3] != "")
            {
                if (isAndReq)
                {
                    strVal += " and ";
                }
                else
                    isAndReq = true;
                strVal += " NameOfEmployee = '" + param[3] + "'";
            }
            return strquery + (String.IsNullOrEmpty(strVal) ? "" : " where " + strVal);

        }
        public String GetRecordById(string tableName, Int32 id)
        {
            return "Select * from " + tableName + " where ID = " + id;
        }
        public String GetDispatchDesireList(string tableName, string desireIds)
        {
            string strquery = "Select distinct* from " + tableName;
            string strVal = String.Empty;

            DesireForms db = new DesireForms();

            strVal += " ID in ( " + desireIds + " )";

            return strquery + (String.IsNullOrEmpty(strVal) ? "" : " where " + strVal);

        }
        public String InsertDesire(DesireForm desireForm)
        {

            string tableName = desireForm.GetType().Name;
            PropertyInfo[] propertiesInfo = desireForm.GetType().GetProperties();

            string query = "Insert into  DesireForm ";
            string columns = "";
            string columnValues = "";
            //Set Column Name 
            foreach (PropertyInfo prop in propertiesInfo)
            {
                if (prop.Name == "ID" || prop.Name == "Referencees")
                    continue;
                columns += prop.Name + ",";
                columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(desireForm) + "'," : prop.GetValue(desireForm) + ",";
            }
            query += "(" + columns.Substring(0, columns.Length - 1) + ")";
            query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";

            return query;
        }
        public String DeleteDesire(Int32 desireId)
        { 
            return  "Delete from DesireForm Where ID = " + desireId; 
             
        }
        public String DeleteDesiresReferences(Int32 desireId)
        {
            return "Delete from ReferencesForDesire Where DesireId = "+ desireId;
             
        }
        public String InsertDesireReferences(ReferencesForDesire desireReferences, int desireId)
        {

            string tableName = desireReferences.GetType().Name;

            //Set Column Name 
            string refereceQuery = String.Empty;
            // foreach(ReferencesForDesire deRef in desireReferences)
            //{
            string query = "Insert into ReferencesForDesire";
            string columns = "";
            string columnValues = "";
            desireReferences.DesireID = desireId;
            PropertyInfo[] propertiesInfo = desireReferences.GetType().GetProperties();

            foreach (PropertyInfo prop in propertiesInfo)
            {

                if (prop.Name == "ID")
                    continue;
                columns += prop.Name + ",";
                columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(desireReferences) + "'," : prop.GetValue(desireReferences) + ",";
            }
            query += "(" + columns.Substring(0, columns.Length - 1) + ")";
            query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";
            refereceQuery = query;

            //} 
            return refereceQuery;
        }
        public String UpdateDesire(DesireForm desireForm)
        {

            string tableName = desireForm.GetType().Name;
            PropertyInfo[] propertiesInfo = desireForm.GetType().GetProperties();

            string query = "Update DesireForm set  ";

            string updateColumnValues = "";
            string whereCondition = "";

            //Set Column Name 
            foreach (PropertyInfo prop in propertiesInfo)
            {
                if (prop.Name == "Referencees")
                    continue;
                if (prop.Name == "ID")
                {
                    whereCondition = " where " + prop.Name + " = " + prop.GetValue(desireForm);
                }
                else
                {
                    updateColumnValues += prop.Name + " = " + ((prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(desireForm) + "'," : prop.GetValue(desireForm) + ", ");
                }
            }
            query = query + updateColumnValues.Substring(0, updateColumnValues.Length - 1) + whereCondition;

            return query;
        }

        public String UpdateDesireReferences(List<ReferencesForDesire> referencees)
        {
            string tableName = String.Empty;
            String updateDesireQuery = String.Empty;
            string query = String.Empty;
            string columns = "";
            string columnValues = "";
            foreach (ReferencesForDesire reference in referencees)
            {
                tableName = reference.GetType().Name;
                PropertyInfo[] propertiesInfo = reference.GetType().GetProperties();
                query += "Insert into " + tableName;
                foreach (PropertyInfo prop in propertiesInfo)
                {
                    if (prop.Name == "ID")
                        continue;

                    columns += prop.Name + ",";
                    columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(reference) + "'," : prop.GetValue(reference) + ",";
                }
                query += "(" + columns.Substring(0, columns.Length - 1) + ")";
                query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";
                updateDesireQuery += "Delete from " + tableName + " Where DesireID = " + reference.DesireID + "; " + query + "; ";
            }

            return updateDesireQuery;
        }

        public String GetDispatchDetailToEdit(string tableName, string dispatchId)
        {
            string strquery = "Select distinct* from " + tableName;
            string strVal = String.Empty;

            DesireForms db = new DesireForms();

            strVal += " ID = " + dispatchId ;

            return strquery + (String.IsNullOrEmpty(strVal) ? "" : " where " + strVal);

        }
        public String InsertDispatch(DispatchDesireInfo dispatchInfo)
        {

            string tableName = dispatchInfo.GetType().Name;
            PropertyInfo[] propertiesInfo = dispatchInfo.GetType().GetProperties();

            string query = "Insert into  DispatchDesireInfo ";
            string columns = "";
            string columnValues = "";
            //Set Column Name 
            foreach (PropertyInfo prop in propertiesInfo)
            {
                if (prop.Name == "ID" || prop.PropertyType == typeof(List<DispatchedDesires>))
                    continue;
                columns += prop.Name + ",";
                columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(dispatchInfo) + "'," : prop.GetValue(dispatchInfo) + ",";
            }
            query += "(" + columns.Substring(0, columns.Length - 1) + ")";
            query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";

            return query;
        }
        public String UpdateDispatch(DispatchDesireInfo dispatchInfo)
        {

            string tableName = dispatchInfo.GetType().Name;
            PropertyInfo[] propertiesInfo = dispatchInfo.GetType().GetProperties();

            string query = "Update DispatchDesireInfo set  ";

            string updateColumnValues = "";
            string whereCondition = "";

            //Set Column Name 
            foreach (PropertyInfo prop in propertiesInfo)
            {

                if (prop.Name == "ID")
                {
                    whereCondition = " where " + prop.Name + " = " + prop.GetValue(dispatchInfo);
                }
                else
                {
                    if (prop.PropertyType == typeof(List<DispatchedDesires>))
                        continue;

                    updateColumnValues += prop.Name + " = " + ((prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(dispatchInfo) + "'," : prop.GetValue(dispatchInfo) + ", ");
                }
            }
            query = query + updateColumnValues.Substring(0, updateColumnValues.Length - 1) + whereCondition;

            return query;
        }
        /// <summary>
        /// Get Query To Delete Desire Dispatch Info.
        /// </summary>
        /// <param name="dispatchInfo"></param>
        /// <returns></returns>
        public String DeleteDispatchInfo(Int32 dispatchId)
        {

            return "Delete from DispatchDesireInfo where ID ="+ dispatchId;
        }
        /// <summary>
        /// Get Query To Delete Desire Dispatch detail.
        /// </summary>
        /// <param name="dispatchInfo"></param>
        /// <returns></returns>
        public String DeleteDispatchDetail(Int32 dispatchId)
        {
            return "Delete from DispatchedDesires where DispatchID =" + dispatchId;
        }
        public String GetFilterDispatchByParam(string tableName, params string[] param)
        {
            string strquery = "Select distinct* from " + tableName;
            string strVal = String.Empty;
            bool isAndReq = false;
            string disireIdByRef = string.Empty;
            if (param[0] != "")
            {
                DesireForms db = new DesireForms();
                strVal += " DispatchTo = '" + param[0] + "'";
                isAndReq = true;
            }
            if (param[1] != "")
            {
                if (isAndReq)
                {
                    strVal += " and ";
                }
                else
                    isAndReq = true;
                strVal += " DispatchNumber = '" + param[1] + "'";

            }

            return strquery + (String.IsNullOrEmpty(strVal) ? "" : " where " + strVal);

        }

        public String InsertDispatcedDesires(ReferencesForDesire desireReferences, int desireId)
        {

            string tableName = desireReferences.GetType().Name;

            //Set Column Name 
            string refereceQuery = String.Empty;
            // foreach(ReferencesForDesire deRef in desireReferences)
            //{
            string query = "Insert into ReferencesForDesire";
            string columns = "";
            string columnValues = "";
            desireReferences.DesireID = desireId;
            PropertyInfo[] propertiesInfo = desireReferences.GetType().GetProperties();

            foreach (PropertyInfo prop in propertiesInfo)
            {

                if (prop.Name == "ID")
                    continue;
                columns += prop.Name + ",";
                columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(desireReferences) + "'," : prop.GetValue(desireReferences) + ",";
            }
            query += "(" + columns.Substring(0, columns.Length - 1) + ")";
            query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";
            refereceQuery = query;

            //} 
            return refereceQuery;
        }
        public String InsertDesireToDispatch(DispatchedDesires desireToDispatch, int dispatchID)
        {

            string tableName = desireToDispatch.GetType().Name;

            //Set Column Name 
            string dispatchQuery = String.Empty;
           
            string query = "Insert into "+tableName;
            string columns = "";
            string columnValues = "";
            desireToDispatch.DispatchID = dispatchID;
            PropertyInfo[] propertiesInfo = desireToDispatch.GetType().GetProperties();

            foreach (PropertyInfo prop in propertiesInfo)
            {
                columns += prop.Name + ",";
                columnValues += (prop.PropertyType == typeof(String)) ? "'" + prop.GetValue(desireToDispatch) + "'," : prop.GetValue(desireToDispatch) + ",";
            }
            query += "(" + columns.Substring(0, columns.Length - 1) + ")";
            query += " Values (" + columnValues.Substring(0, columnValues.Length - 1) + ")";
            dispatchQuery = query;

            return dispatchQuery;
        }
    }
}
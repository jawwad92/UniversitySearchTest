
using Newtonsoft.Json;
using Portal.Helper;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MySql.Data.MySqlClient;
using System.Data;

namespace Portal.Controllers
{
    public class HomeController : BaseController
    {
        #region DbSettings

        private string server;
        private string database;
        private string uid;
        private string password;
        private MySqlConnection connection;

        private MySqlConnection Connection()
        {
            server = "160.153.146.73";
            database = "UniSearch";
            uid = "uniexpo_data";
            password = "w41c4h8m";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            return connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                if(connection.State==ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); return false;  }

        }

        private bool CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return true;
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); return false; }
        }


        //Insert statement
        public bool Insert(string query)
        {
            //string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";
            try
            {
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
                else
                    return false;
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); return false; }
            return true;
            //open connection
           
        }

        //Update statement
        public bool Update(string query)
        {
            // string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

            try
            {
                //Open connection
                if (this.OpenConnection() == true)
                {
                    //create mysql command
                    MySqlCommand cmd = new MySqlCommand();
                    //Assign the query using CommandText
                    cmd.CommandText = query;
                    //Assign the connection using Connection
                    cmd.Connection = connection;

                    //Execute query
                    cmd.ExecuteNonQuery();

                    //close connection
                    this.CloseConnection();
                }
                else
                    return false;
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); return false; }
            return true;
        }

        //Delete statement
        public bool Delete(string query)
        {
            //string query = "DELETE FROM tableinfo WHERE name='John Smith'";
            try
            {
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                }
                else
                    return false;
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); return false; }
            return true;
           
        }

        #endregion

        #region Login_Logout

        public ActionResult Login(bool? isLoginFailed)
        {
            WriteToFile("App started");
            ViewBag.isLoginFailed = isLoginFailed;


            return View();
        }
        public ActionResult LoginUser(Login obj)
        {
            var profileData = new UserSessionData();
            bool isSuccess = false;

            if (obj.Username== ConfigurationManager.AppSettings["admin_username"].ToString() 
                && obj.Password == ConfigurationManager.AppSettings["admin_pass"].ToString())
            {
                profileData.UserName = obj.Username;
                this.Session["UserProfile"] = profileData;
                isSuccess = true;
            }

            if (obj.Username == ConfigurationManager.AppSettings["username1"].ToString()
               && obj.Password == ConfigurationManager.AppSettings["username1_pass"].ToString())
            {
                profileData.UserName = obj.Username;
                this.Session["UserProfile"] = profileData;
                isSuccess = true;
            }

            if (obj.Username == ConfigurationManager.AppSettings["username2"].ToString()
               && obj.Password == ConfigurationManager.AppSettings["username2_pass"].ToString())
            {
                profileData.UserName = obj.Username;
                this.Session["UserProfile"] = profileData;
                isSuccess = true;
            }

            if (isSuccess)
                return RedirectToAction("LanguageIndex");
            else
                return RedirectToAction("Login", new { isLoginFailed = true });//return RedirectToAction("Login/true");

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Add("UserProfile", null);
            return RedirectToAction("Login");
        }

        #endregion

        #region Language

        public ActionResult LanguageIndex()
        {
            return View();
        }

        public ActionResult Language()
        {
            return View();
        }


        public string GetAllLanguage(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var obj = new List<language>();

            try
            {
                Connection();

                string searchColumn = "";
                if (search != null)
                    if (search["value"] != null && search["value"].ToString() != "")
                        searchColumn = search["value"].ToString();

                string query = "SELECT * FROM languages ";
                if (!string.IsNullOrEmpty(searchColumn))
                {
                    query += "where languages_name like '%" + searchColumn + "%' ";
                }
                query += " ORDER BY languages_pk desc ";
                query += " LIMIT  " + length + " OFFSET  " + start + " ";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        try
                        {
                            language language = new language();

                            language.languages_pk = Convert.ToInt16(dataReader["languages_pk"].ToString());
                            language.languages_name = dataReader["languages_name"].ToString();
                            obj.Add(language);
                        }
                        catch (Exception ex) { WriteToFile("GetAllLanguage dataReader ," + ex.ToString()); }
                        
                    }
                    dataReader.Close();
                    this.CloseConnection();

                    totalRecord = obj.Count();
                }

              
            }
            catch (Exception ex) { WriteToFile(ex.ToString()); }


            empData.draw = draw;
            empData.recordsTotal = totalRecord;
            empData.recordsFiltered = totalRecord;
            empData.data = obj;

            //using (DBModel db = new DBModel())
            //{
            //    try
            //    {
            //        string searchColumn = "";
            //        if (search != null)
            //            if (search["value"] != null && search["value"].ToString() != "")
            //                searchColumn = search["value"].ToString();



            //        if (!string.IsNullOrEmpty(searchColumn))
            //        {
            //            obj = db.languages.Where(a => a.languages_name.ToLower().Contains(searchColumn)
            //           ).OrderByDescending(a => a.languages_pk).Skip(start).Take(length).ToList();

            //            totalRecord = obj.Count();
            //        }
            //        else
            //        {
            //            obj = db.languages.OrderByDescending(a => a.languages_pk).Skip(start).Take(length).ToList();

            //            totalRecord = db.languages.Count();
            //        }


            //    }
            //    catch (Exception ex){ WriteToFile(ex.ToString()); }

            //    empData.draw = draw;
            //    empData.recordsTotal = totalRecord;
            //    empData.recordsFiltered = totalRecord;
            //    empData.data = obj;
            //}


            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult LanguageAddOrEdit(int id = 0)
        {
            language obj = new language();
            //using (DBModel db = new DBModel())
            //{
            //    if (id != 0)
            //    {
            //        obj = db.languages.Where(x => x.languages_pk == id).FirstOrDefault<language>();
            //    }
            //}
            try
            {
                if (id != 0)
                {
                    string query = "SELECT * FROM languages ";
                    query += "where languages_pk = '%" + id + "%' ";

                    if (this.OpenConnection() == true)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataReader dataReader = cmd.ExecuteReader();
                        while (dataReader.Read())
                        {
                            try
                            {

                                obj.languages_pk = Convert.ToInt16(dataReader["languages_pk"].ToString());
                                obj.languages_name = dataReader["languages_name"].ToString();

                            }
                            catch (Exception ex) { WriteToFile("GetAllLanguage dataReader ," + ex.ToString()); }

                        }
                        dataReader.Close();
                        this.CloseConnection();

                    }
                }


            }
            catch (Exception ex) { WriteToFile(ex.ToString()); }

            return View(obj);
        }

        [HttpPost]
        public ActionResult LanguageAddOrEdit(language obj)
        {
            string query = "";
            bool isSuccess = true;
            string message = "";
            try
            {

                if (obj.languages_pk == 0)
                {
                    query = "INSERT INTO languages (languages_pk, languages_name,createdAt,createdBy) " +
                        "VALUES('" + CommonMethod.GetLastID("language") + "', '" + obj.languages_name + "','" + CurrentUser.UserName + "','" + CommonMethod.GetCurrentDateTime().ToString() + "')";

                    if (!Insert(query))
                    {
                        isSuccess = false;
                        message = "Exception occured when save data.";
                    }
                }
                else
                {
                    query = "UPDATE languages SET languages_name='"+ obj.languages_name + "', modifiedBy='"+ CurrentUser.UserName + "', modifiedAt='" + CommonMethod.GetCurrentDateTime().ToString() + "' WHERE languages_pk='"+obj.languages_pk+"'";

                    if (!Update(query))
                    {
                        isSuccess = false;
                        message = "Exception occured when update data.";
                    }
                }

                //using (DBModel db = new DBModel())
                //{
                //    if (obj.languages_pk == 0)
                //    {
                //       obj.languages_pk = CommonMethod.GetLastID("language");
                //        obj.createdBy = CurrentUser.UserName;
                //        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                //        db.languages.Add(obj);
                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        obj.modifiedBy = CurrentUser.UserName;
                //        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                //        db.Entry(obj).State = EntityState.Modified;
                //        db.SaveChanges();

                //    }
                //}

                return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LanguageDelete(int id)
        {
            string query = "";
            bool isSuccess = true;
            string message = "";

            try
            {
                query = "DELETE FROM languages WHERE languages_pk='"+id+"'";
                if (!Delete(query))
                {
                    isSuccess = false;
                    message = "Exception occured when delete data.";
                }
               
                //using (DBModel db = new DBModel())
                //{
                //    language obj = db.languages.Where(x => x.languages_pk == id).FirstOrDefault<language>();
                //    db.languages.Remove(obj);
                //    db.SaveChanges();
                //}
                   

                return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Country

        public ActionResult CountryIndex()
        {
            return View();
        }
        public ActionResult Country()
        {
            return View();
        }
        

        public string GetAllCountry(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var country = new List<country>();
            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();

                    

                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        country = db.countries.Where(a => a.country_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.country_pk).Skip(start).Take(length).ToList();

                        totalRecord = country.Count();
                    }
                    else
                    {
                        country = db.countries.OrderByDescending(a => a.country_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.countries.Count();
                    }

                    
                }
                catch (Exception ex){ WriteToFile(ex.ToString()); }

                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                empData.data = country;
            }


            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult CountryAddOrEdit(int id = 0)
        {
            country obj = new country();
            using (DBModel db = new DBModel())
            {
                if (id != 0)
                {
                    obj = db.countries.Where(x => x.country_pk == id).FirstOrDefault<country>();
                }
            }
                
            return View(obj);
        }

        [HttpPost]
        public ActionResult CountryAddOrEdit(country obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.country_pk == 0)
                    {
                        obj.country_pk = CommonMethod.GetLastID("country");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.countries.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                    
                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CountryDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    country obj = db.countries.Where(x => x.country_pk == id).FirstOrDefault<country>();
                    db.countries.Remove(obj);
                    db.SaveChanges();
                }
                   

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region SubjectGroup

        public ActionResult SubjectGroupIndex()
        {
            return View();
        }
        public ActionResult SubjectGroup()
        {
            return View();
        }


        public string GetAllSubjectGroup(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var SubjectGroup = new List<subject_group>();
            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();

                   

                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        SubjectGroup = db.subject_group.Where(a => a.subject_group_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.subject_group_pk).Skip(start).Take(length).ToList();

                        totalRecord = SubjectGroup.Count();
                    }
                    else
                    {
                        SubjectGroup = db.subject_group.OrderByDescending(a => a.subject_group_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.subject_group.Count();
                    }


                   
                }
                catch (Exception ex){ WriteToFile(ex.ToString()); }
                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                empData.data = SubjectGroup;
            }

           

            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult SubjectGroupAddOrEdit(int id = 0)
        {
            subject_group obj = new subject_group();
            using (DBModel db = new DBModel())
            {
                if (id != 0)
                {
                    obj = db.subject_group.Where(x => x.subject_group_pk == id).FirstOrDefault<subject_group>();
                }
            }
               
            return View(obj);
        }

        [HttpPost]
        public ActionResult SubjectGroupAddOrEdit(subject_group obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.subject_group_pk == 0)
                    {
                        obj.subject_group_pk = CommonMethod.GetLastID("subject_group");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.subject_group.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                   

                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SubjectGroupDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    subject_group obj = db.subject_group.Where(x => x.subject_group_pk == id).FirstOrDefault<subject_group>();
                    db.subject_group.Remove(obj);
                    db.SaveChanges();
                }
                   

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region DegreeType

        public ActionResult DegreeTypeIndex()
        {
            return View();
        }
        public ActionResult DegreeType()
        {
            return View();
        }


        public string GetAllDegreeType(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var degree_type = new List<degree_type>();

            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();

                   

                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        degree_type = db.degree_type.Where(a => a.degree_type_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.degree_type_pk).Skip(start).Take(length).ToList();

                        totalRecord = degree_type.Count();
                    }
                    else
                    {
                        degree_type = db.degree_type.OrderByDescending(a => a.degree_type_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.degree_type.Count();
                    }

                }
                catch (Exception ex){ WriteToFile(ex.ToString()); }
                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                empData.data = degree_type;
            }


            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult DegreeTypeAddOrEdit(int id = 0)
        {
            degree_type obj = new degree_type();
            using (DBModel db = new DBModel())
            {
                if (id != 0)
                {
                    obj = db.degree_type.Where(x => x.degree_type_pk == id).FirstOrDefault<degree_type>();
                }
            }
               
            return View(obj);
        }

        [HttpPost]
        public ActionResult DegreeTypeAddOrEdit(degree_type obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.degree_type_pk == 0)
                    {
                        obj.degree_type_pk = CommonMethod.GetLastID("degree_type");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.degree_type.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                  
                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DegreeTypeDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    degree_type obj = db.degree_type.Where(x => x.degree_type_pk == id).FirstOrDefault<degree_type>();
                    db.degree_type.Remove(obj);
                    db.SaveChanges();
                }
                   

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region StudyArea

        public ActionResult StudyAreaIndex()
        {
            return View();
        }
        public ActionResult StudyArea()
        {
            return View();
        }


        public string GetAllStudyArea(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var study_areas = new List<study_areas>();
            var studyArea = new List<StudyArea>();

            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();

                    

                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        study_areas = db.study_areas.Where(a => a.study_areas_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.study_areas_pk).Skip(start).Take(length).ToList();

                        totalRecord = study_areas.Count();

                    }
                    else
                    {
                        study_areas = db.study_areas.OrderByDescending(a => a.study_areas_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.study_areas.Count();
                    }

                    if(study_areas.Count>0)
                    {
                        var studyarea =
                               
                               from sa in study_areas
                               join sg in db.subject_group on sa.subject_group_pk equals sg.subject_group_pk
                               orderby sa.study_areas_pk descending
                               select new { sa.study_areas_pk, sa.study_areas_name,sg.subject_group_name };

                        empData.data = studyarea.ToList();
                    }

                   
                }
                catch (Exception ex){ WriteToFile(ex.ToString()); }
                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                if (empData.data == null)
                    empData.data = studyArea;
            }

          


            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult StudyAreaAddOrEdit(int id = 0)
        {
            StudyArea studyArea = new StudyArea();
            try
            {
                using (DBModel db = new DBModel())
                {
                    study_areas obj = new study_areas();

                    if (id != 0)
                    {
                        obj = db.study_areas.Where(x => x.study_areas_pk == id).FirstOrDefault<study_areas>();
                        studyArea.study_areas_pk = obj.study_areas_pk;
                        studyArea.study_areas_name = obj.study_areas_name;
                        studyArea.subject_group_pk = obj.subject_group_pk;

                    }

                    studyArea.subject_group_list = db.subject_group.ToList();
                }
                   
            }
            catch(Exception ex) {
                WriteToFile(ex.ToString());
                List<subject_group> subject_group_list = new List<subject_group>();
                studyArea.subject_group_list = subject_group_list;
            }
           

            return View(studyArea);
        }

        [HttpPost]
        public ActionResult StudyAreaAddOrEdit(study_areas obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.subject_group_pk == null || obj.subject_group_pk == 0)
                        return Json(new { success = false, message = "Please select subject group" }, JsonRequestBehavior.AllowGet);

                    if (obj.study_areas_pk == 0)
                    {
                        obj.study_areas_pk = CommonMethod.GetLastID("study_area");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.study_areas.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                    
                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StudyAreaDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    study_areas obj = db.study_areas.Where(x => x.study_areas_pk == id).FirstOrDefault<study_areas>();
                    db.study_areas.Remove(obj);
                    db.SaveChanges();
                }
                    

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion



        #region FieldOfStudy

        public ActionResult FieldOfStudyIndex()
        {
            return View();
        }
        public ActionResult FieldOfStudy()
        {
            return View();
        }


        public string GetAllFieldOfStudy(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var field_of_study = new List<field_of_study>();
            var fieldOfStudy = new List<FieldOfStudy>();

            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();



                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        field_of_study = db.field_of_study.Where(a => a.field_of_study_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.study_areas_pk).Skip(start).Take(length).ToList();

                        totalRecord = field_of_study.Count();

                    }
                    else
                    {
                        field_of_study = db.field_of_study.OrderByDescending(a => a.field_of_study_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.field_of_study.Count();
                    }

                    if (field_of_study.Count > 0)
                    {
                        var fieldofstudy =

                               from fos in field_of_study
                               join sa in db.study_areas on fos.study_areas_pk equals sa.study_areas_pk
                               orderby fos.field_of_study_pk descending
                               select new { fos.field_of_study_pk, fos.field_of_study_name,  sa.study_areas_name };

                        empData.data = fieldofstudy.ToList();
                    }


                }
                catch (Exception ex) { WriteToFile(ex.ToString()); }
                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                if (empData.data == null)
                    empData.data = fieldOfStudy;
            }




            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult FieldOfStudyAddOrEdit(int id = 0)
        {
            FieldOfStudy fieldOfStudy = new FieldOfStudy();
            try
            {
                using (DBModel db = new DBModel())
                {
                    field_of_study obj = new field_of_study();

                    if (id != 0)
                    {
                        obj = db.field_of_study.Where(x => x.field_of_study_pk == id).FirstOrDefault<field_of_study>();
                        fieldOfStudy.field_of_study_pk = obj.field_of_study_pk;
                        fieldOfStudy.field_of_study_name = obj.field_of_study_name;
                        fieldOfStudy.study_areas_pk = obj.study_areas_pk;

                    }

                    fieldOfStudy.study_areas_list = db.study_areas.ToList();
                }
                   
            }
            catch (Exception ex) {
                WriteToFile(ex.ToString());
                List<study_areas> study_areas_list = new List<study_areas>();
                fieldOfStudy.study_areas_list = study_areas_list;
            }


            return View(fieldOfStudy);
        }

        [HttpPost]
        public ActionResult FieldOfStudyAddOrEdit(field_of_study obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.study_areas_pk == null || obj.study_areas_pk == 0)
                        return Json(new { success = false, message = "Please select study area" }, JsonRequestBehavior.AllowGet);

                    if (obj.field_of_study_pk == 0)
                    {
                        obj.field_of_study_pk = CommonMethod.GetLastID("field_of_study");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.field_of_study.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                   
                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FieldOfStudyDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    field_of_study obj = db.field_of_study.Where(x => x.field_of_study_pk == id).FirstOrDefault<field_of_study>();
                    db.field_of_study.Remove(obj);
                    db.SaveChanges();
                }
                  

                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region SubjectDegreeType

        public ActionResult SubjectDegreeTypeIndex()
        {
            return View();
        }
        public ActionResult SubjectDegreeType()
        {
            return View();
        }

        public string GetAllSubjectDegreeType(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var sub_degree_type = new List<sub_degree_type>();
            var fieldOfStudy = new List<sub_degree_type>();

            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();



                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        sub_degree_type = db.sub_degree_type.Where(a => a.sub_degree_type_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.sub_degree_type_pk).Skip(start).Take(length).ToList();

                        totalRecord = sub_degree_type.Count();

                    }
                    else
                    {
                        sub_degree_type = db.sub_degree_type.OrderByDescending(a => a.sub_degree_type_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.sub_degree_type.Count();
                    }

                    if (sub_degree_type.Count > 0)
                    {
                        var fieldofstudy =

                               from sdt in sub_degree_type
                               join dt in db.degree_type on sdt.degree_type_pk equals dt.degree_type_pk
                               orderby sdt.sub_degree_type_pk descending
                               select new { sdt.sub_degree_type_pk, sdt.sub_degree_type_name, sdt.sub_degree_type_fullform, dt.degree_type_name };

                        empData.data = fieldofstudy.ToList();
                    }


                }
                catch (Exception ex) { WriteToFile(ex.ToString()); }
                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                if (empData.data == null)
                    empData.data = fieldOfStudy;
            }




            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult SubjectDegreeTypeAddOrEdit(int id = 0)
        {
            SubjectDegreeType subjectDegreeType = new SubjectDegreeType();
            try
            {
                using (DBModel db = new DBModel())
                {
                    sub_degree_type obj = new sub_degree_type();

                    if (id != 0)
                    {
                        obj = db.sub_degree_type.Where(x => x.sub_degree_type_pk == id).FirstOrDefault<sub_degree_type>();
                        subjectDegreeType.sub_degree_type_pk = obj.sub_degree_type_pk;
                        subjectDegreeType.sub_degree_type_name = obj.sub_degree_type_name;
                        subjectDegreeType.sub_degree_type_fullform = obj.sub_degree_type_fullform;
                        subjectDegreeType.degree_type_pk = obj.degree_type_pk;

                    }

                    subjectDegreeType.degree_type_list = db.degree_type.ToList();
                }

            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                List<degree_type> degree_type = new List<degree_type>();
                subjectDegreeType.degree_type_list = degree_type;
            }


            return View(subjectDegreeType);
        }

        [HttpPost]
        public ActionResult SubjectDegreeTypeAddOrEdit(sub_degree_type obj)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    if (obj.degree_type_pk == null || obj.degree_type_pk == 0)
                        return Json(new { success = false, message = "Please select study area" }, JsonRequestBehavior.AllowGet);

                    if (obj.sub_degree_type_pk == 0)
                    {
                        obj.sub_degree_type_pk = CommonMethod.GetLastID("sub_degree_type");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.sub_degree_type.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SubjectDegreeTypeDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    sub_degree_type obj = db.sub_degree_type.Where(x => x.sub_degree_type_pk == id).FirstOrDefault<sub_degree_type>();
                    db.sub_degree_type.Remove(obj);
                    db.SaveChanges();
                }


                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region UniversityName

        public ActionResult UniversityNameIndex()
        {
            return View();
        }
        public ActionResult UniversityName()
        {
            return View();
        }

        public string GetAllUniversityName(int draw, int start, int length, Dictionary<string, string> search)
        {
            responseData empData = new responseData();
            int totalRecord = 0;
            var university_name = new List<university_name>();
            var universityname = new List<university_name>();

            using (DBModel db = new DBModel())
            {
                try
                {
                    string searchColumn = "";
                    if (search != null)
                        if (search["value"] != null && search["value"].ToString() != "")
                            searchColumn = search["value"].ToString();



                    if (!string.IsNullOrEmpty(searchColumn))
                    {
                        university_name = db.university_name.Where(a => a.uni_name.ToLower().Contains(searchColumn)
                       ).OrderByDescending(a => a.uni_pk).Skip(start).Take(length).ToList();

                        totalRecord = university_name.Count();

                    }
                    else
                    {
                        university_name = db.university_name.OrderByDescending(a => a.uni_pk).Skip(start).Take(length).ToList();

                        totalRecord = db.university_name.Count();
                    }

                    if (university_name.Count > 0)
                    {

                        university_name.Where(x => x.completed == null || x.completed=="").ToList().ForEach(y => y.completed = "-");
                        university_name.Where(x => x.admission_open == null || x.admission_open == "").ToList().ForEach(y => y.admission_open = "-");


                        var fieldofstudy =

                               from un in university_name
                               join c in db.countries on un.country_pk equals c.country_pk
                               orderby un.uni_pk descending
                               select new { un.uni_pk, un.uni_name, c.country_name, un.completed,un.ranking,un.admission_open,un.admission_link };

                      

                        empData.data = fieldofstudy.ToList();
                    }


                }
                catch (Exception ex) { WriteToFile(ex.ToString()); }

                empData.draw = draw;
                empData.recordsTotal = totalRecord;
                empData.recordsFiltered = totalRecord;
                if (empData.data == null)
                    empData.data = universityname;
            }




            return JsonConvert.SerializeObject(empData).ToString();
        }

        public ActionResult UniversityNameAddOrEdit(int id = 0)
        {
            UniversityName universityName = new UniversityName();
            try
            {
                using (DBModel db = new DBModel())
                {
                    university_name obj = new university_name();

                    if (id != 0)
                    {
                        obj = db.university_name.Where(x => x.uni_pk == id).FirstOrDefault<university_name>();
                        universityName.uni_pk = obj.uni_pk;
                        universityName.uni_name = obj.uni_name;
                        universityName.completed = obj.completed;
                        universityName.country_pk = obj.country_pk;
                        universityName.ranking = obj.ranking;
                        universityName.admission_open = obj.admission_open;
                        universityName.admission_link = obj.admission_link;

                    }

                    universityName.country_list = db.countries.ToList();
                }

            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                List<country> country = new List<country>();
                universityName.country_list = country;
            }


            return View(universityName);
        }

        [HttpPost]
        public ActionResult UniversityNameAddOrEdit(UniversityName UniObj)
        {
            try
            {
                university_name obj = new university_name();
                obj.admission_link = UniObj.admission_link;
                obj.admission_open = UniObj.admission_open;
                if(UniObj.iscompleted)
                    obj.completed = "yes";
                if (UniObj.isadmission_open)
                    obj.admission_open = "yes";

                obj.uni_pk = UniObj.uni_pk;
                obj.uni_name = UniObj.uni_name;
                obj.ranking = UniObj.ranking;
                obj.country_pk = UniObj.country_pk;

                using (DBModel db = new DBModel())
                {
                    if (obj.country_pk == null || obj.country_pk == 0)
                        return Json(new { success = false, message = "Please select study area" }, JsonRequestBehavior.AllowGet);

                    if (obj.uni_pk == 0)
                    {
                        obj.uni_pk = CommonMethod.GetLastID("university_name");
                        obj.createdBy = CurrentUser.UserName;
                        obj.createdAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.university_name.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.modifiedBy = CurrentUser.UserName;
                        obj.modifiedAt = CommonMethod.GetCurrentDateTime().ToString();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                return Json(new { success = true, message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UniversityNameDelete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    university_name obj = db.university_name.Where(x => x.uni_pk == id).FirstOrDefault<university_name>();
                    db.university_name.Remove(obj);
                    db.SaveChanges();
                }


                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        public void WriteToFile(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\line Log";
            // string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            string logFile = "lineSearchAppLogFile" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            string filePath = path + "\\" + logFile;
            try
            {
                text = CommonMethod.GetCurrentDateTime().ToString("dd/MM/yyyy hh:mm:ss tt")+" => "+ text;

                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                if (!System.IO.File.Exists(path + "\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt"))
                {
                    System.IO.File.Create(path + "\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt");
                }
                using (StreamWriter writer = new StreamWriter(path + "\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt", true))
                {
                    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.Close();
                }

                if (System.IO.File.Exists(path + "\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt"))
                    System.IO.File.Copy(path + "\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt",
                                        path + "\\backup\\log" + DateTime.Now.ToString("ddMMyyyy") + ".txt",
                                        true);

            }
            catch (Exception ex)
            {
               // Console.WriteLine(ioex.Message);
            }

           
        }

    }
}
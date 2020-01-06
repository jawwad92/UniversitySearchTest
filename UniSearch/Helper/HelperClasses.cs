using Portal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Portal.Helper
{

    public class CommonMethod
    {
        public static DateTime GetCurrentDateTime()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimeZone"].ToString());
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }

        public static int GetLastID(string tblName)
        {
            int id = 0;

            DBModel db = new DBModel();

            switch (tblName.ToLower().Trim())
            {

                case "language":
                    var obj = db.languages.OrderByDescending(x=>x.languages_pk).FirstOrDefault();
                    id = obj.languages_pk;
                    id++;
                    break;
                case "country":
                    var obj_country = db.countries.OrderByDescending(x => x.country_pk).FirstOrDefault();
                    id = obj_country.country_pk;
                    id++;
                    break;
                case "subject_group":
                    var obj_subject_group = db.subject_group.OrderByDescending(x => x.subject_group_pk).FirstOrDefault();
                    id = obj_subject_group.subject_group_pk;
                    id++;
                    break;
                case "degree_type":
                    var obj_degree_type = db.degree_type.OrderByDescending(x => x.degree_type_pk).FirstOrDefault();
                    id = obj_degree_type.degree_type_pk;
                    id++;
                    break;
                case "sub_degree_type":
                    var obj_sub_degree_type = db.sub_degree_type.OrderByDescending(x => x.sub_degree_type_pk).FirstOrDefault();
                    id = obj_sub_degree_type.sub_degree_type_pk;
                    id++;
                    break;
                case "study_area":
                    var obj_study_area = db.study_areas.OrderByDescending(x => x.study_areas_pk).FirstOrDefault();
                    id = obj_study_area.study_areas_pk;
                    id++;
                    break;
                case "field_of_study":
                    var obj_field_of_study = db.field_of_study.OrderByDescending(x => x.field_of_study_pk).FirstOrDefault();
                    id = obj_field_of_study.field_of_study_pk;
                    id++;
                    break;
                case "university_name":
                    var obj_university_name = db.university_name.OrderByDescending(x => x.uni_pk).FirstOrDefault();
                    id = obj_university_name.uni_pk;
                    id++;
                    break;
                case "degree_data":
                    var obj_degree_data = db.degree_data.OrderByDescending(x => x.degree_pk).FirstOrDefault();
                    id = obj_degree_data.degree_pk;
                    id++;
                    break;

            }

            return id;
        }

    }

    public class responseData
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public Object data { get; set; }

    }

    public class Login
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    [Serializable]
    public class UserSessionData
    {
        public string UserName { get; set; }


    }

    #region PartialClasses

   public class StudyArea
    {
        public int study_areas_pk { get; set; }
        public string study_areas_name { get; set; }
        public Nullable<int> subject_group_pk { get; set; }
        public string subject_group_name { get; set; }

        public List<subject_group> subject_group_list { get; set; }
    }

    public class FieldOfStudy
    {
        public int field_of_study_pk { get; set; }
        public string field_of_study_name { get; set; }
        public Nullable<int> study_areas_pk { get; set; }
        public string study_areas_name { get; set; }

        public List<study_areas> study_areas_list { get; set; }
    }

    public class SubjectDegreeType
    {
        public int sub_degree_type_pk { get; set; }
        public string sub_degree_type_name { get; set; }
        public Nullable<int> degree_type_pk { get; set; }
        public string sub_degree_type_fullform { get; set; }

        public List<degree_type> degree_type_list { get; set; }
    }

    public class UniversityName
    {
        public int uni_pk { get; set; }
        public string uni_name { get; set; }
        public Nullable<int> country_pk { get; set; }
        public bool iscompleted { get; set; }
        public string completed { get; set; }
        public Nullable<int> ranking { get; set; }

        public bool isadmission_open { get; set; }
        public string admission_open { get; set; }
        public string admission_link { get; set; }
        public List<country> country_list { get; set; }
    }

    #endregion



}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class degree_data
    {
        public int degree_pk { get; set; }
        public string degree_link { get; set; }
        public Nullable<int> degree_type_pk { get; set; }
        public Nullable<int> uni_pk { get; set; }
        public Nullable<int> sub_degree_type_pk { get; set; }
        public Nullable<int> field_of_study_pk { get; set; }
        public Nullable<int> languages_pk { get; set; }
        public Nullable<int> country_pk { get; set; }
        public Nullable<int> study_areas_pk { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string modifiedAt { get; set; }
        public string modifiedBy { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiplomDolgov.DataFolder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            this.Realization = new HashSet<Realization>();
        }
    
        public int IdStaff { get; set; }
        public string LastNameStaff { get; set; }
        public string FirstNameStaff { get; set; }
        public string MiddleNameStaff { get; set; }
        public int IdUser { get; set; }
        public int IdPost { get; set; }
        public string EmailStaff { get; set; }
        public string PhoneNumberStaff { get; set; }
        public byte[] StaffPhoto { get; set; }
        public int IdGender { get; set; }
    
        public virtual Gender Gender { get; set; }
        public virtual Post Post { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Realization> Realization { get; set; }
        public virtual User User { get; set; }
    }
}

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
    
    public partial class Guests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Guests()
        {
            this.Realization = new HashSet<Realization>();
        }
    
        public int IdGuest { get; set; }
        public string LastNameGuest { get; set; }
        public string FirstNameGuest { get; set; }
        public string MiddleNameGuest { get; set; }
        public Nullable<int> IdRoom { get; set; }
        public string PhoneNumberGuest { get; set; }
        public string EmailGuest { get; set; }
        public int IdGender { get; set; }
    
        public virtual Gender Gender { get; set; }
        public virtual Room Room { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Realization> Realization { get; set; }
    }
}

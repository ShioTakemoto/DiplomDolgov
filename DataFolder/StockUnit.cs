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
    
    public partial class StockUnit
    {
        public int IdStockUnit { get; set; }
        public int IdMedicine { get; set; }
        public int Count { get; set; }
    
        public virtual Medicine Medicine { get; set; }
    }
}

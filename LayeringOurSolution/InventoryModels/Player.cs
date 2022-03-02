using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryModels
{
    public class Player : FullAuditModel
    {
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public virtual List<Item> Items { get; set; } = new List<Item>();
    }
}

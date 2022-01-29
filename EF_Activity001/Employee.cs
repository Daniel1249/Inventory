using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF_Activity001
{
    [Table("Employee", Schema = "HumanResources")]
    public partial class Employee
    {
        public Employee()
        {
            EmployeeDepartmentHistory = new HashSet<EmployeeDepartmentHistory>();
            EmployeePayHistory = new HashSet<EmployeePayHistory>();
            JobCandidate = new HashSet<JobCandidate>();
            PurchaseOrderHeader = new HashSet<PurchaseOrderHeader>();
        }

        [Key]
        [Column("BusinessEntityID")]
        public int BusinessEntityId { get; set; }


        /*
        [Required]
        [Column("NationalIDNumber")]
        [StringLength(15)]
        public string NationalIdnumber { get; set; }

         [Required]
        [StringLength(50)]
        public string JobTitle { get; set; }
        [Column(TypeName = "date")]

        public DateTime BirthDate { get; set; }


        
        [Required]
        [StringLength(1)]
        public string MaritalStatus { get; set; }

        

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }

        [Column(TypeName = "date")]
        public DateTime HireDate { get; set; }
        */

        public byte[] NationalIdnumber { get; set; }
        public byte[] JobTitle { get; set; }
        public byte[] BirthDate { get; set; }
        public byte[] MaritalStatus { get; set; }
        public byte[] Gender { get; set; }
        public byte[] HireDate { get; set; }


        [Required]
        [Column("LoginID")]
        [StringLength(256)]
        public string LoginId { get; set; }


        public short? OrganizationLevel { get; set; }

       

        [Required]
        public bool? SalariedFlag { get; set; }
        public short VacationHours { get; set; }
        public short SickLeaveHours { get; set; }
        [Required]
        public bool? CurrentFlag { get; set; }
        [Column("rowguid")]
        public Guid Rowguid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(BusinessEntityId))]
        [InverseProperty(nameof(Person.Employee))]
        public virtual Person BusinessEntity { get; set; }
        [InverseProperty("BusinessEntity")]
        public virtual SalesPerson SalesPerson { get; set; }
        [InverseProperty("BusinessEntity")]
        public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; }
        [InverseProperty("BusinessEntity")]
        public virtual ICollection<EmployeePayHistory> EmployeePayHistory { get; set; }
        [InverseProperty("BusinessEntity")]
        public virtual ICollection<JobCandidate> JobCandidate { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeader { get; set; }

        //for backup
        /*
        [StringLength(15)]
        public string NationalIDNumberBackup { get; set; }
        [StringLength(50)]
        public string JobTitleBackup { get; set; }
        [Column(TypeName = "date")]
        public DateTime BirthDateBackup { get; set; }
        [StringLength(1)]
        public string MaritalStatusBackup { get; set; }
        [StringLength(1)]
        public string GenderBackup { get; set; }
        [Column(TypeName = "date")]
        public DateTime HireDateBackup { get; set; }
        */
    }
}

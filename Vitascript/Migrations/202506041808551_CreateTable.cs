namespace Vitascript.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BrandedMedicine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MedicineName = c.String(nullable: false),
                        BuyingPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MRP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityAvailable = c.Int(nullable: false),
                        GenericMedicineId = c.Int(nullable: false),
                        BrandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brand", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.GenericMedicine", t => t.GenericMedicineId, cascadeDelete: true)
                .Index(t => t.GenericMedicineId)
                .Index(t => t.BrandId);
            
            CreateTable(
                "dbo.Brand",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GenericMedicine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Formula = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrescribedMedicine",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Dose = c.String(),
                        TotalQuantity = c.Int(nullable: false),
                        RemainingQuantity = c.Int(nullable: false),
                        Duration = c.Int(nullable: false),
                        Notes = c.String(),
                        PrescriptionId = c.Int(nullable: false),
                        GenericMedicineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GenericMedicine", t => t.GenericMedicineId, cascadeDelete: true)
                .ForeignKey("dbo.Prescription", t => t.PrescriptionId, cascadeDelete: true)
                .Index(t => t.PrescriptionId)
                .Index(t => t.GenericMedicineId);
            
            CreateTable(
                "dbo.MedicineTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuantityPurchased = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DatePurchased = c.DateTime(nullable: false),
                        PrescribedMedicineId = c.Int(nullable: false),
                        PharmacyId = c.Int(nullable: false),
                        PatientId = c.Int(nullable: false),
                        PaymentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pharmacy", t => t.PharmacyId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.PatientId)
                .ForeignKey("dbo.Payment", t => t.PaymentId)
                .ForeignKey("dbo.PrescribedMedicine", t => t.PrescribedMedicineId, cascadeDelete: true)
                .Index(t => t.PrescribedMedicineId)
                .Index(t => t.PharmacyId)
                .Index(t => t.PatientId)
                .Index(t => t.PaymentId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Password = c.String(),
                        Phone = c.String(),
                        DOB = c.DateTime(nullable: false),
                        Email = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        UserTypeId = c.Int(nullable: false),
                        DoctorTypeId = c.Int(),
                        PharmacyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DoctorType", t => t.DoctorTypeId)
                .ForeignKey("dbo.Pharmacy", t => t.PharmacyId)
                .ForeignKey("dbo.UserType", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId)
                .Index(t => t.DoctorTypeId)
                .Index(t => t.PharmacyId);
            
            CreateTable(
                "dbo.DoctorType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MedicalHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        DateRecorded = c.DateTime(nullable: false),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentDate = c.DateTime(nullable: false),
                        PatientId = c.Int(nullable: false),
                        PrescriptionId = c.Int(nullable: false),
                        PharmacyId = c.Int(nullable: false),
                        PaymentTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.PatientId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentType", t => t.PaymentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Pharmacy", t => t.PharmacyId, cascadeDelete: true)
                .ForeignKey("dbo.Prescription", t => t.PrescriptionId, cascadeDelete: true)
                .Index(t => t.PatientId)
                .Index(t => t.PrescriptionId)
                .Index(t => t.PharmacyId)
                .Index(t => t.PaymentTypeId);
            
            CreateTable(
                "dbo.PaymentType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pharmacy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PharmacyName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PharmacyInventory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AvailableQuantity = c.Int(nullable: false),
                        PharmacyId = c.Int(nullable: false),
                        BrandedMedicineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BrandedMedicine", t => t.BrandedMedicineId, cascadeDelete: true)
                .ForeignKey("dbo.Pharmacy", t => t.PharmacyId, cascadeDelete: true)
                .Index(t => t.PharmacyId)
                .Index(t => t.BrandedMedicineId);
            
            CreateTable(
                "dbo.Prescription",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrescriptionCode = c.String(nullable: false, maxLength: 20),
                        IssuedDate = c.DateTime(nullable: false),
                        DoctorId = c.Int(nullable: false),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.DoctorId)
                .ForeignKey("dbo.User", t => t.PatientId)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);
            
            CreateTable(
                "dbo.UserType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedicineTransaction", "PrescribedMedicineId", "dbo.PrescribedMedicine");
            DropForeignKey("dbo.MedicineTransaction", "PaymentId", "dbo.Payment");
            DropForeignKey("dbo.MedicineTransaction", "PatientId", "dbo.User");
            DropForeignKey("dbo.User", "UserTypeId", "dbo.UserType");
            DropForeignKey("dbo.Payment", "PrescriptionId", "dbo.Prescription");
            DropForeignKey("dbo.PrescribedMedicine", "PrescriptionId", "dbo.Prescription");
            DropForeignKey("dbo.Prescription", "PatientId", "dbo.User");
            DropForeignKey("dbo.Prescription", "DoctorId", "dbo.User");
            DropForeignKey("dbo.MedicineTransaction", "PharmacyId", "dbo.Pharmacy");
            DropForeignKey("dbo.User", "PharmacyId", "dbo.Pharmacy");
            DropForeignKey("dbo.Payment", "PharmacyId", "dbo.Pharmacy");
            DropForeignKey("dbo.PharmacyInventory", "PharmacyId", "dbo.Pharmacy");
            DropForeignKey("dbo.PharmacyInventory", "BrandedMedicineId", "dbo.BrandedMedicine");
            DropForeignKey("dbo.Payment", "PaymentTypeId", "dbo.PaymentType");
            DropForeignKey("dbo.Payment", "PatientId", "dbo.User");
            DropForeignKey("dbo.MedicalHistory", "PatientId", "dbo.User");
            DropForeignKey("dbo.User", "DoctorTypeId", "dbo.DoctorType");
            DropForeignKey("dbo.PrescribedMedicine", "GenericMedicineId", "dbo.GenericMedicine");
            DropForeignKey("dbo.BrandedMedicine", "GenericMedicineId", "dbo.GenericMedicine");
            DropForeignKey("dbo.BrandedMedicine", "BrandId", "dbo.Brand");
            DropIndex("dbo.Prescription", new[] { "PatientId" });
            DropIndex("dbo.Prescription", new[] { "DoctorId" });
            DropIndex("dbo.PharmacyInventory", new[] { "BrandedMedicineId" });
            DropIndex("dbo.PharmacyInventory", new[] { "PharmacyId" });
            DropIndex("dbo.Payment", new[] { "PaymentTypeId" });
            DropIndex("dbo.Payment", new[] { "PharmacyId" });
            DropIndex("dbo.Payment", new[] { "PrescriptionId" });
            DropIndex("dbo.Payment", new[] { "PatientId" });
            DropIndex("dbo.MedicalHistory", new[] { "PatientId" });
            DropIndex("dbo.User", new[] { "PharmacyId" });
            DropIndex("dbo.User", new[] { "DoctorTypeId" });
            DropIndex("dbo.User", new[] { "UserTypeId" });
            DropIndex("dbo.MedicineTransaction", new[] { "PaymentId" });
            DropIndex("dbo.MedicineTransaction", new[] { "PatientId" });
            DropIndex("dbo.MedicineTransaction", new[] { "PharmacyId" });
            DropIndex("dbo.MedicineTransaction", new[] { "PrescribedMedicineId" });
            DropIndex("dbo.PrescribedMedicine", new[] { "GenericMedicineId" });
            DropIndex("dbo.PrescribedMedicine", new[] { "PrescriptionId" });
            DropIndex("dbo.BrandedMedicine", new[] { "BrandId" });
            DropIndex("dbo.BrandedMedicine", new[] { "GenericMedicineId" });
            DropTable("dbo.UserType");
            DropTable("dbo.Prescription");
            DropTable("dbo.PharmacyInventory");
            DropTable("dbo.Pharmacy");
            DropTable("dbo.PaymentType");
            DropTable("dbo.Payment");
            DropTable("dbo.MedicalHistory");
            DropTable("dbo.DoctorType");
            DropTable("dbo.User");
            DropTable("dbo.MedicineTransaction");
            DropTable("dbo.PrescribedMedicine");
            DropTable("dbo.GenericMedicine");
            DropTable("dbo.Brand");
            DropTable("dbo.BrandedMedicine");
        }
    }
}

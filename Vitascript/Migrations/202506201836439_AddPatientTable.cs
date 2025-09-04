namespace Vitascript.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        AssignedDoctorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.AssignedDoctorId)
                .ForeignKey("dbo.User", t => t.PatientId)
                .Index(t => t.PatientId)
                .Index(t => t.AssignedDoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patient", "PatientId", "dbo.User");
            DropForeignKey("dbo.Patient", "AssignedDoctorId", "dbo.User");
            DropIndex("dbo.Patient", new[] { "AssignedDoctorId" });
            DropIndex("dbo.Patient", new[] { "PatientId" });
            DropTable("dbo.Patient");
        }
    }
}

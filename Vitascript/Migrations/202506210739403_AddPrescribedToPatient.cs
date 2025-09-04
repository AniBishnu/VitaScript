namespace Vitascript.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrescribedToPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patient", "Prescribed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patient", "Prescribed");
        }
    }
}

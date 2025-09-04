namespace Vitascript.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriceToMedicine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payment", "Mobilenumber", c => c.Int(nullable: false));
            AddColumn("dbo.Payment", "TransactionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payment", "TransactionId");
            DropColumn("dbo.Payment", "Mobilenumber");
        }
    }
}

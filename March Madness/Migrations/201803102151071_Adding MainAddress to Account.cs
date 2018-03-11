namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMainAddresstoAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MainAddress", c => c.String(nullable: false, maxLength: 42));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MainAddress");
        }
    }
}

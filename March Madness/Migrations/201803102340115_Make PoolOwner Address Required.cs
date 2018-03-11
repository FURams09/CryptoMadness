namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakePoolOwnerAddressRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pools", "OwnerAddress", c => c.String(nullable: false, maxLength: 42));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Pools", "OwnerAddress", c => c.String(maxLength: 42));
        }
    }
}

namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeEntryFeeDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pools", "EntryFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Pools", "EntryFee", c => c.Short(nullable: false));
        }
    }
}

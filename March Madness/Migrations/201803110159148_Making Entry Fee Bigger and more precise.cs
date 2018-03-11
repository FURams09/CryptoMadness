namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingEntryFeeBiggerandmoreprecise : DbMigration
    {
		public override void Up()
		{
			AlterColumn("dbo.Pools", "EntryFee", c => c.Decimal(nullable: false, precision: 29, scale: 38));
		}

		public override void Down()
		{
			AlterColumn("dbo.Pools", "EntryFee", c => c.Short(nullable: false));
		}
	}
}

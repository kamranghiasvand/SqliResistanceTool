namespace SqliResistanceModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginInfoModels", "SpecialTextBeforeLoginPage", c => c.String());
            AddColumn("dbo.LoginInfoModels", "SpecialTextAfterLoginPage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoginInfoModels", "SpecialTextAfterLoginPage");
            DropColumn("dbo.LoginInfoModels", "SpecialTextBeforeLoginPage");
        }
    }
}

namespace SqliResistanceModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1st : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUrl = c.String(),
                        LoginInfo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoginInfoModels", t => t.LoginInfo_Id)
                .Index(t => t.LoginInfo_Id);
            
            CreateTable(
                "dbo.LoginInfoModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginPage = c.String(),
                        LoginDataAsXml = c.String(),
                        LoginButton_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ElementSearchModels", t => t.LoginButton_Id)
                .Index(t => t.LoginButton_Id);
            
            CreateTable(
                "dbo.ElementSearchModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        By = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PageModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        SiteModel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteModels", t => t.SiteModel_Id)
                .Index(t => t.SiteModel_Id);
            
            CreateTable(
                "dbo.FormModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Page_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PageModels", t => t.Page_Id)
                .Index(t => t.Page_Id);
            
            CreateTable(
                "dbo.FormInputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        Src = c.String(),
                        Step = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        Required = c.Boolean(nullable: false),
                        Readonly = c.Boolean(nullable: false),
                        Placeholder = c.String(),
                        Name = c.String(),
                        Alt = c.String(),
                        Checked = c.Boolean(nullable: false),
                        Maxlength = c.Int(nullable: false),
                        Form_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FormModels", t => t.Form_Id)
                .Index(t => t.Form_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PageModels", "SiteModel_Id", "dbo.SiteModels");
            DropForeignKey("dbo.FormModels", "Page_Id", "dbo.PageModels");
            DropForeignKey("dbo.FormInputs", "Form_Id", "dbo.FormModels");
            DropForeignKey("dbo.SiteModels", "LoginInfo_Id", "dbo.LoginInfoModels");
            DropForeignKey("dbo.LoginInfoModels", "LoginButton_Id", "dbo.ElementSearchModels");
            DropIndex("dbo.FormInputs", new[] { "Form_Id" });
            DropIndex("dbo.FormModels", new[] { "Page_Id" });
            DropIndex("dbo.PageModels", new[] { "SiteModel_Id" });
            DropIndex("dbo.LoginInfoModels", new[] { "LoginButton_Id" });
            DropIndex("dbo.SiteModels", new[] { "LoginInfo_Id" });
            DropTable("dbo.FormInputs");
            DropTable("dbo.FormModels");
            DropTable("dbo.PageModels");
            DropTable("dbo.ElementSearchModels");
            DropTable("dbo.LoginInfoModels");
            DropTable("dbo.SiteModels");
        }
    }
}

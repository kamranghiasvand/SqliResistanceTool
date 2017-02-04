namespace SqliResistanceModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.FormInputs", newName: "FormInputModels");
            AddColumn("dbo.SiteModels", "SiteUrlString", c => c.String());
            AddColumn("dbo.SiteModels", "CrawlingDone", c => c.Boolean(nullable: false));
            AddColumn("dbo.SiteModels", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SiteModels", "FinishDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SiteModels", "LastFailReason", c => c.String());
            AddColumn("dbo.LoginInfoModels", "LoginPageString", c => c.String());
            AddColumn("dbo.PageModels", "UrlString", c => c.String());
            AddColumn("dbo.PageModels", "VisitedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PageModels", "RawContent", c => c.String());
            AddColumn("dbo.FormModels", "AcceptCharset", c => c.String());
            AddColumn("dbo.FormModels", "Action", c => c.String());
            AddColumn("dbo.FormModels", "Enctype", c => c.String());
            AddColumn("dbo.FormModels", "Method", c => c.String());
            AddColumn("dbo.FormModels", "Name", c => c.String());
            AddColumn("dbo.FormModels", "Novalidate", c => c.String());
            AddColumn("dbo.FormModels", "Target", c => c.String());
            AddColumn("dbo.FormInputModels", "Disabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.SiteModels", "SiteUrl");
            DropColumn("dbo.LoginInfoModels", "LoginPage");
            DropColumn("dbo.PageModels", "Url");
            DropColumn("dbo.FormInputModels", "Step");
            DropColumn("dbo.FormInputModels", "Required");
            DropColumn("dbo.FormInputModels", "Placeholder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FormInputModels", "Placeholder", c => c.String());
            AddColumn("dbo.FormInputModels", "Required", c => c.Boolean(nullable: false));
            AddColumn("dbo.FormInputModels", "Step", c => c.Int(nullable: false));
            AddColumn("dbo.PageModels", "Url", c => c.String());
            AddColumn("dbo.LoginInfoModels", "LoginPage", c => c.String());
            AddColumn("dbo.SiteModels", "SiteUrl", c => c.String());
            DropColumn("dbo.FormInputModels", "Disabled");
            DropColumn("dbo.FormModels", "Target");
            DropColumn("dbo.FormModels", "Novalidate");
            DropColumn("dbo.FormModels", "Name");
            DropColumn("dbo.FormModels", "Method");
            DropColumn("dbo.FormModels", "Enctype");
            DropColumn("dbo.FormModels", "Action");
            DropColumn("dbo.FormModels", "AcceptCharset");
            DropColumn("dbo.PageModels", "RawContent");
            DropColumn("dbo.PageModels", "VisitedDate");
            DropColumn("dbo.PageModels", "UrlString");
            DropColumn("dbo.LoginInfoModels", "LoginPageString");
            DropColumn("dbo.SiteModels", "LastFailReason");
            DropColumn("dbo.SiteModels", "FinishDate");
            DropColumn("dbo.SiteModels", "StartDate");
            DropColumn("dbo.SiteModels", "CrawlingDone");
            DropColumn("dbo.SiteModels", "SiteUrlString");
            RenameTable(name: "dbo.FormInputModels", newName: "FormInputs");
        }
    }
}

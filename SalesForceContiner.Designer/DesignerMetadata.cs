using System;
using System.Activities.Presentation.Metadata;
using System.Collections.Generic;
using System.ComponentModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(Salesforce_Marketing_Cloud_Scope), new DesignerAttribute(typeof(SalesforceLightningDesigner)));
            builder.AddCustomAttributes(typeof(Add_Campaign), new DesignerAttribute(typeof(AddCampaign)));


            builder.AddCustomAttributes(typeof(Add_List), new DesignerAttribute(typeof(AddList)));
            builder.AddCustomAttributes(typeof(Add_Subscriber), new DesignerAttribute(typeof(AddSubscriber)));
            builder.AddCustomAttributes(typeof(Add_DataExtension), new DesignerAttribute(typeof(AddDataExtension)));
            builder.AddCustomAttributes(typeof(Add_DataExtensionObject), new DesignerAttribute(typeof(AddDataExtensionObject)));

            builder.AddCustomAttributes(typeof(Delete_Campaign), new DesignerAttribute(typeof(DeleteCampaign)));
            builder.AddCustomAttributes(typeof(Delete_List), new DesignerAttribute(typeof(DeleteList)));
            builder.AddCustomAttributes(typeof(Delete_Subscriber), new DesignerAttribute(typeof(DeleteSubscriber)));
            builder.AddCustomAttributes(typeof(Delete_DataExtension), new DesignerAttribute(typeof(DeleteDataExtension)));
            builder.AddCustomAttributes(typeof(Delete_DataExtensionObject), new DesignerAttribute(typeof(DeleteDataExtensionObject)));

            builder.AddCustomAttributes(typeof(Get_Campaign_List), new DesignerAttribute(typeof(GetCampaignList)));
            builder.AddCustomAttributes(typeof(Get_List_List), new DesignerAttribute(typeof(GetListList)));
            builder.AddCustomAttributes(typeof(Get_Subscriber_List), new DesignerAttribute(typeof(GetSubscriberList)));
            builder.AddCustomAttributes(typeof(Get_DataExtension_List), new DesignerAttribute(typeof(GetDataExtensionList)));
            builder.AddCustomAttributes(typeof(Get_DataExtensionObject_List), new DesignerAttribute(typeof(GetDataExtensionObjectList)));
            builder.AddCustomAttributes(typeof(Get_Data), new DesignerAttribute(typeof(GetData)));
            builder.AddCustomAttributes(typeof(Get_Data_Wizard), new DesignerAttribute(typeof(SelectDesigner)));

            builder.AddCustomAttributes(typeof(Update_Campaign), new DesignerAttribute(typeof(UpdateCampaign)));
            builder.AddCustomAttributes(typeof(Update_List), new DesignerAttribute(typeof(UpdateList)));
            builder.AddCustomAttributes(typeof(Update_Subscriber), new DesignerAttribute(typeof(UpdateSubscriber)));
            builder.AddCustomAttributes(typeof(Update_DataExtension), new DesignerAttribute(typeof(UpdateDataExtension)));
            builder.AddCustomAttributes(typeof(Update_DataExtensionObject), new DesignerAttribute(typeof(UpdateDataExtensionObject)));

            builder.AddCustomAttributes(typeof(Format_DateTime_SFDC_API), new DesignerAttribute(typeof(FormatDateTimeSFDCAPI)));
            builder.AddCustomAttributes(typeof(Format_Combobox_SFDC_API), new DesignerAttribute(typeof(FormatComboboxSFDCAPI)));
            builder.AddCustomAttributes(typeof(Format_Listbox_SFDC_API), new DesignerAttribute(typeof(FormatListBoxSFDCAPI)));
            
            var CampaignStringCategoryName =
            new CategoryAttribute("UiPathTeam.Salesforce.Marketing_Cloud.Campaign");
            builder.AddCustomAttributes(typeof(Delete_Campaign), CampaignStringCategoryName);
            builder.AddCustomAttributes(typeof(Add_Campaign), CampaignStringCategoryName);
            builder.AddCustomAttributes(typeof(Update_Campaign), CampaignStringCategoryName);
            builder.AddCustomAttributes(typeof(Get_Campaign_List), CampaignStringCategoryName);

            var ListStringCategoryName =
            new CategoryAttribute("UiPathTeam.Salesforce.Marketing_Cloud.List");
            builder.AddCustomAttributes(typeof(Delete_List), ListStringCategoryName);
            builder.AddCustomAttributes(typeof(Add_List), ListStringCategoryName);
            builder.AddCustomAttributes(typeof(Update_List), ListStringCategoryName);
            builder.AddCustomAttributes(typeof(Get_List_List), ListStringCategoryName);

            var SubscriberStringCategoryName =
            new CategoryAttribute("UiPathTeam.Salesforce.Marketing_Cloud.Subscriber");
            builder.AddCustomAttributes(typeof(Add_Subscriber), SubscriberStringCategoryName);
            builder.AddCustomAttributes(typeof(Delete_Subscriber), SubscriberStringCategoryName);
            builder.AddCustomAttributes(typeof(Update_Subscriber), SubscriberStringCategoryName);
            builder.AddCustomAttributes(typeof(Get_Subscriber_List), SubscriberStringCategoryName);

            var DataExtensionStringCategoryName =
            new CategoryAttribute("UiPathTeam.Salesforce.Marketing_Cloud.DataExtension");
            builder.AddCustomAttributes(typeof(Add_DataExtension), DataExtensionStringCategoryName);
            builder.AddCustomAttributes(typeof(Delete_DataExtension), DataExtensionStringCategoryName);
            builder.AddCustomAttributes(typeof(Update_DataExtension), DataExtensionStringCategoryName);
            builder.AddCustomAttributes(typeof(Get_DataExtension_List), DataExtensionStringCategoryName);

            var DataExtensionObjStringCategoryName =
            new CategoryAttribute("UiPathTeam.Salesforce.Marketing_Cloud.DataExtensionObject");
            builder.AddCustomAttributes(typeof(Add_DataExtensionObject), DataExtensionObjStringCategoryName);
            builder.AddCustomAttributes(typeof(Delete_DataExtensionObject), DataExtensionObjStringCategoryName);
            builder.AddCustomAttributes(typeof(Update_DataExtensionObject), DataExtensionObjStringCategoryName);
            builder.AddCustomAttributes(typeof(Get_DataExtensionObject_List), DataExtensionObjStringCategoryName);

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}

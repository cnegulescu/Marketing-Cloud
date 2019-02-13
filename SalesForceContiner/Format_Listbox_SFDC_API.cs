using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Browsable(false)]
    [Description("Prepare any Listbox for the Salesforce instance input or update. You must provide your String and List as an input.")]
    [DisplayName("Format listbox for SFDC")]
    public class Format_Listbox_SFDC_API : CodeActivity
    {
        public Format_Listbox_SFDC_API()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Format_Listbox_SFDC_API>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Category("Input")]
        [DisplayName("Input String")]
        [RequiredArgument]
        [Description("Text value to format.")]
        public InArgument<string> Input_Value { get; set; }

        [Category("Input")]
        [DisplayName("List of Labels")]
        [Description("Valid fields labels from the listbox")]
        public InArgument<IEnumerable<string>> Input_List_Label { get; set; }

        [Category("Input")]
        [DisplayName("List of Values")]
        [Description("Valid fields values from the listbox")]
        public InArgument<IEnumerable<string>> Input_List_Value { get; set; }


        [Category("Output")]
        [DisplayName("Output Text")]
        [Description("Text value prepared for SFDC.")]
        public OutArgument<string> Output_Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String Error ="";
            String str = Input_Value.Get(context);
            IEnumerable<string> ListofLabel = Input_List_Label.Get(context);
            IEnumerable<string> ListofValue = Input_List_Value.Get(context);
            Boolean found = false;
            HelpMatch tmpMatch = new HelpMatch();
            try
            {
                foreach (String itm in ListofLabel)
                {
                    if (str.Trim().ToUpper().Equals(itm.ToUpper()))
                    {
                        str = itm;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    foreach (String itm in ListofValue)
                    {
                        if (str.Trim().ToUpper().Equals(itm.ToUpper()))
                        {
                            str = itm;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    foreach (String itm in ListofLabel)
                    {
                        if (tmpMatch.Decidematch(str.Trim().ToUpper(), itm.ToUpper()))
                        {
                            str = itm;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    foreach (String itm in ListofValue)
                    {
                        if (tmpMatch.Decidematch(str.Trim().ToUpper(), itm.ToUpper()))
                        {
                            str = itm;
                            found = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                str = "NO DATA";
            }
            Output_Text.Set(context, str);
        }
    }
}

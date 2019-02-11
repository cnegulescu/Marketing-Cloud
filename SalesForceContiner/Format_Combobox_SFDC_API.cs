using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Description("Prepare any combobox for the Salesforce instance input or update. You must provide your String and a List as an input.")]
    [DisplayName("Format combobox for SFDC")]
    public class Format_Combobox_SFDC_API : CodeActivity
    {
        public Format_Combobox_SFDC_API()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Format_Combobox_SFDC_API>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Category("Input")]
        [DisplayName("Input String")]
        [RequiredArgument]
        [Description("Text value to format.")]
        public InArgument<string> Input_Value { get; set; }

        [Category("Input")]
        [DisplayName("List of Labels")]
        [Description("Valid fields labels from the combobox")]
        public InArgument<IEnumerable<string>> Input_List_Label { get; set; }

        [Category("Input")]
        [DisplayName("List of Values")]
        [Description("Valid fields values from the combobox")]
        public InArgument<IEnumerable<string>> Input_List_Value { get; set; }


        [Category("Output")]
        [DisplayName("Output Text")]
        [Description("Text value prepared for SFDC.")]
        public OutArgument<string> Output_Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String Error = "";
            String str_out = "";
            String listofstr = Input_Value.Get(context);
            // i need to analize to what is the spliter I will go for ,

            try
            {
                IEnumerable<string> ListofLabel = Input_List_Label.Get(context);
                IEnumerable<string> ListofValue = Input_List_Value.Get(context);
                Boolean found = false;
                /// identify the splitter
                String Selected = ",";
                Int32 max = listofstr.Split(',').Length - 1;
                if (max < (listofstr.Split(';').Length - 1))
                {
                    Selected = ";";
                    max = listofstr.Split(';').Length - 1;
                }
                if (max < (listofstr.Split('\n').Length - 1))
                {
                    Selected = "\n";
                    max = listofstr.Split('\n').Length - 1;
                    if (max == (listofstr.Split(new string[] { "\r\n" }, StringSplitOptions.None).Length - 1)) Selected = "\r\n";
                }
                String[] splitedstr = listofstr.Split(new string[] { Selected }, StringSplitOptions.None);
                HelpMatch tmpMatch = new HelpMatch();
                foreach (String str in splitedstr)
                {
                    foreach (String itm in ListofLabel)
                    {
                        if (str.Trim().ToUpper().Equals(itm.ToUpper()))
                        {
                            str_out += itm + ";";
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
                                str_out += itm + ";";
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        foreach (String itm in ListofLabel)
                        {
                            if (tmpMatch.Decidematch(str.Trim().ToUpper(),itm.ToUpper()))
                            {
                                str_out += itm + ";";
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
                                str_out += itm + ";";
                                found = true;
                                break;
                            }
                        }
                    }
                }
                str_out = str_out.Remove(str_out.Length - 1);
            }           
            catch (Exception ex)
            {
                Error = ex.ToString();
                str_out = "NO DATA";
            }
            Output_Text.Set(context, str_out);
        }
    }
}

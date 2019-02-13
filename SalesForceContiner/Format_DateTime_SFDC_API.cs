using System;
using System.Activities;
using System.ComponentModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Browsable(false)]
    [Description("Prepare any String datetime for the Salesforce instance input or update. You must provide your String date as an input.")]
    [DisplayName("Format date for SFDC")]
    public class Format_DateTime_SFDC_API : CodeActivity
    {
        public Format_DateTime_SFDC_API()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Format_DateTime_SFDC_API>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Category("Input")]
        [DisplayName("Input Date")]
        [RequiredArgument]
        [Description("Text Date to format.")]
        public InArgument<string> Input_Date { get; set; }

        [Category("Input")]
        [DisplayName("Date Structure")]        
        [Description("Text field where you describe the format. This field is not manadatory. \r\n Examples of format: \r\n MM/dd/yy \r\n dd-MM-yyyy \r\n dd-MM-yyyy \r\n dd MMMM yyyy")]
        public InArgument<string> Input_Format { get; set; }


        [Category("Output")]
        [DisplayName("Output Text")]
        [Description("Text Date prepared for SFDC.")]
        public OutArgument<string> Output_Text { get; set; }

        [Category("Output")]
        [DisplayName("Output DateTime")]
        [Description("DateTime variable that can be use.")]
        public OutArgument<DateTime> Output_DateTime { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String Error ="";
            String str = Input_Date.Get(context);
            String format = ""+Input_Format.Get(context);
            DateTime tmpD = new DateTime();
            try
            {
                if (format.Length < 2)
                {
                    str = str.Trim();
                    if (str.Length >= 10)
                    {
                        str = str.Substring(0, str.IndexOf(" ")).Trim();
                        tmpD = Convert.ToDateTime(str);
                    }
                }
                else
                {
                    tmpD = DateTime.ParseExact(str, format, null);
                }
                str = tmpD.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                str = "1970-01-01";
                tmpD = new DateTime(1970, 1, 1);
            }
            Output_Text.Set(context, str);
            Output_DateTime.Set(context, tmpD);
        }
    }
}

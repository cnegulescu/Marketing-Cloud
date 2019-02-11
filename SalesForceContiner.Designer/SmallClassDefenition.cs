using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    public class ConvertModelToString
    {
        public String ConvertModelItem(object inputvalue)
        {
            //ModelItem modelItem = inputvalue as ModelItem;
            if (inputvalue != null)
            {
                InArgument<string> inArgument = inputvalue as InArgument<string>;

                if (inArgument != null)
                {
                    Activity<string> expression = inArgument.Expression;
                    VisualBasicValue<string> vbexpression = expression as VisualBasicValue<string>;
                    Literal<string> literal = expression as Literal<string>;

                    if (literal != null)
                    {
                        return literal.Value;
                    }
                    else if (vbexpression != null)
                    {
                        return vbexpression.ExpressionText;
                    }
                }
            }
            return null;
        }
    }

    public class InitConnectionData
    {
        public Dictionary<String, String> ReturnDict()
        {
            String sfdcConsumerkey = Salesforce_Marketing_Cloud_Scope.Design_KEY;
            String sfdcConsumerSecret = Salesforce_Marketing_Cloud_Scope.Design_SECRET;

            var dictionaryForUrl = new Dictionary<String, String>
                        {
                            {"clientId", sfdcConsumerkey},
                            {"clientSecret", sfdcConsumerSecret},
                        };
            return dictionaryForUrl;
        }
    }

    public class ComboBoxItem
    {
        public String ValueName { get; set; }
        public string ValueString { get; set; }
    }
    public class ViewModelEnum : INotifyPropertyChanged
    {

        /// Need a void constructor in order to use as an object element 
        /// in the XAML.
        public ViewModelEnum()
        {
        }

        private String _name = "";
        public String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// Need to implement this interface in order to get data binding
        /// to work properly.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

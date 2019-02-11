using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Windows;
using System.Xml;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{

    public enum Type_of_Environment
    {
        Design_and_Test = 0,
        Production
    }

    public enum BooleanType
    {
        False = 0,
        True
    }

    public enum Type_of_Search
    {
        Type_and_ID = 0,
        Type_ID_Name
    }

    public enum Type_of_Command
    {
        AddCustom = 0,
        AddAsset,
        AddContact,
        AddList,
        AddDataExtension,
        AddDataExtensionObject,
        AddCampaign,
        AddSubscriber,
        AddJourney,
        AssignFile,
        AddNewVerFile,
        UpdateList,
        UpdateAsset,
        UpdateContact,
        UpdateCampaign,
        UpdateSubscriber,
        UpdateFile,
        UpdateDataExtension,
        UpdateDataExtensionObject,
        DeleteSubscriber,
        DeleteJourney,
        DeleteCustom,
        DeleteList,
        DeleteCampaign,
        DeleteDataExtension,
        DeleteDataExtensionObject,
        Connection,
        SOQLcommand,
        ObjectList,
        CheckID,
        GenericSearch,
        ExecuteReport,
        JustDisplay,
        DownloadFile,
        SendDirectMessage,
        GetMandatoryCampaign,
        GetMandatoryContact,
        GetMandatoryList,
        GetMandatorySubscriber,
        GetMandatoryDataExtension,
        GetMandatoryDataExtensionObject,
    }

    public class SalesForceProperty
    {
        public String AuthToken { get; set; }
        public String ServiceURL { get; set; }
        public String UserName { get; set; }
        public Boolean isTest { get; set; }
        public MCser.SoapClient SoapClient { get; set; }

        public SalesForceProperty(MCser.SoapClient _soapClient, Boolean _isTest, String _AuthToken, String _ServiceURL)
        {
            SoapClient = _soapClient;
            isTest = _isTest;
            AuthToken = _AuthToken;
            ServiceURL = _ServiceURL;
        }
    }

    public class ExtractValues
    {
        public ExtractValues(String str, out String Property, out String[] Value, out MCser.SimpleOperators Operator)
        {
            Int32 endofProperty = 1;
            Int32 startofValue = 1;
            Operator = MCser.SimpleOperators.equals;
            if (str.Contains("<="))
            {
                Operator = MCser.SimpleOperators.lessThanOrEqual;
                endofProperty = str.IndexOf("<=");
                startofValue = endofProperty + 2;
            }
            else if (str.Contains(">="))
            {
                Operator = MCser.SimpleOperators.greaterThanOrEqual;
                endofProperty = str.IndexOf(">=");
                startofValue = endofProperty + 2;
            }
            else if (str.Contains("<"))
            {
                Operator = MCser.SimpleOperators.lessThan;
                endofProperty = str.IndexOf("<");
                startofValue = endofProperty + 1;
            }
            else if (str.Contains(">"))
            {
                Operator = MCser.SimpleOperators.greaterThan;
                endofProperty = str.IndexOf(">");
                startofValue = endofProperty + 1;
            }
            else
            {
                Operator = MCser.SimpleOperators.equals;
                endofProperty = str.IndexOf("=");
                startofValue = endofProperty + 1;
            }
            Property = str.Substring(0, endofProperty);
            Value = new String[] { str.Substring(startofValue) };
        }
    }

    public class ParametersArgument
    {
        public InArgument Parameter { get; set; }
        public InArgument ValueData { get; set; }
        public Boolean isEnabled { get; set; }

        public Visibility visi { get; set; }

    }

    public class CheckParentConstraint
    {
        public static Constraint GetCheckParentConstraint<ActivityType>(string parentTypeName, string validationMessage = null) where ActivityType : Activity
        {
            validationMessage = validationMessage ?? ValidationMessage(parentTypeName);

            DelegateInArgument<ValidationContext> context = new DelegateInArgument<ValidationContext>();
            var element = new DelegateInArgument<ActivityType>();
            var parent = new DelegateInArgument<Activity>();
            var result = new Variable<bool>();
            Variable<IEnumerable<Activity>> parentList = new Variable<IEnumerable<Activity>>();

            return new Constraint<ActivityType>
            {
                Body = new ActivityAction<ActivityType, ValidationContext>
                {
                    Argument1 = element,
                    Argument2 = context,
                    Handler = new Sequence
                    {
                        Variables =
                        {
                            result,
                            parentList
                        },
                        Activities =
                        {
                            new Assign<IEnumerable<Activity>>
                            {
                                To = parentList,
                                Value = new GetParentChain
                                {
                                    ValidationContext = context
                                }
                            },
                            new ForEach<Activity>
                            {
                                Values = parentList,
                                Body = new ActivityAction<Activity>
                                {
                                    Argument = parent,
                                    Handler = new If
                                    {
                                        Condition = new InArgument<bool>(ctx => parent.Get(ctx).GetType().Name.Equals(parentTypeName)),
                                        Then = new Assign<bool>
                                        {
                                            Value = true,
                                            To = result
                                        }
                                    }
                                }
                            },
                            new AssertValidation
                            {
                                Assertion = new InArgument<bool>(result),
                                Message = new InArgument<string> (validationMessage),
                            }
                        }
                    }
                }
            };
        }
        private static string ValidationMessage(string parentTypeName)
        {
            return string.Format("Activity is valid only inside {0}.", parentTypeName.Replace("_", " "));
        }
    }


    public class ObjectFromSalesforce
    {
        public String LabelName { get; set; }
        public String Type { get; set; }
        public String ParameterName { get; set; }
        public List<String> Values { get; set; }
        public List<String> Labels { get; set; }
        public String ExcelName { get; set; }
        public Boolean isID { get; set; }

        public ObjectFromSalesforce(String labelname, String paramName, String type, List<String> values, List<String> labels)
        {
            LabelName = labelname;
            Type = type;
            Values = values;
            Labels = labels;
            ParameterName = paramName;
            ExcelName = "";
            isID = false;
        }
    }

    [Browsable(false)]
    public class HelpMatch
    {
        private int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) && String.IsNullOrEmpty(b))
            {
                return 0;
            }
            if (String.IsNullOrEmpty(a))
            {
                return b.Length;
            }
            if (String.IsNullOrEmpty(b))
            {
                return a.Length;
            }
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }

        public Boolean Decidematch(String A, String B)
        {
            Boolean decision = false;
            //85% similaraty
            if (Math.Truncate((Double)((CalcLevenshteinDistance(A, B) * 100) / A.Length)) < 15) decision = true;
            return decision;
        }
    }

    public class FuelOAuthHeader : MessageHeader
    {
        public FuelOAuthHeader(string accessToken)
        {
            _accessToken = accessToken;
        }

        private string _accessToken;

        public override string Name => "fueloauth";

        public override string Namespace => "http://exacttarget.com";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteElementString("fueloauth", _accessToken);
        }
    }

    public class FuelOAuthInspector : IClientMessageInspector
    {
        public FuelOAuthInspector(string accessToken)
        {
            _accessToken = accessToken;
        }

        private string _accessToken;

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            request.Headers.Add(new FuelOAuthHeader(_accessToken));

            return Guid.NewGuid();
        }
    }

    public class FuelOAuthHeaderBehavior : IEndpointBehavior
    {
        public FuelOAuthHeaderBehavior(string accessToken)
        {
            _accessToken = accessToken;
        }

        private string _accessToken;

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new FuelOAuthInspector(_accessToken));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }

    public class SFConvert
    {
        public SFConvert()
        {           
        }

        public String FixString(String Value_To_Fix)
        {
            String str = Value_To_Fix;
            String Error = "";
            try
            {
                str = str.Replace(Environment.NewLine, " ");
                str = str.Replace("\r\n", " ");
                str = str.Replace("\n", " ");
                str = str.Replace("\r", " ");
                str = str.Trim();
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
            return str;
        }

        public String FixDate(String Date_To_Fix)
        {
            String Error = "";
            String str = Date_To_Fix;
            DateTime tmpD = new DateTime();
            try
            {
                str = str.Trim();
                if (str.Length >= 10)
                {
                    str = str.Substring(0, str.IndexOf(" ")).Trim();
                    tmpD = Convert.ToDateTime(str);
                }
                str = tmpD.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
                str = "1970-01-01";
                tmpD = new DateTime(1970, 1, 1);
            }
            return str;
        }
    }
    
}

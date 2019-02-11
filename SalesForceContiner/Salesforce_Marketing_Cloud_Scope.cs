#define TEST
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Security;
using System.Runtime.InteropServices;
using System.ServiceModel;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Description("Container for all the Salesforce commands. Inside of this activity you configure also all the parameters to connect to the Salesforce server.")]
    [DisplayName("Salesforce MC Application Scope")]
    public class Salesforce_Marketing_Cloud_Scope : NativeActivity
    {

        [Browsable(false)]
        public ActivityAction<SalesForceProperty> Body { get; set; }

        [Category("Connection")]
        public Type_of_Environment EnvironmentType { get; set; }

        [Category("Authentication Design")]
        [OverloadGroup("Authentication")]
        [Description("UserName")]
        [RequiredArgument]
        public InArgument<string> UserName { get; set; }

        [Category("Authentication Design")]
        [OverloadGroup("Authentication")]
        [Description("Password")]
        [RequiredArgument]
        public InArgument<String> Password { get; set; }

        [Category("Authentication Design")]
        [Description("Service URL")]
        public InArgument<string> ServiceURL { get; set; }

        [Category("Authentication Design")]
        [OverloadGroup("Authentication")]
        [Description("ConsumerKey")]
        [RequiredArgument]
        public InArgument<string> ConsumerKey { get; set; }

        [Category("Authentication Design")]
        [OverloadGroup("Authentication")]
        [Description("ConsumerSecret")]
        [RequiredArgument]
        public InArgument<string> ConsumerSecret { get; set; }


        [Category("Authentication Production")]
        [OverloadGroup("AuthenticationProd")]
        [Description("UserName")]
        [DisplayName("UserName Production")]
        [RequiredArgument]
        public InArgument<string> UserNameProd { get; set; }

        [Category("Authentication Production")]
        [OverloadGroup("AuthenticationProd")]
        [Description("Password")]
        [DisplayName("Password Production")]
        [RequiredArgument]
        public InArgument<SecureString> PasswordProd { get; set; }

        [Category("Authentication Production")]
        [Description("Service URL")]
        [DisplayName("Service URL Production")]
        public InArgument<SecureString> ServiceURLProd { get; set; }

        [Category("Authentication Production")]
        [OverloadGroup("AuthenticationProd")]
        [Description("ConsumerKey")]
        [DisplayName("ConsumerKey Production")]
        [RequiredArgument]
        public InArgument<string> ConsumerKeyProd { get; set; }

        [Category("Authentication Production")]
        [OverloadGroup("AuthenticationProd")]
        [Description("ConsumerSecret")]
        [DisplayName("ConsumerSecret Production")]
        [RequiredArgument]
        public InArgument<SecureString> ConsumerSecretProd { get; set; }


        [Category("Output")]
        [Description("Authentification string.")]
        public OutArgument<string> ResponseAuth { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }

        [Category("Use existing connection")]
        [DisplayName("Existing Authentification")]
        [OverloadGroup("Use existing connection")]
        [Description("Valid Authentification string.")]
        [RequiredArgument]
        public InArgument<string> ExistingAuth { get; set; }

        [Category("Use existing connection")]
        [DisplayName("Existing ServiceURL")]
        [OverloadGroup("Use existing connection")]
        [Description("Valid ServiceURL string.")]
        [RequiredArgument]
        public InArgument<string> ExistingServ { get; set; }

#if TEST

        [Browsable(false)]
        public static String Design_USER = "cristian.negulescu@uipath.com";

        [Browsable(false)]
        public static String Design_PASSWORD = "CristianUiPath2018*";

        [Browsable(false)]
        public static String Design_ServiceURL = "https://mc5b24d2svspmvqctfwlg9syq4f1.rest.marketingcloudapis.com";

        [Browsable(false)]
        public static String Design_KEY = "pui1i9b1s4js3rnbhpe6tqd0";

        [Browsable(false)]
        public static String Design_SECRET = "CvGtt8t4AwRMNcGR5qTh5ji0";
#else
        [Browsable(false)]
        public static String Design_USER = "";

        [Browsable(false)]
        public static String Design_PASSWORD = "";

        [Browsable(false)]
        public static String Design_ServiceURL = "";

        [Browsable(false)]
        public static String Design_KEY = "";

        [Browsable(false)]
        public static String Design_SECRET = "";
#endif
        [Browsable(false)]
        public static String Design_AUTH { get; set; }

        [Browsable(false)]
        public static MCser.SoapClient Design_SOAP { get; set; }

        [Browsable(false)]
        public static String Design_SERVICES { get; set; }

        [Browsable(false)]
        public static Boolean Design_VALIDCONN = false;

        [Browsable(false)]
        public static List<String> Desing_LISTOBJECT = new List<String>();

        [Browsable(false)]
        public static String Collect_Add_Data = "";

        [Browsable(false)]
        public static String Collect_Update_Data = "";

        [Browsable(false)]
        public static String Collect_Delete_Data = "";

        [Browsable(false)]
        public static Int32 Collect_Add_Count = 0;

        [Browsable(false)]
        //public static Hashtable<String,Hashtable> Desing_HASHOBJECT = new Hashtable<String,Hashtable>();
        public static Hashtable Desing_HASHOBJECT = new Hashtable();

        internal static string SalesForcePropertyTag { get { return "SalesForceScope"; } }

        private String RespAuthToken = "";
        private String RespServiceURL = "";
        public Salesforce_Marketing_Cloud_Scope()
        {
            Body = new ActivityAction<SalesForceProperty>
            {
                Argument = new DelegateInArgument<SalesForceProperty>(SalesForcePropertyTag),
                Handler = new Sequence { DisplayName = "Do" }
            };
        }

        private String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        } 
        protected override void Execute(NativeActivityContext context)
        {
            
            RestSharp.RestClient client = new RestSharp.RestClient();
            MCser.SoapClient soapClient = null;

            String ExistAuthToken = "" + ExistingAuth.Get(context);
            String ExistServiceURL = "" + ExistingServ.Get(context);

            if (ExistAuthToken.Trim().Length > 1)
            {
                RespAuthToken = ExistAuthToken.Trim();
                RespServiceURL = ExistServiceURL.Trim();
            }
            else
            {
                String sfdcConsumerkey = "";
                String sfdcConsumerSecret = "";
                String sfdcServiceURL = "";
                String sfdcUserName = "";
                String sfdcPassword = "";
                Boolean EnvType = (EnvironmentType == Type_of_Environment.Design_and_Test) ? true : false;
                if (EnvType)
                {
                    sfdcConsumerkey = ConsumerKey.Get(context);
                    sfdcConsumerSecret = ConsumerSecret.Get(context);
                    sfdcServiceURL = ServiceURL.Get(context);
                    sfdcUserName = UserName.Get(context);
                    sfdcPassword = Password.Get(context);
                }
                else
                {
                    sfdcConsumerkey = ConsumerKeyProd.Get(context);
                    sfdcConsumerSecret = SecureStringToString(ConsumerSecretProd.Get(context));
                    sfdcServiceURL = "" + SecureStringToString(ServiceURLProd.Get(context));
                    sfdcUserName = UserNameProd.Get(context);
                    sfdcPassword = SecureStringToString(PasswordProd.Get(context));
                }

                try
                {
                    client.BaseUrl = new Uri("https://auth.exacttargetapis.com/v1/requestToken");

                    var request2 = new RestRequest(Method.POST);
                    request2.RequestFormat = DataFormat.Json;
                    request2.AddParameter("clientId", sfdcConsumerkey);
                    request2.AddParameter("clientSecret", sfdcConsumerSecret);

                    JObject jsonObj = JObject.Parse(client.Post(request2).Content);
                    RespAuthToken = (String)jsonObj["accessToken"];
                    String ErrorType = "";
                    ErrorType = (String)jsonObj["error"];
                    String ErrorMsg = "";
                    ErrorMsg = (String)jsonObj["error_description"];

                                       
                    if ((RespAuthToken != null && RespAuthToken != "") && ErrorMsg == null)
                    {
                        BasicHttpsBinding binding = new BasicHttpsBinding();
                        binding.Name = "MyServicesSoap";
                        binding.CloseTimeout = TimeSpan.FromMinutes(1);
                        binding.OpenTimeout = TimeSpan.FromMinutes(1);
                        binding.ReceiveTimeout = TimeSpan.FromMinutes(60);
                        binding.SendTimeout = TimeSpan.FromMinutes(1);
                        binding.AllowCookies = false;
                        binding.BypassProxyOnLocal = false;
                        binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                        binding.MaxBufferSize = 20000000;
                        binding.MaxBufferPoolSize = 20000000;
                        binding.MaxReceivedMessageSize = 20000000;
                        binding.MessageEncoding = WSMessageEncoding.Text;
                        binding.TextEncoding = System.Text.Encoding.UTF8;
                        binding.TransferMode = TransferMode.Buffered;
                        binding.UseDefaultWebProxy = true;

                        binding.ReaderQuotas.MaxDepth = 32;
                        binding.ReaderQuotas.MaxStringContentLength = 8192;
                        binding.ReaderQuotas.MaxArrayLength = 16384;
                        binding.ReaderQuotas.MaxBytesPerRead = 4096;
                        binding.ReaderQuotas.MaxNameTableCharCount = 16384;

                        binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                        binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                        binding.Security.Transport.Realm = "";
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                        binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;


                        String endpointStr = "https://webservice.s10.exacttarget.com/Service.asmx";
                        EndpointAddress endpoint = new EndpointAddress(endpointStr);
                        soapClient = new MCser.SoapClient(binding, endpoint);
                        soapClient.ClientCredentials.UserName.UserName = sfdcUserName;
                        soapClient.ClientCredentials.UserName.Password = sfdcPassword;
                        soapClient.Endpoint.EndpointBehaviors.Add(new FuelOAuthHeaderBehavior(RespAuthToken));
                        RespServiceURL = sfdcServiceURL;
                    }
                    else if (RespAuthToken == null && (ErrorMsg != "" && ErrorMsg != null))
                    {
                        RespAuthToken = "Error Type: " + ErrorType;
                        RespServiceURL = "Error: " + ErrorMsg;
                    }
                }
                catch (Exception ex)
                {
                    RespAuthToken = "Error Type: " + ex.ToString();
                    RespServiceURL = "Error: " + ex.ToString();
                }
            }

            var salesForceProperty = new SalesForceProperty(soapClient, true,RespAuthToken,RespServiceURL);

            if (Body != null)
            {
                context.ScheduleAction<SalesForceProperty>(Body, salesForceProperty, OnCompleted, OnFaulted);
            }
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
           /// BizagiConnection.Set(faultContext, _bizagi);
           /// faultContext.CancelChildren();
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            ResponseAuth.Set(context, RespAuthToken);
        }

    }



}
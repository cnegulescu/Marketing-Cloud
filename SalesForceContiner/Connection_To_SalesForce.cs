using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SalesforceAPI
{
    public class Connection_to_Salesforce : AsyncCodeActivity<CmdRestAPI>
    {
        [Category("Connection")]
        public Type_of_Server ServerType { get; set; }

        [Category("Authentication")]
        [Description("UserName")]
        [RequiredArgument]
        public InArgument<string> UserName { get; set; }

        [Category("Authentication")]
        [Description("Password")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Category("Authentication")]
        [Description("SecurityToken")]
        [RequiredArgument]
        public InArgument<string> SecurityToken { get; set; }

        [Category("Authentication")]
        [Description("ConsumerKey")]
        [RequiredArgument]
        public InArgument<string> ConsumerKey { get; set; }

        [Category("Authentication")]
        [Description("ConsumerSecret")]
        [RequiredArgument]
        public InArgument<string> ConsumerSecret { get; set; }

        [Category("Output")]
        [Description("Authentification string.")]
        public OutArgument<string> ResponseAuth { get; set; }

        [Category("Output")]
        [Description("String with link of the service.")]
        public OutArgument<string> ResponseService { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }


        //private TraceSource traceSource = new TraceSource("Workflow");
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            //   traceSource.TraceEvent(TraceEventType.Information, 0, "0");

              /* String sfdcConsumerkey = "3MVG9CxwbdV68qJIIz27ukIaKmSfOCVO8I6JbMGHkEt4s5KwlreFqQqE.5UIpNcOds3xKVoib9cEZpUk3f5os";
               String sfdcConsumerSecret = "8782286442880574610";
               String sfdcUserName = "cristian.negulescu@uipath.com.uat";
               String sfdcPassword = "rewq432!";
               String sfdcSecurityToken = "dg912XkpjYZjhtj2kOMqjh4qU";*/

            String sfdcConsumerkey = ConsumerKey.Get(context);
            String sfdcConsumerSecret = ConsumerSecret.Get(context);
            String sfdcUserName = UserName.Get(context);
            String sfdcPassword = Password.Get(context);
            String sfdcSecurityToken = SecurityToken.Get(context);

            String SfdcloginPassword = sfdcPassword + sfdcSecurityToken;
            Boolean TestServer = (ServerType == Type_of_Server.Test) ? true : false;

            var dictionaryForUrl = new Dictionary<String, String>
            {
                {"grant_type","password" },
                {"client_id", sfdcConsumerkey},
                {"client_secret", sfdcConsumerSecret},
                {"username", sfdcUserName},
                {"password", SfdcloginPassword}
            };

            var task = (new CmdRestAPI(dictionaryForUrl,TestServer)).ExecuteAsync();
            var tcs = new TaskCompletionSource<CmdRestAPI>(state);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(t.Result);
                if (callback != null) callback(tcs.Task);
            });

            return tcs.Task;
        }

        protected override CmdRestAPI EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            CmdRestAPI temp = ((Task<CmdRestAPI>)result).Result;
            ResponseAuth.Set(context, temp.RespAuthToken);
            ResponseService.Set(context, temp.RespServiceURL);
            ValidConnection.Set(context, temp.ValidConnection);
            var property = context.DataContext.GetProperties()[Salesforce_Application_Scope.SalesForcePropertyTag];
            property.SetValue(context.DataContext, new SalesForceProperty("","","","","", (ServerType == Type_of_Server.Test) ? true : false,temp.RespAuthToken, temp.RespServiceURL));                
            return temp;
        }

    }
}

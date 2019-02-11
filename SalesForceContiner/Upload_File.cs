using System;
using System.Activities;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SalesforceAPI
{
    public class Upload_File : AsyncCodeActivity<CmdRestAPI>
    {

        [Category("Input")]
        [OverloadGroup("ContactID")]
        [RequiredArgument]
        [Description("ContactID to delete.")]
        public InArgument<string> ContactID { get; set; }

        [Category("Output")]
        [Description("String response from the server.")]
        public OutArgument<string> Response { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var property = context.DataContext.GetProperties()[Salesforce_Application_Scope.SalesForcePropertyTag];
            var salesForceProperty = property.GetValue(context.DataContext) as SalesForceProperty;

            String id = ContactID.Get(context);

            var task = (new CmdRestAPI(salesForceProperty.AuthToken, salesForceProperty.ServiceURL, null,"Contact",id,"",Type_of_Command.DeleteContact)).ExecuteAsync();
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
            Response.Set(context, temp.Response);
            ValidConnection.Set(context, temp.ValidConnection);
            return temp;
        }
        
    }
}

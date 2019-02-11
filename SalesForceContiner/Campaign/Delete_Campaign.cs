using System;
using System.Activities;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Description("Delete an campaing from the Salesforce instance. You must provide CampaingID")]
    public class Delete_Campaign : AsyncCodeActivity<CmdRestAPI>
    {
        public Delete_Campaign()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Delete_Campaign>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Browsable(false)]
        public new CmdRestAPI Result { get; set; }

        [Category("Input")]
        [OverloadGroup("CampaignID")]
        [RequiredArgument]
        [Description("CampaignID to delete.")]
        public InArgument<string> CampaignID { get; set; }

        [Category("Output")]
        [Description("String response from the server.")]
        public OutArgument<string> Response { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var property = context.DataContext.GetProperties()[Salesforce_Marketing_Cloud_Scope.SalesForcePropertyTag];
            var salesForceProperty = property.GetValue(context.DataContext) as SalesForceProperty;

            String id = CampaignID.Get(context);

            var task = (new CmdRestAPI(salesForceProperty.AuthToken, salesForceProperty.ServiceURL, salesForceProperty.SoapClient, null, "campaigns", id,"",Type_of_Command.DeleteCampaign)).ExecuteAsync();
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.AspNet.SignalR;
using AplicacaoMedicina.Hubs;
using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;

namespace AplicacaoMedicina.Helper
{
    public class NotificationComponents
    {
        public void RegisterNotification(DateTime currentTIme)
        {
            string conStr    = ConfigurationManager.ConnectionStrings["DefaultConnection_DatabasePublish"].ConnectionString;
            string sqlComand = "@SELECT [ID_NotifPaci],[Data_NotifPaci],[ID_Paci] from [dbo].[Notificacao] where [Data_NotifPaci] > @Data_NotifPaci";

            using(SqlConnection conn = new SqlConnection(conStr)){

                SqlCommand cmd = new SqlCommand(sqlComand, conn);
                cmd.Parameters.AddWithValue("@Data_NotifPaci", currentTIme);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                cmd.Notification     = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                }

            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");

                RegisterNotification(DateTime.Now);

            }
        }

        public List<Notificacao> GetContacts(DateTime afterDate)
        {
            using (DrMedContext db = new DrMedContext())
            {
                return db.Notificacao.Where(a => a.Data_NotifPaci > afterDate).OrderByDescending(a => a.Data_NotifPaci).ToList();
            }
        } 
    }

}
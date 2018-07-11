using System;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Write auditing information regarding mapping and assembly behavior to event log, GR_Cfg_Audit, and send email for error notifications.
    /// </summary>
    public class Audit
    {
        static string auditMessageL = "";
        public static string[] mE;
        public static int mEIndex;
        public static bool resetmE;
        public static void SetTruncate(string field, int fieldLen, int maxLen, string order_num, int order_line_num, string fieldContent)
        {
            if (resetmE == true)
            {
                mE = new string[50];
                mEIndex = 0;
                resetmE = false;
            }
            switch (fieldLen > maxLen)
            {
                case false:
                    return;
                default:
                    if (field == "MAPPING")
                    {
                        auditMessageL = "XML Mapping Error Has Occurred: " + fieldContent;
                    }
                    else
                    {
                        auditMessageL = "Truncated Data - Order: " + order_num + "  Line: " + order_line_num + "  Field: " + field + "  Field Length: " + fieldLen + "  Max Length: " + maxLen + "  Field Content: " + fieldContent;
                        DatabaseFactory.WriteAuditRecord(auditMessageL, order_num, order_line_num, "TRUNCATION");
                        Triggers.logEvent = auditMessageL;
                        System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
                        auditMessageL = "Truncated Data - Order: " + order_num + "  Line: " + order_line_num + "  Field: " + field + "  Field Length: " + fieldLen + "  Max Length: " + maxLen + Environment.NewLine + "  Field Content: " + fieldContent;
                        
                    }
                    mE[mEIndex] = auditMessageL;
                    mEIndex += 1;
                    break;
            }
        }
        public static void ProcessingCompleted(string auditMessage)
        {
            try
            {
                //check to see if there are any elements in the mE array and if so, send via email in digest format
                int ub = mEIndex;
                string outMessage = "";
                if (ub > 0)
                {
                    for (int i = 0; i < ub; i += 1) { if (mE[i].Length != 0 && mE[i] != null) { outMessage = outMessage + "ERROR #" + (i + 1) + Environment.NewLine + mE[i] + Environment.NewLine + Environment.NewLine; } }
                    SendMail.MailMessage(outMessage, "Configure One XML Mapping Errors");
                }
                
                Array.Clear(mE, 0, mE.Length);
                DatabaseFactory.WriteAuditRecord(auditMessage, StagingUtilities.globalOrderNum, StagingUtilities.globalOrderLineNum, "PROCESSING COMPLETED");
            }
            catch (Exception ex5)
            {
                Triggers.logEvent = ex5.Message + " -> " + ex5.Source + " -> " + ex5.InnerException;
                System.Diagnostics.EventLog.WriteEntry(Triggers.logSource, Triggers.logEvent, System.Diagnostics.EventLogEntryType.Warning, 234);
            }
        }
    }
}

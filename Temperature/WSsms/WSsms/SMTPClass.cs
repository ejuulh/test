using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace WSsms
{
    class SMTPClass
    {
        public SMTPClass()
        {

        }

        public bool SendMail(string host, bool ssl, bool anonymous, string user, string password, string domain, string from, string to, string bcc, string subject, string body, string AttachName, byte[] attach)
        {
            if (to == bcc)
                bcc = "";
            
            bool rtn = false;

            try
            {

                SmtpClient client = new SmtpClient();
                client.Host = host;
                client.EnableSsl = ssl;
                if (anonymous)
                {
                    client.Credentials = null;
                    client.UseDefaultCredentials = false;
                }
                //else if (rbDefault.Checked)
                //{
                //    client.Credentials = null;
                //    client.UseDefaultCredentials = true;
                //}
                else
                {
                    client.Credentials = new NetworkCredential(user, password, domain);
                    client.UseDefaultCredentials = false;
                }

                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(from, from);
                //message.ReplyTo = new MailAddress(txtFrom.Text, txtFrom.Text);

                // To Addresses may be seperated by semicolon (;)
                //-----------------------------------------------
                string[] addrs = to.Split(';');
                foreach (string addr in addrs)
                {
                    message.To.Add(new MailAddress(addr));
                }

                if (bcc != null && bcc != "")
                {
                    string[] bccs = bcc.Split(';');
                    foreach (string bc in bccs)
                    {
                        message.Bcc.Add(new MailAddress(bc));
                    }
                }

                message.Subject = subject;
                message.Body = body;
                if (attach != null && attach.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(attach);

                    //"application/ms-excel"
                    message.Attachments.Add(new Attachment(ms, AttachName, "text/plain"));
                }

                client.Send(message);
                //log.WriteLog("Mail Succeeded: " + to + "  " + subject, 0);
                //logMessageBox.Show("SendMail Succeeded", "SendMail Ok", MessageBoxButtons.OK, MessageBoxIcon.Information);

                rtn = true;

               
            }
            catch (Exception exp)
            {
                //log.WriteLog("Mail error: " + exp.Message.ToString(), 0);
                //LogClass logEntry = new LogClass();
                //log loginEntry = new log();
                //loginEntry.Bestil = true;
                //loginEntry.ErrorLevel = 2;
                //if (exp.InnerException != null)
                //    loginEntry.Tekst = exp.InnerException.ToString();
                //else
                //    loginEntry.Tekst = exp.ToString();
                //logEntry.WriteLog(loginEntry);
            }

            return rtn;
        }
    }
}

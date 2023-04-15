using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmailReadImapEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Limilabs.Mail;
using Limilabs.Client.IMAP;
using System.Net.Sockets;
using Limilabs.Proxy;

namespace GmailReadImapEmail.Tests
{
    [TestClass()]
    public class MailAccessorTests
    {
        [TestMethod()]
        public void MailAccessorTest()
        {
            Dictionary<string, string> emails = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader("emails.txt"))
            {
                string l = "";
                while((l = sr.ReadLine()) != null && l != "")
                {
                    string[] data = l.Split(':');
                    emails.Add(data[0], data[1]);
                }
            }
            for (int i = 0; i < emails.Count; i++)
            {
                MailAccessor mailAccessor = new MailAccessor("host", 993, true, emails.Keys.ElementAt(i), emails.Values.ElementAt(i));
                IMail[] mails = mailAccessor.GetEmails(Flag.All).ToList().OrderByDescending(x => x.Date).ToArray();
                mailAccessor.CloseConnection();
            }
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using Limilabs.Mail;
using Limilabs.Client.IMAP;
using System.Net.Sockets;
using Limilabs.Proxy;
using System;

namespace GmailReadImapEmail
{
    public class MailAccessor
    {
        private Imap client;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">imap.gmail.com</param>
        /// <param name="port">993</param>
        /// <param name="useSsl">true/false</param>
        /// <param name="login">Your login from email</param>
        /// <param name="password">Your password from email</param>
        public MailAccessor(string host, int port, bool useSsl, string login, string password)
        {
            client = new Imap();

            client.Connect(host, port, useSsl);  // or ConnectSSL for SSL
            client.UseBestLogin(login, password);
        }
        public MailAccessor(string host, int port, bool useSsl, string login, string password, ProxyType proxyType, string proxyIp, int proxyPort)
        {
            try
            {
                ProxyFactory factory = new ProxyFactory();
                IProxyClient proxy = factory.CreateProxy(
                   proxyType, proxyIp, proxyPort);

                Socket socket;
                client = new Imap();

                if (useSsl)
                {
                    socket = proxy.Connect(host, Imap.DefaultSSLPort);
                    client.AttachSSL(socket, host);
                }
                else
                {
                    socket = proxy.Connect(host, Imap.DefaultPort);
                    client.Attach(socket);
                }

                //client.Connect(host, port, useSsl);  // or ConnectSSL for SSL
                client.UseBestLogin(login, password);
            }
            catch(Exception ex)
            {
                if(client?.Connected == true)
                {
                    client.Close();
                }

                throw ex;
            }
        }
        public MailAccessor(string host, int port, bool useSsl, string login, string password, ProxyType proxyType, string proxyIp, int proxyPort, string proxyUser, string proxyPassword)
        {
            try
            {
                ProxyFactory factory = new ProxyFactory();
                IProxyClient proxy = factory.CreateProxy(
                   proxyType, proxyIp, proxyPort, proxyUser, proxyPassword);

                Socket socket;
                client = new Imap();

                if (useSsl)
                {
                    socket = proxy.Connect(host, Imap.DefaultSSLPort);
                    client.AttachSSL(socket, host);
                }
                else
                {
                    socket = proxy.Connect(host, Imap.DefaultPort);
                    client.Attach(socket);
                }

                //client.Connect(host, port, useSsl);  // or ConnectSSL for SSL
                client.UseBestLogin(login, password);
            }
            catch (Exception ex)
            {
                if (client?.Connected == true)
                {
                    client.Close();
                }

                throw ex;
            }
        }
        /// <summary>
        /// Get all emails from specific folder
        /// </summary>
        /// <param name="flag">Type of emails</param>
        /// <param name="folder">Folder's name (inbox)</param>
        /// <returns>Returns list of IMail objects</returns>
        public IEnumerable<IMail> GetEmails(Flag flag, string folder) 
        {
            client.Select(folder);
            List<long> uids = client.Search(flag);
            foreach (long uid in uids)
            {
                var eml = client.GetMessageByUID(uid);
                IMail email = new MailBuilder().CreateFromEml(eml);

                yield return email;
            }
        }
        /// <summary>
        /// Get all emails from all folders
        /// </summary>
        /// <param name="flag">Type of emails</param>
        /// <returns>Returns list of IMail objects</returns>
        public IEnumerable<IMail> GetEmails(Flag flag)
        {
            foreach (FolderInfo folder in client.GetFolders())
            {
                client.Select(folder);
                List<long> uids = client.Search(flag);
                foreach (long uid in uids)
                {
                    var eml = client.GetMessageByUID(uid);
                    IMail email = new MailBuilder().CreateFromEml(eml);

                    yield return email;
                }
            }
        }
        public void CloseConnection()
        {
            client.Close();
        }
    }
}
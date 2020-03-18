using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPMailer
{
    public partial class IPMailer : Form
    {
        public IPMailer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtEmail.Text = Properties.Settings.Default.email;
            lblIP.Text = Properties.Settings.Default.ip;
            chkAuto.Checked = Properties.Settings.Default.auto;
            txtLog.Text = "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
            btnOK.Text = "Running";
            txtLog.Text = "";
            Application.DoEvents();
            
            // Get IP address
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect("8.8.8.8", 80);
                IPAddress ipAddr = ((IPEndPoint)s.LocalEndPoint).Address;
                lblIP.Text = ipAddr.ToString();
                txtLog.Text += "Present IP: " + lblIP.Text + Environment.NewLine;
            }
            catch (Exception ex)
            {
                txtLog.Text += ex.ToString() + Environment.NewLine;
                btnOK.Enabled = true;
                btnOK.Text = "OK";
                return;
            }


            if (Properties.Settings.Default.ip != lblIP.Text)
            {
                // Send mail
                try
                {
                    Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                    Microsoft.Office.Interop.Outlook.MailItem email = (Microsoft.Office.Interop.Outlook.MailItem)(oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem));
                    email.Recipients.Add(txtEmail.Text);
                    email.Subject = "Updated IP" + lblIP.Text;
                    email.Body = "See subject...";
                    ((Microsoft.Office.Interop.Outlook.MailItem)email).Send();
                    txtLog.Text += "Mail sent." + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    txtLog.Text += ex.ToString() + Environment.NewLine;
                    btnOK.Enabled = true;
                    btnOK.Text = "OK";
                    return;
                }
                chkAuto.Checked = true;
                tmrAuto.Start();

                Properties.Settings.Default.ip = lblIP.Text;
                Properties.Settings.Default.email = txtEmail.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                txtLog.Text += "IP unchanged." + Environment.NewLine;
            }
            
            if (Properties.Settings.Default.auto != chkAuto.Checked)
            {
                if (chkAuto.Checked)
                    StartupManager.AddApplicationToCurrentUserStartup();
                else
                    StartupManager.RemoveApplicationFromCurrentUserStartup();
                Properties.Settings.Default.auto = chkAuto.Checked;
                Properties.Settings.Default.Save();
            }

            btnOK.Enabled = true;
            btnOK.Text = "OK";
        }

        private void tmrAuto_Tick(object sender, EventArgs e)
        {
            if(chkAuto.Checked)
                btnOK_Click(sender, e);
        }

        private void IPMailer_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void IPMailer_Shown(object sender, EventArgs e)
        {
            if (chkAuto.Checked)
            {
                WindowState = FormWindowState.Minimized;
                //Hide();
            }
        }
    }
}

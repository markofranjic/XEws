﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using XEws.CmdletAbstract;

namespace XEws.Cmdlet
{
    [Cmdlet("Import", "XEwsSession")]
    public class ImportXEwsSession : XEwsSessionCmdlet
    {
        private string autodiscoverEmail = null;
        [Parameter()]
        public string AutodiscoverEmail
        {
            get
            {
                return autodiscoverEmail;
            }
            set
            {
                autodiscoverEmail = value;
            }
        }

        protected override void ProcessRecord()
        {
            this.SetSessionVariable(this.UserName, this.Password, this.EwsUri, this.autodiscoverEmail, this.ImpersonationEmail, this.TraceEnabled, this.TraceOutputFolder, this.TraceFlags);

            WriteVerbose(String.Format("Using following Ews endpoint: {0}", this.GetSessionVariable().Url.ToString()));
        }
    }
}

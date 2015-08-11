﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Microsoft.Exchange.WebServices.Data;
using XEws.CmdletAbstract;

namespace XEws.Cmdlet
{
    [Cmdlet(VerbsCommon.Add, "XEwsFolder")]
    public class AddXEwsFolder : XEwsFolderCmdlet
    {

        [Parameter(Mandatory = false, Position = 0)]
        public string FolderName
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            if (this.FolderRoot == null)
                this.FolderRoot = XEwsFolderCmdlet.GetWellKnownFolder(WellKnownFolderName.Inbox, this.GetSessionVariable());

            try
            {
                this.AddFolder(this.FolderName, this.FolderRoot, this.GetSessionVariable());
                WriteVerbose(String.Format("Successfully created folder '{0}' under '{1}'.", this.FolderName, this.FolderRoot.DisplayName));
            }
            catch (Exception)
            {
                throw;
            }            
        }

        #region Overriden methods

        new private SwitchParameter Recurse
        {
            get;
            set;
        }


        #endregion
    }
}
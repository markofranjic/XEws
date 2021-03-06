﻿namespace XEws.Cmdlet
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.Exchange.WebServices.Data;
    using XEws.CmdletAbstract;

    [Cmdlet(VerbsCommon.Get, "XEwsFolder")]
    public sealed class GetXEwsFolder : XEwsFolderCmdlet
    {

        protected override void ProcessRecord()
        {
            /*            
                If search root is not specified, default to Inbox.
            */

            if (this.FolderRoot == null)
                this.FolderRoot = XEwsFolderCmdlet.GetWellKnownFolder(WellKnownFolderName.MsgFolderRoot, this.GetSessionVariable());

            List<Folder> folders = new List<Folder>();

            /*
                Find all folders and subfolders under search root.
            */

            if (this.Recurse)
            {
                this.GetRecurseFolder(this.FolderRoot, ref folders);
            }

            /*
                Find folders only directly under search root.
            */

            else
            {
                folders = this.GetFolder(this.FolderRoot);
            }

            WriteObject(folders, true);
        }
    }
}

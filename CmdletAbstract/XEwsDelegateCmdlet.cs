﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.ObjectModel;

namespace XEws.CmdletAbstract
{
    public abstract class XEwsDelegateCmdlet : XEwsCmdlet
    {
        private string delegateEmailAddress = String.Empty;
        [Parameter()]
        public string DelegateEmailAddress
        {
            get
            {
                return delegateEmailAddress;
            }
            set
            {
                delegateEmailAddress = value;
            }
        }

        private bool viewPrivateItems = false;
        [Parameter()]
        public bool ViewPrivateItems
        {
            get
            {
                return viewPrivateItems;
            }
            set
            {
                viewPrivateItems = value;
            }
        }

        private bool receiveCopyOfMeetings = false;
        [Parameter()]
        public bool ReceiveCopyOfMeetings
        {
            get
            {
                return receiveCopyOfMeetings;
            }
            set
            {
                receiveCopyOfMeetings = value;
            }
        }

        private DelegateFolderPermissionLevel calendarPermission = DelegateFolderPermissionLevel.None;
        [Parameter()]
        public DelegateFolderPermissionLevel CalendarPermission
        {
            get
            {
                return calendarPermission;
            }
            set
            {
                calendarPermission = value;
            }
        }

        private DelegateFolderPermissionLevel inboxPermission = DelegateFolderPermissionLevel.None;
        [Parameter()]
        public DelegateFolderPermissionLevel InboxPermission
        {
            get
            {
                return inboxPermission;
            }
            set
            {
                inboxPermission = value;
            }
        }

        private DelegateFolderPermissionLevel taskPermission = DelegateFolderPermissionLevel.None;
        [Parameter()]
        public DelegateFolderPermissionLevel TaskPermission
        {
            get
            {
                return taskPermission;
            }
            set
            {
                taskPermission = value;
            }
        }

        private DelegateFolderPermissionLevel contactPermission = DelegateFolderPermissionLevel.None;
        [Parameter()]
        public DelegateFolderPermissionLevel ContactPermission
        {
            get
            {
                return contactPermission;
            }
            set
            {
                contactPermission = value;
            }
        }

        internal List<XEwsDelegate> GetDelegate(ExchangeService ewsSession)
        {
            List<XEwsDelegate> xewsDelegate = new List<XEwsDelegate>();

            Mailbox currentMailbox = new Mailbox(ewsSession.ImpersonatedUserId.Id);

            DelegateInformation xewsDelegateInformation = ewsSession.GetDelegates(currentMailbox, true);

            foreach (DelegateUserResponse delegateResponse in xewsDelegateInformation.DelegateUserResponses)
            {
                if (delegateResponse.Result == ServiceResult.Success)
                {
                    xewsDelegate.Add(new XEwsDelegate(delegateResponse));
                }
            }

            return xewsDelegate;            
        }

        internal XEwsDelegate GetDelegate(string delegateEmailAddress, ExchangeService ewsSession)
        {
            ValidateUserName(delegateEmailAddress);

            List<XEwsDelegate> ewsDelegates = this.GetDelegate(ewsSession);

            foreach (XEwsDelegate ewsDelegate in ewsDelegates)
            {
                if (ewsDelegate.DelegateUserId.ToLower() == delegateEmailAddress.ToLower())
                {
                    return ewsDelegate;
                }
            }

            throw new InvalidOperationException("No delegate found with email address: " + delegateEmailAddress);
        }

        internal void SetDelegate(XEwsDelegate xewsDelegate, DelegateAction delegateAction, ExchangeService ewsSession)
        {
            ValidateUserName(xewsDelegate.DelegateUserId);

            DelegateUser delegateUser = new DelegateUser(xewsDelegate.DelegateUserId);
            Mailbox currentMailbox = new Mailbox(ewsSession.ImpersonatedUserId.Id);

            delegateUser.ReceiveCopiesOfMeetingMessages = xewsDelegate.ReceivesCopyOfMeeting;
            delegateUser.Permissions.CalendarFolderPermissionLevel = xewsDelegate.CalendarFolderPermission;
            delegateUser.Permissions.InboxFolderPermissionLevel = xewsDelegate.InboxFolderPermission;
            delegateUser.Permissions.ContactsFolderPermissionLevel = xewsDelegate.ContactFolderPermission;
            delegateUser.Permissions.TasksFolderPermissionLevel = xewsDelegate.TaskFolderPermission;
            
            switch (delegateAction)
            {
                case DelegateAction.Update:
                    ewsSession.UpdateDelegates(currentMailbox, MeetingRequestsDeliveryScope.DelegatesAndMe, delegateUser);
                    break;

                case DelegateAction.Add:
                    ewsSession.AddDelegates(currentMailbox, MeetingRequestsDeliveryScope.DelegatesAndMe, delegateUser);
                    break;

                default:
                    break;
            }
        }

        internal enum DelegateAction
        {
            Update,
            Add
        }

        public sealed class XEwsDelegate
        {
            public XEwsDelegate(DelegateUserResponse delegateResponse)
            {
                this.DelegateUserId = delegateResponse.DelegateUser.UserId.PrimarySmtpAddress;
                this.ReceivesCopyOfMeeting = delegateResponse.DelegateUser.ReceiveCopiesOfMeetingMessages;
                this.ViewPrivateItems = delegateResponse.DelegateUser.ViewPrivateItems;
                this.CalendarFolderPermission = delegateResponse.DelegateUser.Permissions.CalendarFolderPermissionLevel;
                this.InboxFolderPermission = delegateResponse.DelegateUser.Permissions.InboxFolderPermissionLevel;
                this.ContactFolderPermission = delegateResponse.DelegateUser.Permissions.ContactsFolderPermissionLevel;
                this.TaskFolderPermission = delegateResponse.DelegateUser.Permissions.TasksFolderPermissionLevel;
            }

            public XEwsDelegate(string delegateEmailAddress, bool receiveCopiesOfMeeting, bool viewPrivateItems, params DelegateFolderPermissionLevel[] delegatePermissionLevel )
            {
                this.DelegateUserId = delegateEmailAddress;
                this.ReceivesCopyOfMeeting = receiveCopiesOfMeeting;
                this.ViewPrivateItems = viewPrivateItems;
                this.CalendarFolderPermission = delegatePermissionLevel[0];
                this.InboxFolderPermission = delegatePermissionLevel[1];
                this.TaskFolderPermission = delegatePermissionLevel[2];
                this.ContactFolderPermission = delegatePermissionLevel[3];
            }


            public string DelegateUserId
            {
                get;
                private set;
            }

            public bool ReceivesCopyOfMeeting
            {
                get;
                internal set;
            }

            public bool ViewPrivateItems
            {
                get;
                internal set;
            }

            public DelegateFolderPermissionLevel CalendarFolderPermission
            {
                get;
                internal set;
            }

            public DelegateFolderPermissionLevel InboxFolderPermission
            {
                get;
                internal set;
            }

            public DelegateFolderPermissionLevel ContactFolderPermission
            {
                get;
                internal set;
            }

            public DelegateFolderPermissionLevel TaskFolderPermission
            {
                get;
                internal set;
            }

        }
    }
}
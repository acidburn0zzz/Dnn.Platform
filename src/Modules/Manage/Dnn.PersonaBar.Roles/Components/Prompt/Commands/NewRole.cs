﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using Dnn.PersonaBar.Roles.Components.Prompt.Models;
using Dnn.PersonaBar.Roles.Services.DTO;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Roles;

namespace Dnn.PersonaBar.Roles.Components.Prompt.Commands
{
    [ConsoleCommand("new-role", "Creates a new DNN security roles in the portal.", new[]{
        "name",
        "description",
        "public",
        "autoassign"
    })]
    public class NewRole : ConsoleCommandBase
    {
        protected override string LocalResourceFile => Constants.LocalResourcesFile;

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(NewRole));

        private const string FlagIsPublic = "public";
        private const string FlagAutoAssign = "autoassign";
        private const string FlagRoleName = "name";
        private const string FlagDescription = "description";
        private const string FlagStatus = "status";


        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool AutoAssign { get; set; }
        public RoleStatus Status { get; set; }


        public override void Init(string[] args, PortalSettings portalSettings, UserInfo userInfo, int activeTabId)
        {
            base.Init(args, portalSettings, userInfo, activeTabId);
            RoleName = GetFlagValue(FlagRoleName, "Rolename", string.Empty, true, true);
            Description = GetFlagValue(FlagDescription, "Description", string.Empty);
            IsPublic = GetFlagValue(FlagIsPublic, "Is Public", false, true);
            AutoAssign = GetFlagValue(FlagAutoAssign, "Auto Assign", false, true);
            var status = GetFlagValue(FlagStatus, "Status", "approved");
            switch (status)
            {
                case "pending":
                    Status = RoleStatus.Pending;
                    break;
                case "approved":
                    Status = RoleStatus.Approved;
                    break;
                case "disabled":
                    Status = RoleStatus.Disabled;
                    break;
                default:
                    AddMessage(string.Format(LocalizeString("Prompt_InvalidRoleStatus"), FlagStatus));
                    break;
            }
        }

        public override ConsoleResultModel Run()
        {
            try
            {
                var lstResults = new List<RoleModel>();
                var roleDto = new RoleDto
                {
                    Id = Null.NullInteger,
                    Description = Description,
                    Status = Status,
                    Name = RoleName,
                    AutoAssign = AutoAssign,
                    IsPublic = IsPublic,
                    GroupId = -1,
                    IsSystem = false,
                    SecurityMode = SecurityMode.SecurityRole
                };
                KeyValuePair<HttpStatusCode, string> message;
                var success = RolesController.Instance.SaveRole(PortalSettings, roleDto, false, out message);
                if (!success) return new ConsoleErrorResultModel(message.Value);

                lstResults.Add(new RoleModel(RoleController.Instance.GetRoleById(PortalId, roleDto.Id)));
                return new ConsoleResultModel(LocalizeString("RoleAdded.Message")) { Data = lstResults, Records = lstResults.Count };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ConsoleErrorResultModel(LocalizeString("RoleAdded.Error"));
            }

        }
    }
}
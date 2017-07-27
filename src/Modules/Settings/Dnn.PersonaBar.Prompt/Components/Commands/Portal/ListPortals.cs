﻿using System.Collections.Generic;
using System.Text;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using Dnn.PersonaBar.Prompt.Components.Models;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace Dnn.PersonaBar.Prompt.Components.Commands.Portal
{
    [ConsoleCommand("list-portals", "Retrieves a list of portals for the current DNN Installation")]
    public class ListPortals : ConsoleCommandBase
    {
        protected override string LocalResourceFile => Constants.LocalResourcesFile;

        public int? PortalIdFlagValue { get; private set; }

        public override void Init(string[] args, PortalSettings portalSettings, UserInfo userInfo, int activeTabId)
        {
            base.Init(args, portalSettings, userInfo, activeTabId);
            if (args.Length == 1)
            {
                // do nothing
            }
            else
            {
                AddMessage("The get-portal command does not take any arguments or flags; ");
            }
        }

        public override ConsoleResultModel Run()
        {
            var pc = PortalController.Instance;
            var lst = new List<PortalModelBase>();

            var alPortals = pc.GetPortals();
            foreach (PortalInfo portal in alPortals)
            {
                lst.Add(new PortalModelBase(portal));
            }

            return new ConsoleResultModel(string.Empty) { Data = lst, Records = lst.Count };
        }


    }
}
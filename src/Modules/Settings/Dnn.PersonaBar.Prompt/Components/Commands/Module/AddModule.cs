﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using Dnn.PersonaBar.Prompt.Components.Models;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;

namespace Dnn.PersonaBar.Prompt.Components.Commands.Module
{
    [ConsoleCommand("add-module", "Adds a new module instance to a page", new[] { "name", "pageid", "pane", "title" })]
    public class AddModule : ConsoleCommandBase
    {
        protected override string LocalResourceFile => Constants.LocalResourcesFile;

        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(AddModule));

        private const string FlagModuleName = "name";
        private const string FlagPageId = "pageid";
        private const string FlagPane = "pane";
        private const string FlagModuleTitle = "title";

        private string ModuleName { get; set; }
        private int PageId { get; set; }    // the page on which to add the module
        private string Pane { get; set; }
        private string ModuleTitle { get; set; }   // title for the new module. defaults to friendly name

        public override void Init(string[] args, PortalSettings portalSettings, UserInfo userInfo, int activeTabId)
        {
            base.Init(args, portalSettings, userInfo, activeTabId);
            ModuleName = GetFlagValue(FlagModuleName, "Module Name", string.Empty, true, true);
            PageId = GetFlagValue(FlagPageId, "Page Id", -1, true, false, true);
            ModuleTitle = GetFlagValue(FlagModuleTitle, "Module Title", string.Empty);
            Pane = GetFlagValue(FlagPane, "Pane", "ContentPane");
        }

        public override ConsoleResultModel Run()
        {
            try
            {
                var desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(ModuleName, PortalId);
                if (desktopModule == null)
                {
                    return new ConsoleErrorResultModel(string.Format(LocalizeString("Prompt_DesktopModuleNotFound"), ModuleName));
                }

                KeyValuePair<HttpStatusCode, string> message;
                var addedModules = ModulesController.Instance.AddNewModule(PortalSettings, ModuleTitle, desktopModule.DesktopModuleID, PageId, Pane, 0, 0, null, out message);
                if (addedModules == null)
                {
                    return new ConsoleErrorResultModel(message.Value);
                }
                if (addedModules.Count == 0)
                    return new ConsoleErrorResultModel(LocalizeString("Prompt_NoModulesAdded"));
                var modules = addedModules.Select(newModule => ModuleInstanceModel.FromDnnModuleInfo(ModuleController.Instance.GetTabModule(newModule.TabModuleID))).ToList();

                return new ConsoleResultModel(string.Format(LocalizeString("Prompt_ModuleAdded"), modules.Count, modules.Count == 1 ? string.Empty : "s")) { Data = modules, Records = modules.Count };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ConsoleErrorResultModel(LocalizeString("Prompt_AddModuleError"));
            }
        }
    }
}
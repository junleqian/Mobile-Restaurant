namespace Gour.Web
{
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Gour.Web.Infrastructure;

    public class WebRole : RoleEntryPoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This method initializes the Web role.")]
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += this.RoleEnvironmentChanging;

            // If no valid Connection String is found in the Web Role configuration, then the Web Role shouldn't start
            if (!UpdateMembershipProviderConnectionString())
            {
                return false;
            }

            return base.OnStart();
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
        private static bool UpdateMembershipProviderConnectionString()
        {
            using (var server = new Microsoft.Web.Administration.ServerManager())
            {
                var siteNameFromServiceModel = "Web";
                var siteName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);

                var configFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\Web.config", server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath);
                var xml = System.Xml.Linq.XElement.Load(configFilePath);

                if (UpdateAttributeWithRoleSetting(xml.Element("connectionStrings").Element("add").Attribute("connectionString"), "SqlSampleDataContextConnectionString"))
                {
                    xml.Save(configFilePath);
                    return true;
                }

                return false;
            }
        }

        private static bool UpdateAttributeWithRoleSetting(System.Xml.Linq.XAttribute attribute, string settingName)
        {
            var settingValue = ConfigReader.GetConfigValue(settingName, false);
            if (!string.IsNullOrWhiteSpace(settingValue))
            {
                attribute.Value = settingValue;
            }
            else if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return false;
            }

            return true;
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}

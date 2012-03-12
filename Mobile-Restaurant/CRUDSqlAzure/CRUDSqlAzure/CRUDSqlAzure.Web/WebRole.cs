// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Web
{
    using System.Linq;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WebRole : RoleEntryPoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This method initializes the Web role.")]
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += this.RoleEnvironmentChanging;

            // If no valid Connection String is found in the Web Role configuration, then the Web Role shouldn't start
            if (!UpdateMembershipProviderConnectionString() || !UpdateNorthwindProviderConnectionString() || !UpdateInfrastructureEntitiesConnectionString())
            {
                return false;
            }

            // If no valid WIF settings are found in the Web Role configuration, then the Web Role shouldn't start
            if (!UpdateWifSettings())
            {
                return false;
            }

            return base.OnStart();
        }

        private static System.Xml.Linq.XAttribute GetConnectionStringAttribute(System.Xml.Linq.XElement root, string connectionStringName)
        {
            var addElements = root.Descendants("connectionStrings").FirstOrDefault().Descendants();
            var connectionStringNode = addElements.Where(el => el.Attribute("name").Value.Equals(connectionStringName)).FirstOrDefault();
            return connectionStringNode.Attribute("connectionString");
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

                var attribute = GetConnectionStringAttribute(xml, "DefaultConnection");

                if (UpdateAttributeWithRoleSetting(attribute, "SqlSampleDataContextConnectionString"))
                {
                    xml.Save(configFilePath);
                    return true;
                }

                return false;
            }
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
        private static bool UpdateNorthwindProviderConnectionString()
        {
            using (var server = new Microsoft.Web.Administration.ServerManager())
            {
                var siteNameFromServiceModel = "Web";
                var siteName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);

                var configFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\Web.config", server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath);
                var xml = System.Xml.Linq.XElement.Load(configFilePath);

                var connectionString = ConfigReader.GetConfigValue("SqlSampleDataContextConnectionString", false);
                if (string.IsNullOrEmpty(connectionString))
                {
                    return false;
                }

                var attribute = GetConnectionStringAttribute(xml, "NorthwindEntities");

                attribute.Value = string.Format(@"metadata=res://*/Models.Northwind.csdl|res://*/Models.Northwind.ssdl|res://*/Models.Northwind.msl;provider=System.Data.SqlClient;provider connection string=""{0};App=EntityFramework""", connectionString);
                xml.Save(configFilePath);
                return true;
            }
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
        private static bool UpdateInfrastructureEntitiesConnectionString()
        {
            using (var server = new Microsoft.Web.Administration.ServerManager())
            {
                var siteNameFromServiceModel = "Web";
                var siteName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);

                var configFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\Web.config", server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath);
                var xml = System.Xml.Linq.XElement.Load(configFilePath);

                var connectionString = ConfigReader.GetConfigValue("SqlSampleDataContextConnectionString", false);
                if (string.IsNullOrEmpty(connectionString))
                {
                    return false;
                }

                var attribute = GetConnectionStringAttribute(xml, "InfrastructureEntities");

                attribute.Value = string.Format(@"metadata=res://*/Models.Infrastructure.csdl|res://*/Models.Infrastructure.ssdl|res://*/Models.Infrastructure.msl;provider=System.Data.SqlClient;provider connection string=""{0};App=EntityFramework""", connectionString);
                xml.Save(configFilePath);
                return true;
            }
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
        private static bool UpdateWifSettings()
        {
            using (var server = new Microsoft.Web.Administration.ServerManager())
            {
                var siteNameFromServiceModel = "Web";
                var siteName = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", RoleEnvironment.CurrentRoleInstance.Id, siteNameFromServiceModel);

                var configFilePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, @"{0}\Web.config", server.Sites[siteName].Applications[0].VirtualDirectories[0].PhysicalPath);
                var xml = System.Xml.Linq.XElement.Load(configFilePath);
                var identityModelService = xml.Element("microsoft.identityModel").Element("service");

                if (UpdateAttributeWithRoleSetting(identityModelService.Element("audienceUris").Element("add").Attribute("value"), "realm") &&
                    UpdateAttributeWithRoleSetting(identityModelService.Element("issuerTokenResolver").Element("serviceKeys").Element("add").Attribute("serviceName"), "realm") &&
                    UpdateAttributeWithRoleSetting(identityModelService.Element("issuerTokenResolver").Element("serviceKeys").Element("add").Attribute("serviceKey"), "serviceKey") &&
                    UpdateAttributeWithRoleSetting(identityModelService.Element("issuerNameRegistry").Element("trustedIssuers").Element("add").Attribute("issuerIdentifier"), "trustedIssuersIdentifier") &&
                    UpdateAttributeWithRoleSetting(identityModelService.Element("issuerNameRegistry").Element("trustedIssuers").Element("add").Attribute("name"), "trustedIssuerName"))
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
            if (!string.IsNullOrEmpty(settingValue))
            {
                attribute.Value = settingValue;
            }
            else if (string.IsNullOrEmpty(attribute.Value))
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

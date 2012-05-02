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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public static class ConfigReader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This method gets a configuration setting value.")]
        public static string GetConfigValue(string key, bool validate = true)
        {
            var value = string.Empty;
            if (RoleEnvironment.IsAvailable)
            {
                value = RoleEnvironment.GetConfigurationSettingValue(key);
            }
            else
            {
                value = ConfigurationManager.AppSettings[key];
            }

            if (validate)
            {
                Validate(key, value);
            }

            return value;
        }

        public static string GetWebConfigValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            Validate(key, value);

            return value;
        }

        private static void Validate(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The '{0}' setting is not available.", key), "key");
            }
        }
    }
}
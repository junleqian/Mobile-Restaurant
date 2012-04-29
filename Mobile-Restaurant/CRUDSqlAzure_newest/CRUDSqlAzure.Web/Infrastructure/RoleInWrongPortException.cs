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
    using System.Runtime.Serialization;

    [Serializable]
    public class RoleInWrongPortException : Exception
    {
        public RoleInWrongPortException()
            : base("The Web Role is running in a wrong port. Please review the troubleshooting section of the Windows Azure Toolkit for Windows Phone for guidance in this issue.")
        {
        }

        public RoleInWrongPortException(string message)
            : base(message)
        {
        }

        public RoleInWrongPortException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RoleInWrongPortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
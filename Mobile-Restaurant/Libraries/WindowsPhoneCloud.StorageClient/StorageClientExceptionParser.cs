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

namespace Microsoft.Samples.WindowsPhoneCloud.StorageClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Xml.Linq;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Exceptions;

    public class StorageClientExceptionParser
    {
        private const string DataServicesMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        private static XName lowerCaseDataServiceMessageXName = XName.Get("message", DataServicesMetadataNamespace);
        private static XName upperCaseDataServiceMessageXName = XName.Get("Message", DataServicesMetadataNamespace);
        private static XName dataServiceInnerErrorXName = XName.Get("innererror", DataServicesMetadataNamespace);
        
        private static XName lowerCaseMessageXName = XName.Get("message");
        private static XName upperCaseMessageXName = XName.Get("Message");
        private static XName innerErrorXName = XName.Get("innererror");

        private StorageClientExceptionParser()
        {
        }

        public static Exception ParseDataServiceException(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            try
            {
                var baseException = exception.GetBaseException();
                var doc = XDocument.Parse(baseException.Message);

                return ParseXmlExceptionDocument(doc.Root);
            }
            catch
            {
                return exception.GetBaseException();
            }
        }

        public static Exception ParseXmlWebException(WebException webException)
        {
            if (webException == null)
            {
                return null;
            }

            try
            {
                var stream = webException.Response.GetResponseStream();
                var streamReader = new StreamReader(stream);
                var doc = XDocument.Parse(streamReader.ReadToEnd());

                return ParseXmlExceptionDocument(doc.Root);
            }
            catch
            {
                if (webException.Response is HttpWebResponse)
                {
                    var response = webException.Response as HttpWebResponse;
                    return new HttpWebException(
                        string.IsNullOrWhiteSpace(response.StatusDescription) ? webException.Message : response.StatusDescription,
                        response.StatusCode,
                        webException);
                }
                else
                {
                    return webException.GetBaseException();
                }
            }
        }

        public static Exception ParseStringWebException(WebException webException)
        {
            if (webException == null)
            {
                return null;
            }

            try
            {
                var serializer = new DataContractSerializer(typeof(string));
                var stream = webException.Response.GetResponseStream();
                var errorMessage = serializer.ReadObject(stream) as string;

                return new Exception(errorMessage);
            }
            catch
            {
                if (webException.Response is HttpWebResponse)
                {
                    var response = webException.Response as HttpWebResponse;
                    return new HttpWebException(
                        string.IsNullOrWhiteSpace(response.StatusDescription) ? webException.Message : response.StatusDescription,
                        response.StatusCode,
                        webException);
                }
                else
                {
                    return webException.GetBaseException();
                }
            }
        }

        public static HttpWebException ParseHttpWebException(WebException webException)
        {
            if (webException == null)
            {
                return null;
            }

            var statusCode = default(HttpStatusCode);

            try
            {
                var response = webException.Response as HttpWebResponse;
                statusCode = response.StatusCode;

                var serializer = new DataContractSerializer(typeof(string));
                var stream = response.GetResponseStream();
                var errorMessage = serializer.ReadObject(stream) as string;

                return new HttpWebException(errorMessage, statusCode, webException);
            }
            catch
            {
                return new HttpWebException(webException.Message, statusCode, webException);
            }
        }

        private static Exception ParseXmlExceptionDocument(XElement errorElement)
        {
            switch (errorElement.Name.LocalName)
            {
                case "error":
                case "Error":
                case "innererror":
                    var innerException = errorElement.Element(innerErrorXName) != null
                                        ? ParseXmlExceptionDocument(errorElement.Element(innerErrorXName))
                                        : errorElement.Element(dataServiceInnerErrorXName) != null
                                            ? ParseXmlExceptionDocument(errorElement.Element(dataServiceInnerErrorXName))
                                            : null;
                    
                    var errorMessage = string.Empty;
                    if (errorElement.Element(lowerCaseMessageXName) != null)
                    {
                        errorMessage = errorElement.Element(lowerCaseMessageXName).Value.ToString();
                    }
                    else if (errorElement.Element(upperCaseMessageXName) != null)
                    {
                        errorMessage = errorElement.Element(upperCaseMessageXName).Value.ToString();
                    }
                    else if (errorElement.Element(lowerCaseDataServiceMessageXName) != null)
                    {
                        errorMessage = errorElement.Element(lowerCaseDataServiceMessageXName).Value.ToString();
                    }
                    else if (errorElement.Element(upperCaseDataServiceMessageXName) != null)
                    {
                        errorMessage = errorElement.Element(upperCaseDataServiceMessageXName).Value.ToString();
                    }

                    return new Exception(errorMessage, innerException);

                default:
                    return null;
            }
        }
    }
}

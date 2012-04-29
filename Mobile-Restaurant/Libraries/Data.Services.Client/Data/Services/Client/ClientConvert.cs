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

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

namespace Microsoft.Samples.Data.Services.Client
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;

    #endregion Namespaces.

    internal static class ClientConvert
    {
        private static readonly Type[] knownTypes = CreateKnownPrimitives();

        private static readonly Dictionary<string, Type> namedTypesMap = CreateKnownNamesMap();

        internal enum StorageType
        {
            Boolean,

            Byte,

            ByteArray,

            Char,

            CharArray,

            DateTime,

            DateTimeOffset,

            Decimal,

            Double,

            Guid,

            Int16,

            Int32,

            Int64,

            Single,

            String,

            SByte,

            TimeSpan,

            Type,

            UInt16,

            UInt32,

            UInt64,

            Uri,

            XDocument,

            XElement,
        }

        internal static object ChangeType(string propertyValue, Type propertyType)
        {
            Debug.Assert(null != propertyValue, "should never be passed null");
            try
            {
                switch ((StorageType)IndexOfStorage(propertyType))
                {
                    case StorageType.Boolean:
                        return XmlConvert.ToBoolean(propertyValue);
                    case StorageType.Byte:
                        return XmlConvert.ToByte(propertyValue);
                    case StorageType.ByteArray:
                        return Convert.FromBase64String(propertyValue);
                    case StorageType.Char:
                        return XmlConvert.ToChar(propertyValue);
                    case StorageType.CharArray:
                        return propertyValue.ToCharArray();
                    case StorageType.DateTime:
                        return XmlConvert.ToDateTime(propertyValue, XmlDateTimeSerializationMode.RoundtripKind);
                    case StorageType.DateTimeOffset:
                        return XmlConvert.ToDateTimeOffset(propertyValue);
                    case StorageType.Decimal:
                        return XmlConvert.ToDecimal(propertyValue);
                    case StorageType.Double:
                        return XmlConvert.ToDouble(propertyValue);
                    case StorageType.Guid:
                        return new Guid(propertyValue);
                    case StorageType.Int16:
                        return XmlConvert.ToInt16(propertyValue);
                    case StorageType.Int32:
                        return XmlConvert.ToInt32(propertyValue);
                    case StorageType.Int64:
                        return XmlConvert.ToInt64(propertyValue);
                    case StorageType.Single:
                        return XmlConvert.ToSingle(propertyValue);
                    case StorageType.String:
                        return propertyValue;
                    case StorageType.SByte:
                        return XmlConvert.ToSByte(propertyValue);
                    case StorageType.TimeSpan:
                        return XmlConvert.ToTimeSpan(propertyValue);
                    case StorageType.Type:
                        return Type.GetType(propertyValue, true);
                    case StorageType.UInt16:
                        return XmlConvert.ToUInt16(propertyValue);
                    case StorageType.UInt32:
                        return XmlConvert.ToUInt32(propertyValue);
                    case StorageType.UInt64:
                        return XmlConvert.ToUInt64(propertyValue);
                    case StorageType.Uri:
                        return Util.CreateUri(propertyValue, UriKind.RelativeOrAbsolute);
                    case StorageType.XDocument:
                        return (0 < propertyValue.Length ? System.Xml.Linq.XDocument.Parse(propertyValue) : new System.Xml.Linq.XDocument());
                    case StorageType.XElement:
                        return System.Xml.Linq.XElement.Parse(propertyValue);
                    default:
                        Debug.Assert(false, "new StorageType without update to knownTypes");
                        return propertyValue;
                }
            }
            catch (FormatException ex)
            {
                propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
                throw Error.InvalidOperation(Strings.Deserialize_Current(propertyType.ToString(), propertyValue), ex);
            }
            catch (OverflowException ex)
            {
                propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
                throw Error.InvalidOperation(Strings.Deserialize_Current(propertyType.ToString(), propertyValue), ex);
            }
        }

        internal static bool TryKeyPrimitiveToString(object value, out string result)
        {
            Debug.Assert(value != null, "value != null");

            return Microsoft.Samples.Data.Services.Parsing.WebConvert.TryKeyPrimitiveToString(value, out result);
        }

        internal static bool ToNamedType(string typeName, out Type type)
        {
            type = typeof(string);
            return string.IsNullOrEmpty(typeName) || ClientConvert.namedTypesMap.TryGetValue(typeName, out type);
        }

        internal static string ToTypeName(Type type)
        {
            Debug.Assert(type != null, "type != null");
            foreach (var pair in ClientConvert.namedTypesMap)
            {
                if (pair.Value == type)
                {
                    return pair.Key;
                }
            }

            return type.FullName;
        }

        internal static string ToString(object propertyValue, bool atomDateConstruct)
        {
            Debug.Assert(null != propertyValue, "null should be handled by caller");
            switch ((StorageType)IndexOfStorage(propertyValue.GetType()))
            {
                case StorageType.Boolean:
                    return XmlConvert.ToString((bool)propertyValue);
                case StorageType.Byte:
                    return XmlConvert.ToString((byte)propertyValue);
                case StorageType.ByteArray:
                    return Convert.ToBase64String((byte[])propertyValue);
                case StorageType.Char:
                    return XmlConvert.ToString((char)propertyValue);
                case StorageType.CharArray:
                    return new string((char[])propertyValue);
                case StorageType.DateTime:
                    DateTime dt = (DateTime)propertyValue;
                    return XmlConvert.ToString(dt.Kind == DateTimeKind.Unspecified && atomDateConstruct ? new DateTime(dt.Ticks, DateTimeKind.Utc) : dt, XmlDateTimeSerializationMode.RoundtripKind);
                case StorageType.DateTimeOffset:
                    return XmlConvert.ToString((DateTimeOffset)propertyValue);
                case StorageType.Decimal:
                    return XmlConvert.ToString((decimal)propertyValue);
                case StorageType.Double:
                    return XmlConvert.ToString((double)propertyValue);
                case StorageType.Guid:
                    return ((Guid)propertyValue).ToString();
                case StorageType.Int16:
                    return XmlConvert.ToString((Int16)propertyValue);
                case StorageType.Int32:
                    return XmlConvert.ToString((Int32)propertyValue);
                case StorageType.Int64:
                    return XmlConvert.ToString((Int64)propertyValue);
                case StorageType.Single:
                    return XmlConvert.ToString((Single)propertyValue);
                case StorageType.String:
                    return (string)propertyValue;
                case StorageType.SByte:
                    return XmlConvert.ToString((SByte)propertyValue);
                case StorageType.TimeSpan:
                    return XmlConvert.ToString((TimeSpan)propertyValue);
                case StorageType.Type:
                    return ((Type)propertyValue).AssemblyQualifiedName;
                case StorageType.UInt16:
                    return XmlConvert.ToString((ushort)propertyValue);
                case StorageType.UInt32:
                    return XmlConvert.ToString((uint)propertyValue);
                case StorageType.UInt64:
                    return XmlConvert.ToString((ulong)propertyValue);
                case StorageType.Uri:
                    return ((Uri)propertyValue).ToString();
                case StorageType.XDocument:
                    return ((System.Xml.Linq.XDocument)propertyValue).ToString();
                case StorageType.XElement:
                    return ((System.Xml.Linq.XElement)propertyValue).ToString();
                default:
                    Debug.Assert(false, "new StorageType without update to knownTypes");
                    return propertyValue.ToString();
            }
        }

        internal static bool IsKnownType(Type type)
        {
            return (0 <= IndexOfStorage(type));
        }

        internal static bool IsKnownNullableType(Type type)
        {
            return IsKnownType(Nullable.GetUnderlyingType(type) ?? type);
        }

        internal static bool IsSupportedPrimitiveTypeForUri(Type type)
        {
            return Util.ContainsReference(namedTypesMap.Values.ToArray(), type);
        }

        internal static string GetEdmType(Type propertyType)
        {
            switch ((StorageType)IndexOfStorage(propertyType))
            {
                case StorageType.Boolean:
                    return XmlConstants.EdmBooleanTypeName;
                case StorageType.Byte:
                    return XmlConstants.EdmByteTypeName;
                case StorageType.ByteArray:
                    return XmlConstants.EdmBinaryTypeName;
                case StorageType.DateTime:
                    return XmlConstants.EdmDateTimeTypeName;
                case StorageType.Decimal:
                    return XmlConstants.EdmDecimalTypeName;
                case StorageType.Double:
                    return XmlConstants.EdmDoubleTypeName;
                case StorageType.Guid:
                    return XmlConstants.EdmGuidTypeName;
                case StorageType.Int16:
                    return XmlConstants.EdmInt16TypeName;
                case StorageType.Int32:
                    return XmlConstants.EdmInt32TypeName;
                case StorageType.Int64:
                    return XmlConstants.EdmInt64TypeName;
                case StorageType.Single:
                    return XmlConstants.EdmSingleTypeName;
                case StorageType.SByte:
                    return XmlConstants.EdmSByteTypeName;
                case StorageType.DateTimeOffset:
                case StorageType.TimeSpan:
                case StorageType.UInt16:
                case StorageType.UInt32:
                case StorageType.UInt64:
                    throw new NotSupportedException(Strings.ALinq_CantCastToUnsupportedPrimitive(propertyType.Name));
                case StorageType.Char:
                case StorageType.CharArray:
                case StorageType.String:
                case StorageType.Type:
                case StorageType.Uri:
                case StorageType.XDocument:
                case StorageType.XElement:
                    return null;
                default:
                    Debug.Assert(false, "knowntype without reverse mapping");
                    return null;
            }
        }

        private static Type[] CreateKnownPrimitives()
        {
            Type[] types = new Type[1 + (int)StorageType.XElement];
            types[(int)StorageType.Boolean] = typeof(bool);
            types[(int)StorageType.Byte] = typeof(byte);
            types[(int)StorageType.ByteArray] = typeof(byte[]);
            types[(int)StorageType.Char] = typeof(char);
            types[(int)StorageType.CharArray] = typeof(char[]);
            types[(int)StorageType.DateTime] = typeof(DateTime);
            types[(int)StorageType.DateTimeOffset] = typeof(DateTimeOffset);
            types[(int)StorageType.Decimal] = typeof(decimal);
            types[(int)StorageType.Double] = typeof(double);
            types[(int)StorageType.Guid] = typeof(Guid);
            types[(int)StorageType.Int16] = typeof(Int16);
            types[(int)StorageType.Int32] = typeof(Int32);
            types[(int)StorageType.Int64] = typeof(Int64);
            types[(int)StorageType.Single] = typeof(Single);
            types[(int)StorageType.String] = typeof(string);
            types[(int)StorageType.SByte] = typeof(SByte);
            types[(int)StorageType.TimeSpan] = typeof(TimeSpan);
            types[(int)StorageType.Type] = typeof(Type);
            types[(int)StorageType.UInt16] = typeof(ushort);
            types[(int)StorageType.UInt32] = typeof(uint);
            types[(int)StorageType.UInt64] = typeof(ulong);
            types[(int)StorageType.Uri] = typeof(Uri);
            types[(int)StorageType.XDocument] = typeof(System.Xml.Linq.XDocument);
            types[(int)StorageType.XElement] = typeof(System.Xml.Linq.XElement);

            return types;
        }

        private static Dictionary<string, Type> CreateKnownNamesMap()
        {
            Dictionary<string, Type> named = new Dictionary<string, Type>(EqualityComparer<string>.Default);

            named.Add(XmlConstants.EdmStringTypeName, typeof(string));
            named.Add(XmlConstants.EdmBooleanTypeName, typeof(bool));
            named.Add(XmlConstants.EdmByteTypeName, typeof(byte));
            named.Add(XmlConstants.EdmDateTimeTypeName, typeof(DateTime));
            named.Add(XmlConstants.EdmDecimalTypeName, typeof(decimal));
            named.Add(XmlConstants.EdmDoubleTypeName, typeof(double));
            named.Add(XmlConstants.EdmGuidTypeName, typeof(Guid));
            named.Add(XmlConstants.EdmInt16TypeName, typeof(Int16));
            named.Add(XmlConstants.EdmInt32TypeName, typeof(Int32));
            named.Add(XmlConstants.EdmInt64TypeName, typeof(Int64));
            named.Add(XmlConstants.EdmSByteTypeName, typeof(SByte));
            named.Add(XmlConstants.EdmSingleTypeName, typeof(Single));
            named.Add(XmlConstants.EdmBinaryTypeName, typeof(byte[]));
            return named;
        }

        private static int IndexOfStorage(Type type)
        {
            int index = Util.IndexOfReference(ClientConvert.knownTypes, type);

            return index;
        }
    }
}

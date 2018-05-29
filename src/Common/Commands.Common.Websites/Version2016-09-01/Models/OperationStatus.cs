// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.WebSites.Version2016_09_01.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for OperationStatus.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperationStatus
    {
        [EnumMember(Value = "InProgress")]
        InProgress,
        [EnumMember(Value = "Failed")]
        Failed,
        [EnumMember(Value = "Succeeded")]
        Succeeded,
        [EnumMember(Value = "TimedOut")]
        TimedOut,
        [EnumMember(Value = "Created")]
        Created
    }
    internal static class OperationStatusEnumExtension
    {
        internal static string ToSerializedValue(this OperationStatus? value)
        {
            return value == null ? null : ((OperationStatus)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this OperationStatus value)
        {
            switch( value )
            {
                case OperationStatus.InProgress:
                    return "InProgress";
                case OperationStatus.Failed:
                    return "Failed";
                case OperationStatus.Succeeded:
                    return "Succeeded";
                case OperationStatus.TimedOut:
                    return "TimedOut";
                case OperationStatus.Created:
                    return "Created";
            }
            return null;
        }

        internal static OperationStatus? ParseOperationStatus(this string value)
        {
            switch( value )
            {
                case "InProgress":
                    return OperationStatus.InProgress;
                case "Failed":
                    return OperationStatus.Failed;
                case "Succeeded":
                    return OperationStatus.Succeeded;
                case "TimedOut":
                    return OperationStatus.TimedOut;
                case "Created":
                    return OperationStatus.Created;
            }
            return null;
        }
    }
}

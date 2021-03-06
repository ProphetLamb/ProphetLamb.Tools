﻿using Groundbeef.SharedResources;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Groundbeef.Core
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public static class EnumHelper<T> where T : struct, Enum, IConvertible
    {
        private static readonly Type s_type;

        static EnumHelper()
        {
            s_type = typeof(T);
            if (!s_type.IsEnum)
                throw new NotSupportedException(ExceptionResource.TYPE_MUST_BE_ENUM);
        }

        public static T[] GetValues()
        {
            FieldInfo[] fields = s_type.GetFields(BindingFlags.Static | BindingFlags.Public);
            var enumValues = new T[fields.Length];
            for (int i = 0; i < fields.Length; i++)
                enumValues[i] = Enum.Parse<T>(fields[i].Name, false);
            return enumValues;
        }

        public static T Parse(in ReadOnlySpan<char> value) => Parse(value.ToString());

        public static T Parse(in string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException(ExceptionResource.STRING_NULLEMPTY, nameof(value));
            return Enum.Parse<T>(value, true);
        }

        public static IEnumerable<string> GetNames()
        {
            return s_type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name);
        }

        public static IEnumerable<string?> GetDisplayValues()
        {
            return GetNames().Select(obj => GetDisplayValue(Parse(obj)));
        }

        private static string? LookupResource(Type resourceManagerProvider, string resourceKey, CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty(resourceKey))
                throw new ArgumentException(ExceptionResource.STRING_NULLEMPTY, nameof(resourceKey));
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager? resourceManager = staticProperty.GetValue(null, null) as System.Resources.ResourceManager;
                    return resourceManager?.GetString(resourceKey, cultureInfo);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string? GetDisplayValue(T value) => GetDisplayValue(value, CultureInfo.CurrentUICulture);

        public static string? GetDisplayValue(T value, CultureInfo cultureInfo)
        {
            string? name = value.ToString();
            if (name is null) return null;
            FieldInfo? fieldInfo = value.GetType().GetField(name);
            if (fieldInfo is null) return null;
            var descriptionAttributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false);
            if (descriptionAttributes[0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name, cultureInfo);
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }

    public static class EnumExtention
    {
        /// <summary>
        /// Indicates whether a enum <paramref name="value"/> has any <paramref name="flags"/>.
        /// </summary>
        /// <param name="value">The value of the enum.</param>
        /// <param name="flags">The enum flags to test for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyFlag<T>(this T value, T flags) where T : struct, Enum, IConvertible
        {
            return (value.ToUInt64(null) & flags.ToUInt64(null)) != 0;
        }

        /// <summary>
        /// Indicates whether a enum <paramref name="value"/> has all <paramref name="flags"/>.
        /// </summary>
        /// <param name="value">The value of the enum.</param>
        /// <param name="flags">The enum flags to test for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAllFlags<T>(this T value, T flags) where T : struct, Enum, IConvertible
        {
            return (value.ToUInt64(null) & flags.ToUInt64(null)) == flags.ToUInt64(null);
        }
    }
}

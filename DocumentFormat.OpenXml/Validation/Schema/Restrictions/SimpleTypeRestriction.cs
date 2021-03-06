﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace DocumentFormat.OpenXml.Validation.Schema.Restrictions
{
    /// <summary>
    /// Base class for simple type constraint.
    /// </summary>
    [DebuggerDisplay("RestrictionField={RestrictionField}")]
    [DataContract]
    internal abstract class SimpleTypeRestriction
    {
        /// <summary>
        /// Gets or sets the FileFormat version of this restriction.
        /// </summary>
        internal FileFormatVersions FileFormat { get; set; }

        /// <summary>
        /// Gets the XsdType type defined in schema.
        /// </summary>
        public abstract XsdType XsdType { get; }

        /// <summary>
        /// Gets the corresponding CLR type name. The name will be used to report error.
        /// </summary>
        public virtual string ClrTypeName => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether this simple type is an enum
        /// </summary>
        public virtual bool IsEnum => false;

        /// <summary>
        /// Gets a value indicating whether this simple type is a list
        /// </summary>
        public virtual bool IsList => false;

        /// <summary>
        /// Gets or sets the pattern
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the maxLength facets.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public virtual int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the minLength facets.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public virtual int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the length facets.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public virtual int Length { get; set; }

        /// <summary>
        /// Gets the restriction value in string in CultureInfo.CurrentUICulture.
        /// </summary>
        /// <param name="restrictionField">The facet to be retrieved.</param>
        /// <returns>The value in string.</returns>
        public virtual string GetRestrictionValue(RestrictionField restrictionField)
        {
            switch (restrictionField)
            {
                case RestrictionField.Pattern:
                    return this.Pattern;

                case RestrictionField.Length:
                    return this.Length.ToString(CultureInfo.CurrentUICulture);

                case RestrictionField.MinLength:
                    return this.MinLength.ToString(CultureInfo.CurrentUICulture);

                case RestrictionField.MaxLength:
                    return this.MaxLength.ToString(CultureInfo.CurrentUICulture);

                default:
                    Debug.Assert(false);
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the effective constraint facets used in this instance.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public RestrictionField RestrictionField { get; set; }

        /// <summary>
        /// Validating the specified value is valid according the XsdType.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns>False if the specified value is not valid.</returns>
        public virtual bool ValidateValueType(OpenXmlSimpleType attributeValue)
        {
            return attributeValue.HasValue;
        }

        /// <summary>
        /// Validate the value to all the constraints.
        /// </summary>
        /// <param name="attributeValue">The value to be validated.</param>
        /// <returns>An bit flag, a bit is set on if the corresponding constraint is failed.</returns>
        public RestrictionField Validate(OpenXmlSimpleType attributeValue)
        {
            RestrictionField resultFlag = RestrictionField.None;

            if ((this.RestrictionField & RestrictionField.Pattern) == RestrictionField.Pattern)
            {
                if (!this.IsPatternValid(attributeValue))
                {
                    resultFlag |= RestrictionField.Pattern;
                }
            }

            if ((this.RestrictionField & RestrictionField.Length) == RestrictionField.Length)
            {
                if (!this.IsLengthValid(attributeValue))
                {
                    resultFlag |= RestrictionField.Length;
                }
            }

            if ((this.RestrictionField & RestrictionField.MinLength) == RestrictionField.MinLength)
            {
                if (!this.IsMinLengthValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MinLength;
                }
            }

            if ((this.RestrictionField & RestrictionField.MaxLength) == RestrictionField.MaxLength)
            {
                if (!this.IsMaxLengthValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MaxLength;
                }
            }

            if ((this.RestrictionField & RestrictionField.MinInclusive) == RestrictionField.MinInclusive)
            {
                if (!this.IsMinInclusiveValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MinInclusive;
                }
            }

            if ((this.RestrictionField & RestrictionField.MinExclusive) == RestrictionField.MinExclusive)
            {
                if (!this.IsMinExclusiveValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MinExclusive;
                }
            }

            if ((this.RestrictionField & RestrictionField.MaxInclusive) == RestrictionField.MaxInclusive)
            {
                if (!this.IsMaxInclusiveValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MaxInclusive;
                }
            }

            if ((this.RestrictionField & RestrictionField.MaxExclusive) == RestrictionField.MaxExclusive)
            {
                if (!this.IsMaxExclusiveValid(attributeValue))
                {
                    resultFlag |= RestrictionField.MaxExclusive;
                }
            }

            return resultFlag;
        }

        /// <summary>
        /// Test whether the attribute value is valid according the patten constraint.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public virtual bool IsPatternValid(OpenXmlSimpleType attributeValue) => true;

        /// <summary>
        /// Validate whether the "length" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns>True if the length of the value is same as defined.</returns>
        /// <remarks>
        /// A value in a ·value space· is facet-valid with respect to ·length·, determined as follows:
        /// 1 if the {variety} is ·atomic· then
        ///   1.1 if {primitive type definition} is string or anyURI, then the length of the value, as measured in characters ·must· be equal to {value};
        ///   1.2 if {primitive type definition} is hexBinary or base64Binary, then the length of the value, as measured in octets of the binary data, ·must· be equal to {value};
        ///   1.3 if {primitive type definition} is QName or NOTATION, then any {value} is facet-valid.
        /// 2 if the {variety} is ·list·, then the length of the value, as measured in list items, ·must· be equal to {value}
        /// </remarks>
        public virtual bool IsLengthValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "length" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns>True if the length of the value is same as defined.</returns>
        /// <remarks>
        /// A value in a ·value space· is facet-valid with respect to ·length·, determined as follows:
        /// 1 if the {variety} is ·atomic· then
        ///   1.1 if {primitive type definition} is string or anyURI, then the length of the value, as measured in characters ·must· be equal to {value};
        ///   1.2 if {primitive type definition} is hexBinary or base64Binary, then the length of the value, as measured in octets of the binary data, ·must· be equal to {value};
        ///   1.3 if {primitive type definition} is QName or NOTATION, then any {value} is facet-valid.
        /// 2 if the {variety} is ·list·, then the length of the value, as measured in list items, ·must· be equal to {value}
        /// </remarks>
        public virtual bool IsMinLengthValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "length" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns>True if the length of the value is same as defined.</returns>
        /// <remarks>
        /// A value in a ·value space· is facet-valid with respect to ·length·, determined as follows:
        /// 1 if the {variety} is ·atomic· then
        ///   1.1 if {primitive type definition} is string or anyURI, then the length of the value, as measured in characters ·must· be equal to {value};
        ///   1.2 if {primitive type definition} is hexBinary or base64Binary, then the length of the value, as measured in octets of the binary data, ·must· be equal to {value};
        ///   1.3 if {primitive type definition} is QName or NOTATION, then any {value} is facet-valid.
        /// 2 if the {variety} is ·list·, then the length of the value, as measured in list items, ·must· be equal to {value}
        /// </remarks>
        public virtual bool IsMaxLengthValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "minInclusive" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public virtual bool IsMinInclusiveValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "minExclusive" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public virtual bool IsMinExclusiveValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "maxInclusive" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public virtual bool IsMaxInclusiveValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate whether the "maxExclusive" constraint is ok.
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public virtual bool IsMaxExclusiveValid(OpenXmlSimpleType attributeValue)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        public virtual void Verify()
        {
        }
#endif
    }
}

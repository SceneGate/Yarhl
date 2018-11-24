//  ConverterMetadata.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2018 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.FileFormat
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Metadata associated to a IConverter interface.
    /// </summary>
    public class ConverterMetadata : IExportMetadata
    {
        /// <summary>
        /// Gets or sets the full name of the type. Shortcut of Type.FullName.
        /// </summary>
        /// <value>The full name of the type.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of class implemeting the converter.
        /// </summary>
        /// <value>Type of the converter.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets a single type or list of types that the converter
        /// can convert from.
        /// </summary>
        /// <value>Single or list of types for conversion.</value>
        public object Sources { get; set; }

        /// <summary>
        /// Gets or sets a single type or list of types the converter can
        /// convert to.
        /// </summary>
        /// <value>Single or list of types the converter can convert to.</value>
        public object Destinations { get; set; }

        /// <summary>
        /// Gets a list of source types that can convert from.
        /// </summary>
        /// <returns>List of source types that can convert from.</returns>
        public Type[] GetSources()
        {
            if (!(Sources is Type[] sourceList)) {
                sourceList = new[] { (Type)Sources };
            }

            return sourceList;
        }

        /// <summary>
        /// Gets a list of destination types it can convert to.
        /// </summary>
        /// <returns>Destination types it can convert to.</returns>
        public Type[] GetDestinations()
        {
            if (!(Destinations is Type[] destList)) {
                destList = new[] { (Type)Destinations };
            }

            return destList;
        }

        /// <summary>
        /// Check if the associated converter can convert from a given type.
        /// It checks applying covariance rules.
        /// </summary>
        /// <param name="source">Source type for conversion.</param>
        /// <returns>If this converter can realize the operation.</returns>
        public bool CanConvert(Type source)
        {
            Type[] sources = GetSources();
            for (int i = 0; i < sources.Length; i++) {
                if (sources[i].IsAssignableFrom(source)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the associated converter can convert from a given type
        /// into another. It checks applying covariance and contravariance
        /// rules.
        /// </summary>
        /// <param name="source">Source type for conversion.</param>
        /// <param name="dest">Destination type for conversion.</param>
        /// <returns>If this converter can realize the operation.</returns>
        public bool CanConvert(Type source, Type dest)
        {
            Type[] sources = GetSources();
            Type[] dests = GetDestinations();

            for (int i = 0; i < sources.Length; i++) {
                bool matchSource = sources[i].IsAssignableFrom(source);
                bool matchDest = dest.IsAssignableFrom(dests[i]);
                if (matchSource && matchDest) {
                    return true;
                }
            }

            return false;
        }
    }
}

// IInitializer.cs
//
// Copyright (c) 2019 SceneGate Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.FileFormat
{
    /// <summary>
    /// Initialization interface.
    /// </summary>
    /// <typeparam name="T">Type of the parameters for the initialize.</typeparam>
    public interface IInitializer<T>
    {
        /// <summary>
        /// Initialize the instance with the specified parameters.
        /// </summary>
        /// <param name="parameters">Parameters for the initialize.</param>
        void Initialize(T parameters);
    }
}

// -----------------------------------------------------------------------
// <copyright file="FormatValidation.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleonex</author>
// <email>benito356@gmail.com</email>
// <date>18/09/2013</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.Addins;

namespace Libgame
{
	[TypeExtensionPoint]
	public abstract class FormatValidation
	{
		protected enum ValidationResult
	    {
			Invalid  = 0,
			No       = 0,
			CouldBe  = 40,
			ShouldBe = 70,
			Sure     = 100,
		}

		protected List<string> dependencies = new List<string>();

		public FormatValidation()
		{
			this.AutosetFormat = false;
		}

		/// <summary>
		/// File format that this instance can validate.
		/// </summary>
		/// <value>The type of the format.</value>
		public abstract Type FormatType {
			get;
		}
		
		public ReadOnlyCollection<String> Dependencies {
			get { return new ReadOnlyCollection<String>(this.dependencies); }
		}

		public bool Result {
			get;
			private set;
		}

		public bool AutosetFormat {
			get;
			set;
		}

		public void RunTests(GameFile file)
		{
			if (file.Format != null)
				throw new Exception("The file already has a format.");

			double result = 0;

			result += (int)this.TestByTags(file.Tags) * 0.75;
			result += (int)this.TestByData(file.Stream) * 0.50;
			result += (int)this.TestByRegexp(file.Path, file.Name) * 0.25;

			this.Result = (result >= 50) ? true : false;

			if (this.Result) {
				this.GuessDependencies(file);

				if (this.AutosetFormat) {
					file.SetFormat(this.FormatType);
					file.Format.IsGuessed = true;
					file.Format.File = file;
				}
			}
		}

		protected abstract ValidationResult TestByTags(IDictionary<string, string> tags);
		protected abstract ValidationResult TestByData(DataStream stream);
		protected abstract ValidationResult TestByRegexp(string filepath, string filename);

		protected abstract void GuessDependencies(GameFile file);
	}
}


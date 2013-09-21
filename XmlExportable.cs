//-----------------------------------------------------------------------
// <copyright file="XmlExportable.cs" company="none">
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
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>11/06/2013</date>
//-----------------------------------------------------------------------
namespace Libgame
{
    using System;
    using System.Xml.Linq;
	using Mono.Addins;
	using Libgame.IO;
	
    /// <summary>
    /// Description of XmlExportable.
    /// </summary>
    public abstract class XmlExportable : Format
    {              
		public override void Import(params DataStream[] strIn)
        {
			XDocument doc = XDocument.Load(strIn[0].BaseStream);
            
            if (doc.Root.Name.LocalName != this.FormatName)
                throw new FormatException();
            
            this.Import(doc.Root);
        }
        
		public override void Export(params DataStream[] strOut)
		{
			XDocument doc = new XDocument();
			doc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

			XElement root = new XElement(this.FormatName);
			this.Export(root);
			doc.Add(root);

			doc.Save(strOut[0].BaseStream, SaveOptions.None);
		}

        protected abstract void Import(XElement root);
        
        protected abstract void Export(XElement root);
    }
}

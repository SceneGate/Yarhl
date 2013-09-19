//-----------------------------------------------------------------------
// <copyright file="IFormat.cs" company="none">
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
    
    /// <summary>
    /// Description of IFormat.
    /// </summary>
    public abstract class Format
    {
		private bool isRead = false;

		protected Format(GameFile file)
		{
			this.File = file;
		}

        public abstract string FormatName {
            get;
        }
        
		protected GameFile File {
			get;
			private set;
		}

		public bool IsGuessed {
			set;
			get;
		}

		public void Read()
		{
			if (this.isRead)
				return;

			this.Read(this.File.Stream);
			this.isRead = true;
		}

		public void Write()
		{
			DataStream newStream = new DataStream(new System.IO.MemoryStream(), 0, -1);
			this.File.ChangeStream(newStream);
			this.Write(newStream);
		}

		public void Import(string filePath)
		{
			DataStream stream = new DataStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			this.Import(stream);
			stream.Dispose();
		}

        protected abstract void Read(DataStream strIn);
        
        public abstract void Write(DataStream strOut);
        
        public abstract void Import(DataStream strIn);
        
        public abstract void Export(DataStream strOut);
        
        public abstract bool Disposable();
    }
}

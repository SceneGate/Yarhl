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
	using Mono.Addins;
	using Libgame.IO;
    
	[TypeExtensionPoint]
    /// <summary>
    /// Description of IFormat.
    /// </summary>
    public abstract class Format
    {
		private bool isRead = false;
		private bool isInitialized = false;
		protected Object[] parameters;

        public abstract string FormatName {
            get;
        }
        
		public GameFile File {
			get;
			private set;
		}

		public bool IsGuessed {
			set;
			get;
		}

		public virtual void Initialize(GameFile file, params Object[] parameters)
		{
			this.File = file;
			this.isInitialized = true;
			this.parameters = parameters;
		}

		public void Read()
		{
			if (!this.isInitialized)
				throw new Exception("The format has not been initialized.");

			if (this.isRead)
				return;

			this.Read(this.File.Stream);
			this.isRead = true;
		}

		public void Write()
		{
			if (!this.isInitialized)
				throw new Exception("The format has not been initialized.");

			DataStream newStream = new DataStream(new System.IO.MemoryStream(), 0, -1);
			this.File.ChangeStream(newStream);
			this.Write(newStream);
		}

		public void Write(string outfile)
		{
			DataStream outStream = new DataStream(
				outfile,
				System.IO.FileMode.Create,
				System.IO.FileAccess.Write);
			this.Write(outStream);
			outStream.Flush();
			outStream.Dispose();
		}

		public void Import(params string[] filesPath)
		{
			DataStream[] streams = new DataStream[filesPath.Length];
			for (int i = 0; i < filesPath.Length; i++)
				streams[i] = new DataStream(filesPath[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);

			this.Import(streams);

			foreach (DataStream stream in streams)
				stream.Dispose();
		}

		public void Export(params string[] filesPath)
		{
			DataStream[] streams = new DataStream[filesPath.Length];
			for (int i = 0; i < filesPath.Length; i++)
				streams[i] = new DataStream(filesPath[i], System.IO.FileMode.Create, System.IO.FileAccess.Write);

			this.Export(streams);

			foreach (DataStream stream in streams) {
				stream.Flush();
				stream.Dispose();
			}
		}

		public abstract void Read(DataStream strIn);
        
        public abstract void Write(DataStream strOut);
        
		public abstract void Import(params DataStream[] strIn);
        
		public abstract void Export(params DataStream[] strOut);
    }
}

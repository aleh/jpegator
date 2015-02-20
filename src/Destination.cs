// JPEGator .NET Compact Framework Library.
// Copyright (C) 2005-2009, Aleh Dzenisiuk. All Rights Reserved.
// http://dzenisiuk.info/jpegator/
//
// When redistributing JPEGator source code the above copyright notice and 
// this messages should be left intact. In case changes are made by you in 
// this source code it should be clearly indicated in the beginning of each 
// changed file.

using System;
using System.Runtime.InteropServices;
using System.IO;

namespace JPEGator
{
	internal sealed class Destination : NativeStruct
	{
		public const int BufferSize = 4096;
		
		private const int StructSize = 5 * 4;
		
		private NativeStruct dataBuffer;
		
		private NativeCallback initDestination;
		private NativeCallback emptyOutputBuffer;
		private NativeCallback termDestination;
		
		private Stream stream;
		private bool autoClose;		
		
		public Destination()
			: base(StructSize)
		{		
			dataBuffer = new NativeStruct(BufferSize);
			initDestination = new NativeCallback(1, new NativeCallbackHandler(InitDestination));
			emptyOutputBuffer = new NativeCallback(1, new NativeCallbackHandler(EmptyOutputBuffer));
			termDestination = new NativeCallback(1, new NativeCallbackHandler(TermDestination));
			int[] buf = new int[] {
				dataBuffer,
				dataBuffer.Size,
				initDestination,
				emptyOutputBuffer,
				termDestination
			};
			Marshal.Copy(buf, 0, Ptr, buf.Length);
		}

		private bool disposed;
		
		protected override void Dispose(bool disposing)
		{	
			try
			{
				if (!disposed)
				{
					if (disposing)
					{
						if (stream != null)
						{
							stream.Flush();
							if (autoClose)
								stream.Close();
						}
					
						dataBuffer.Dispose();
						initDestination.Dispose();
						emptyOutputBuffer.Dispose();
						termDestination.Dispose();
					}
					disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		public void SetStream(Stream s, bool autoClose)
		{
			if (stream != null)
			{
				stream.Flush();
				if (autoClose)
					stream.Close();
			}
		
			this.stream = s;
			this.autoClose = autoClose;
		}
		
		private IntPtr NextOutputByte
		{
			get { return (IntPtr)Marshal.ReadInt32(Ptr); }
			set { Marshal.WriteInt32(Ptr, (int)value); }
		}
		
		private int FreeInBuffer
		{
			get { return Marshal.ReadInt32(Ptr, 4); }
			set { Marshal.WriteInt32(Ptr, 4, value); }
		}
				
		// TODO: Eat or handle managed exceptions
		private int InitDestination(int[] paramList)
		{
			NextOutputByte = dataBuffer.Ptr;
			FreeInBuffer = dataBuffer.Size;			
			return 0;
		}
		
		// TODO: Eat or handle managed exceptions
		private int EmptyOutputBuffer(int[] paramList)
		{
			try
			{
				byte[] buf = new byte[dataBuffer.Size];
				Marshal.Copy(dataBuffer.Ptr, buf, 0, buf.Length);
				stream.Write(buf, 0, buf.Length);
				
				NextOutputByte = dataBuffer.Ptr;
				FreeInBuffer = dataBuffer.Size;			
				
				return 1;
			}			
			catch
			{
				return 0;
			}			
		}
		
		// TODO: Eat or handle managed exceptions
		private int TermDestination(int[] paramList)
		{
			byte[] buf = new byte[dataBuffer.Size - FreeInBuffer];
			Marshal.Copy(dataBuffer.Ptr, buf, 0, buf.Length);
			stream.Write(buf, 0, buf.Length);
			stream.Flush();
			if (autoClose)
				stream.Close();
			stream = null;
			
			return 0;
		}
	}
}
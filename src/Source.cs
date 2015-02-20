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
using System.Diagnostics;

namespace JPEGator
{
	/// <summary>
	/// Wrapper over the data source for JPEG decompressor.
	/// </summary>
	internal sealed class Source : NativeStruct
	{
		private const int StructureSize = 7 * 4;		
		private const int BufferSize = 65536;
		
		private NativeStruct buffer;
		
		private NativeCallback initSource;
		private NativeCallback fillInputBuffer;
		private NativeCallback skipInputData;
		private NativeCallback resyncToRestart;
		private NativeCallback termSource;
				
		public Source()
			: base(StructureSize)
		{
			initSource = new NativeCallback(1, new NativeCallbackHandler(InitSource));
			fillInputBuffer = new NativeCallback(1, new NativeCallbackHandler(FillInputBuffer));
			skipInputData = new NativeCallback(2, new NativeCallbackHandler(SkipInputData));
			resyncToRestart = new NativeCallback(2, new NativeCallbackHandler(ResyncToRestart));
			termSource = new NativeCallback(1, new NativeCallbackHandler(TermSource));
			
			int[] buf = new int[] {
				0,
				0,
				initSource,
				fillInputBuffer,
				skipInputData,
				resyncToRestart,
				termSource
			};
			
			Marshal.Copy(buf, 0, Ptr, buf.Length);
		}
		
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					initSource.Dispose();
					fillInputBuffer.Dispose();
					skipInputData.Dispose();
					resyncToRestart.Dispose();
					termSource.Dispose();
					if (buffer != null)
						buffer.Dispose();
					if (autoClose && stream != null)
					{						
						stream.Close();
						stream = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		private Stream stream;
		private bool autoClose;

		public void SetStream(Stream s, bool autoClose)
		{
			this.stream = s;
			this.autoClose = autoClose;
		}		
		
		private int InitSource(int[] paramList)
		{
			buffer = new NativeStruct(BufferSize);
		
			Marshal.WriteInt32(Ptr, 0, (int)buffer.Ptr);
			Marshal.WriteInt32(Ptr, 4, 0);
			
			return 0;
		}
		
		private int FillInputBuffer(int[] paramList)
		{
			byte[] buf = new byte[buffer.Size];
			int bytesRead = stream.Read(buf, 0, buffer.Size);
			Marshal.Copy(buf, 0, buffer.Ptr, bytesRead);
			Marshal.WriteInt32(Ptr, 0, (int)buffer.Ptr);
			Marshal.WriteInt32(Ptr, 4, bytesRead);
			return 1;
		}
		
		private int SkipInputData(int[] paramList)
		{
			Debug.WriteLine(string.Format("Skipping {0} bytes", paramList[1]));
			
			int numBytes = paramList[1];
			if (numBytes <= 0)
				return 0;
				
			int bytesInBuffer = Marshal.ReadInt32(Ptr, 4);
			if (numBytes <= bytesInBuffer)
			{				
				int dataPtr = Marshal.ReadInt32(Ptr);
				Marshal.WriteInt32(Ptr, 0, dataPtr + numBytes);
				Marshal.WriteInt32(Ptr, 4, bytesInBuffer - numBytes);
			}
			else
			{
				int moveTo = numBytes - bytesInBuffer;
				if (stream.CanSeek)
					stream.Seek(moveTo, SeekOrigin.Current);
				else
				{
					byte[] buf = new byte[BufferSize];					
					int bytesRead = 0;
					while (bytesRead < moveTo)
					{
						int readNow = stream.Read(buf, 0, Math.Min(moveTo - bytesRead, buf.Length));
						if (readNow == 0)
							break;
						bytesRead += readNow;
					}
				}
				FillInputBuffer(paramList);
			}
			return 0;
		}
		
		private int ResyncToRestart(int[] paramList)
		{
			return JpegDll.ResyncToRestart((IntPtr)paramList[0], paramList[1]) ? 1 : 0;
		}
		
		private int TermSource(int[] paramList)
		{
			buffer.Dispose();
			buffer = null;
			
			if (autoClose)
				stream.Close();
			stream = null;
			
			return 0;
		}
	}
}

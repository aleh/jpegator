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
using Microsoft.WindowsCE.Forms;
using System.Threading;
using System.Collections;

namespace JPEGator
{
	/// <summary>
	/// Represents a pointer to some DLL entry point.
	/// </summary>
	internal sealed class DllEntryCallback : IDisposable
	{
		private IntPtr dllHandle;

		private IntPtr procPtr;

		public DllEntryCallback(string dllName, string procName)
		{
			dllHandle = LoadLibrary(dllName);
			if (dllHandle == IntPtr.Zero)
				throw new TypeLoadException(dllName);

			procPtr = GetProcAddress(dllHandle, procName);
			if (procPtr == IntPtr.Zero)
			{
				FreeLibrary(dllHandle);
				dllHandle = IntPtr.Zero;
				throw new MissingMethodException(procName);
			}
		}

		~DllEntryCallback()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (dllHandle != IntPtr.Zero)
			{
				FreeLibrary(dllHandle);
				dllHandle = IntPtr.Zero;
			}
		}

		public IntPtr Ptr
		{
			get { return procPtr; }
		}

		public static implicit operator IntPtr(DllEntryCallback c)
		{
			return c.Ptr;
		}

		public static implicit operator int(DllEntryCallback c)
		{
			return (int)c.Ptr;
		}

		[DllImport("coredll.dll")]
		private static extern IntPtr LoadLibrary(string fileName);

		[DllImport("coredll.dll")]
		private static extern bool FreeLibrary(IntPtr handle);

		[DllImport("coredll.dll")]
		private static extern IntPtr GetProcAddress(IntPtr handle, string procName);
	}
}

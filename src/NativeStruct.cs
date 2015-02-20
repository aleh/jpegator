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

namespace JPEGator
{
	/// <summary>
	/// Represents some unmanaged structure, i.e. block of unmanaged memory of some size.
	/// Used to wrap original structures of <a href="http://www.ijg.org/">the Independent JPEG Group's software</a>.
	/// </summary>
	public class NativeStruct : IDisposable
	{
		private IntPtr ptr;
		private int size;

		/// <summary>
		/// Implicitly obtains address of memory block this object is referenced to.
		/// </summary>
		public static implicit operator IntPtr(NativeStruct b)
		{
			return b.Ptr;
		}

		/// <summary>
		/// Implicitly obtains address of memory block this object is referenced to.
		/// </summary>
		public static implicit operator int(NativeStruct b)
		{
			return (int)b.Ptr;
		}

		/// <summary>
		/// Allocates <paramref name="size"/> bytes of unmanaged memory
		/// and stores pointer to them.
		/// </summary>
		public NativeStruct(int size)
		{
			this.size = size;
			this.ptr = LocalAlloc(Lptr, size);
			if (this.ptr == IntPtr.Zero)
				throw new OutOfMemoryException();
		}

		~NativeStruct()
		{
			Dispose(false);
		}

		/// <summary>
		/// Frees allocated unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (ptr != IntPtr.Zero)
			{
				LocalFree(ptr);
				ptr = IntPtr.Zero;
			}
		}

		/// <summary>
		/// A pointer to an unmanaged structure on the heap.
		/// </summary>
		public IntPtr Ptr
		{
			get { return ptr; }
		}

		/// <summary>
		/// A size of an unmanaged structure in bytes.
		/// </summary>
		public int Size
		{
			get { return size; }
		}

		private const int Lptr = 0x0040;

		[DllImport("coredll.dll")]
		private static extern IntPtr LocalAlloc(int flags, int size);

		[DllImport("coredll.dll")]
		private static extern IntPtr LocalFree(IntPtr ptr);
	}
}

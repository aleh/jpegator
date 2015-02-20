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
	internal delegate int NativeCallbackHandler(int[] paramList);

	/// <summary>
	/// Callback object that can be used as a parameter for calls of native functions.
	/// </summary>
	internal class NativeCallback : IDisposable
	{
		private const int CallbackMessage = 0x82c4;

		private sealed class CallbackSink : MessageWindow
		{
			private int lastCookie;
			private Hashtable handlers = new Hashtable();

			private sealed class MethodInfo
			{
				public NativeCallbackHandler Handler;
				public int ParamCount;

				public MethodInfo(NativeCallbackHandler handler, int paramCount)
				{
					this.Handler = handler;
					this.ParamCount = paramCount;
				}
			}

			public int Add(NativeCallbackHandler handler, int paramCount)
			{
				int cookie = ++lastCookie;
				handlers.Add(cookie, new MethodInfo(handler, paramCount));
				return cookie;
			}

			public void Remove(int cookie)
			{
				handlers.Remove(cookie);
			}

			public CallbackSink()
			{
			}

			// TODO: Eat or handle managed exceptions
			protected override void WndProc(ref Message msg)
			{
				if (msg.Msg == CallbackMessage)
				{
					MethodInfo info = (MethodInfo)handlers[(int)msg.WParam];
					if (info != null)
					{
						int[] paramList = new int[info.ParamCount];
						Marshal.Copy((IntPtr)msg.LParam, paramList, 0, paramList.Length);
												
						msg.Result = (IntPtr)info.Handler(paramList);
					}
					else
						System.Diagnostics.Debug.WriteLine("Unknown method was called", GetType().FullName);
				}
				else
					base.WndProc(ref msg);
			}
		}

		private static IntPtr dllHandle;

		private static IntPtr sendMessagePtr;

		private static LocalDataStoreSlot slot;

		static NativeCallback()
		{
			dllHandle = LoadLibrary("coredll.dll");
			if (dllHandle == IntPtr.Zero)
				throw new TypeLoadException();

			sendMessagePtr = GetProcAddress(dllHandle, "SendMessageW");
			if (sendMessagePtr == IntPtr.Zero)
			{
				FreeLibrary(dllHandle);
				dllHandle = IntPtr.Zero;
				throw new TypeLoadException();
			}

			slot = Thread.GetNamedDataSlot(typeof(NativeCallback).FullName);
		}

		private CallbackSink sink;

		private int cookie;

		private IntPtr thunkPtr;

		public NativeCallback(int paramCount, NativeCallbackHandler handler)
		{
			sink = (CallbackSink)Thread.GetData(slot);
			if (sink == null)
			{
				sink = new CallbackSink();
				Thread.SetData(slot, sink);
			}
			cookie = sink.Add(handler, paramCount);

			int[] thunkData;
			unchecked
			{
				thunkData = new int[]
				{
					(int)0xe1a0c00d, //	 mov       r12, sp
					(int)0xe92d000f, //	 stmdb     sp!, {r0 - r3}
					(int)0xe92d5010, //	 stmdb     sp!, {r4, r12, lr}
					(int)0xe59f4024, //	 ldr       r4, [pc, #0x24]
					(int)0xe59f201c, //	 ldr       r2, [pc, #0x1c]
					(int)0xe28d300c, //	 add       r3, sp, #0xC
					(int)0xe59f1010, //	 ldr       r1, [pc, #0x10]
					(int)0xe59f0008, //	 ldr       r0, [pc, #8]
					(int)0xe1a0e00f, //	 mov       lr, pc
					(int)0xe1a0f004, //	 mov       pc, r4
					(int)0xe89da010, //	 ldmia     sp, {r4, sp, pc}
					(int)sink.Hwnd,
					CallbackMessage,
					cookie,
					(int)sendMessagePtr
				};
			}

			// TODO: Using of VirtualAlloc is vasting much virtual memory, so some other allocation method should be used
			// Unfortunately heaps do not work for this, I guess because of default memory protection flags heaps routines
			// use (there is no execute flag there).

			thunkPtr = VirtualAlloc(
				IntPtr.Zero,
				thunkData.Length * 4,
				MemCommit,
				PageExecuteReadWrite
			);
			if (thunkPtr == IntPtr.Zero)
				throw new OutOfMemoryException();

			Marshal.Copy(thunkData, 0, thunkPtr, thunkData.Length);
		}

		~NativeCallback()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				sink.Remove(cookie);
			if (thunkPtr != IntPtr.Zero)
			{
				VirtualFree(thunkPtr, 0, MemRelease);
				thunkPtr = IntPtr.Zero;
			}
		}

		public IntPtr Ptr
		{
			get { return thunkPtr; }
		}

		public static NativeCallback CreateStub()
		{
			return new NativeCallback(0, new NativeCallbackHandler(Stub));
		}

		private static int Stub(int[] paramList)
		{
			return 0;
		}

		public static implicit operator IntPtr(NativeCallback c)
		{
			return c.Ptr;
		}

		public static implicit operator int(NativeCallback c)
		{
			return (int)c.Ptr;
		}

		private const int PageExecuteReadWrite = 0x40;

		private const int MemRelease = 0x8000;
		private const int MemCommit = 0x1000;

		[DllImport("coredll.dll")]
		private static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect);

		[DllImport("coredll.dll")]
		private static extern bool VirtualFree(IntPtr lpAddress, int dwSize, int dwFreeType);

		[DllImport("coredll.dll")]
		private static extern IntPtr LoadLibrary(string fileName);

		[DllImport("coredll.dll")]
		private static extern bool FreeLibrary(IntPtr handle);

		[DllImport("coredll.dll")]
		private static extern IntPtr GetProcAddress(IntPtr handle, string procName);
	}
}

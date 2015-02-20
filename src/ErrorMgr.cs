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
	internal sealed class ErrorMgr : NativeStruct
	{
		private const int JmpbufSize = 11 * 4;
		private const int ErrorFlagSize = 4;
		internal const int StructSize = 132 + JmpbufSize + ErrorFlagSize;

		private DllEntryCallback exitError;

		private NativeCallback emitMessage;

		private NativeCallback outputMessage;

		private NativeCallback formatMessage;

		private NativeCallback resetErrorMgr;

		public ErrorMgr()
			: base(StructSize)
		{
			exitError = new DllEntryCallback(JpegDll.DllName, "jpegator_error_exit");
			emitMessage = new NativeCallback(2, new NativeCallbackHandler(EmitMessage));
			outputMessage = NativeCallback.CreateStub();
			formatMessage = NativeCallback.CreateStub();
			resetErrorMgr = new NativeCallback(2, new NativeCallbackHandler(ResetErrorMgr));

			int[] buf = new int[Size / 4];
			buf[0] = exitError;
			buf[1] = emitMessage;
			buf[2] = outputMessage;
			buf[3] = formatMessage;
			buf[4] = resetErrorMgr;
			Marshal.Copy(buf, 0, Ptr, buf.Length);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				exitError.Dispose();
				emitMessage.Dispose();
				outputMessage.Dispose();
				formatMessage.Dispose();
				resetErrorMgr.Dispose();
			}
		}

		public int MessageCode
		{
			get { return Marshal.ReadInt32(Ptr, 5 * 4); }
		}

		public int NumWarnings
		{
			get { return Marshal.ReadInt32(Ptr, 6 * 4 + 80 + 4); }
		}

		public int[] GetMessageParams()
		{
			int[] paramList = new int[8];
			Marshal.Copy((IntPtr)((int)this + 6 * 4), paramList, 0, paramList.Length);
			return paramList;
		}

/*
		public string Message
		{
			get
			{
				IntPtr messageTablePtr = (IntPtr)Marshal.ReadInt32(Ptr, 6 * 4 + 80 + 2 * 4);
				Console.WriteLine(messageTablePtr);
				IntPtr messagePtr = (IntPtr)Marshal.ReadInt32(messageTablePtr, MessageCode);
				Console.WriteLine(messagePtr);
				return Marshal.PtrToStringUni(messagePtr);
			}
		}
*/

		// TODO: Eat or handle managed exceptions
		// TODO: Create appropriate event, treating this call as error message for example
/*
		private int ErrorExit(int[] paramList)
		{
			throw new Exception(
				string.Format(
					"JPEG operation failed. Message code: {0}",
					MessageCode
				)
			);
		}
*/
		// TODO: Eat or handle managed exceptions
		// TODO: Create appropriate event, i.e. 'OnWarning'
		private int EmitMessage(int[] paramList)
		{
			if (paramList[1] < 0)
				System.Diagnostics.Debug.WriteLine(
					string.Format(
						"{0} #{1}",
						paramList[1],
						MessageCode
					),
					GetType().Name
				);
			return 0;
		}

		// TODO: Eat or handle managed exceptions
		private int ResetErrorMgr(int[] paramList)
		{
			Marshal.WriteInt32(Ptr, 5 * 4, 0);
			Marshal.WriteInt32(Ptr, 6 * 4 + 80 + 4, 0);
			return 0;
		}

	}
}

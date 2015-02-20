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
	internal sealed class ProgressMgr : NativeStruct
	{
		private const int StructSize = 5 * 4;
		
		private NativeCallback progressMonitor;
		
		public ProgressMgr()
			: base(StructSize)
		{
			progressMonitor = new NativeCallback(1, new NativeCallbackHandler(ProgressMonitor));
			Marshal.WriteInt32(Ptr, progressMonitor);
		}
		
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
					progressMonitor.Dispose();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		public int Percent
		{
			get 
			{ 
				unsafe
				{
					int* p = (int*)Ptr;
					return (p[4] != 0) ? (p[3] * 100 + p[1] * 100 / p[2]) / p[4] : 0;
				}
			}
		}
		
		public ProgressEventHandler Progress;
		
		private int threshold = 10;
		
		public int Threshold
		{
			get { return threshold; }
			set { threshold = value; }
		}
		
		private int oldPercent;
		
		// TODO: Eat or handle managed exceptions
		private int ProgressMonitor(int[] paramList)
		{
			int percent = Percent;
			if ((percent < oldPercent || percent - oldPercent >= threshold) 
				&& (Progress != null))
			{
				Progress(this, new ProgressEventArgs(percent));
				oldPercent = percent;
			}
			return 0;
		}		
	}
}
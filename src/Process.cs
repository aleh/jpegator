// JPEGator .NET Compact Framework Library.
// Copyright (C) 2005-2009, Aleh Dzenisiuk. All Rights Reserved.
// http://dzenisiuk.info/jpegator/
//
// When redistributing JPEGator source code the above copyright notice and 
// this messages should be left intact. In case changes are made by you in 
// this source code it should be clearly indicated in the beginning of each 
// changed file.

using System;

#if TRIAL
using System.Windows.Forms;
#endif

using System.Runtime.InteropServices;
using System.Diagnostics;

namespace JPEGator
{
	/// <summary>
	/// A base class for <see cref="Compress"/> and <see cref="Decompress"/>,
	/// contains progress indication features common to both these classes.
	/// </summary>
	///
	/// <threadsafety static="true" instance="false"/>
	///
	public class Process : NativeStruct
	{
		internal const int ErrorMgrOffset    = 0 * 4;
		internal const int ProgressMgrOffset = 2 * 4;

		private ErrorMgr errorMgr = new ErrorMgr();

		private ProgressMgr progressMgr = new ProgressMgr();

		private ProgressEventHandler progress;
		
		#if TRIAL
		private static bool nagScreenShown = false;
		#endif
		
		[Conditional("TRIAL")]
		protected static void CheckTrial()
		{
		#if TRIAL
			if (!nagScreenShown && (System.DateTime.Now.Second % 3 == 0))
			{ 
				System.Windows.Forms.MessageBox.Show(
					"Unregistered version of JPEGator library is used in this application.\r\n\r\n" +
					"Please register it at http://dzenisiuk.info/jpegator/",
					"JPEGator Trial Notice"
				);
				nagScreenShown = true;
			}
		#endif
		}
		
		protected Process(int size)
			: base(size)
		{
			Marshal.WriteInt32(this, ErrorMgrOffset, errorMgr);
		}

		/// <summary>
		/// An event which is fired when comression or decompression job progress is changed at least by
		/// <see cref="ProgressThreshold"/> percents.
		/// </summary>
		/// <remarks>
		/// Progress notification can be time consuming, so use it only
		/// for operations that are noticeable lengthy. Also, try to set <see cref="ProgressThreshold"/>
		/// to some reasonable value (it is set to 10% by default).
		/// </remarks>
		public event ProgressEventHandler Progress
		{
			add
			{
				if (progress == null)
				{
					progressMgr.Progress += new ProgressEventHandler(OnProgress);
					Marshal.WriteInt32(this, ProgressMgrOffset, progressMgr);
				}
				progress += value;
			}

			remove
			{
				progress -= value;
				if (progress == null)
				{
					Marshal.WriteInt32(this, ProgressMgrOffset, 0);
					progressMgr.Progress -= new ProgressEventHandler(OnProgress);
				}
			}
		}

		/// <summary>
		/// Specifies how often to invoke the <see cref="Progress"/> event:
		/// percents of finished work should have changed at least by this
		/// value to allow its invocation. Default value is 10%.
		/// </summary>
		public int ProgressThreshold
		{
			get { return progressMgr.Threshold; }
			set { progressMgr.Threshold = value; }
		}

		private void OnProgress(object sender, ProgressEventArgs e)
		{
			if (progress != null)
				progress(this, e);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					errorMgr.Dispose();
					progressMgr.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/* 
		private const int TrialSizeLimit = 1023;

		[Conditional("TRIAL")]
		protected void CheckSize(int width, int height)
		{
			if (width > TrialSizeLimit || height > TrialSizeLimit)
				throw new ApplicationException(
					string.Format(
						"This is the trial version of JPEGator library, which has an artificial limitation on image size: both height and width of processed image cannot be less than {0} pixels, while image you are trying to work with is {1}x{2} pixels in size\r\n"
						+ "Please, register JPEGator to get rid of this exception. See readme.txt or reference documentation on how to do this.",
						TrialSizeLimit,
						width,
						height
					)
				);
		}
		*/
	}
}

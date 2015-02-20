// JPEGator .NET Compact Framework Library.
// Copyright (C) 2005-2009, Aleh Dzenisiuk. All Rights Reserved.
// http://dzenisiuk.info/jpegator/
//
// When redistributing JPEGator source code the above copyright notice and 
// this messages should be left intact. In case changes are made by you in 
// this source code it should be clearly indicated in the beginning of each 
// changed file.

using System;

namespace JPEGator
{
	/// <summary>
	/// Color space constants. Used to specify a color space of output or input image.
	/// </summary>
	public enum ColorSpace : int
	{
		/// <summary>
		/// Unspecified or invalid color space.
		/// </summary>
		Unknown,

		/// <summary>
		/// Monochrome image, each pixel takes 1 byte that represents its brightness.
		/// </summary>
		Grayscale,

		/// <summary>
		/// RGB color space: by default every pixel consists of 3 bytes
		/// in the following order red, green, blue.
		/// </summary>
		RGB,

		/// <summary>
		/// YCbCr, also known as Yuv color space.
		/// Typically 3 bytes per pixel in order Y, Cb, Cr.
		/// </summary>
		YCbCr,

		/// <summary>
		/// CMYK color space, 4 bytes per pixel: C, M, Y and K components.
		/// </summary>
		CMYK,

		/// <summary>
		/// Y/Cb/Cr/K color space, typically 4 bytes per pixel.
		/// </summary>
		YCCK
	}

	/// <summary>
	/// DCT (Discrete Cosine Transform) method used by compressor or decompressor.
	/// </summary>
	public enum DCTMethod : int
	{
		/// <summary>
		/// Slow but accurate integer algorithm.
		/// </summary>
		Slow,

		/// <summary>
		/// Faster, but less accurate integer algorithm.
		/// </summary>
		Fast,

		/// <summary>
		/// Floating-point algorithm, accurate but fast only on fast hardware.
		/// When in doubt what to select: <c>Slow</c> or <c>Float</c>,
		/// then just select <c>Slow</c>, it should work faster on ARM, but
		/// give the same results as the <c>Float</c>.
		/// </summary>
		Float
	}

	/// <summary>
	/// Arguments for the <see cref="Process.Progress"/> event.
	/// </summary>
	public class ProgressEventArgs : EventArgs
	{
		public ProgressEventArgs(int percent)
		{
			this.percent = percent;
		}

		protected int percent;

		/// <summary>
		/// Percent of job completeness (number from range [0, 100]).
		/// </summary>
		public int Percent
		{
			get { return percent; }
		}
	}

	/// <summary>
	/// Handler for <see cref="Process.Progress"/> event.
	/// </summary>
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

	/// <summary>
	/// Exception thrown when some critical error occurs during compressiion or decompression.
	/// </summary>
	public sealed class JpegException : ApplicationException
	{
		public JpegException(string message, int errorCode)
			: base(message)
		{
			this.errorCode = errorCode;
		}

		private int errorCode;

		/// <summary>
		/// Native error code received from the Independent JPEG Group's library.
		/// </summary>
		public int ErrorCode
		{
			get { return errorCode; }
		}
	}

	internal class Utils
	{
		/// <summary>
		/// Returns default components count for given color space.
		/// </summary>
		public static int GetComponentsCount(ColorSpace c)
		{
			switch (c)
			{
				case ColorSpace.Grayscale:
					return 1;

				case ColorSpace.RGB:
				case ColorSpace.YCbCr:
					return 3;

				case ColorSpace.YCCK:
				case ColorSpace.CMYK:
					return 4;

				default:
					throw new NotSupportedException(
						string.Format(
							"Color space {0} is not supported",
							c
						)
					);
			}
		}
	}
}

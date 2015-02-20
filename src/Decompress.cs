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
	/// <summary>
	///   JPEG decompressor.
	/// </summary>
	///
	/// <threadsafety static="true" instance="false"/>
	///
	/// <remarks>
	///   <para>
	///   (For those who are familiar with the <a href="http://www.ijg.org/">Independent JPEG
	///   Group</a>'s software: this class wraps the <c>jpeg_decompress_struct</c> C-structure).
	///   </para>
	///   <para>
	///     To use decompressor:
	///   </para>
	///   <list type="number">
	///     <item>
	///        Create an instance of <see cref="Decompress"/> object.
	///     </item>
	///     <item>
	///        Setup progress handler using <see href="Progress"/> event (use it
	///        only if decompression is going to take significant amount of time).
	///     </item>
	///     <item>
	///        Call <see cref="Start(System.IO.Stream)"/> method given it JPEG file stream as a source.
	///        On this stage decompressor reads JPEG stream header and know some information about
	///        the image being decompressed.
	///     </item>
	///     <item>
	///        Allocate proper image or scanline buffer using properties
	///        <see cref="Decompress.InputWidth"/>, <see cref="Decompress.InputHeight"/>,
	///        and <see cref="Decompress.InputColorSpace"/> to calculate image (or scanline) size.
	///     </item>
	///     <item>
	///        Call <see cref="Decompress.ReadScanline(byte[])"/> method for every line of source image
	///        and process them in whatever way you need.
	///     </item>
	///     <item>
	///        Free allocated unmanaged resources using Dispose() method.
	///        (In C# it is convenient to use <c>using</c> construct for this.)
	///     </item>
	///   </list>
	/// </remarks>
	///
	/// <example>
	/// See <b>Resizer</b> example from <b>\examples</b> folder for the full sample.
	/// <code lang="C#">
	/// // Create decompressor instance
	/// using (JPEGator.Decompress decomp = new JPEGator.Decompress())
	/// {
	/// 	// Setup decompression progress handler
	/// 	decomp.Progress += new JPEGator.ProgressEventHandler(decomp_Progress);
	///
	/// 	// Start decompression process (setup source stream, read source picture header)
	/// 	decomp.Start(System.IO.File.OpenRead(openFileDialog.FileName));
	///
	/// 	int sourWidth = decomp.InputWidth;
	/// 	int sourHeight = decomp.InputHeight;
	///
	/// 	byte[] line = new byte[sourWidth * 3];         // Scanline of source image
	///
	/// 	for (int i = 0; i &lt; sourHeight; i++)
	/// 	{
	/// 		decomp.ReadScanline(line);
	/// 		// ...
	/// 		// Do something here with the scanline we have just read
	/// 		// ...
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class Decompress : Process
	{
		private const int StructureSize = 432;

		private const int SourceOffset            =  6 * 4;
		private const int InputWidthOffset        =  7 * 4;
		private const int InputHeightOffset       =  8 * 4;
		private const int InputComponentsOffset   =  9 * 4;
		private const int InputColorSpaceOffset   = 10 * 4;
		private const int OutputColorSpaceOffset  = 11 * 4;
		private const int ScaleNumOffset          = 12 * 4;
		private const int ScaleDenomOffset        = 13 * 4;
		private const int OutputGammaOffset       = 14 * 4;
		private const int DCTMethodOffset         = 17 * 4;
		private const int OutputWidthOffset       = 23 * 4;
		private const int OutputHeightOffset      = 24 * 4;
		private const int OutputComponentsOffset  = 25 * 4;
		private const int DoFancyUpsamplingOffset = 18 * 4;
		private const int DoBlockSmoothingOffset  = 73;
		private const int OutputScanlineOffset    = 30 * 4;
		private const int DataPrecisionOffset     = 48 * 4;
		private const int MarkerListOffset        = 67 * 4;

		private bool created;

		private Source source = new Source();

		/// <summary>
		/// Constructs decompress object. Initializes all parameters to reasonable defaults.
		/// </summary>
		public Decompress()
			: base(StructureSize)
		{
			JpegDll.CreateDecompress(Ptr, JpegDll.Version, StructureSize);
			created = true;
		}

		private bool disposed;

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!disposed)
				{
					if (disposing)
						source.Dispose();

					if (created)
					{
						JpegDll.DestroyDecompress(Ptr);
						created = false;
					}
					disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>
		///   Width of source image.
		/// </summary>
		/// <remarks>
		///   This field contains correct value only after successfull
		///   call of <see cref="Start(System.IO.Stream)"/> method.
		/// </remarks>
		public int InputWidth
		{
			get { return Marshal.ReadInt32(Ptr, InputWidthOffset); }
		}

		/// <summary>
		/// Height of source image.
		/// </summary>
		/// <remarks>
		///   This field contains correct value only after successfull
		///   call of <see cref="Start(System.IO.Stream)"/> method.
		/// </remarks>
		public int InputHeight
		{
			get { return Marshal.ReadInt32(Ptr, InputHeightOffset); }
		}

		/// <summary>
		/// Count of color components in a source JPEG image.
		/// </summary>
		public int InputComponents
		{
			get { return Marshal.ReadInt32(Ptr, InputComponentsOffset); }
		}

		/// <summary>
		/// Color space of source JPEG image.
		/// </summary>
		public ColorSpace InputColorSpace
		{
			get { return (ColorSpace)Marshal.ReadInt32(Ptr, InputColorSpaceOffset); }
		}

		/// <summary>
		/// Color space of output (decompressed) image.
		/// </summary>
		public ColorSpace OutputColorSpace
		{
			get { return (ColorSpace)Marshal.ReadInt32(Ptr, OutputColorSpaceOffset); }
		}

		/// <summary>
		/// Color components count for output (decompressed) image.
		/// </summary>
		public int OutputComponents
		{
			get { return Marshal.ReadInt32(Ptr, OutputComponentsOffset); }
		}

		protected DCTMethod dctMethod = DCTMethod.Fast;

		/// <summary>
		/// DCT method used for decompression.
		/// <note>This value should to be set before decompression starts.</note>
		/// </summary>
		public DCTMethod DCTMethod
		{
			get { return dctMethod; }
			set { dctMethod = value; }
		}
/*
		public int OutputWidth
		{
			get { return Marshal.ReadInt32(Ptr, OutputWidthOffset); }
		}

		public int OutputHeight
		{
			get { return Marshal.ReadInt32(Ptr, OutputHeightOffset); }
		}
*/
		/// <overloads>
		/// <summary>Starts decompression.</summary>
		/// </overloads>
		///
		/// <summary>
		///   Starts decompression process from a stream. Output color space is set to <c>ColorSpace.RGB</c>.
		/// </summary>
		///
		/// <param name="stream">A stream from which JPEG bytes will be read.</param>
		public void Start(Stream stream)
		{
			Start(stream, ColorSpace.RGB);
		}

		/// <summary>
		///   Starts decompression process from a stream and with specified output color space.
		/// </summary>
		///
		/// <param name="stream">A stream from which JPEG bytes will be read.</param>
		/// <param name="outputColorSpace">A color space of output image.</param>
		public void Start(Stream stream, ColorSpace outputColorSpace)
		{
			source.SetStream(stream, false);
			Start(outputColorSpace);
		}

		/// <summary>
		///   Starts decompression process using <paramref name="stream"/> as
		///   a stream for source image.
		/// </summary>
		private void Start(ColorSpace outputColorSpace)
		{
			Marshal.WriteInt32(Ptr, SourceOffset, source);

			JpegDll.ReadHeader(Ptr, true);

			CheckTrial();

			Marshal.WriteInt32(Ptr, OutputColorSpaceOffset, (int)outputColorSpace);

			Marshal.WriteInt32(Ptr, DCTMethodOffset, (int)dctMethod);

			JpegDll.StartDecompress(Ptr);
		}

		protected int linesCount;

		/// <overloads>
		/// <summary>
		/// Reads (decompresses) one line of a source image.
		/// </summary>
		/// </overloads>
		///
		/// <summary>
		///   Reads current line of source image into an array described by the
		///   <paramref name="line"/> parameter.
		/// </summary>
		///
		/// <param name="line">The array where current scanline should be stored.</param>
		public void ReadScanline(byte[] line)
		{
			ReadScanline(line, 0);
		}

		/// <summary>
		///   Reads current line of source image into array described by
		///   <paramref name="line"/> parameter.
		/// </summary>
		/// <param name="line">The array where current scanline should be stored.</param>
		/// <param name="startIndex">An offset to the first byte of scanline in the supplied array.</param>
		public void ReadScanline(byte[] line, int startIndex)
		{
			if (startIndex + InputComponents * InputWidth > line.Length)
				throw new ArgumentOutOfRangeException("startIndex");

			unsafe
			{
				fixed (byte* linePtr = line)
				{
					byte* ptr = linePtr + startIndex;
					JpegDll.ReadScanlines(Ptr, &ptr, 1);
				}
			}

			linesCount++;
			if (linesCount >= InputHeight)
				JpegDll.FinishDecompress(Ptr);
		}

		/// <summary>
		///   Reads the current line of the source image into a memory block array described by the
		///   <paramref name="linePtr"/> parameter.
		/// </summary>
		/// <param name="linePtr">Pointer to the block of memory where current scanline should be stored.</param>
		/// <param name="startIndex">An offset from the beginning of the memory block to the first byte
		/// of scanline.</param>
		public unsafe void ReadScanline(IntPtr linePtr, int startIndex)
		{
			unsafe
			{
				byte* ptr = (byte*)linePtr + startIndex;
				JpegDll.ReadScanlines(Ptr, &ptr, 1);
			}

			linesCount++;
			if (linesCount >= InputHeight)
				JpegDll.FinishDecompress(Ptr);
		}

		/// <summary>
		/// Skips specified number of scanlines of input image.
		/// </summary>
		/// <param name="scanlinesCount">Number of scanlines to skip.</param>
		/// <remarks>
		/// This operation is still time consuming, despite the fact it is a bit faster than
		/// several calls of ReadScanline with a dummy buffer.
		/// </remarks>
		public void SkipScanlines(int scanlinesCount)
		{
			JpegDll.SkipScanlines(Ptr, scanlinesCount);
		}
	}

}

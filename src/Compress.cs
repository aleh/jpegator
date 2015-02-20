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
	/// JPEG compressor.
	/// </summary>
	///
	/// <threadsafety static="true" instance="false"/>
	///
	/// <remarks>
	/// <para>
	///   (For those who are familiar with the <a href="http://www.ijg.org/">Independent JPEG
	///   Group</a>'s software: this class wraps the <c>jpeg_compress_struct</c> C-structure).
	/// </para>
	/// To use <c>Compressor</c>:
	/// <list type="number">
	///   <item>
	///     Create an instance of the <see cref="Compress"/> class, setup image size and source
	///     color space (if needed).
	///   </item>
	///   <item>
	///      Call <see cref="JPEGator.Compress.Start(string, int)"/> method given it a destination,
	///      JPEG quality and output color space (if differs from standard) as parameters.
	///   </item>
	///   <item>
	///      For every horizontal line of your picture call <see cref="WriteScanline(byte[], int)"/> method.
	///   </item>
	///   <item>
	///      Free allocated unmanaged resources using <see cref="Dispose"/> method.
	///      (In C# it is convenient to use <c>using</c> construct for this.)
	///   </item>
	/// </list>
	/// </remarks>
	///
	/// <example>
	/// The following is en excerpt from the <b>Triangles</b> example, which you can find in <b>\examples</b>
	/// folder. It creates several artificial images with different quality settings using only one instance of
	/// <see cref="Compress"/> class.
	/// <code lang="C#">
	///
	/// // Create instance of compressor object and setup size of destination image
	/// JPEGator.Compress c = new JPEGator.Compress(1024, 1024);
	///
	/// // Subscribe on progress notifications
	/// c.Progress += new JPEGator.ProgressEventHandler(OnProgress);
	///
	/// // Prepare buffer for one line of source image, buffer format is RGB, one byte per color component
	/// // As you can see, only buffer of size 3 * width bytes of memory required to generate relative big image
	/// byte[] line = new byte[c.ImageWidth * 3];
	///
	/// // Generate ImagesCount pictures
	/// for (imageIndex = 1; imageIndex &lt;= ImagesCount; imageIndex++)
	/// {
	/// 	// File name for current image
	/// 	string fileName = string.Format("Triangles-{0}.jpg", imageIndex);
	///
	/// 	// Change text on progress label
	/// 	progressLabel.Text = string.Format("Creating {0}...", fileName);
	/// 	progressLabel.Refresh();
	///
	/// 	// Starting compression. Using file as output, scale quality linearly depending on current image number
	/// 	c.Start(fileName, (100 / ImagesCount) * imageIndex, JPEGator.ColorSpace.YCbCr);
	///
	/// 	// Go through all lines of image
	/// 	for (int i = 0; i &lt; c.ImageHeight; i++)
	/// 	{
	/// 		// Fill one line of image using some function
	/// 		for (int j = 0; j &lt; c.ImageWidth; j++)
	/// 		{
	/// 			line[3 * j] = (byte)((7 * i - 5 * j) / 2);
	/// 			line[3 * j + 1] = (byte)((7 * i - j) / 2);
	/// 			line[3 * j + 2] = (byte)((2 * i + 3 * j) / 2);
	/// 		}
	///
	/// 		// Write generated line to the destination
	/// 		c.WriteScanline(line, 0);
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class Compress : Process
	{
		private const int StructSize = 360;

		private const int DestinationOffset      =  6 * 4;
		private const int ImageWidthOffset       =  7 * 4;
		private const int ImageHeightOffset      =  8 * 4;
		private const int InputComponentsOffset  =  9 * 4;
		private const int InputColorSpaceOffset  = 10 * 4;
		private const int InputGammaOffset       = 12 * 4;
		private const int OutputComponentsOffset = 15 * 4;
		private const int OutputColorSpaceOffset = 16 * 4;

		private bool created;

		private Destination destination = new Destination();

		private int linesCount;


		/// <overloads>
		/// <summary>
		/// Prepares compressor for working with images of certain size and color space.
		/// </summary>
		/// <remarks>
		/// <see cref="Compress"/> can be used to create several images, but they all
		/// will have the same size and color space settings.
		/// </remarks>
		/// </overloads>
		///
		/// <summary>
		/// Prepares compressor for working with images of specified size.
		/// Assumes that source color space is <c>ColorSpace.RGB</c>
		/// and components count is 3.
		/// </summary>
		///
		/// <param name="imageWidth">A width of destination images.</param>
		/// <param name="imageHeight">A height of destination images.</param>
		public Compress(int imageWidth, int imageHeight)
			: this(imageWidth, imageHeight, ColorSpace.RGB, 3)
		{
		}

		/// <summary>
		/// Prepares compressor for working with images of specifed size and color space,
		/// default components count depends on supplied color space value.
		/// </summary>
		public Compress(int imageWidth, int imageHeight, ColorSpace inputColorSpace)
			: this(imageWidth, imageHeight, inputColorSpace, Utils.GetComponentsCount(inputColorSpace))
		{
		}

		/// <summary>
		/// Prepares compressor for working with images of specified size, color space and components count.
		/// </summary>
		///
		/// <param name="imageWidth">A width of destination images.</param>
		/// <param name="imageHeight">A height of destination images.</param>
		/// <param name="inputColorSpace">A color space of a source image.</param>
		/// <param name="inputComponents">A number of components in one pixel of source image.</param>
		public Compress(int imageWidth, int imageHeight, ColorSpace inputColorSpace, int inputComponents)
			: base(StructSize)
		{
			CheckTrial();

			JpegDll.CreateCompress(Ptr, JpegDll.Version, Size);
			created = true;

			Marshal.WriteInt32(Ptr, DestinationOffset, destination);

			Marshal.WriteInt32(Ptr, ImageWidthOffset, imageWidth);
			Marshal.WriteInt32(Ptr, ImageHeightOffset, imageHeight);

			ColorSpace space = (inputColorSpace == ColorSpace.Unknown) ? ColorSpace.RGB : inputColorSpace;
			int components = (inputComponents == 0) ? Utils.GetComponentsCount(space) : inputComponents;

			Marshal.WriteInt32(Ptr, InputColorSpaceOffset, (int)space);
			Marshal.WriteInt32(Ptr, InputComponentsOffset, components);

			JpegDll.SetDefaults(Ptr);
		}

		/// <summary>
		/// Width of image.
		/// </summary>
		public int ImageWidth
		{
			get { return Marshal.ReadInt32(Ptr, ImageWidthOffset); }
		}

		/// <summary>
		/// Height of image.
		/// </summary>
		public int ImageHeight
		{
			get { return Marshal.ReadInt32(Ptr, ImageHeightOffset); }
		}

		/// <summary>
		/// Color space of source image.
		/// </summary>
		public ColorSpace InputColorSpace
		{
			get { return (ColorSpace)Marshal.ReadInt32(Ptr, InputColorSpaceOffset); }
		}

		/// <summary>
		/// Color components count for source image.
		/// </summary>
		public int InputComponents
		{
			get { return Marshal.ReadInt32(Ptr, InputComponentsOffset); }
		}

		/// <summary>
		/// Color space of compressed image.
		/// </summary>
		public ColorSpace OutputColorSpace
		{
			get { return (ColorSpace)Marshal.ReadInt32(Ptr, OutputColorSpaceOffset); }
		}

		/// <summary>
		/// Count of color components in compressed image.
		/// </summary>
		public int OutputComponents
		{
			get { return Marshal.ReadInt32(Ptr, OutputComponentsOffset); }
		}

		/// <overloads>
		/// Starts compression.
		/// </overloads>
		///
		/// <summary>
		/// Starts compression into a file with a given name using the <c>ColorSpace.Unknown</c>
		/// output color space.
		/// </summary>
		///
		/// <param name="fileName">A name of a file where to store the compressed image.</param>
		/// <param name="quality">A JPEG quality value for output JPEG file. Should be from the range [0; 100].</param>
		public void Start(string fileName, int quality)
		{
			Start(fileName, quality, ColorSpace.Unknown);
		}

		/// <summary>
		/// Starts compression of source image into a file with given file name.
		/// </summary>
		///
		/// <param name="fileName">A name of a file where to store the compressed image.</param>
		/// <param name="quality">A JPEG quality value for output JPEG file. Should be from the range [0; 100].</param>
		/// <param name="outputColorSpace">A color space of output JPEG file. See <see cref="JPEGator.ColorSpace"/>.</param>
		public void Start(string fileName, int quality, ColorSpace outputColorSpace)
		{
			this.destination.SetStream(System.IO.File.Create(fileName), true);
			Start(quality, outputColorSpace);
		}

		/// <summary>
		/// Starts compression into some <see cref="Stream"/>.
		/// </summary>
		///
		/// <param name="stream">A stream where JPEG file stream will be written.</param>
		/// <param name="quality">A JPEG quality settings, should be from the range [0; 100].</param>
		/// <param name="outputColorSpace">A color space for the output file. See <see cref="JPEGator.ColorSpace"/>.</param>
		///
		/// <remarks>
		///   <note>
		///     Stream is not closed automatically after end of compression.
		///   </note>
		/// </remarks>
		public void Start(Stream stream, int quality, ColorSpace outputColorSpace)
		{
			this.destination.SetStream(stream, false);
			Start(quality, outputColorSpace);
		}

		private void Start(int quality, ColorSpace outputColorSpace)
		{
			ColorSpace space = (outputColorSpace == ColorSpace.Unknown) ? ColorSpace.YCbCr : outputColorSpace;
			Marshal.WriteInt32(Ptr, OutputColorSpaceOffset, (int)space);
			Marshal.WriteInt32(Ptr, OutputComponentsOffset, Utils.GetComponentsCount(space));

			JpegDll.SetQuality(Ptr, quality, true);

			JpegDll.StartCompress(Ptr, true);
			linesCount = 0;
		}

		/// <overloads>
		/// <summary>
		/// Writes single line (scanline) of source image.
		/// </summary>
		/// </overloads>
		///
		/// <summary>
		/// Writes single horizontal line (scanline) of source image to destination
		/// (compressed) stream.
		/// </summary>
		///
		/// <param name="line">An array that contains the current scan line of the source image.</param>
		/// <param name="offset">An offset to the first byte of scan line.</param>
		public void WriteScanline(byte[] line, int offset)
		{
			int length = InputComponents * ImageWidth;
			if (line.Length < length)
				throw new ArgumentOutOfRangeException("line");
			if (offset < 0 || offset + length > line.Length)
				throw new ArgumentOutOfRangeException("offset");

			unsafe
			{
				fixed (byte* linePtr = line)
				{
					byte* ptr = linePtr + offset;
					JpegDll.WriteScanlines(Ptr, &ptr, 1);
				}
			}
			linesCount++;
			if (linesCount == ImageHeight)
				JpegDll.FinishCompress(Ptr);
		}

		/// <summary>
		/// Writes single horizontal line (scanline) of source image to destination
		/// (compressed) stream.
		/// </summary>
		///
		/// <param name="linePtr">Pointer to a block of memory that contains the
		/// current scan line of the source image.</param>
		/// <param name="offset">An offset to the first byte of scan line from the beginning of the memory block.</param>
		public unsafe void WriteScanline(IntPtr linePtr, int offset)
		{
			unsafe
			{
				byte* ptr = (byte*)linePtr + offset;
				JpegDll.WriteScanlines(Ptr, &ptr, 1);
			}
			linesCount++;
			if (linesCount == ImageHeight)
				JpegDll.FinishCompress(Ptr);
		}

		private bool disposed;

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!disposed)
				{
					if (created)
					{
						JpegDll.DestroyCompress(Ptr);
						created = false;
					}

					if (disposing)
						destination.Dispose();

					disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}
}

// JPEGator .NET Compact Framework Library.
// Copyright (C) 2005-2009, Aleh Dzenisiuk. All Rights Reserved.
// http://dzenisiuk.info/jpegator/
//
// When redistributing JPEGator source code the above copyright notice and 
// this messages should be left intact. In case changes are made by you in 
// this source code it should be clearly indicated in the beginning of each 
// changed file.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;

namespace JPEGator
{
	/// <summary>
	/// Contains some helper functions for working with JPEG files/streams
	/// and standard <see cref="System.Drawing.Bitmap"/> class: ones that allow
	/// creating of thumbnail bitmaps from JPEG files,
	/// loading JPEGs using minimum amount of memory,
	/// and saving bitmaps.
	/// </summary>
	///
	/// <threadsafety static="true" instance="false"/>
	public class BitmapUtils
	{
		/// <overloads>
		/// <summary>
		/// Saves a <see cref="System.Drawing.Bitmap"/> to some JPEG file or stream.
		/// </summary>
		/// <example>
		/// Here is the bitmap saving code from the <b>JPEGPad</b> example, which
		/// you can find in <b>\examples</b> folder.
		/// <code lang="Visual Basic">
		///
		/// ' Quality of saved JPEG picture
		/// Private Const JPEGQuality As Integer = 85
		///
		/// Private Sub SaveMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveMenuItem.Click
		///     Dim result As DialogResult = SaveFileDialog.ShowDialog()
		///     If result = DialogResult.OK Then
		///         JPEGator.BitmapUtils.SaveBitmap(Image, SaveFileDialog.FileName, JPEGQuality)
		///     End If
		/// End Sub
		/// </code>
		/// </example>
		/// </overloads>
		///
		/// <summary>
		/// Compresses a <see cref="System.Drawing.Bitmap"/> to some JPEG file on disk
		/// with given quality settings.
		/// </summary>
		///
		/// <param name="bitmap">A <see cref="System.Drawing.Bitmap"/> that needs to be saved as JPEG file.</param>
		/// <param name="fileName">A name of the file to save this bitmap into.</param>
		/// <param name="quality">JPEG quality settings. Should be from the range [0; 100].</param>
		public static void SaveBitmap(System.Drawing.Bitmap bitmap, string fileName, int quality)
		{
			System.IO.Stream s = System.IO.File.OpenWrite(fileName);
			try
			{
				SaveBitmap(bitmap, s, quality);
			}
			finally
			{
				s.Close();
			}
		}

#if NETCF1
		private static void GetBitmapInternals(
			System.Drawing.Bitmap bitmap,
			out IntPtr newBitmapHandle,
			out IntPtr dcHandle,
			out IntPtr bitmapBitsPtr)
		{
			// Get its internal .NETCF's handle
			IntPtr how = Image.GetHowFromImage(bitmap);
			// Reference this handle and obtaining pointer to Bitmap instance data
			IntPtr ptr = NSLHandle_Reference(0x11, how);
			// Obtaining pointer to bitmap information structure
			IntPtr objPtr = (IntPtr)Marshal.ReadInt32(ptr);

			// Handle to device context where bitmap is selected
			dcHandle = (IntPtr)Marshal.ReadInt32(objPtr, 1 * 4);

			// Describe our bits
			BITMAPINFO bm = new BITMAPINFO();
			bm.Size = Marshal.SizeOf(bm);
			bm.Width = bitmap.Width;
			bm.Height = -bitmap.Height;
			bm.Planes = 1;
			bm.BitCount = 24;

			// Try to create DIB section from our bits
			newBitmapHandle = CreateDIBSection(dcHandle, ref bm, 0, out bitmapBitsPtr, IntPtr.Zero, 0);

			// If it appears that there is not enough memory then try to collect some
			// garbage and then allocate bitmap again
			if (newBitmapHandle == IntPtr.Zero)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				newBitmapHandle = CreateDIBSection(dcHandle, ref bm, 0, out bitmapBitsPtr, IntPtr.Zero, 0);
			}
		}
#endif

#if NETCF1
		private static bool checkedLockBits;
		private static ConstructorInfo bitmapConstructor;
		private static MethodInfo lockBitsMethod;
		private static MethodInfo unlockBitsMethod;
		private static PropertyInfo scan0Property;
		private static PropertyInfo strideProperty;
		private static object readOnly;
		private static object writeOnly;
		private static object format24bppRgb;

		private static bool HasLockBits()
		{
			if (!checkedLockBits)
			{
				Type bitmapType = typeof(System.Drawing.Bitmap);
				lockBitsMethod = bitmapType.GetMethod("LockBits");

				if (lockBitsMethod != null)
				{
					bitmapConstructor = bitmapType.GetConstructor(
						new Type[] {
							Type.GetType("System.Int32"),
							Type.GetType("System.Int32"),
							Type.GetType("System.Drawing.Imaging.PixelFormat, System.Drawing"),
						}
					);
					unlockBitsMethod = bitmapType.GetMethod("UnlockBits");

					Type bitmapDataType = Type.GetType("System.Drawing.Imaging.BitmapData, System.Drawing");
					scan0Property = bitmapDataType.GetProperty("Scan0");
					strideProperty = bitmapDataType.GetProperty("Stride");

					Type imageLockModeType = Type.GetType("System.Drawing.Imaging.ImageLockMode, System.Drawing");
					readOnly = Enum.ToObject(imageLockModeType, (int)0x00000001); // ImageLockMode.ReadOnly
					writeOnly = Enum.ToObject(imageLockModeType, (int)0x00000002); // ImageLockMode.WriteOnly

					format24bppRgb = Enum.ToObject(Type.GetType("System.Drawing.Imaging.PixelFormat, System.Drawing"), (int)0x00021808); // PixelFormat.Format24bppRgb
				}
				checkedLockBits = true;
			}
			return lockBitsMethod != null;
		}
#endif

		/// <summary>
		/// Compresses a <see cref="System.Drawing.Bitmap"/> to some JPEG stream
		/// with given quality settings.
		/// </summary>
		///
		/// <param name="bitmap">A <see cref="System.Drawing.Bitmap"/> that needs to be saved as JPEG file.</param>
		/// <param name="stream">Stream where output JPEG bytes should be written to.</param>
		/// <param name="quality">JPEG quality settings. Should be from the range [0; 100].</param>
		///
		/// <remarks>
		/// <note>As this functions has not opened the stream, it will not close it either.</note>
		/// </remarks>
		public static void SaveBitmap(System.Drawing.Bitmap bitmap, System.IO.Stream stream, int quality)
		{
#if NETCF1
			if (!HasLockBits())
			{
				IntPtr newBitmapHandle, dcHandle, bitmapBitsPtr;
				GetBitmapInternals(bitmap, out newBitmapHandle, out dcHandle, out bitmapBitsPtr);
				if (newBitmapHandle != IntPtr.Zero)
				{
					try
					{
						//
						// Converting our 24 bit bitmap to device-compatible one
						//
						IntPtr dc = CreateCompatibleDC(dcHandle);
						if (dc != IntPtr.Zero)
						{
							try
							{
								// Select and copy image from System.Drawing.Bitmap DC into our bitmap
								IntPtr prevBitmap = SelectObject(dc, newBitmapHandle);
								BitBlt(dc, 0, 0, bitmap.Width, bitmap.Height, dcHandle, 0, 0, 0x00CC0020 /* SRCCOPY */);
								SelectObject(dc, prevBitmap);
							}
							finally
							{
								DeleteDC(dc);
							}
						}
						else
							throw new OutOfMemoryException("Unable to create a temporary device context");

						//
						// Compress image to JPEG
						//
						using (JPEGator.Compress c = new JPEGator.Compress(bitmap.Width, bitmap.Height))
						{
							int offset = 0;
							int bitmapLineSize = (bitmap.Width * 3 + 3) & ~3;
							c.Start(stream, quality, JPEGator.ColorSpace.YCbCr);
							for (int i = 0; i < bitmap.Height; i++)
							{
								c.WriteScanline(bitmapBitsPtr, offset);
								offset += bitmapLineSize;
							}
						}
					}
					finally
					{
						DeleteObject(newBitmapHandle);
					}
				}
				else
					throw new OutOfMemoryException("Unable to create a temporary bitmap");
			}
			else
			{
				object bits = lockBitsMethod.Invoke(
					bitmap,
					new object[] {
						new Rectangle(0, 0, bitmap.Width, bitmap.Height),
						readOnly,
						format24bppRgb
					}
				);

				IntPtr scan0 = (IntPtr)scan0Property.GetValue(bits, null);
				int stride = (int)strideProperty.GetValue(bits, null);

				using (JPEGator.Compress c = new JPEGator.Compress(bitmap.Width, bitmap.Height))
				{
					int offset = 0;
					c.Start(stream, quality, JPEGator.ColorSpace.YCbCr);
					for (int i = 0; i < bitmap.Height; i++)
					{
						c.WriteScanline(scan0, offset);
						offset += stride;
					}
				}

				unlockBitsMethod.Invoke(bitmap, new object[] { bits });
			}
#elif NETCF2

			BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			using (JPEGator.Compress c = new JPEGator.Compress(bitmap.Width, bitmap.Height))
			{
				int offset = 0;
				c.Start(stream, quality, JPEGator.ColorSpace.YCbCr);
				for (int i = 0; i < bitmap.Height; i++)
				{
					c.WriteScanline(bits.Scan0, offset);
					offset += bits.Stride;
				}
			}

			bitmap.UnlockBits(bits);

#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif
		}

		/// <summary>
		/// Creates a <see cref="System.Drawing.Bitmap" /> from an array of bytes in BGR24 format.
		/// </summary>
		///
		/// <param name="bitmapBits">An array of raw BGR24 data with strides aligned to the 4 bytes boundary.</param>
		/// <param name="offset">An offset to a first byte of image in <paramref name="bitmapBits"/> array.</param>
		/// <param name="width">A width of the image described by this buffer.</param>
		/// <param name="height">A height of the image described by this buffer.</param>
		///
		/// <remarks>
		/// <para>
		/// Bitmap in <paramref name="bitmapBits"/> array should have standard DIB (BMP) format:
		/// </para>
		/// <code>
		/// B G R B G R ... B G R [padding]
		/// B G R B G R ... B G R [padding]
		/// ....
		/// </code>
		/// <para>
		/// Upper line of bitmap goes first, every line is padded with some amount of bytes,
		/// so count of bytes that every line takes is a multiple of 4.
		/// I.e. a length of <paramref name="bitmapBits"/> array should be at least
		/// <c>((width * 3 + 3) &amp; ~3) * height</c>.
		/// </para>
		/// </remarks>
		public static System.Drawing.Bitmap BitmapFromBGR24(byte[] bitmapBits, int offset, int width, int height)
		{
			int length = ((width * 3 + 3) & ~3) * height;
			if (bitmapBits.Length < offset + length)
				throw new ArgumentOutOfRangeException("Bitmap buffer is too small");

			// Creating temporary bitmap
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);

#if NETCF1

			if (!HasLockBits())
			{
				IntPtr newBitmapHandle, dcHandle, bitmapBitsPtr;
				GetBitmapInternals(bitmap, out newBitmapHandle, out dcHandle, out bitmapBitsPtr);

				if (newBitmapHandle != IntPtr.Zero)
				{
					//
					// Converting our 24 bit bitmap into a device-compatible one
					//
					IntPtr dc = CreateCompatibleDC(dcHandle);
					if (dc != IntPtr.Zero)
					{
						// Copy our bits to created DIB section
						Marshal.Copy(bitmapBits, offset, bitmapBitsPtr, length);

						// Select and copy our bitmap
						IntPtr prevBitmap = SelectObject(dc, newBitmapHandle);
						BitBlt(dcHandle, 0, 0, width, height, dc, 0, 0, 0x00CC0020 /* SRCCOPY */);
						SelectObject(dc, prevBitmap);
						DeleteDC(dc);
					}
					DeleteObject(newBitmapHandle);
				}
				else
					throw new OutOfMemoryException("There is not enough memory for temporary bitmap");
			}
			else
			{
				object data = lockBitsMethod.Invoke(
					bitmap,
					new object[] {
						new Rectangle(0, 0, width, height),
						writeOnly,
						format24bppRgb
					}
				);

				IntPtr ptr = (IntPtr)scan0Property.GetValue(data, null);

				Marshal.Copy(bitmapBits, offset, ptr, length);

				unlockBitsMethod.Invoke(bitmap, new object[] { data });
			}

#elif NETCF2
			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			Marshal.Copy(bitmapBits, offset, data.Scan0, length);
			bitmap.UnlockBits(data);
#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif
			return bitmap;
		}

		/// <summary>
		/// Copies content of the <see cref="System.Drawing.Bitmap" /> instance into an array of bytes in BGR24 format.
		/// </summary>
		///
		/// <param name="bitmap">A source bitmap to copy content from.</param>
		/// <param name="bitmapBits">A destination array of raw BGR24 data with strides aligned to the 4 bytes boundary.</param>
		/// <param name="offset">An offset to the first byte of image in <paramref name="bitmapBits"/> array.</param>
		///
		/// <remarks>
		/// <para>
		/// Bitmap in <paramref name="bitmapBits"/> array will have standard DIB (BMP) format:
		/// </para>
		/// <code>
		/// B G R B G R ... B G R [padding]
		/// B G R B G R ... B G R [padding]
		/// ....
		/// </code>
		/// <para>
		/// Upper line of bitmap goes first, every line is padded with some amount of bytes,
		/// so count of bytes that every line takes is a multiple of 4.
		/// I.e. a length of <paramref name="bitmapBits"/> array should be at least
		/// <c>((width * 3 + 3) &amp; ~3) * height</c>.
		/// </para>
		/// </remarks>
		public static void BGR24FromBitmap(System.Drawing.Bitmap bitmap, byte[] bitmapBits, int offset)
		{
			int length = ((bitmap.Width * 3 + 3) & ~3) * bitmap.Height;
			if (bitmapBits.Length < offset + length)
				throw new ArgumentOutOfRangeException("Bitmap buffer is too small");

#if NETCF1
			if (!HasLockBits())
			{
				IntPtr newBitmapHandle, dcHandle, bitmapBitsPtr;
				GetBitmapInternals(bitmap, out newBitmapHandle, out dcHandle, out bitmapBitsPtr);

				if (newBitmapHandle != IntPtr.Zero)
				{
					try
					{
						//
						// Converting our 24 bit bitmap into a device-compatible one
						//
						IntPtr dc = CreateCompatibleDC(dcHandle);
						if (dc != IntPtr.Zero)
						{
							try
							{
								// Select and copy our bitmap
								IntPtr prevBitmap = SelectObject(dc, newBitmapHandle);
								BitBlt(dc, 0, 0, bitmap.Width, bitmap.Height, dcHandle, 0, 0, 0x00CC0020 /* SRCCOPY */);
								SelectObject(dc, prevBitmap);
							}
							finally
							{
								DeleteDC(dc);
							}

							Marshal.Copy(bitmapBitsPtr, bitmapBits, offset, length);
						}
					}
					finally
					{
						DeleteObject(newBitmapHandle);
					}
				}
				else
					throw new OutOfMemoryException("There is not enough memory for temporary bitmap");
			}
			else
			{
				object data = lockBitsMethod.Invoke(
					bitmap,
					new object[] {
						new Rectangle(0, 0, bitmap.Width, bitmap.Height),
						readOnly,
						format24bppRgb
					}
				);

				IntPtr ptr = (IntPtr)scan0Property.GetValue(data, null);

				Marshal.Copy(ptr, bitmapBits, offset, length);

				unlockBitsMethod.Invoke(bitmap, new object[] { data });
			}

#elif NETCF2
			BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			Marshal.Copy(data.Scan0, bitmapBits, offset, length);
			bitmap.UnlockBits(data);
#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif
		}

		/// <overloads>
		///
		/// <summary>
		/// Loads a large JPEG image from a file or a stream and returns it as a
		/// <see cref="System.Drawing.Bitmap"/>. See remarks on why this function can
		/// be handy despite is has analogues in .NETCF.
		/// </summary>
		///
		/// <remarks>
		/// Standard constructor of <see cref="System.Drawing.Bitmap"/> class (both file and stream versions)
		/// always loads the file into memory before decoding it (not sure about .NETCF 2.0 however), while
		/// <c>LoadBitmap</c> function always reads an image content directly from
		/// file/stream and uses only reasonable amount of memory (order of image width, i.e. the whole file
		/// is not loaded, amount of bytes used does not depend on image height, only does on its width.)
		/// </remarks>
		///
		/// <example>
		/// For complete example see <b>JPEGPad</b> sample from <b>\examples</b> folder.
		/// <code lang="Visual Basic">
		///
		/// ' Offer user to select some JPEG file
		/// Dim Result As DialogResult = OpenFileDialog.ShowDialog()
		///
		/// ' Load the file if user selected some file and have not cancelled the dialog
		/// If Result = DialogResult.OK Then
		///
		///     Try
		///         Cursor.Current = Cursors.WaitCursor
		///
		///         ' Cleanup previously loaded image
		///         If Not IsNothing(Image) Then
		///             PictureBox.Image = Nothing
		///             Image.Dispose()
		///             Image = Nothing
		///             PictureBox.Enabled = False
		///             PictureBox.Size = New Size(0, 0)
		///         End If
		///
		///         Try
		///             ' Load bitmap without any scaling using the JPEGator
		///             Image = JPEGator.BitmapUtils.LoadBitmap(OpenFileDialog.FileName)
		///
		///             ' Setup PictureBox
		///             PictureBox.Image = Image
		///             PictureBox.Size = Image.Size
		///             PictureBox.Location = New Point(0, 0)
		///             PictureBox.Enabled = True
		///
		///         Finally
		///             Cursor.Current = Cursors.Default
		///         End Try
		///
		///     Catch ex As Exception
		///
		///         ' Show exception information, if any
		///         Windows.Forms.MessageBox.Show( _
		///             String.Format("Failed to load file {0}:", OpenFileDialog.FileName), _
		///             "Error", _
		///             MessageBoxButtons.OK, _
		///             MessageBoxIcon.Hand, _
		///             MessageBoxDefaultButton.Button1 _
		///         )
		///     End Try
		/// End If
		/// </code>
		/// </example>
		///
		/// </overloads>
		///
		/// <summary>
		/// Loads a large JPEG image from a file and returns it as a
		/// <see cref="System.Drawing.Bitmap"/>
		/// </summary>
		///
		/// <param name="fileName">A name of a JPEG file to load.</param>
		public static System.Drawing.Bitmap LoadBitmap(string fileName)
		{
			return LoadBitmap(System.IO.File.OpenRead(fileName));
		}

		/// <summary>
		/// Loads a large JPEG image from a stream and returns it as a
		/// <see cref="System.Drawing.Bitmap"/>.
		/// </summary>
		///
		/// <param name="stream">A stream that should be used as a source for JPEG bytes.</param>
		///
		/// <remarks>
		/// <note>As this function have not opened the stream, it does not close it either.
		/// So you should close the stream by yourself (if you need to have it closed, of course).</note>
		/// </remarks>
		public static System.Drawing.Bitmap LoadBitmap(System.IO.Stream stream)
		{
			// Create decompressor instance
			using (JPEGator.Decompress decomp = new JPEGator.Decompress())
			{
				// Start decompression process (setup source stream, read source picture header)
				decomp.Start(stream);

				return LoadBitmapFromDecompressor(decomp);
			}
		}

		private const int TempBitmapHeight = 8;

		private static System.Drawing.Bitmap LoadBitmapFromDecompressor(JPEGator.Decompress decomp)
		{
				// Create shortcuts for input image sizes
				int width = decomp.InputWidth;
				int height = decomp.InputHeight;
				
#if NETCF1
				System.Drawing.Bitmap bitmap;

				if (!HasLockBits())
				{
					//
					// .NETCF 1.0 version
					//

					int bitmapLineSize = (width * 3 + 3) & ~3;

					// Creating dummy bitmap
					bitmap = new System.Drawing.Bitmap(width, height);

					IntPtr newBitmapHandle, dcHandle, bitmapBitsPtr;
					GetBitmapInternals(bitmap, out newBitmapHandle, out dcHandle, out bitmapBitsPtr);

					if (newBitmapHandle != IntPtr.Zero)
					{
						try
						{
							//
							// Converting our 24 bit bitmap to device-compatible one
							//
							IntPtr dc = CreateCompatibleDC(dcHandle);
							if (dc != IntPtr.Zero)
							{
								try
								{
									IntPtr prevBitmap;

									int bitmapOffset = 0;
									int tempY = 0;
									int i;
									for (i = 0; i < height; i++)
									{
										decomp.ReadScanline(bitmapBitsPtr, bitmapOffset);

										tempY++;
										bitmapOffset += bitmapLineSize;

										if (tempY >= TempBitmapHeight)
										{
											// Copy current stripe
											prevBitmap = SelectObject(dc, newBitmapHandle);
											try
											{
												BitBlt(dcHandle, 0, i - TempBitmapHeight + 1, width, TempBitmapHeight, dc, 0, 0, 0x00CC0020 /* SRCCOPY */);
											}
											finally
											{
												SelectObject(dc, prevBitmap);
											}
											tempY = 0;
											bitmapOffset = 0;
										}
									}

									// Copy last stripe
									prevBitmap = SelectObject(dc, newBitmapHandle);
									try
									{
										int h = height % TempBitmapHeight;
										if (h == 0)
											h = TempBitmapHeight;
										BitBlt(dcHandle, 0, height - h, width, h, dc, 0, 0, 0x00CC0020 /* SRCCOPY */);
									}
									finally
									{
										SelectObject(dc, prevBitmap);
									}

								}
								finally
								{
									DeleteDC(dc);
								}
							}
						}
						finally
						{
							DeleteObject(newBitmapHandle);
						}
					}
					else
						throw new ApplicationException("Failed to create thumbnail bitmap. Probably there is not enough memory available");
				}
				else
				{
					// Creating dummy 24bit RGB bitmap
					bitmap = (System.Drawing.Bitmap)bitmapConstructor.Invoke(
						new object[] {
							width,
							height,
							format24bppRgb
						}
					);

					object bits = lockBitsMethod.Invoke(
						bitmap,
						new object[] {
							new Rectangle(0, 0, width, height),
							writeOnly,
							format24bppRgb
						}
					);

					IntPtr ptr = (IntPtr)scan0Property.GetValue(bits, null);
					int stride = (int)strideProperty.GetValue(bits, null);

					int bitmapOffset = 0;
					for (int i = 0; i < height; i++)
					{
						decomp.ReadScanline(ptr, bitmapOffset);
						bitmapOffset += stride;
					}

					unlockBitsMethod.Invoke(bitmap, new object[] { bits });
				}

#elif NETCF2

				//
				// .NETCF 2.0 version
				//

				// Creating dummy 24bit RGB bitmap
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

				BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
				int bitmapOffset = 0;
				for (int i = 0; i < height; i++)
				{
					decomp.ReadScanline(bits.Scan0, bitmapOffset);
					bitmapOffset += bits.Stride;
				}

				bitmap.UnlockBits(bits);
#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif
				return bitmap;
		}

#if NETCF1
		[DllImport(@"\windows\mscoree1_0.dll")]
		private static extern IntPtr NSLHandle_Reference(int type, IntPtr how);

		private struct BITMAPINFO
		{
			public int Size;
			public int Width;
			public int Height;
			public short Planes;
			public short BitCount;
			public int Compression;
			public int SizeImage;
			public int XPelsPerMeter;
			public int YPelsPerMeter;
			public int ClrUsed;
			public int ClrImportant;
		}

		[DllImport("coredll.dll")]
		private static extern IntPtr CreateDIBSection(
			IntPtr hdc,
			ref BITMAPINFO pbmi,
			int usage,
			out IntPtr bitsPtr,
			IntPtr section,
			int offset
		);

		[DllImport("coredll.dll")]
		private static extern IntPtr CreateBitmap(
			int nWidth,
			int nHeight,
			uint cPlanes,
			uint cBitsPerPel,
			byte[] bits
		);

		[DllImport("coredll.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("coredll.dll")]
		private static extern bool BitBlt(
			IntPtr hdcDest,
			int nXDest, int nYDest,
			int nWidth, int nHeight,
			IntPtr hdcSrc,
			int nXSrc, int nYSrc,
			uint dwRop
		);

		[DllImport("coredll.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("coredll.dll")]
		private static extern bool DeleteObject(IntPtr hgdiobj);

		[DllImport("coredll.dll")]
		private static extern bool DeleteDC(IntPtr hdc);
#endif

		private BitmapUtils()
		{
		}

		// Squeeze single scanline of source image
		private static void SqueezeLine(byte[] line, uint[] destLine, uint sourWidth, uint destWidth)
		{
			uint stepW = sourWidth;
			uint stepR = stepW % destWidth;
			uint destWidth3 = destWidth * 3;

			uint s1w = destWidth; // (s + 1) * destWidth
			uint s1wend = sourWidth * destWidth;
			uint d3 = 0;
			uint s3 = 0;

			uint l = 0;
			uint nextW = stepW;
			uint r = stepR;

			while (true)
			{
				// Squeeze whole pixels
				while (s1w <= nextW)
				{
					destLine[d3 + 0] += line[s3++];
					destLine[d3 + 1] += line[s3++];
					destLine[d3 + 2] += line[s3++];
					s1w += destWidth;
				}

				if (s1w >= s1wend)
					break;

				// Add left part of partial pixel
				destLine[d3++] += line[s3 + 0] * r / destWidth;
				destLine[d3++] += line[s3 + 1] * r / destWidth;
				destLine[d3++] += line[s3 + 2] * r / destWidth;

				if (d3 >= destWidth3)
					break;

				l = destWidth - r;

				r += stepR;
				if (r >= destWidth)
					r -= destWidth;

				// Set next pixel to the right part of partial pixel
				destLine[d3 + 0] += line[s3++] * l / destWidth;
				destLine[d3 + 1] += line[s3++] * l / destWidth;
				destLine[d3 + 2] += line[s3++] * l / destWidth;

				s1w += destWidth;

				nextW += stepW;
			}
		}

		/// <overloads>
	    /// <summary>
		/// Creates a thumbnail <see cref="Bitmap"/> of specified size and with specified
		/// quality vs. speed settings from some JPEG stream or file.
	    /// </summary>
		/// <example>
		/// For complete example see <c>ThumbnailViewer</c> from <b>\examples</b> folder.
		/// <code lang="C#">
		///
		/// // Create a thumbnail image
		/// pictureBox.Image = JPEGator.BitmapUtils.LoadThumbnail(
		/// 	// Put filename of your picture here
		/// 	fileName,
		/// 	// Put here the maximum size of thumbnail image you want to obtain.
		/// 	// If proportional flag is false then this will be an exact size
		/// 	// of thumbnail image, if not then this will be the size corrected
		/// 	// to retain proportions of original image
		/// 	pictureBox.Width, pictureBox.Height,
		/// 	// Select thumbnail quality here: image scaling method and DCT method depend on it;
		/// 	// the lower is quality, the better is speed.
		/// 	quality,
		/// 	// Indicates whether thumbnail image will retain proportions of original image
		/// 	proportions
		/// );
		/// </code>
		/// </example>
		/// </overloads>
		///
		/// <summary>
		/// Creates a thumbnail <see cref="Bitmap"/> of specified size and with specified
		/// quality vs. speed settings from a JPEG file.
	    /// </summary>
		///
		/// <param name="fileName">A name of an existing JPEG file, for which the thumbnail image should be created.</param>
	    /// <param name="destinationWidth">A width of the thumbnail image.</param>
		/// <param name="destinationHeight">A height of the thumbnail image.</param>
		/// <param name="quality">A quality versus speed settings. See <see cref="ThumbnailQuality"/>.</param>
	    /// <param name="proportional">If <c>true</c>, then this thumbnail image will retain its original proportions even
		/// if <c>destWidth/destHeight</c> ratio differs from proportions of a source JPEG image.
		/// <note>
		/// Images that are smaller than a destination image will always have their original sizes and
		/// proportions.
		/// </note>
		/// </param>
		///
	    /// <returns>An instance of the <see cref="System.Drawing.Bitmap"/> class containing needed thumbnail image.
		///
		/// <note>
		/// Note that if <c>proportional</c> pramater is <c>true</c>, then size of resulting image most likely will differ from
		/// <paramref name="destWidth"/> x <paramref name="destHeight"/>.
		/// </note>
		///
		/// </returns>
		public static System.Drawing.Bitmap LoadThumbnail(string fileName, int destinationWidth, int destinationHeight, ThumbnailQuality quality, bool proportional)
		{
			System.IO.Stream s = System.IO.File.OpenRead(fileName);
			try
			{
				return LoadThumbnail(s, destinationWidth, destinationHeight, quality, proportional);
			}
			finally
			{
				s.Close();
			}
		}

		/// <summary>
		/// Creates a thumbnail <see cref="Bitmap"/> of specified size and with specified
		/// quality vs. speed settings from a JPEG stream.
	    /// </summary>
		///
	    /// <param name="stream">A source JPEG stream.</param>
	    /// <param name="destinationWidth">A width of thumbnail image.</param>
		/// <param name="destinationHeight">A height of thumbnail image.</param>
		/// <param name="quality">A quality versus speed settings. See <see cref="ThumbnailQuality"/>.</param>
	    /// <param name="proportional">If <c>true</c>, then this thumbnail image will retain its original proportions even
		/// if <c>destWidth/destHeight</c> ratio differs from proportions of a source JPEG image.
		/// <note>
		/// Images that are smaller than a destination image will always have their original sizes and
		/// proportions.
		/// </note>
		/// </param>
		///
	    /// <returns>An instance of the <see cref="System.Drawing.Bitmap"/> class containing needed thumbnail image.
		///
		/// <note>
		/// If <c>proportional</c> pramater is <c>true</c>, then size of resulting image most likely will differ from
		/// <paramref name="destWidth"/> x <paramref name="destHeight"/>.
		/// </note>
		///
		/// </returns>
		public static System.Drawing.Bitmap LoadThumbnail(System.IO.Stream stream, int destinationWidth, int destinationHeight, ThumbnailQuality quality, bool proportional)
		{
			uint destWidth = (uint)destinationWidth;			
			uint destHeight = (uint)destinationHeight;
			
			// Create decompressor instance
			using (JPEGator.Decompress decomp = new JPEGator.Decompress())
			{
				// Select DCT method depending on thumbnail quality
				decomp.DCTMethod = (quality >= ThumbnailQuality.Low) ? DCTMethod.Slow : DCTMethod.Fast;

				// Start decompression process (setup source stream, read source picture header)
				decomp.Start(stream);

				// Create shortcuts for input image sizes
				uint sourWidth = (uint)decomp.InputWidth;
				uint sourHeight = (uint)decomp.InputHeight;

				// Check whether thumbnail size is less than whole image size
				if (destWidth > sourWidth)
					destWidth = sourWidth;
				if (destHeight > sourHeight)
					destHeight = sourHeight;

				// If proportional mode is in effect, then adjust thumbnail size to retain proportions of original image
				if (proportional)
				{
					if (destWidth * sourHeight < sourWidth * destHeight)
						destHeight = sourHeight * destWidth / sourWidth;
					else
						destWidth = sourWidth * destHeight / sourHeight;
				}

				if (sourWidth <= destWidth && sourHeight <= destHeight)
					return LoadBitmapFromDecompressor(decomp);

#if NETCF1
				// Allocating bitmap buffer
				int bitmapLineSize = ((int)destWidth * 3 + 3) & ~3;
				byte[] bitmapBits = new byte[bitmapLineSize * destHeight];
#elif NETCF2
				System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)destWidth, (int)destHeight);
				System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
					new Rectangle(0, 0, (int)destWidth, (int)destHeight),
					System.Drawing.Imaging.ImageLockMode.WriteOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb
				);

				int bitmapLineSize = data.Stride;

				unsafe	{

				byte* bitmapBits = (byte*)data.Scan0;
#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif

				int bitmapOffset = 0;

				// Allocating scanline buffer
				byte[] line = new byte[sourWidth * 3];

				// Squeeze image depending on quality settings
				if (quality >= ThumbnailQuality.High)
				{
					//
					// High quality downscaling: using linear interpolation
					//

					// Getting shortcuts for some values (not sure if they can be optimized by JIT)
					uint sourWidthHeight = sourWidth * sourHeight;
					uint destWidthHeight = destWidth * destHeight;

					//
					// Prepare temporary buffers
					//
					uint[] destLine = new uint[destWidth * 3];       // Temp scanline of destination image
					byte[] destByteLine = new byte[destWidth * 3]; // Ready for output scanline of destination image

					//
					// Prepare scaling algorithm vars
					//
					uint stepH = sourHeight;
					uint stepB = sourHeight % destHeight;
					uint nextH = stepH;
					uint b = stepB;

					uint i = 0;
					uint d = 0;

					// Go through scanlines and squeeze them
					while (true)
					{
						// Squeeze scanlines that fit into a single scanline of the destination image
						while ((i + 1) * destHeight <= nextH)
						{
							decomp.ReadScanline(line);
							SqueezeLine(line, destLine, sourWidth, destWidth);
							i++;
						}

						//
						// Squeeze source scanline that partially belongs to two scanlines of destination image
						//

						// Squeeze first part
						uint[] tempLine = new uint[destWidth * 3];
						if (i < sourHeight)
						{
							decomp.ReadScanline(line);

							SqueezeLine(line, tempLine, sourWidth, destWidth);
							int offset = bitmapOffset;
							for (d = 0; d < destWidth * 3; d++)
							{
								bitmapBits[offset] = (byte)((destLine[d] * destHeight + tempLine[d] * b) * destWidth / sourWidthHeight);
								offset++;
							}
							bitmapOffset += bitmapLineSize;
						}
						else
						{
							System.Diagnostics.Trace.Assert(b == 0);

							int offset = bitmapOffset;
							for (d = 0; d < destWidth * 3; d++)
							{
								bitmapBits[offset] = (byte)(destLine[d] * destWidthHeight / sourWidthHeight);
								offset++;
							}

							break;
						}

						// Squeeze second part
						uint t = destHeight - b;
						for (d = 0; d < destWidth * 3; d++)
							destLine[d] = tempLine[d] * t / destHeight;

						b += stepB;
						if (b >= destHeight)
							b -= destHeight;

						i++;
						nextH += stepH;
					}
				}
				else
				{
					//
					// Low quality downscaling: just throw away some pixels of original image
					//
					uint lineNumber = 0;
					uint y = 0;
					uint sourHeightDestHeight = sourHeight * destHeight;
					uint sourWidthDestWidth = sourWidth * destWidth;
					unsafe
					{
#if NETCF1
						fixed (byte *bitmapBitsPtr = bitmapBits)
#elif NETCF2
						byte *bitmapBitsPtr = bitmapBits;
#endif
						fixed (byte* linePtr = line)

							while (true)
							{
								decomp.ReadScanline(line);
								lineNumber += destHeight;

								uint x = 0;
								int d = bitmapOffset;
								while (x < sourWidthDestWidth)
								{
									uint xx = 3 * (x / destWidth);

									bitmapBitsPtr[d++] = line[xx];
									bitmapBitsPtr[d++] = line[xx + 1];
									bitmapBitsPtr[d++] = line[xx + 2];

									x += sourWidth;
								}

								bitmapOffset += bitmapLineSize;

								y += sourHeight;
								if (y >= sourHeightDestHeight)
									break;

								uint linesToSkip = (y - lineNumber) / destHeight;
								if (linesToSkip > 0)
									decomp.SkipScanlines((int)linesToSkip);

								lineNumber += linesToSkip * destHeight;
							}
					}
				}

#if NETCF1
				return BitmapFromBGR24(bitmapBits, 0, (int)destWidth, (int)destHeight);
#elif NETCF2
				}
				bitmap.UnlockBits(data);
				return bitmap;
#else
	#error Framework is not supported or NETCF1/2 flag is not defined
#endif
			}
		}

	}

}

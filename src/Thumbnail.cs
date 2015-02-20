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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace JPEGator
{
	/// <summary>
	/// Quality of thumbnail image. Then better is quality,
	/// then lower is speed of decompression.
	/// </summary>
	public enum ThumbnailQuality
	{
		/// <summary>
		/// Affordable quality, but better speed. Optimal when it is needed to obtain many
		/// thumbnails quickly.
		/// </summary>
		Lowest,

		/// <summary>
		/// Quality is non-significantly better then for <c>Lowest</c>, while speed is a bit less.
		/// </summary>
		Low,

		/// <summary>
		/// High quality thumbnail, but created much slower than for <c>Lowest</c> quality.
		/// </summary>
		High
	}
}

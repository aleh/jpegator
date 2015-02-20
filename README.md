# JPEGator

JPEGator is a class library which allows to compress and decompress JPEG
images under .NET Compact Framework 1.0 or 2.0 using C# or Visual Basic.

This is quite an old library I was selling between 2005 and 2009, 
but now sharing it here under a MIT license in case someone needs 
to support old code.

This software is based in part on the work of the Independent JPEG Group [http://www.ijg.org/].

For class library reference see \doc\JPEGator Reference.chm.

## Files

This package has the following directory layout:

\doc
    - JPEGator Reference.chm - Library reference documentation.

- \examples

    - \vs-2003 - Examples prepared for Visual Studio 2003.
    - \vs-2005 - Examples prepared for Visual Studio 2005.

- \lib

    - \netcf-1.0
        JPEGator.dll - Managed part of JPEGator library for .NETCF 1.0.

    - \netcf-2.0
        JPEGator.dll - Managed part of JPEGator library for .NETCF 2.0.

    - JPEGator.jpeg.dll - Unmanaged part of JPEGator library (Independent JPEG Groups' Software).

- \src                       - Source code.

As you can see you will need to ship 2 files along with your application:
JPEGator.jpeg.dll and JPEGator.dll. The last one should be referenced from
your .NETCF project (depending on framework version you are using).

## Examples

The \examples folder of this package contains three C# examples (triangles,
thumbnailviewer, resizer) and one VB.NET example (JPEGPad). Their projects
are included into solution files \examples\vs-2005\examples.sln and
\examples\vs-2003\examples.sln. Choose any of them depending on version of
Visual Studio you are going to complie these examples with.

Take note, that you should manually add reference to assembly
\lib\netcf-1.0\JPEGator.dll into all vs-2003 projects, because Visual
Studio 2003 does not support relative paths for assembly references in
Smart Devices projects.

Triangles is an example of a program that needs to generate some big image
and output it directly to a JPEG file on a disk. It demonstrates how to use
JPEGator.Compress class.

Resizer is more complicated example, which shows how to process some
existing JPEG image. It utilizes both JPEGator.Compress and
JPEGator.Decompress classes. Input image is scaled down using integer
linear interpolation.

Two other examples show how to use helper functions from BitmapUtils class.
ThumbnailViewer example demonstrates BitmapUtils.LoadThumbnail, while
JPEGPad --- BitmapUtils.LoadBitmap and BitmapUtils.SaveBitmap.

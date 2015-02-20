JPEGator PRO v. 1.7.1549 full-pro
Copyright (C) 2005-2009, Aleh Dzenisiuk

JPEGator is a class library which allows to compress and decompress JPEG
images under .NET Compact Framework 1.0 or 2.0 using C# or Visual Basic.

This software is based in part on the work of the Independent JPEG Group
[http://www.ijg.org/].

For class library reference see \doc\JPEGator Reference.chm.

Registration URL (in case you have trial version):

http://dzenisiuk.info/jpegator/register.php


== Files

This package has the following directory layout:

    \doc
        JPEGator Reference.chm - Library reference documentation.

    \examples
        \vs-2003               - Examples prepared for Visual Studio 2003.

        \vs-2005               - Examples prepared for Visual Studio 2005.

    \lib
        \netcf-1.0
            JPEGator.dll       - Managed part of JPEGator library for .NETCF 1.0.

        \netcf-2.0
            JPEGator.dll       - Managed part of JPEGator library for .NETCF 2.0.

        JPEGator.jpeg.dll      - Unmanaged part of JPEGator library
                                 (Independent JPEG Groups' Software)
                                 
    \src                       - Source code (available in PRO version only).

As you can see you will need to ship 2 files along with your application:
JPEGator.jpeg.dll and JPEGator.dll. The last one should be referenced from
your .NETCF project (depending on framework version you are using).


== Examples

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

Please note, that if you have the trial version of the JPEGator.dll
assembly, then you will get trial messages from time to time. Use the link 
below to get the full version without such notices:

http://dzenisiuk.info/jpegator/register.php


== Legal Notes

Any company or individual who register the full version is entitled to 
unlimited use of JPEGator in their projects and mobile devices whilst 
they obey the license conditions.

This software partially based on the work of the Independent JPEG Group, 
so in using the JPEGator library you are also using the Independent JPEG 
Group's software too. According to the license agreement, you must state 
somewhere in your documentation that "this software is based in part on 
the work of the Independent JPEG Group".

The source code of the library itself (included with the PRO version) can 
be changed as you wish, but it cannot be publicly redistributed. Private 
redistribution (such as from you to your customers along with the source 
code of the software you use JPEGator with) is allowed provided you retain 
the copyright notices and in case of any changes make it clear that these 
changes were made by you.

== Feedback

I would really like to hear from you any questions, comments, bug reports
and everything you would like to tell. Please use the following link 
to contact me:

http://dzenisiuk.info/contact.php

﻿using System;
using System.IO;

namespace ProphetLamb.Tools.Core
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public static class BinaryReaderExtention
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            const int bufferSize = 4096;
            using var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                ms.Write(buffer, 0, count);
            return ms.ToArray();
        }
    }
}
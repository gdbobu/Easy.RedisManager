﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Easy.Common
{
    public static class CsvStreamExtensions
    {
        public static void WriteCsv<T>(this Stream outputStream, IEnumerable<T> records)
        {
            using (var textWriter = new StreamWriter(outputStream))
            {
                textWriter.WriteCsv(records);
            }
        }

        public static void WriteCsv<T>(this TextWriter writer, IEnumerable<T> records)
        {
            CsvWriter<T>.Write(writer, records);
        }

    }
}

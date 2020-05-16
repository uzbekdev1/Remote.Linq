﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class BinaryDataFormatter
    {
        public static void Write(this Stream stream, object obj)
        {
            byte[] data;
            using (MemoryStream dataStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(dataStream, obj);
                dataStream.Position = 0;
                data = dataStream.ToArray();
            }

            long size = data.LongLength;
            byte[] sizeData = BitConverter.GetBytes(size);

            stream.Write(sizeData, 0, sizeData.Length);
            stream.WriteByte(obj is Exception ? (byte)1 : (byte)0);
            stream.Write(data, 0, data.Length);
        }

        public static T Read<T>(this Stream stream)
        {
            byte[] bytes = new byte[256];

            stream.Read(bytes, 0, 8);
            long size = BitConverter.ToInt64(bytes, 0);

            bool isException = stream.ReadByte() != 0;

            object obj;
            using (MemoryStream dataStream = new MemoryStream())
            {
                int count = 0;
                do
                {
                    int length = size - count < bytes.Length
                        ? (int)(size - count)
                        : bytes.Length;

                    int i = stream.Read(bytes, 0, length);
                    count += i;

                    dataStream.Write(bytes, 0, i);
                }
                while (count < size);

                dataStream.Position = 0;

                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(dataStream);
            }

            if (isException)
            {
                Exception exception = (Exception)obj;
                throw exception;
            }

            return (T)obj;
        }
    }
}

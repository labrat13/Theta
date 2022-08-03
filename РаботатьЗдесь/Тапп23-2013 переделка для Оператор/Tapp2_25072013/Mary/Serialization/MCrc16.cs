using System;
using System.IO;

namespace Mary.Serialization
{
    public static class MCrc16
    {
        /// <summary>
        /// Calculate simple checksum value
        /// </summary>
        /// <param name="values">Data buffer</param>
        /// <returns></returns>
        public static Int16 CalculateCrc16(Byte[] values)
        {
            Int16 res = 0;
            foreach (Byte b in values)
            {
                res += b;
            }

            return res;
        }
        /// <summary>
        /// Calculate simple checksum value
        /// <param name="res">Previous sum value or 0</param>
        /// <param name="buf">Data buffer</param>
        /// <param name="size">size of data in buffer</param>
        /// <returns></returns>
        private static short CalculateCrc16(short res, byte[] buf, int size)
        {
            Int16 val = res;
            for (int i = 0; i < size; i++)
                val += buf[i];
            return val;
        }

        /// <summary>
        /// Calculate crc16 for stream from position, restore current position
        /// </summary>
        /// <param name="st">Stream for read bytes</param>
        /// <param name="Position">Stream position for start reading</param>
        /// <param name="length">length of data</param>
        /// <returns></returns>
        public static Int16 CalculateCrc16FromStream(Stream st, long Position, int length)
        {
            const int pg = 4096;//memory page size = buffer size
            Int64 curPos = st.Position; //store current position
            st.Position = Position; //to begin data
            Byte[] buf = new Byte[pg];
            short res = 0;
            while (length > pg)
            {
                st.Read(buf, 0, pg);
                res = CalculateCrc16(res, buf, pg);
                length -= pg;
            }
            //length <= pg
            st.Read(buf, 0, length);
            res = CalculateCrc16(res, buf, length);
            //restore current position
            st.Position = curPos;
            return res;
        }

    }
}

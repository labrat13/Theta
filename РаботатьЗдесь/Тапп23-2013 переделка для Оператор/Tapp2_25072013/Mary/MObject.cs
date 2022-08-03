using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mary
{
    public abstract class MObject
    {
        /// <summary>
        /// Convert object data to binary stream
        /// </summary>
        /// <param name="writer"></param>
        public abstract void toBinary(BinaryWriter writer);
        /// <summary>
        /// Convert object data from binary stream
        /// </summary>
        /// <param name="reader"></param>
        public abstract void fromBinary(BinaryReader reader);
        /// <summary>
        /// Convert object data to byte array
        /// </summary>
        /// <returns></returns>
        public abstract byte[] toBinaryArray();

        /// <summary>
        /// Convert object data to text string
        /// </summary>
        /// <param name="withHex">True - include HEX representation of binary data</param>
        /// <returns></returns>
        public abstract string toTextString(bool withHex);
        /// <summary>
        /// Convert object data to text stream
        /// </summary>
        /// <param name="writer">text stream writer</param>
        /// <param name="withHex">True - include HEX representation of binary data</param>
        public abstract void toText(TextWriter writer, bool withHex);
        /// <summary>
        /// Convert object data from text stream
        /// </summary>
        /// <param name="reader">text stream reader</param>
        public abstract void fromText(TextReader reader);



    }
}

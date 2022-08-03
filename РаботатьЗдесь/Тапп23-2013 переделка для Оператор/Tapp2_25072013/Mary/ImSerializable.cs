using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mary
{
    /// <summary>
    /// Интерфейс сериализации в двоичный или текстовый поток
    /// </summary>
    /// <remarks></remarks>
    /// <seealso cref=""/>
    public interface ImSerializable
    {
        /// <summary>
        /// Serialize object data to binary stream
        /// </summary>
        /// <param name="writer">Binary writer for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        void toBinary(BinaryWriter writer);

        /// <summary>
        /// Deserialize object data from binary stream
        /// </summary>
        /// <param name="reader">Binary reader for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        void fromBinary(BinaryReader reader);

        /// <summary>
        /// Serialize object data to text stream
        /// </summary>
        /// <param name="writer">Text writer for data</param>
        /// <param name="withHex">True - include HEX representation of binary data.False - text representation only.</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        void toText(TextWriter writer, bool withHex);

        /// <summary>
        /// Deserialize object data from text stream
        /// </summary>
        /// <param name="reader">Text reader for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        void fromText(TextReader reader);
    }
}

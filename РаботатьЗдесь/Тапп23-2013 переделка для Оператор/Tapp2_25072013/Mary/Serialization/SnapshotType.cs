using System;
using System.Collections.Generic;
using System.Text;

namespace Mary.Serialization
{
    public enum MSnapshotType
    {
        /// <summary>
        /// Unknown snapshot type
        /// </summary>
        Unknown,
        /// <summary>
        /// Полный снимок структуры проекта
        /// </summary>
        Solid,
        /// <summary>
        /// Частичный снимок структуры проекта
        /// </summary>
        Partial,
    }
}

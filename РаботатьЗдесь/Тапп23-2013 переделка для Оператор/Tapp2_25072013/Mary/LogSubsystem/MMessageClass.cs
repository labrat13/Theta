using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Набор битовых полей, указывающих класс сообщения, для работы фильтров.
    /// </summary>
    [FlagsAttribute] 
    public enum MMessageClass : uint
    {
        /// <summary>
        /// Nothing allowed
        /// </summary>
        Nothing = 0,
        UserMessage = 1,
        Exception = 2,
        Session = 4,
        Transaction = 8,
        StepFile = 16,
        SqlOperation = 32,
        CellRead = 64,
        CellChange = 128,
        LinkRead = 256,
        LinkChange = 512,
        Optimizer = 1024,
        /// <summary>
        /// All kind of messages allowed
        /// </summary>
        All = 0xFFFFFFFF
    }
}

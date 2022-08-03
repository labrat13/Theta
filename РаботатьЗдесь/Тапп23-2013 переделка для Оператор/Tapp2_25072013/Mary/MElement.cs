using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public abstract class MElement : MObject
    {
        /// <summary>
        /// текстовое описание, String.Empty по умолчанию.
        /// </summary>
        public abstract string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Flag is element active or deleted 
        /// Default true
        /// </summary>
        public abstract bool isActive
        {
            get;
            set;
        }

        /// <summary>
        /// Поле для значения, используемого в сервисных операциях (поиск в графе,  обслуживание и так далее) //default 0
        /// </summary>
        public abstract int ServiceFlag
        {
            get;
            set;
        }

        /// <summary>
        /// Вынесен из подклассов как общее свойство. Link state id. //default 0
        /// </summary>
        public abstract MID State
        {
            get;
            set;
        }

        public override void toBinary(System.IO.BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void fromBinary(System.IO.BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override byte[] toBinaryArray()
        {
            throw new NotImplementedException();
        }

        public override string toTextString(bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void toText(System.IO.TextWriter writer, bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void fromText(System.IO.TextReader reader)
        {
            throw new NotImplementedException();
        }
    }

 
}

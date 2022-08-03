using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Базовый класс для версий солюшена и движка
    /// </summary>
    public class MVersionInfoBase: ImSerializable
    {
        /// <summary>
        /// Текстовый разделитель полей номера версии
        /// </summary>
        private const char delimiter = '.';

        #region *** Fields ***
        /// <summary>
        /// НомерВерсииДвижка обозначает изменения, несовместимые с предыдущими версиями.
        /// </summary>
        protected int m_Major;
        /// <summary>
        /// НомерПодверсииДвижка обозначает изменения, при которых сохраняется совместимость с предыдущими версиями
        /// </summary>
        protected int m_Minor;
        /// <summary>
        /// НомерРевизииДвижка - тут должны были перечисляться хотфиксы и всякие фишки, но, честно говоря, номер сборки вполне мог бы их заменить. Ну, пусть будет ревизия, для соответствия стандартам.
        /// </summary>
        protected int m_Revision;
        /// <summary>
        /// НомерСборкиДвижка - Порядковый номер сборки внутри версии.
        /// </summary>
        protected int m_Build;
        #endregion


        /// <summary>
        /// NT-Конструктор
        /// </summary>
        protected MVersionInfoBase()
        {
            this.m_Major = 0;
            this.m_Minor = 0;
            this.m_Revision = 0;
            this.m_Build = 0;

        }
        /// <summary>
        /// NT-Конструктор из строки версии
        /// </summary>
        /// <param name="text">Строка версии. Пример: 1.16.2.32</param>
        protected MVersionInfoBase(String text)
        {
            fromTextString(text);
        }


        /// <summary>
        /// Check that specified engine version is compatible with current version
        /// </summary>
        /// <param name="ver">Engine version info object</param>
        /// <returns>Returns True if specified version compatible with current version. Return False otherwise.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public bool isCompatibleVersion(MVersionInfoBase ver)
        {
            //сейчас совместимость версий проверяется по majorversion
            //TODO: для нового релиза отредактируйте код, чтобы показать совместимость сверху вниз.
            return (this.m_Major == ver.m_Major);
        }
        /// <summary>
        /// NT-Проверить, что оба объекта одинаковы
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool isEqual(MVersionInfoBase other)
        {
            return ((this.m_Build == other.m_Build) && (this.m_Major == other.m_Major) 
                && (this.m_Minor == other.m_Minor) && (this.m_Revision == other.m_Revision));
        }

        /// <summary>
        /// Get string representation of object
        /// </summary>
        /// <returns>Return string representation of object, like v23.0</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public override string ToString()
        {
            return this.toTextString();
        }


        #region ImSerializable Members
        /// <summary>
        /// NT-Вывести данные объекта версии в двоичный поток.
        /// </summary>
        /// <param name="writer"></param>
        public void toBinary(System.IO.BinaryWriter writer)
        {
            writer.Write(this.m_Major);
            writer.Write(this.m_Minor);
            writer.Write(this.m_Revision);
            writer.Write(this.m_Build);

            return;
        }
        /// <summary>
        /// NT-Извлечь данные объекта версии из двоичного потока
        /// </summary>
        /// <param name="reader"></param>
        public void fromBinary(System.IO.BinaryReader reader)
        {
            this.m_Major = reader.ReadInt32();
            this.m_Minor = reader.ReadInt32();
            this.m_Revision = reader.ReadInt32();
            this.m_Build = reader.ReadInt32();

            return;
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="withHex"></param>
        public void toText(System.IO.TextWriter writer, bool withHex)
        {
            writer.Write(this.toTextString());
        }
        /// <summary>
        /// NR-
        /// </summary>
        /// <param name="reader"></param>
        public void fromText(System.IO.TextReader reader)
        {
            throw new NotImplementedException();//TODO: Add code here...
            //тут непонятно, как разбирать входной поток - нет разделителей данных
            //а городить парсер-автомат состояний мне не хочется без необходимости.
        }
        /// <summary>
        /// NT-Возвращает строку текстового представления версии
        /// </summary>
        /// <returns></returns>
        public string toTextString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_Major);
            sb.Append(delimiter);
            sb.Append(m_Minor);
            sb.Append(delimiter);
            sb.Append(m_Revision);
            sb.Append(delimiter);
            sb.Append(m_Build);

            return sb.ToString();
        }
        /// <summary>
        /// NT-Распарсить строку версии
        /// </summary>
        /// <param name="text"></param>
        public void fromTextString(string text)
        {
            String[] sar = text.Split(delimiter);
            if (sar.Length != 4)
                throw new ArgumentException("Invalid VersionInfo value");
            this.m_Major = Int32.Parse(sar[0]);
            this.m_Minor = Int32.Parse(sar[1]);
            this.m_Revision = Int32.Parse(sar[2]);
            this.m_Build = Int32.Parse(sar[3]);

            return;
        }

        #endregion
    }

    /// <summary>
    /// Обозначает версию движка
    /// </summary>
    public class MEngineVersionInfo : MVersionInfoBase
    {
//TODO: - добавить переопределения функций если надо
        public MEngineVersionInfo()
            : base()
        {
        }
        
        public MEngineVersionInfo(string text)
            : base(text)
        {
        }

        /// <summary>
        /// обозначает изменения, несовместимые с предыдущими версиями.
        /// </summary>
        public int EngineVersionNumber
        {
            get
            {
                return this.m_Major;
            }
            set
            {
                this.m_Major = value;
            }
        }

        /// <summary>
        /// обозначает изменения, при которых сохраняется совместимость с предыдущими версиями
        /// </summary>
        public int EngineSubversionNumber
        {
            get
            {
                return this.m_Minor;
            }
            set
            {
                this.m_Minor = value;
            }
        }

        /// <summary>
        /// обозначает хотфиксы и всякие фишки, не влияющие на совместимость версий.
        /// </summary>
        public int EngineRevisionNumber
        {
            get
            {
                return m_Revision;
            }
            set
            {
                this.m_Revision = value;
            }
        }

        /// <summary>
        /// Порядковый номер сборки внутри версии.
        /// </summary>
        public int EngineBuildNumber
        {
            get
            {
                return this.m_Build;
            }
            set
            {
                this.m_Build = value;
            }
        }
    }

    /// <summary>
    /// Обозначает версию солюшена
    /// </summary>
    public class MSolutionVersionInfo : MVersionInfoBase
    {
        public MSolutionVersionInfo()
            : base()
        {
        }
        
        public MSolutionVersionInfo(string text)
            : base(text)
        {
        }


        //TODO:  - добавить переопределения функций если надо
        public int SolutionVersionNumber
        {
            get
            {
                return this.m_Major;
            }
            set
            {
                this.m_Major = value;
            }
        }

        public int SolutionSubversionNumber
        {
            get
            {
                return this.m_Minor;
            }
            set
            {
                this.m_Minor = value;
            }
        }

        public int SolutionRevisionNumber
        {
            get
            {
                return this.m_Revision;
            }
            set
            {
                this.m_Revision = value;
            }
        }

        /// <summary>
        /// Номер последовательного этапа развития СтруктураСущностей для обозначения версии ПолныйСнимокСтруктуры
        /// </summary>
        public int SolutionStepNumber
        {
            get
            {
                return this.m_Build;
            }
            set
            {
                this.m_Build = value;
            }
        }
    }
}

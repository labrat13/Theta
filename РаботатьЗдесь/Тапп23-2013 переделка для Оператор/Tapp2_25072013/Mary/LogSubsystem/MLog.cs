using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mary
{
    public class MLog
    {
        private bool m_addSymbolicData;
        private StreamWriter m_writer;
        private Encoding m_codepage;
        private static char m_limiter;
        private MEngine m_container;
        /// <summary>
        /// Normal constructor
        /// </summary>
        public MLog(MEngine backref)
        {
            m_container = backref;
            m_limiter = '`';
            m_codepage = Encoding.UTF8;
            m_addSymbolicData = false;
            m_writer = null;
        }
#region  Properties

        /// <summary>
        /// Набор битовых полей, указывающих включить в лог соответствующие классы сообщений.
        /// </summary>
        public MMessageClass logDetail
        {
            get
            {
                return m_container.SolutionSettings.LogDetailsFlags;
            }
            set
            {
                m_container.SolutionSettings.LogDetailsFlags = value;
            }
        }

        /// <summary>
        /// Включать в лог расшифровку для пользователя
        /// </summary>
        public bool addSymbolicData
        {
            get
            {
                return m_addSymbolicData;
            }
            set
            {
                 m_addSymbolicData = value;
            }
        }

        /// <summary>
        /// Писатель файла лога.
        /// </summary>
        public StreamWriter Writer
        {
            get
            {
                return m_writer;
            }
        }

        /// <summary>
        /// Кодировка файла лога. Должна храниться в СвойстваСолюшена, но пока что задается тут в коде. 
        /// </summary>
        public Encoding CodePage
        {
            get
            {
                return m_codepage;
            }
            set
            {
                m_codepage = value;
            }
        }

        /// <summary>
        /// CSV разделитель записей, часть формата. Должна храниться в СвойстваСолюшена, но пока что задается тут в коде.
        /// </summary>
        public static char Limiter
        {
            get
            {
                return m_limiter;
            }
            set
            {
                m_limiter = value;
            }
        }

        /// <summary>
        /// Log file number
        /// </summary>
        public int LogFileNumber
        {
            get { return m_container.SolutionSettings.LogfileNumber; }
            set { m_container.SolutionSettings.LogfileNumber = value; }
        }

#endregion


        /// <summary>
        /// Init log subsystem
        /// </summary>
        internal void Open(MSolutionInfo solutionInfo)
        {
            //get new log file number
            solutionInfo.LogfileNumber = solutionInfo.LogfileNumber + 1;

            //create writer
            m_writer = new StreamWriter(this.getLogPathname(), true, m_codepage);
            m_writer.AutoFlush = true;
            //add message about session start
        }

        /// <summary>
        /// Exit log subsystem
        /// </summary>
        internal void Close()
        {
            //add message about session end
            //close log file
            if (m_writer != null)
            {
                m_writer.Flush();
                m_writer.Close();
            }
        }


        /// <summary>
        /// Добавить сообщение в лог.
        /// </summary>
        /// <remarks>Если класс сообщения разрешен в MLog.LogDetail, то добавить сообщение в лог. Иначе пропустить.</remarks>
        /// <param name="msg">message</param>
        public void addMsg(MLogMsg msg)
        {
            
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Insert text,  for simple debug only
        /// </summary>
        /// <param name="text"></param>
        public void addSomeText(String text)
        {
            m_writer.WriteLine(text);
        }

        /// <summary>
        /// NT-заменяет в сообщениях символ-разделитель на аналоги, чтобы не нарушать формат CSV.
        /// </summary>
        public static string replaceCsvLimiter(string text)
        {
            return text.Replace(MLog.m_limiter, '"');
        }

        /// <summary>
        /// Get pathname for log file opening
        /// </summary>
        /// <returns></returns>
        internal string getLogPathname()
        {
            //name like ..\SolutionName.01.log
            string file = this.m_container.SolutionSettings.getSolutionName16();//TAG:RENEW-13112017
            //add log file number and ext
            file = String.Format("{0}.{1}.log", file, this.LogFileNumber);
            //add full path
            string s = Path.Combine(this.m_container.SolutionManager.LogFolderPath, file);
            return s;
        }



    }
}

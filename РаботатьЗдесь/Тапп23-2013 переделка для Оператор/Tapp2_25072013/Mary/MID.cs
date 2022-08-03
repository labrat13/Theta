using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Cell identifier
    /// </summary>
    public class MID
    {
        /// <summary>
        /// Cell identifier value
        /// </summary>
        private int m_id;
        /// <summary>
        /// Cell identifier value
        /// </summary>
        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public MID()
        {
            m_id = 0;
        }
        /// <summary>
        /// Param constructor
        /// </summary>
        /// <param name="id"></param>
        public MID(int id)
        {
            m_id = id;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="id">source object</param>
        public MID(MID id)
        {
            m_id = id.m_id;
        }

        /// <summary>
        /// Check ID value, throw Exception
        /// </summary>
        /// <param name="cellid">cell id for check</param>
        public static void checkID(int cellid)
        {
            if (cellid == 0) throw new Exception("Invalid cell identifier");
            else if((cellid == Int32.MaxValue) || (cellid == Int32.MinValue)) throw new Exception("Too big cell identifier");
        }
        /// <summary>
        /// Check ID value, throw Exception
        /// </summary>
        public void checkID()
        {
            MID.checkID(m_id);
        }

        /// <summary>
        /// Return true if id is temporary
        /// </summary>
        /// <param name="cellid">cell id for check</param>
        /// <returns></returns>
        public static bool isTemporaryID(int cellid)
        {
            return (cellid < 0);
        }
        /// <summary>
        /// Return true if id is temporary
        /// </summary>
        public bool isTemporaryID()
        {
            return (m_id < 0);
        }

        /// <summary>
        /// Get invalid cell identifier
        /// </summary>
        public static MID InvalidID
        {
            get { return new MID(0); }
        }

        /// <summary>
        /// Get new cell id for new constant cell
        /// </summary>
        /// <param name="maxId">max of existing constant cell id</param>
        /// <returns>Returns new cell id for new constant cell</returns>
        public static int getNewConstId(int maxId)
        {
            return maxId + 1;
        }



        /// <summary>
        /// Get new cell id for new temporary cell
        /// </summary>
        /// <param name="maxId">max of existing temporary cell id</param>
        /// <returns>Returns new cell id for new temporary cell</returns>
        public static int getNewTempId(int maxId)
        {
            return maxId - 1;
        }

        /// <summary>
        /// Return true if identifiers is equal
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public bool isEqual(MID tid)
        {
            return (tid.m_id == this.m_id);
        }

        /// <summary>
        /// Return string representation of object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("?:{0}", m_id);
        }
        /// <summary>
        /// Get 64-bit representation of ID
        /// </summary>
        /// <returns>Returns 0x00000000nnnnnnnn</returns>
        public UInt64 toU64()
        {
            UInt64 r = 0;
            r = r | ((UInt64)((UInt32)this.m_id));
            return r;
        }
        ///// <summary>
        ///// NR-set up ID from 64-bit representation
        ///// </summary>
        ///// <param name="val"></param>
        //public void fromI64(Int64 val)
        //{
        //    this.m_id = (int)(val & 0xFFFFFFFF);

        //}

        /// <summary>
        /// Get id from 64-bit representation
        /// </summary>
        /// <param name="val">64-bit ID value</param>
        /// <returns></returns>
        public static MID fromU64(UInt64 val)
        {
            Int32 r = (Int32)(val & 0xFFFFFFFF);
            return new MID(r);
        }
    }
}

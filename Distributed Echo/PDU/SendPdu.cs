using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Distributed_Echo.PDU
{
    public class SendPdu
    {
        public Method method { get; set; }
        public int message { get; set; }

        public enum Method
        {
            INFO,
            ECHO
        }

        public SendPdu(Method method, int message)
        {
            this.method = method;
            this.message = message;
        }

        public SendPdu()
        {
        }

        public struct KnotMessage
        {
            public Method Method;
            public int message;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LoggerMessage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String message;
        }


        /**
         * Converts a KnotMessage into a byte array 
         */
        public byte[] getBytes(KnotMessage str)
        {
            var size = Marshal.SizeOf(str);
            var arr = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(str, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return arr;
        }

        /**
         * Converts a byte Array back to a KnotMessage object
         */
        public KnotMessage fromBytes(byte[] arr)
        {
            var str = new KnotMessage();
            var size = Marshal.SizeOf(str);
            var ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(arr, 0, ptr, size);
                str = (KnotMessage) Marshal.PtrToStructure(ptr, str.GetType());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return str;
        }
    }
}
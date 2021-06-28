using System;
using System.Runtime.InteropServices;

namespace Distributed_Echo.PDU
{
    public class SendPdu
    {
        private Method method { get; set; }
        private String Message { get; set; }

        public enum Method
        {
            INFO,
            ECHO,
            START,
            LOG
        }

        public SendPdu(Method method, String message)
        {
            this.method = method;
            this.Message = message;
        }

        public SendPdu()
        {
        }

        public struct KnotMessage
        {
            public Method Method;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String message;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LoggerMessage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
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
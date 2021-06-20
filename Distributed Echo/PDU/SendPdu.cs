namespace Distributed_Echo.PDU
{
    public class SendPdu
    {
        public enum Method
        {
            SEND,
            ECHO
        }

        public Method method { get; set; }
        public char[] message { get; set; }

        public SendPdu()
        {
        }

        public SendPdu(Method method, char[] message)
        {
            this.method = method;
            this.message = message;
        }
    }
}
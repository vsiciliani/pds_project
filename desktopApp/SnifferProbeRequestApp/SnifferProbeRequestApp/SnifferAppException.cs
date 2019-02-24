using System;

namespace SnifferProbeRequestApp
{
    class SnifferAppException : Exception
    {
        //costruttore con il solo messaggio di errore
        public SnifferAppException(string message)
        : base(message)
        {
        }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

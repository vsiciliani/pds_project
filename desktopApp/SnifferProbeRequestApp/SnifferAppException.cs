using System;

namespace SnifferProbeRequestApp
{
    class SnifferAppException : Exception {
        //costruttore con il solo messaggio di errore
        public SnifferAppException(string message): base(message){ }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppException(string message, Exception inner): base(message, inner) { }
    }

    class SnifferAppSocketException : SnifferAppException {
        //costruttore con il solo messaggio di errore
        public SnifferAppSocketException(string message) : base(message) { }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppSocketException(string message, Exception inner) : base(message, inner) { }
    }

    class SnifferAppSocketTimeoutException : SnifferAppException {
        //costruttore con il solo messaggio di errore
        public SnifferAppSocketTimeoutException(string message) : base(message) { }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppSocketTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

    class SnifferAppDBConnectionException : SnifferAppException
    {
        //costruttore con il solo messaggio di errore
        public SnifferAppDBConnectionException(string message) : base(message) { }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppDBConnectionException(string message, Exception inner) : base(message, inner) { }
    }

    class SnifferAppSqlException : SnifferAppException {
        //costruttore con il solo messaggio di errore
        public SnifferAppSqlException(string message) : base(message) { }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppSqlException(string message, Exception inner) : base(message, inner) { }
    }

    class SnifferAppThreadException : SnifferAppException {
        //costruttore con il solo messaggio di errore
        public SnifferAppThreadException(string message) : base(message) { }

        //costruttore con il messaggio di errore e lo stackTrace
        public SnifferAppThreadException(string message, Exception inner) : base(message, inner) { }
    }

}

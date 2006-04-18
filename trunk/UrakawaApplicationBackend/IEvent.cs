using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend
{
    //an interface that defines base behavior
    //for all events within the urakawa application
    public interface IEvent
    {
        void setSender(Object sender);
        Object getSender();
    }
}

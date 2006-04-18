using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events
{
    //This is a generic  class that can be
    //used, but ideally one uses the typed
    //subclasses of this class
    class Event : IEvent
    {
        private Object mSender;
        void IEvent.setSender(Object sender)
        {
            mSender = sender;
        }
        Object IEvent.getSender()
        {
            return mSender;
        }
    }
}

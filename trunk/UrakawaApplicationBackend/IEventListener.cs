using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend
{
    //base eventlistener interface
    public interface IEventListener
    {
        void notify(IEvent e);
    }
}

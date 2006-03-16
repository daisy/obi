using System;
using System.Collections;
using System.Text;
using System.Collections;

namespace urakawaApplication
{
    class EventController : IEventController
    {
        private ArrayList mListeners = new ArrayList();

        public void register(IEventListener listener)
        {
            mListeners.Add(listener);
        }

        public void unregister(IEventListener listener)
        {
            mListeners.Remove(listener);
        }

        public void receiveEvent(IEvent e, bool notifySender)
        {
           for (int i=0; i<mListeners.Count; i++)
           {
			    if (notifySender == false && e.getSender() == mListeners[i])
			    {
                    break;
                }
                else
                {
                    //WARNING
                    //this first part needs to be rewritten to express the idea
                    //"if IEvent e is actually of type VU Meter Event object"
				    if (e is urakawaApplication.events.vuMeterEvents.VuMeterEvent 
                        && mListeners[i] is eventListeners.IVuMeterEventListener)
				    {
                        ((IEventListener)mListeners[i]).notify(e);
				    }
			    }
            }
        }
    }
}

using System;
using System.Collections;
using System.Text;


namespace urakawaApplication
{
 
    /* An IEventController functions as a hub
     * throuh which all events risen by application
     * components are passed. An implementation
     * of IEventController is typically implemented 
     * using the singleton pattern.
     */

    public interface IEventController
    {
        // Register an object that implements the IEventListener interface
        void register(IEventListener listener);

        // Unregister an object that implements the IEventListener interface
        void unregister(IEventListener listener);

        /* Recieve an incoming event and send iton to all 
         * listeners that are interested in the event type.
         * The listeners declare interest by implementing certain
         * IEventListener subinterfaces, or the IEventListener interface
         * itself, if intersted in every event emitted by the entire application.
         */
        void receiveEvent(IEvent e, bool notifySender);
    }
}

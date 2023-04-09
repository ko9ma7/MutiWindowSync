using mutiWindowSync.Entity;
using Prism.Events;

namespace mutiWindowSync.Events
{
    public class AddHandleEvent : PubSubEvent<HandleInfo>
    {
        
    }
    
    public class StarAllHandleEvent : PubSubEvent
    {
        
    }
    
    public class ClearHandleEvent : PubSubEvent
    {
        
    }
}
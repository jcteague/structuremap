<!--Title: Object Lifecycles-->
<!--Url: object-lifecycle-->

One of the most valuable functions of using an IoC container tool is its ability to manage the object _lifecycle_ (creating and disposing objects) and _scoping_ (shared objects)
of your system's services so you can focus on the actual functionality instead of busy work. 

Some services like data cache's and [event aggregators](http://martinfowler.com/eaaDev/EventAggregator.html) in your system will need to be shared across requests, screens, and handlers.
Other services should be created new every time you ask the container for one.  Another set of services need to be _scoped_ to an HTTP request or a logical transaction such that
every request to the container for a service originating within the context of a single HTTP request gets the same object instance. 

In all the cases above, StructureMap will assemble object graphs for you using the correct scoping of each dependency.  Likewise, **in some cases**, StructureMap can help you
with object cleanup by calling <code>IDisposable.Dispose()</code> as appropriate when an object lifecyle is completed.

<div class="alert alert-info" role="alert">StructureMap isn't magic, you'll still need to be cognizant of the need for object cleanup.  See the section on <linkto:best-practices]> for more guidance.</div>
 

## Motivation for Container Managed Scope 
 
In the bad old days, we used the infamous [Singleton pattern](http://c2.com/cgi/wiki?SingletonPattern)
as shown below for objects that really needed to be scoped as one single instance for the entire system:

<[sample:evil-singleton]>

The code above isn't terribly difficult to write or understand, but using a Singleton has some negative effects on your code as explained in [Singletons are Evil](http://c2.com/cgi/wiki?SingletonsAreEvil) and my own writing at [Chill out on the Singleton Fetish](http://codebetter.com/jeremymiller/2005/08/04/chill-out-on-the-singleton-fetish).

Instead, let's just use StructureMap to handle the singleton scoping instead and rewrite the code sample above as:

<[sample:no-singleton]>

The big advantage to the second, StructureMap-managed singleton scoping is that my <code>DependencyUser</code> class does not have to have any knowledge of
how the <code>ISingletonDependency</code> object is built and certainly no coupling to the hard Singleton mechanics from the first sample that was so harmful in the past. 



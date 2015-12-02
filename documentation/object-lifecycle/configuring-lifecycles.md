<!--Title: Configuring Lifecycles-->
<!--Url: configuring-lifecycles-->

Unless designated otherwise, StructureMap uses **Transient** as the default scope for all configured _Instance's_.  

<div class="alert alert-info" role="alert">StructureMap's default scoping to <i>Transient</i> is not a universal assumption across IoC containers in .Net.  Be careful with this if you are coming to StructureMap from tools that choose <i>Singleton</i> as their default scope.</div>

The resolution of the lifecycle for any given _Instance_ is to check:

1. Use the explicitly configured lifecycle for the _Instance_
1. If no lifecycle is configured for the _Instance_, use the explicitly configured lifecycle for the _Plugin Type_
1. If no lifecycle is configured for either the _Instance_ or the _Plugin Type_, use the default _Transient_ lifecycle

The lifecycle precedence rules and the syntax for configuring object lifecycles are shown below:

<[sample:lifecycle-rules]>

## Using Attributes

New in StructureMap 4.0 are some simple attributes to mark either a complete _Plugin Type_ or a single _Instance_ as either
a singleton or using the always unique lifecycle:

The usage is shown below:

<[sample:using-lifecycle-attributes]>

See <[linkto:registration/attributes]> for information about adding your own custom attibutes for other lifecycles.

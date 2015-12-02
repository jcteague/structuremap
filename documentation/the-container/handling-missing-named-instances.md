<!--Title: Handling Missing Named Instances-->
<!--Url: handling-missing-named-instances-->


Let's say that something asks StructureMap to resolve a named instance of a type that StructureMap does not know about. What if instead
of throwing the exception for an unknown named service, StructureMap could be taught to create a new registration for that type for the name
requested? That's the exact purpose of the _Missing Named Instance_ feature in StructureMap.

<div class="alert alert-info" role="alert">I've always thought of this feature as being somewhat akin to Ruby's <i><a href="http://rubylearning.com/satishtalim/ruby_method_missing.html">Method Missing</a></i> language feature.</div>

Using the contrived example from the StructureMap tests for this feature, let's say that you have a simple interface and object implementation like this:

<[sample:missing-instance-objects]>

If a user asks the container for a named `Rule` by a name and that rule doesn't exist, we'll just build
a `ColorRule` where the `Color` property should be the name of the Instance requested. That registration
and the usage is shown below:

<[sample:missing-instance-simple-usage]>

The _missing named instance_ rules are evaluated last, meaning that the container will still resolve
explicitly registered instances:

<[sample:missing-instance-does-not-override-explicit]>

You also have the ability to explicitly supply an `Instance` to be evaluated in the _missing named instance_
resolution:

<[sample:missing-instance-with-Instance-registration]>

## Multi-Tenancy

To the best of my recollection, the feature described in this section was designed for a multi-tenancy situation where
we needed to allow the business rules to vary by client, but most clients would still be using the default rules. In our
implemenation, we would make a _service location_ call to the container for the rules by the client id and use the
object returned to calculate the business rules (it was an invoice processing service).

If our rules logic class structure looked like:

<[sample:missing-instance-mt-domain]>

then we could allow client specific rules while still allowing the container to fall through to the default rules
for clients that don't need customized rules:

<[sample:missing-instance-mt-fallthrough]>

In a more complex usage, maybe you need to pull client specific information from a database or configuration files
to construct the rules object. The code below is a partial sample of how you might use the _missing named instance_
feature to do data lookups inside of StructureMap:

<[sample:missing-instance-mt-lookup]>


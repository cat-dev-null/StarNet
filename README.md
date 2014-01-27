# StarNet

A Starbound multiplayer network.

Think IRC, but for Starbound. The goal is to enable MMO-style gameplay with thousands of players in the
same universe. There are a number of lightweight orchestrators (this repository is that software) in
the network. When a player connects to the universe, they are assigned an orchestrator by means of a DNS
round-robin. That orchestrator then connects them to a server (as a proxy). Servers manage one or more
star systems. They are selected by the orchestrators with respect to their current load when a new star
system needs to be assigned.

Orchestrators are responsible for:

* All connections go through an orchestrator
* Moving players between servers and star systems
* Chat between star systems
* A few other things

Each server can be run by the official server software, or (presumably) any alternatives, if alternatives
are ever written. In a perfect world, anyone can contribute a server to the network, but I'm not sure
about how that would work out.

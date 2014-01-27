# StarNet

A Starbound multiplayer network.

## Technical Info

**How does it work?**

It's basically IRC, for Starbound. The goal is to enable MMO-style gameplay with thousands of players
in the same universe. There are a number of lightweight load-balancers that handle connections directly,
and that facilitate communication with a number of game servers. Each game server runs some number of
planets and plays host to several users. These servers can run on the stock Starbound server software,
or potentially third party servers if anyone ever writes some. The load balancers (StarNet servers,
that's what this repo is) are responsible for delegating planets to servers based on their ability to
cope with the load, and they handle a few other things. Distributing between load balancers is a simple
DNS round-robin.

Things to think about:

* Redundancy, where do we keep planets when servers go down?
* Who gets to run a server?
* What happens to malicious nodes?

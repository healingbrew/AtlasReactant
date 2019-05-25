# Server Architecture

The server exists in 2 major types and a web server to access the database.
This document tries to outline the purpose and identity of all 3 servers implemented by this project.

## Directory Server

The Directory Server (DServer) is a load balancing server and authentication server.
The server attempts to verify the login details of the user and provides a connection to a [Lobby Server](#lobby-server).
Beyond that this server does nothing, it's a "Directory of Lobby Servers."

## Lobby Server

The Lobby Server (LServer) is the main server the game performs most actions on.
This includes quests, inventory, and matchmaking.
Initially the server verifies the login token.
This token is the same token provided to the Directory Server, so if you're at this point it should already be a valid token.

### Matches

Matches are made by the lobby server matchmaking algorithm, from there the connections are peer-to-peer(?)

## Web Server

The Web Server (WServer) is a frontend for creating and managing accounts. 
This is entirely custom code specific for this project.

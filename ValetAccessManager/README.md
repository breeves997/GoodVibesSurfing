# Valet Access Manager

This project creates [valet keys](https://msdn.microsoft.com/en-us/library/dn568102.aspx) for direct access to data 
stores. Based off the aboved linked cloud pattern, the intent is to provide restricted, pre-authenticated access 
to a cloud data store for usage by the client application. 

The benefits to this pattern is that it allows you to strictly control access to the external storage without consuming 
resources from the primary application. In this program, we create 
a separate microservice to handle the creation and distribution of the valet keys. Keys can be consumed by interfacing 
with the service for internal calls, or through the stateless web API for client-side calls.

## Setup Information
This mofo listens on port 8200, the baddest of the ports. Port 8200 is the type of port that watches port 8281 get shanked and just walks off. Damn.

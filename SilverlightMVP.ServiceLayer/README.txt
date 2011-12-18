This service layer is a very simple and entirely unrealistic implementation.
I kept it so simple because the whole part of this sample is to demonstrate
the client-side architecture, so there's no reason to focus on the implementation
of this project. It doesn't use a database, it doesn't authenticate its users, nor
does it authorize the service calls, all data is kept in memory and i
paid no attention to thread-safety with regards to concurrent users.  I also
didn't write any tests for this since that's not the point of this sample.

Just something to keep in mind before you start complaining ;)
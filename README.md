# COM-.NET

Introduction:

This library is intended as a simplification of the process of reading / writing via serial ports between Windows-based systems
and other systems of any architecture and operating system to provide information via serial port.

https://en.wikipedia.org/wiki/Serial_port

It is interesting and useful in embedded systems and microcontrollers that communicate in this way such as arduino, raspberry pii,
Intel Edison ... among many others.

Attempts have been abstracting the best possible solution to this problem in order to give a solution most cohesive
and low hookup as possible, facilitating reading and writing through the port by intuitive methods as storage in a "buffer"
reading or sending data received to a function previously defined by the programmer like an event handler (see EventHandler)

With this, the CoreCOM class encapsulates it all the necessary attributes to provide the aforementioned solution is created.

To start using the library, simply instantiate an object of class Corecom and open one of its ports (which used to read or write bytes, characters or strings).

In the Example.vb file you can see a complete example of reading and writing after opening one of the ports available system.

- In reading data, we can find two ways to do this:

ReadLine -> Read a line received by the serial port and stores content attribute "Buffer" by a thread that reads indefinitely over the harbor.

ReadAdviser -> Executes a function previously passed as a parameter in the constructor of the class. Each time you read data on the serial port,
This function is invoked as an EventHandler object.

-within Writing data, we can find two ways to do so for various types of data (using function overloading):

-WriteLine (String) -> Write to the serial port including a newline character at the end.
-Write (Byte [] | string | char []) -> Write data types byte [], string or char [] for the serial port.

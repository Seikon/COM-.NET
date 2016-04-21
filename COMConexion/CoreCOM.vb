Imports System.IO.Ports
Public Class CoreCOM

    Private _PortList As New List(Of SerialPort)
    Private _PortOpened As New SerialPort
    Private _ThreadRead As New Threading.Thread(AddressOf ReadThread)
    Private _ModeRead As ModeRead
    Private _Buffer As String
    Private _BufferByte As New List(Of Byte)
    Private _BufferChar As New List(Of Char)
    Private _FuncAsync As EventHandler

    Public Enum ModeRead
        ReadLine = 1
        ReadAdviser = 2
        ReadByte = 3
        ReadChar = 4
    End Enum
    ''' <summary>
    ''' constructor
    ''' </summary>
    ''' <param name="fillList">Expecify if the constructor fill automaticaly the port list with all the available ports</param>
    ''' <remarks></remarks>
    Public Sub New(Optional ByVal fillList As Boolean = True)
        If fillList Then
            GetPortsEnabled()
        End If

    End Sub
    ''' <summary>
    ''' constructor overload
    ''' </summary>
    ''' <param name="adviser">Function invoked when read with the method 'Read Adviser'</param>
    ''' <param name="fillList"></param>
    ''' <remarks>Expecify if the constructor fill automaticaly the port list with all the available ports</remarks>
    Public Sub New(ByVal adviser As EventHandler, Optional ByVal fillList As Boolean = True)
        If fillList Then
            GetPortsEnabled()
        End If
        _FuncAsync = adviser
    End Sub

    Public Property PortList As List(Of SerialPort)
        Set(value As List(Of SerialPort))
            _PortList = value
        End Set
        Get
            Return _PortList
        End Get
    End Property

    Public Property Buffer
        Set(value)
            _Buffer = value
        End Set
        Get
            Return _Buffer
        End Get
    End Property

    Public Property BufferByte As List(Of Byte)
        Set(value As List(Of Byte))
            _BufferByte = value
        End Set
        Get
            Return _BufferByte
        End Get
    End Property

    Public Property BufferChar As List(Of Char)
        Set(value As List(Of Char))
            _BufferChar = value
        End Set
        Get
            Return _BufferChar
        End Get
    End Property


    ''' <summary>
    ''' Fill the internal port list and return a list with the available SerialPorts
    ''' </summary>
    ''' <returns>List(Of SerialPort)</returns>
    ''' <remarks></remarks>
    Public Function GetPortsEnabled() As List(Of SerialPort)
        _PortList = New List(Of SerialPort)
        For Each strPort In System.IO.Ports.SerialPort.GetPortNames()
            _PortList.Add(New SerialPort(strPort))
        Next
        Return _PortList
    End Function
    ''' <summary>
    '''Fill the internal port list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FillPortsEnabled()
        _PortList = New List(Of SerialPort)
        For Each strPort In System.IO.Ports.SerialPort.GetPortNames()
            _PortList.Add(New SerialPort(strPort))
        Next
    End Sub
    ''' <summary>
    ''' Open a serial port. by port name. Necesary before write / read data ffrom a port
    ''' </summary>
    ''' <param name="name">port name</param>
    ''' <param name="baudSpeed">speed in bauds for open port</param>
    ''' <remarks></remarks>
    Public Sub OpenPort(ByVal name As String, baudSpeed As Integer)

        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        Dim PortOpen As SerialPort = Nothing

        If _PortList.Count = 0 Then
            Throw New PortListEmpty("The ports list is empty, try to fill the list with FillPortsEnabled or with the constructor of the class.")
        Else
            For Each port As SerialPort In _PortList
                If port.PortName.Equals(name) Then
                    PortOpen = port
                    Exit For
                End If
            Next

            If Not IsNothing(PortOpen) Then

                If Not PortOpen.IsOpen Then
                    _PortOpened = PortOpen
                    _PortOpened.BaudRate = baudSpeed
                    _PortOpened.Open()
                Else
                    Throw New PortAlreadyOpened("the port " & name & " has been already opened. Close it before open it again.")
                End If
            Else
                Throw New PortNotFound("port " & name & " not found.")
            End If
        End If
    End Sub
    ''' <summary>
    ''' Open a serial port. by index in the internal ports list. Necesary before write / read data ffrom a port
    ''' </summary>
    ''' <param name="index">index in list of the internal port list</param>
    ''' <param name="baudSpeed">speed in bauds for open port</param>
    ''' <remarks></remarks>
    Public Sub OpenPort(ByVal index As Integer, ByVal baudSpeed As Integer)
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        If Not _PortList(index).IsOpen Then
            _PortOpened = _PortList(index)
            _PortOpened.BaudRate = baudSpeed
            _PortOpened.Open()
        Else
            Throw New PortAlreadyOpened("the port " & index & " has been already opened. Close it before open it again.")
        End If

    End Sub
    ''' <summary>
    ''' Open a serial port by an reference instace of the SerialPort class. Necesary before write / read data ffrom a port
    ''' </summary>
    ''' <param name="port">index in list of the internal port list</param>
    ''' <param name="baudSpeed">speed in bauds for open port</param>
    ''' <remarks></remarks>
    Public Sub OpenPort(ByVal port As SerialPort, ByVal baudSpeed As Integer)
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        If Not port.IsOpen Then
            _PortOpened = port
            _PortOpened.BaudRate = baudSpeed
            _PortOpened.Open()
        Else
            Throw New PortAlreadyOpened("the port " & port.PortName & " has been already opened. Close it before open it again.")
        End If

    End Sub
    ''' <summary>
    ''' close a port
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClosePort()
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        Else
            Throw New PortClosed("the port is already closed.")
        End If
    End Sub
    ''' <summary>
    ''' Stop thread that read that
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopReading()
        _ThreadRead.Abort()
    End Sub
    ''' <summary>
    ''' Start to read data from a port expecifing the method
    ''' </summary>
    ''' <param name="mode">Mode of the serial port. For the moment we found three methods: Storing the data received in buffer field(String), Storing synchronous data in byte, char list field, or expecifing a function that will be invoked each read</param>
    ''' <remarks></remarks>
    Public Sub ReadFromOpenedPort(ByVal mode As ModeRead)
        _ModeRead = mode
        _ThreadRead.Start()
    End Sub
    ''' <summary>
    ''' Write string data in opened serial port
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="lineMethod"></param>
    ''' <remarks></remarks>
    Public Sub WriteToOpenedPort(ByVal text As String, Optional lineMethod As Boolean = False)
        If lineMethod Then
            _PortOpened.WriteLine(text)
        Else
            _PortOpened.Write(text)
        End If

    End Sub
    ''' <summary>
    ''' Write char data in opened serial port
    ''' </summary>
    ''' <param name="buffer">char array to send</param>
    ''' <param name="start">initial position to start to send</param>
    ''' <param name="count">number of chars to send of the array</param>
    ''' <remarks></remarks>
    Public Sub WriteToOpenedPort(ByVal buffer() As Char, ByVal start As Integer, ByVal count As Integer)
        _PortOpened.Write(buffer, start, count)
    End Sub
    ''' <summary>
    ''' Write byte data in opened serial port
    ''' </summary>
    ''' <param name="buffer">byte array to send</param>
    ''' <param name="start">initial position to start to send</param>
    ''' <param name="count">number of bytes to send of the array</param>
    Public Sub WriteToOpenedPort(ByVal buffer() As Byte, ByVal start As Integer, ByVal count As Integer)
        _PortOpened.Write(buffer, start, count)
    End Sub
    ''' <summary>
    ''' Internal thread to read data 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ReadThread()
        While True
            Select Case _ModeRead
                Case ModeRead.ReadLine
                    _Buffer &= _PortOpened.ReadLine()
                Case ModeRead.ReadAdviser
                    _Buffer &= _PortOpened.ReadLine()
                    _FuncAsync.Invoke(Me, Nothing)
                Case ModeRead.ReadByte
                    _BufferByte.Add(_PortOpened.ReadByte())
                Case ModeRead.ReadChar
                    _BufferChar.Add(Convert.ToChar(_PortOpened.ReadChar()))

            End Select
        End While
    End Sub
End Class

#Region "Exceptions"

Public Class PortListEmpty
    Inherits Exception
    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

Public Class PortAlreadyOpened
    Inherits Exception
    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

Public Class PortClosed
    Inherits Exception
    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

Public Class PortNotFound
    Inherits Exception
    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

#End Region
Imports System.IO.Ports
Public Class CoreCOM

    Private _PortList As New List(Of SerialPort)
    Private _PortOpened As New SerialPort
    Private _ThreadRead As New Threading.Thread(AddressOf ReadThread)
    Private _ThreadWrite As New Threading.Thread(AddressOf WriteThread)
    Private _ModeRead As ModeRead
    Private _Buffer As String

    Public Enum ModeRead
        ReadToFindValue = 1
        ReadLine = 2
        Read = 3
    End Enum

    Public Sub New(Optional ByVal fillList As Boolean = False)
        If fillList Then
            GetPortsEnabled()
        End If
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

    Public Function GetPortsEnabled() As List(Of SerialPort)
        _PortList = New List(Of SerialPort)
        For Each strPort In System.IO.Ports.SerialPort.GetPortNames()
            _PortList.Add(New SerialPort(strPort))
        Next
        Return _PortList
    End Function

    Public Sub FillPortsEnabled()
        _PortList = New List(Of SerialPort)
        For Each strPort In System.IO.Ports.SerialPort.GetPortNames()
            _PortList.Add(New SerialPort(strPort))
        Next
    End Sub

    Public Sub OpenPort(ByVal name As String, baudSpeed As Integer)

        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        Dim PortOpen As SerialPort = Nothing

        If _PortList.Count = 0 Then
            'List Empty
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
                    'Port has been already opened
                End If
            Else
                'Port not found
            End If
        End If
    End Sub

    Public Sub OpenPort(ByVal index As Integer, ByVal baudSpeed As Integer)
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        If Not _PortList(index).IsOpen Then
            _PortOpened = _PortList(index)
            _PortOpened.BaudRate = baudSpeed
            _PortOpened.Open()
        Else
            'Puerto ya abierto
        End If

    End Sub

    Public Sub OpenPort(ByVal port As SerialPort, ByVal baudSpeed As Integer)
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        End If

        If Not port.IsOpen Then
            _PortOpened = port
            _PortOpened.BaudRate = baudSpeed
            _PortOpened.Open()
        Else
            'Puerto ya abierto
        End If

    End Sub

    Public Sub ClosePort()
        If _PortOpened.IsOpen Then
            _PortOpened.Close()
        Else
            'Puerto no abierto
        End If
    End Sub

    Public Sub StopReading()
        _ThreadRead.Abort()
    End Sub

    Public Sub ReadFromOpenedPort(ByVal mode As ModeRead)
        _ModeRead = mode
        _ThreadRead.Start()
    End Sub

    Public Sub WriteToOpenedPort()

    End Sub

    Private Sub ReadThread()
        While True
            Select Case _ModeRead
                Case ModeRead.Read
                Case ModeRead.ReadLine
                    _PortOpened.ReadLine()
                Case ModeRead.ReadToFindValue
            End Select
        End While
    End Sub

    Private Sub WriteThread()

    End Sub

End Class

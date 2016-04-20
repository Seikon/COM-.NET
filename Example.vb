Imports COMConexion
Module Example

    Sub Main()
        'Instance the object of the class with a handler function that will be invoked
        'each time when the thread read data from serial port
        Dim serialCom As New COMConexion.CoreCOM(AddressOf handler, True)
        'Open the port, if port doesn't exit, an exception will be thrown
        serialCom.OpenPort("COM4", 9600)
        'Start to read from opened port. In ReadAdviser mode, the function send by parameter'
        'will be invoked per each data reading.
        serialCom.ReadFromOpenedPort(CoreCOM.ModeRead.ReadAdviser)
    End Sub
    'The function invoked by the proccess. it will recive a objet that
    'represent and instance of coreCom initialiced
    Public Sub handler(ByVal coreCom As CoreCOM, ByVal e As System.EventArgs)
        Console.WriteLine(coreCom.Buffer)
    End Sub

End Module

Imports COMConexion
Module ExampleSpanish

    Sub Main()
        'Instanciamos el objeto cuyo manejador de evento de lectura de datos pasamos por parametro.
        'Esta función será invocada cada vez que el hilo lector detecte entradas por el puero serial abierto.
        Dim serialCom As New COMConexion.CoreCOM(AddressOf handler, True)
        'Abrimos el puerto, si este no esta disponible o no existe, enviará una excepción
        serialCom.OpenPort("COM4", 9600)
        'Empezamos a leer desde el puerto serie, en el modo "Read adviser", la función que aparece abajo se ejecutará
        'cada vez que encontremos una entrada de datos.
        serialCom.ReadFromOpenedPort(CoreCOM.ModeRead.ReadAdviser)
    End Sub
    'Función event handler que definimos y pasamos, para la lectura asyncrona de datos
    'Recibe el objeto de la clase que contiene el buffer de datos que recibimos.
    Public Sub handler(ByVal coreCom As CoreCOM, ByVal e As System.EventArgs)
        Console.WriteLine(coreCom.Buffer)
    End Sub

End Module

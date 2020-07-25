Imports System.Net.Sockets
Imports System.IO
Imports System.Text
Imports System.Net.Security

Friend Class Internet

    Const LogOn As Boolean = False
    Const LogPath As String = "C:\users\fernando\internet.vb.log"

    Shared Sub Log(Buff As Byte())
        If LogOn Then
            My.Computer.FileSystem.WriteAllBytes(LogPath, Buff, True)
        End If
    End Sub

    Shared Sub Log(S As String)
        If LogOn Then
            My.Computer.FileSystem.WriteAllText(LogPath, S, True)
        End If
    End Sub

    Shared Function PostHttpResponse(Uri As Uri, PostData As String, ContentType As String) As HttpResponse

        Dim SB As New StringBuilder
        SB.AppendLine(String.Format("POST {0} HTTP/1.1", Uri.PathAndQuery))
        SB.AppendLine(String.Format("Host: {0}", Uri.Host))
        SB.AppendLine(String.Format("Content-Type: {0}; charset=UTF-8", ContentType))
        SB.AppendLine(String.Format("Content-Length: {0}", PostData.Length))
        SB.AppendLine()
        SB.Append(PostData)

        Dim Request As String = SB.ToString

        Return SendHttpRequest(Uri, Request)

    End Function

    Shared Function GetHttpResponse(Uri As Uri, Optional Cookie As String = Nothing) As HttpResponse

        Dim SB As New StringBuilder
        SB.AppendLine(String.Format("GET {0} HTTP/1.1", Uri.PathAndQuery))
        SB.AppendLine(String.Format("Host: {0}", Uri.Host))
        If Not String.IsNullOrEmpty(Cookie) Then
            SB.AppendLine(String.Format("Cookie: flag=1; {0}", Cookie))
        End If
        SB.AppendLine()
        SB.AppendLine()

        Dim Request As String = SB.ToString

        Return SendHttpRequest(Uri, Request)

    End Function

    Shared Function SendHttpRequest(URI As Uri, Request As String, Optional Encoding As System.Text.Encoding = Nothing) As HttpResponse
        If Encoding Is Nothing Then
            Encoding = System.Text.Encoding.UTF8
        End If

        Dim RequestBuff As Byte() = Encoding.GetBytes(Request.Replace(vbCrLf, vbLf).Replace(vbLf, vbCrLf))

        Dim TcpClient As New TcpClient
        TcpClient.Connect(URI.Host, URI.Port)

        Dim Ns As NetworkStream = TcpClient.GetStream
        Dim Stream As Stream

        If URI.Scheme = "https" Then
            Dim SslStream As New SslStream(Ns)
            SslStream.AuthenticateAsClient(URI.Host)
            Stream = SslStream
        Else
            Stream = Ns
        End If

        Stream.Write(RequestBuff, 0, RequestBuff.Count)

        Log(RequestBuff)
        Log(LogPath & vbCrLf & vbCrLf)

        Return ParseHttpResponse(Stream)

    End Function

    Shared Function ParseHttpResponse(Stream As Stream, Optional Progress As Action(Of Integer) = Nothing) As HttpResponse

        Dim HttpResponse As New HttpResponse

        Dim BuffSize As Integer = 2 * 1024 * 1024 '64 * 1024
        Dim Buff(BuffSize - 1) As Byte
        Dim DownloadedBytes As Integer
        Dim BytesRead As Integer

        Dim DataInit As Byte()

        Do
            BytesRead = Stream.Read(Buff, 0, BuffSize)

            Dim S As String = System.Text.Encoding.ASCII.GetString(Buff, 0, BytesRead)

            Dim PHeader As Integer = S.IndexOf(vbCrLf & vbCrLf)

            If PHeader = 0 Then
                Dim ReceivedHeaders As String() = S.Split(vbCrLf)
                HttpResponse.Headers.Items.AddRange(ReceivedHeaders)
            Else
                Dim HeaderPart As String = S.Substring(0, PHeader)
                Dim ReceivedHeaders As String() = HeaderPart.Split(vbCrLf)

                For Each RH In ReceivedHeaders
                    HttpResponse.Headers.Items.Add(RH.Trim)
                Next

                Dim DataStart As Integer = PHeader + 4
                Dim DataCount = BytesRead - DataStart
                ReDim DataInit(DataCount - 1)

                Buffer.BlockCopy(Buff, DataStart, DataInit, 0, DataCount)
                Exit Do
            End If
        Loop

        'must check if Transfer-Encoding: chunked is present
        'http://greenbytes.de/tech/webdav/rfc2616.html#introduction.of.transfer-encoding
        'length := 0
        'read chunk-size, chunk-extension (if any) and CRLF
        'while (chunk-size > 0) {
        '        read chunk - Data And CRLF
        '   append chunk-data to entity-body
        '   length := length + chunk-size
        '        read chunk - Size And CRLF
        '}
        '        read entity - header
        'while (entity-header not empty) {
        '   append entity-header to existing header fields
        '            read entity - header
        '}
        'Content-Length := length
        'Remove "chunked" from Transfer-Encoding

        'http://stackoverflow.com/questions/318489/chunked-encoding-implementation-in-net-or-at-least-pseudo-code

        'HTTP/1.1 304 Not Modified
        'HTTP/1.1 200 OK
        Dim ResponseLine As String = HttpResponse.Headers.Items.Item(0)
        Dim ResponseParts As String() = ResponseLine.Split(" ")
        Dim ResponseCode As Integer = ResponseParts(1)

        Select Case ResponseCode
            Case 304
                Return Nothing
        End Select





        Dim ContentLengthHeader As String = HttpResponse.Headers.Items.SingleOrDefault(Function(x) x.StartsWith("Content-Length"))

        If ContentLengthHeader IsNot Nothing Then
            Dim ContentLength As Integer = ContentLengthHeader.Split(":")(1).Trim

            Using Ms As New MemoryStream
                Ms.Write(DataInit, 0, DataInit.Length)
                DownloadedBytes += DataInit.Length

                Do Until DownloadedBytes <= ContentLength
                    BytesRead = Stream.Read(Buff, 0, BuffSize)
                    Ms.Write(Buff, 0, BytesRead)

                    DownloadedBytes += BytesRead

                    If Progress IsNot Nothing Then
                        Progress.Invoke(DownloadedBytes / ContentLength * 100)
                    End If

                Loop

                HttpResponse.ResponseContent = Ms.ToArray
            End Using

        Else

            Using Ms As New MemoryStream
                Ms.Write(DataInit, 0, DataInit.Length)
                DownloadedBytes += DataInit.Length

                'http://stackoverflow.com/questions/318489/chunked-encoding-implementation-in-net-or-at-least-pseudo-code
                Do
                    Stream.ReadTimeout = 1000

                    Try
                        BytesRead = Stream.Read(Buff, 0, BuffSize)
                        Ms.Write(Buff, 0, BytesRead)
                    Catch ex As Exception
                        Exit Do
                    End Try

                    DownloadedBytes += BytesRead

                    If BytesRead = 0 Then
                        Exit Do
                    End If
                Loop


                If Progress IsNot Nothing Then
                    Progress.Invoke(100)
                End If

                HttpResponse.ResponseContent = Ms.ToArray

            End Using


        End If



        Log(HttpResponse.Headers.ToString & vbCrLf & vbCrLf)
        Log(HttpResponse.ResponseContent)
        Log(vbCrLf & vbCrLf)

        Return HttpResponse
    End Function

End Class

Class HttpResponse
    Property Headers As New HttpResponseHeaders
    Property ResponseContent As Byte()

    Function ResponseAsUTF8() As String
        If ResponseContent IsNot Nothing Then
            Return System.Text.Encoding.UTF8.GetString(ResponseContent)
        Else
            Return Nothing
        End If
    End Function

    Function ResponseAsASCII() As String
        If ResponseContent IsNot Nothing Then
            Return System.Text.Encoding.ASCII.GetString(ResponseContent)
        Else
            Return Nothing
        End If
    End Function

    Function ResponseAsISO88591() As String
        If ResponseContent IsNot Nothing Then
            Return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(ResponseContent)
        Else
            Return Nothing
        End If
    End Function

End Class

Class HttpResponseHeaders

    Property Items As New List(Of String)

    ReadOnly Property ResponseCode As Integer
        Get
            If Items.Count = 0 Then
                Throw New ArgumentNullException("Items (headers) is empty")
            End If

            Return Items.Item(0).Split(" ")(1)
        End Get
    End Property

    ReadOnly Property CurrentDate As Date
        Get
            Return GetHeaderValue("Current-Date")
        End Get
    End Property

    ReadOnly Property LastModified As Date
        Get
            Return GetHeaderValue("Last-Modified")
        End Get
    End Property

    ReadOnly Property Server As String
        Get
            Return GetHeaderValue("Server")
        End Get
    End Property

    ReadOnly Property ContentLength As Integer
        Get
            Return GetHeaderValue("Content-Length")
        End Get
    End Property
    ReadOnly Property ContentType As String
        Get
            Return GetHeaderValue("Content-Type")
        End Get
    End Property
    ReadOnly Property Location As String
        Get
            Return GetHeaderValue("Location")
        End Get
    End Property
    ReadOnly Property Expires As Date
        Get
            Return GetHeaderValue("Expires")
        End Get
    End Property
    ReadOnly Property SetCookie As String
        Get
            Return GetHeaderValue("Set-Cookie")
        End Get
    End Property

    ReadOnly Property Cookie As String
        Get
            Dim Sc As String = SetCookie

            Dim p As Integer = Sc.IndexOf(";")

            If p = 0 Then
                Return Sc
            Else
                Return Sc.Substring(0, p)
            End If

        End Get
    End Property

    ReadOnly Property CacheControl As String
        Get
            Return GetHeaderValue("Cache-Control")
        End Get
    End Property

    Function GetHeaderValue(HeaderName As String) As String
        Dim Header As String = Items.FirstOrDefault(Function(x) x.ToLower.StartsWith(HeaderName.ToLower))

        If String.IsNullOrEmpty(Header) Then
            Return Nothing
        End If

        Dim P As Integer = Header.IndexOf(":")

        If P <= 0 Then
            Throw New ArgumentException("Header don't have a : separator")
        Else
            P += 2
        End If

        Dim Value As String = Header.Substring(P)

        Return Value
    End Function

    Shared Widening Operator CType(ByVal Headers As List(Of String)) As HttpResponseHeaders
        Dim Ret As New HttpResponseHeaders
        Ret.Items = Headers
        Return Ret
    End Operator

    Shared Widening Operator CType(HttpResponseHeaders As HttpResponseHeaders) As List(Of String)
        Return HttpResponseHeaders.Items
    End Operator

    Public Overrides Function ToString() As String
        Return String.Join(vbCrLf, Items.ToArray)
    End Function

End Class

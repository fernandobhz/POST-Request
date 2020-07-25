Class HTMLParser

    Private HTML As String

    Sub New(HTML As String)
        Me.HTML = HTML
    End Sub

    Private Function SuperTrim(S As String)
        S = S.Trim

        Do While S.IndexOf("  ") > 0
            S = S.Replace("  ", " ")
        Loop

        Do While S.IndexOf("&nbsp;") > 0
            S = S.Replace("&nbsp;", " ")
        Loop

        Return S
    End Function

    Friend Function ExtractValue(RefNo As Integer, Reference As String, GreaterThanCount As Integer) As String
        Dim P1 As Integer = Jump(Reference, 0, RefNo)
        P1 = Jump(">", P1, GreaterThanCount)

        Dim Value As String = ExtractInPoint(P1)
        Value = SuperTrim(Value)
        Return Value
    End Function


    Friend Function ExtractValue(Reference As String, GreaterThanCount As Integer) As String
        Dim P1 As Integer = GetPoint(Reference, 0)
        P1 = Jump(">", P1, GreaterThanCount)

        Dim Value As String = ExtractInPoint(P1)
        Value = SuperTrim(Value)
        Return Value
    End Function



    Friend Function ExtractInPoint(P1 As Integer) As String
        Dim P2 As Integer = GetPoint("<", P1)
        Dim Value As String = HTML.Substring(P1, P2 - P1)
        Value = SuperTrim(Value)
        Return Value
    End Function

    Friend Function Jump(Token As String, StartPoint As Integer, Count As Integer) As Integer

        Dim P1 As Integer = StartPoint

        For i As Integer = 1 To Count
            P1 = HTML.IndexOf(Token, P1)

            If P1 < 0 Then
                Throw New ArgumentException(String.Format("Could jump to token '{0}' n{1} ", Token, i))
            Else
                P1 += 1
            End If
        Next

        Return P1

    End Function

    Friend Function GetPoint(Token As String, StartPoint As Integer) As Integer
        Dim P2 As Integer = HTML.IndexOf(Token, StartPoint)

        If P2 < 0 Then
            Throw New ArgumentException(String.Format("Could not found the point '{0}'", Token))
        End If

        Return P2
    End Function

    Friend Function GetAfterPoint(Token As String, StartPoint As Integer) As Integer
        Dim P As Integer = GetPoint(Token, StartPoint)
        Return P + Token.Length
    End Function

End Class

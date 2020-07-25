Public Class Form1

    Private Sub MLS_REQUEST_Click(sender As Object, e As EventArgs) Handles MLS_REQUEST.Click
        Me.URL.Text = "https://location.services.mozilla.com/v1/search?key=test"

        Dim BSSIDs = GetAllBSSID()
        Me.POSTDATA.Text = MLSSearch.Build(BSSIDs)
    End Sub

    Private Sub BUILD_REQUEST_Click(sender As Object, e As EventArgs) Handles BUILD_REQUEST.Click
        Me.URL.Text = "https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyC_cLPqGNoVVU2bISjvLnqNkWBqDrdMq0s"

        Dim BSSIDs = GetAllBSSID()
        Me.POSTDATA.Text = GoogleMapsGeolocation.Build(BSSIDs)
    End Sub

    Private Sub SEND_Click(sender As Object, e As EventArgs) Handles SEND.Click
        Dim R = Internet.PostHttpResponse(New Uri(Me.URL.Text), Me.POSTDATA.Text, "application/json")
        Me.RESPONSE.Text = R.Headers.ToString & vbCrLf & vbCrLf & R.ResponseAsASCII
    End Sub

    Function GetAllBSSID()
        Dim P As New Process
        P.StartInfo.CreateNoWindow = True
        P.StartInfo.FileName = "netsh"
        P.StartInfo.Arguments = "wlan show networks mode=bssid"
        P.StartInfo.RedirectStandardOutput = True
        P.StartInfo.UseShellExecute = False
        P.Start()

        Dim Output = P.StandardOutput.ReadToEnd
        P.WaitForExit()

        Dim BSSIDs As New List(Of String)

        Dim M As New MLSSearch

        Dim Lines() As String = Output.Replace(vbCrLf, vbCr).Split(vbCr)

        For Each L In Lines
            Dim Current As String = L.Trim

            If Current.StartsWith("BSSID") Then
                Dim Point As Integer = Current.IndexOf(":")
                Dim BSSID As String = Current.Substring(Point + 1).Trim

                BSSIDs.Add(BSSID)
            End If
        Next

        Return BSSIDs
    End Function


    
End Class




Class MLSSearch
    Property wifi As New List(Of MLSWifi)

    Sub Add(macAddress As String)
        wifi.Add(New MLSWifi(macAddress.Trim))
    End Sub

    Sub New()

    End Sub

    Sub New(BSSIDs As List(Of String))
        For Each BSSID In BSSIDs
            Add(BSSID)
        Next
    End Sub


    Shared Function Build(BSSIDs As List(Of String)) As String
        Dim X As New MLSSearch(BSSIDs)
        Return Serializers.ToJson(X)
    End Function
End Class

Class MLSWifi
    Property key As String

    Sub New(key As String)
        Me.key = key
    End Sub
End Class




Class GoogleMapsGeolocation
    'https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyC_cLPqGNoVVU2bISjvLnqNkWBqDrdMq0s

    Property wifiAccessPoints As New List(Of GoogleMapsWifiAccessPoints)

    Sub Add(macAddress As String)
        wifiAccessPoints.Add(New GoogleMapsWifiAccessPoints(macAddress.Trim))
    End Sub

    Sub New()

    End Sub

    Sub New(BSSIDs As List(Of String))
        For Each BSSID In BSSIDs
            Add(BSSID)
        Next
    End Sub

    Shared Function Build(BSSIDs As List(Of String)) As String
        Dim X As New GoogleMapsGeolocation(BSSIDs)
        Return Serializers.ToJson(X)
    End Function
End Class

Class GoogleMapsWifiAccessPoints
    Property macAddress As String

    Sub New(macAddress As String)
        Me.macAddress = macAddress
    End Sub
End Class
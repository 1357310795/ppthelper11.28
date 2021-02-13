Public Class AboutWindow
    Inherits Window

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        FlushMemory.Flush()
    End Sub
End Class

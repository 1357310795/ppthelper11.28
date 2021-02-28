Public Class SettingWindow
    Private mw As MainWindow1 = Application.Current.MainWindow
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ToggleButton1.IsChecked = CType(GetKeyValue("main", "StartAnimation", "true", inipath), Boolean)
        ToggleButton2.IsChecked = mw.simplemode

        AddHandler ToggleButton1.Unchecked, AddressOf ToggleButton1_Unchecked
        AddHandler ToggleButton1.Checked, AddressOf ToggleButton1_Checked
        AddHandler ToggleButton2.Unchecked, AddressOf ToggleButton2_Unchecked
        AddHandler ToggleButton2.Checked, AddressOf ToggleButton2_Checked

    End Sub

    Private Sub ToggleButton1_Unchecked(sender As Object, e As RoutedEventArgs)
        SetKeyValue("main", "StartAnimation", "false", inipath)
    End Sub

    Private Sub ToggleButton1_Checked(sender As Object, e As RoutedEventArgs)
        SetKeyValue("main", "StartAnimation", "true", inipath)
    End Sub

    Private Sub ToggleButton2_Unchecked(sender As Object, e As RoutedEventArgs)
        SetKeyValue("main", "simplemode", "false", inipath)
        mw.simplemode = False
        mw.SetSimpleMode()
    End Sub

    Private Sub ToggleButton2_Checked(sender As Object, e As RoutedEventArgs)
        SetKeyValue("main", "simplemode", "true", inipath)
        mw.simplemode = True
        mw.SetSimpleMode()
    End Sub
End Class

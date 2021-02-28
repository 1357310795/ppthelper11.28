Imports System.Windows.Ink
Imports System.Windows.Media.Animation

Public Class BoardView
    Inherits UserControl

    Public scale = 1

    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        InkCanvas1.EraserShape = New Ink.RectangleStylusShape(30, 50)
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        inks = New List(Of StrokeCollection)
        inks.Add(New StrokeCollection())
        n = 0
    End Sub
#Region "page_control"
    Public inks As List(Of StrokeCollection)
    Public n As Int32
    Public Sub ChangePage(x As StrokeCollection)
        InkCanvas1.Strokes = x
    End Sub

    Public Sub AddPage()
        inks(n) = InkCanvas1.Strokes
        inks.Add(New StrokeCollection)
        n = inks.Count - 1
        ChangePage(inks(n))
    End Sub

    Public Sub PrevPage()
        If n = 0 Then
            Exit Sub
        End If
        inks(n) = InkCanvas1.Strokes
        n = n - 1
        ChangePage(inks(n))
    End Sub

    Public Sub NextPage()
        If n = inks.Count - 1 Then
            Exit Sub
        End If
        inks(n) = InkCanvas1.Strokes
        n = n + 1
        'cv.InkCanvas1.Strokes = s(n)
        ChangePage(inks(n))
    End Sub

    Public Function getlabel() As String
        Return CStr(n + 1) + "/" + CStr(inks.Count)
    End Function
#End Region
    Private Sub InkCanvas1_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles InkCanvas1.SizeChanged
        If e.NewSize.Width <> MyBackControl.ActualWidth Then
            InkCanvas1.Width = MyBackControl.ActualWidth
        End If
        If e.NewSize.Height <> MyBackControl.ActualHeight Then
            InkCanvas1.Height = MyBackControl.ActualHeight
        End If
    End Sub

End Class

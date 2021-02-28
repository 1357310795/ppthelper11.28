Imports System.Windows.Ink
Imports System.Windows.Media.Animation
Imports System.Math

Public Class PicView
    Inherits UserControl
    Public scale = 1

    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub

    Private Sub InkCanvas1_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles InkCanvas1.SizeChanged
        If e.NewSize.Width <> BackImage.ActualWidth Then
            InkCanvas1.Width = BackImage.ActualWidth
        End If
        If e.NewSize.Height <> BackImage.ActualHeight Then
            InkCanvas1.Height = BackImage.ActualHeight
        End If
    End Sub
#Region "FreeScale"
    Private TotalScale = 1, TotalRotation, TotalX, TotalY As Double

    Private Sub Canvas1_ManipulationStarting(ByVal sender As Object, ByVal e As ManipulationStartingEventArgs)
        e.ManipulationContainer = Canvas1
    End Sub

    Private Sub Canvas1_ManipulationStarted(ByVal sender As Object, ByVal e As ManipulationStartedEventArgs)
    End Sub

    Private Sub Canvas1_ManipulationDelta(ByVal sender As Object, ByVal e As ManipulationDeltaEventArgs)
        If InkCanvas1.Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
        Dim scale = e.DeltaManipulation.Scale
        Dim expansion = e.DeltaManipulation.Expansion
        Dim rotation = e.DeltaManipulation.Rotation
        Dim translation = e.DeltaManipulation.Translation
        Dim cumulativeScale = e.CumulativeManipulation.Scale
        Dim cumulativeExpansion = e.CumulativeManipulation.Expansion
        Dim cumulativeRotation = e.CumulativeManipulation.Rotation
        Dim cumulativeRranslation = e.CumulativeManipulation.Translation

        TotalScale *= scale.X
        TotalRotation += rotation
        TotalX += translation.X
        TotalY += translation.Y

        Do While TotalRotation < 0
            TotalRotation += 360
        Loop
        Do While TotalRotation >= 360
            TotalRotation -= 360
        Loop

        Dim h As Double = Grid1.ActualHeight
        Dim w As Double = Grid1.ActualWidth
        Dim t As Double = TotalRotation / 180 * PI
        Dim w1 As Double = Abs(h * Sin(t)) + Abs(w * Cos(t)) * TotalScale
        Dim h1 As Double = Abs(h * Cos(t)) + Abs(w * Sin(t)) * TotalScale

        ScaleTransform.ScaleX *= scale.X
        ScaleTransform.ScaleY *= scale.Y
        RotateTransform.Angle += rotation
        Canvas.SetLeft(Grid1, TotalX + w / 2 - w1 / 2)
        Canvas.SetTop(Grid1, TotalY + h / 2 - h1 / 2)
        't1.Text = TotalX.ToString + "," + TotalY.ToString
        't2.Text = x.ToString + "," + y.ToString
    End Sub

    Private Sub Canvas1_ManipulationCompleted(ByVal sender As Object, ByVal e As ManipulationCompletedEventArgs)

    End Sub

    Private Function max4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Max(Math.Max(x1, x2), Math.Max(x3, x4))
    End Function

    Private Function min4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Min(Math.Min(x1, x2), Math.Min(x3, x4))
    End Function
#End Region

End Class

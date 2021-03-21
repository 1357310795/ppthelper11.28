Imports System.Windows.Ink
Imports System.Windows.Media.Animation
Imports System.Math
Imports System.Globalization

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

        ScaleTransform.ScaleX *= scale.X
        ScaleTransform.ScaleY *= scale.Y
        RotateTransform.Angle += rotation

        TranslateTransform.X += translation.X
        TranslateTransform.Y += translation.Y

        Do While RotateTransform.Angle < 0
            RotateTransform.Angle += 360
        Loop
        Do While RotateTransform.Angle >= 360
            RotateTransform.Angle -= 360
        Loop

        SetPicLocation()
        't1.Text = TotalX.ToString + "," + TotalY.ToString
        't2.Text = x.ToString + "," + y.ToString
    End Sub

    Public Sub SetPicLocation()
        Dim h As Double = Grid1.ActualHeight
        Dim w As Double = Grid1.ActualWidth
        Dim t As Double = RotateTransform.Angle / 180 * PI
        Dim w1 As Double = Abs(h * Sin(t)) * ScaleTransform.ScaleX + Abs(w * Cos(t)) * ScaleTransform.ScaleX
        Dim h1 As Double = Abs(h * Cos(t)) * ScaleTransform.ScaleX + Abs(w * Sin(t)) * ScaleTransform.ScaleX
        'Console.WriteLine(w1)
        'Console.WriteLine(h1)
        Canvas.SetLeft(Grid1, TranslateTransform.X - w1 / 2)
        Canvas.SetTop(Grid1, TranslateTransform.Y - h1 / 2)
    End Sub

    Private Sub Canvas1_ManipulationCompleted(ByVal sender As Object, ByVal e As ManipulationCompletedEventArgs)

    End Sub

    Private Function max4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Max(Math.Max(x1, x2), Math.Max(x3, x4))
    End Function

    Private Function min4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Min(Math.Min(x1, x2), Math.Min(x3, x4))
    End Function

    Dim p0, p1, p2 As Point

    Private Sub Canvas1_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Canvas1.MouseDown
        If InkCanvas1.Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
        p1 = e.GetPosition(Canvas1)
    End Sub

    Private Sub Canvas1_MouseMove(sender As Object, e As MouseEventArgs) Handles Canvas1.MouseMove
        If InkCanvas1.Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
        p2 = e.GetPosition(Canvas1)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim v = p2 - p1
            TranslateTransform.X += v.X
            TranslateTransform.Y += v.Y
            SetPicLocation()
            p1 = p2
        ElseIf e.RightButton = MouseButtonState.Pressed Then
            p0 = New Point(TranslateTransform.X, TranslateTransform.Y)
            If p2 = p0 Then Exit Sub
            Dim ang = Math.Atan2(p2.Y - p0.Y, p2.X - p0.X) - Math.Atan2(p1.Y - p0.Y, p1.X - p0.X)
            ang = ang / Math.PI * 180
            Do While ang < -180
                ang += 360
            Loop
            Do While ang >= 180
                ang -= 360
            Loop
            RotateTransform.Angle += ang
            SetPicLocation()
            p1 = p2
        End If
    End Sub

    Private Sub Canvas1_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Canvas1.MouseUp
        If InkCanvas1.Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
    End Sub

    Private Sub Canvas1_MouseWheel(sender As Object, e As MouseWheelEventArgs) Handles Canvas1.MouseWheel
        If InkCanvas1.Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
        p0 = New Point(TranslateTransform.X, TranslateTransform.Y)
        p1 = e.GetPosition(Canvas1)
        Dim v1 As Vector = p0 - p1, v2 As Vector
        If e.Delta > 0 Then
            ScaleTransform.ScaleX *= 1.1
            ScaleTransform.ScaleY *= 1.1
            v2 = v1 * 1.1
        Else
            ScaleTransform.ScaleX *= 0.9
            ScaleTransform.ScaleY *= 0.9
            v2 = v1 * 0.9
        End If
        p2 = p1 + v2
        TranslateTransform.X = p2.X
        TranslateTransform.Y = p2.Y
        SetPicLocation()
    End Sub

    'Public Class PicTopConverter
    '    Implements IValueConverter

    '    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    '        Dim tg As Transform = TryCast(value, Transform)
    '        Dim st As ScaleTransform = FindScaleTransform(tg)
    '        Dim tt As TranslateTransform = FindTranslateTransform(tg)
    '        Dim rt As RotateTransform = FindRotateTransform(tg)

    '    End Function

    '    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
    '        Throw New NotImplementedException()
    '    End Function
    'End Class
#End Region

End Class

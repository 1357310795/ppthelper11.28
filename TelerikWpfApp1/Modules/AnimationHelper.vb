Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Media.Animation

Public Class AnimationHelper
    'Public Sub SaveFrameworkElementToImage(ByVal ui As FrameworkElement, ByVal filepath As String)
    '    Try
    '        Dim ms As FileStream = New FileStream(filepath, FileMode.Create)
    '        Dim bmp As RenderTargetBitmap = New RenderTargetBitmap(CInt(ui.ActualWidth), CInt(ui.ActualHeight), 96.0R, 96.0R, PixelFormats.Pbgra32)
    '        bmp.Render(ui)
    '        Dim encoder As PngBitmapEncoder = New PngBitmapEncoder()
    '        encoder.Frames.Add(BitmapFrame.Create(bmp))
    '        encoder.Save(ms)
    '        ms.Close()
    '    Catch ex As Exception
    '    End Try
    'End Sub
    'Public Sub RenderVisual(ByVal elt As UIElement, ByVal filepath As String)
    '    Dim source As PresentationSource = PresentationSource.FromVisual(elt)
    '    Dim rtb As RenderTargetBitmap = New RenderTargetBitmap(CInt(elt.RenderSize.Width), CInt(elt.RenderSize.Height), 96, 96, PixelFormats.[Default])
    '    Dim sourceBrush As VisualBrush = New VisualBrush(elt)
    '    Dim drawingVisual As DrawingVisual = New DrawingVisual()
    '    Dim drawingContext As DrawingContext = drawingVisual.RenderOpen()

    '    Using drawingContext
    '        drawingContext.DrawRectangle(sourceBrush, Nothing, New Rect(New Point(0, 0), New Point(elt.RenderSize.Width, elt.RenderSize.Height)))
    '    End Using

    '    rtb.Render(drawingVisual)
    '    Dim encoder As PngBitmapEncoder = New PngBitmapEncoder()
    '    encoder.Frames.Add(BitmapFrame.Create(rtb))
    '    Dim ms As FileStream = New FileStream(filepath, FileMode.Create)
    '    encoder.Save(ms)
    '    ms.Close()
    'End Sub
    'Public Function RenderVisualAsImage(ByVal elt As UIElement) As BitmapImage
    '    Dim source As PresentationSource = PresentationSource.FromVisual(elt)
    '    Dim rtb As RenderTargetBitmap = New RenderTargetBitmap(CInt(elt.RenderSize.Width), CInt(elt.RenderSize.Height), 96, 96, PixelFormats.[Default])
    '    Dim sourceBrush As VisualBrush = New VisualBrush(elt)
    '    Dim drawingVisual As DrawingVisual = New DrawingVisual()
    '    Dim drawingContext As DrawingContext = drawingVisual.RenderOpen()

    '    Using drawingContext
    '        drawingContext.DrawRectangle(sourceBrush, Nothing, New Rect(New Point(0, 0), New Point(elt.RenderSize.Width, elt.RenderSize.Height)))
    '    End Using

    '    rtb.Render(drawingVisual)
    '    Return ConvertRenderTargetBitmapToBitmapImage(rtb)

    'End Function

    'Public Function ConvertRenderTargetBitmapToBitmapImage(ByVal wbm As RenderTargetBitmap) As BitmapImage
    '    Dim bmp As BitmapImage = New BitmapImage()

    '    Using stream As MemoryStream = New MemoryStream()
    '        Dim encoder As BmpBitmapEncoder = New BmpBitmapEncoder()
    '        encoder.Frames.Add(BitmapFrame.Create(wbm))
    '        encoder.Save(stream)
    '        bmp.BeginInit()
    '        bmp.CacheOption = BitmapCacheOption.OnLoad
    '        bmp.CreateOptions = BitmapCreateOptions.PreservePixelFormat
    '        bmp.StreamSource = New MemoryStream(stream.ToArray())
    '        bmp.EndInit()
    '        bmp.Freeze()
    '    End Using

    '    Return bmp
    'End Function

    'Public Sub RenderVisual(ByVal elt As UIElement, ByVal filepath As String, ByVal x As Integer, ByVal y As Integer)
    '    Dim source As PresentationSource = PresentationSource.FromVisual(elt)
    '    Dim rtb As RenderTargetBitmap = New RenderTargetBitmap(x, y, 96, 96, PixelFormats.[Default])
    '    Dim sourceBrush As VisualBrush = New VisualBrush(elt)
    '    Dim drawingVisual As DrawingVisual = New DrawingVisual()
    '    Dim drawingContext As DrawingContext = drawingVisual.RenderOpen()

    '    Using drawingContext
    '        drawingContext.DrawRectangle(sourceBrush, Nothing, New Rect(New Point(0, 0), New Point(x, y)))
    '    End Using

    '    rtb.Render(drawingVisual)
    '    Dim encoder As PngBitmapEncoder = New PngBitmapEncoder()
    '    encoder.Frames.Add(BitmapFrame.Create(rtb))
    '    Dim ms As FileStream = New FileStream(filepath, FileMode.Create)
    '    encoder.Save(ms)
    '    ms.Close()
    'End Sub

    'Public Function getsnapshotname()
    '    Return "HandyDraw_" + Now.ToString.Replace(":", "_").Replace(" ", "_").Replace("/", "_") + "_" + Now.Millisecond.ToString + ".png"
    'End Function
    'Public Function xiegang(p As String)
    '    Return IIf(p.Substring(p.Length - 1) = "\", "", "\")
    'End Function
    'Public Structure save_task
    '    Public a As Integer
    '    Public w As Integer
    '    Public h As Integer
    '    Public path As String
    'End Structure
    'Public save_queue As List(Of save_task)

    Public Shared Function CubicBezierDoubleAnimation(d As TimeSpan, s As Double, t As Double, Bezier As String) As DoubleAnimationUsingKeyFrames
        Dim dkf As DoubleKeyFrame = New LinearDoubleKeyFrame
        dkf.KeyTime = TimeSpan.FromSeconds(0.0)
        dkf.Value = s
        Dim sp As SplineDoubleKeyFrame = New SplineDoubleKeyFrame
        sp.KeyTime = d
        Dim p() As String = Bezier.Split(",")
        Dim controlPoint1 As Point = New Point(p(0), p(1))
        Dim controlPoint2 As Point = New Point(p(2), p(3))
        sp.KeySpline = New KeySpline With {
            .ControlPoint1 = controlPoint1,
            .ControlPoint2 = controlPoint2
        }
        sp.Value = t
        Dim da As DoubleAnimationUsingKeyFrames = New DoubleAnimationUsingKeyFrames
        da.KeyFrames.Add(dkf)
        da.KeyFrames.Add(sp)
        Return da
    End Function
    Public Shared Function CubicBezierDoubleAnimation(st As TimeSpan, d As TimeSpan, s As Double, t As Double, Bezier As String) As DoubleAnimationUsingKeyFrames
        Dim dkf1 As DoubleKeyFrame = New LinearDoubleKeyFrame
        dkf1.KeyTime = TimeSpan.FromSeconds(0.0)
        dkf1.Value = s
        Dim dkf As DoubleKeyFrame = New LinearDoubleKeyFrame
        dkf.KeyTime = st
        dkf.Value = s
        Dim sp As SplineDoubleKeyFrame = New SplineDoubleKeyFrame
        sp.KeyTime = d
        Dim p() As String = Bezier.Split(",")
        Dim controlPoint1 As Point = New Point(p(0), p(1))
        Dim controlPoint2 As Point = New Point(p(2), p(3))
        sp.KeySpline = New KeySpline With {
            .ControlPoint1 = controlPoint1,
            .ControlPoint2 = controlPoint2
        }
        sp.Value = t
        Dim da As DoubleAnimationUsingKeyFrames = New DoubleAnimationUsingKeyFrames
        da.KeyFrames.Add(dkf1)
        da.KeyFrames.Add(dkf)
        da.KeyFrames.Add(sp)
        Return da
    End Function
End Class

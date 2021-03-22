Public Enum Edit_Mode_Enum As Integer
    Cursor = 1
    Pen = 2
    Marker = 4
    Eraser = 8
    Selectt = 16
End Enum
Public Enum App_Mode_Enum As Integer
    PPT = 0
    Board = 1
    Camera = 2
    Scale = 3
End Enum
Public Structure RECT
    Dim Left As Integer
    Dim Top As Integer
    Dim Right As Integer
    Dim Bottom As Integer
End Structure

Public Class DpiDecorator
    Inherits Decorator

    Public Sub New()
        AddHandler Me.Loaded, Sub(s, e)
                                  Dim r = ScreenHelper.GetLogicalWidth / 1920
                                  Dim m As Matrix = PresentationSource.FromVisual(Me).CompositionTarget.TransformToDevice
                                  Dim dpiTransform As ScaleTransform = New ScaleTransform(r * 1 / m.M11, r * 1 / m.M22)
                                  If dpiTransform.CanFreeze Then dpiTransform.Freeze()
                                  Me.LayoutTransform = dpiTransform
                              End Sub
    End Sub
End Class

Public Class ColorValueConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim c As Color = CType(value, Color)
        Dim b As SolidColorBrush = New SolidColorBrush(c)
        Return b
    End Function
    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class
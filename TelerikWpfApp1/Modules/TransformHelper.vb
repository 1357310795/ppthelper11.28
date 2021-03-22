Public Class TransformHelper
    Public Shared Function FindScaleTransform(ByVal hayStack As Transform) As ScaleTransform
        If TypeOf hayStack Is ScaleTransform Then
            Return CType(hayStack, ScaleTransform)
        End If

        If TypeOf hayStack Is TransformGroup Then
            Dim group As TransformGroup = TryCast(hayStack, TransformGroup)

            For Each child In group.Children

                If TypeOf child Is ScaleTransform Then
                    Return CType(child, ScaleTransform)
                End If
            Next
        End If

        Return Nothing
    End Function
    Public Shared Function FindRotateTransform(ByVal hayStack As Transform) As RotateTransform
        If TypeOf hayStack Is RotateTransform Then
            Return CType(hayStack, RotateTransform)
        End If

        If TypeOf hayStack Is TransformGroup Then
            Dim group As TransformGroup = TryCast(hayStack, TransformGroup)

            For Each child In group.Children

                If TypeOf child Is RotateTransform Then
                    Return CType(child, RotateTransform)
                End If
            Next
        End If
        Return Nothing
    End Function
    Public Shared Function FindTranslateTransform(ByVal hayStack As Transform) As TranslateTransform
        If TypeOf hayStack Is TranslateTransform Then
            Return CType(hayStack, TranslateTransform)
        End If

        If TypeOf hayStack Is TransformGroup Then
            Dim group As TransformGroup = TryCast(hayStack, TransformGroup)

            For Each child In group.Children

                If TypeOf child Is TranslateTransform Then
                    Return CType(child, TranslateTransform)
                End If
            Next
        End If
        Return Nothing
    End Function
End Class

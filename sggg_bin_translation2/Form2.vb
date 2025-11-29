Public Class Form2
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        TextBox1.Focus()
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        TextBox1.Focus()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        ListView1.Items.Clear()
        If TextBox1.Text.Length > 1 Then
            Dim i As Integer = 0
            For Each row As String() In Form1.data
                If RadioButton1.Checked = True Then
                    If InStr(row(3), TextBox1.Text) Then
                        ListView1.Items.Add(New ListViewItem({i + 1, row(0), row(3), row(4)}))
                    End If
                ElseIf RadioButton2.Checked = True Then
                    If InStr(row(4).ToLower, TextBox1.Text.ToLower) Then
                        ListView1.Items.Add(New ListViewItem({i + 1, row(0), row(3), row(4)}))
                    End If
                End If
                i += 1
            Next
        End If

    End Sub


    Private Sub ListView1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseDoubleClick
        If ListView1.SelectedItems.Count > 0 Then
            Dim i As Integer = ListView1.SelectedItems(0).SubItems(0).Text
            Form1.ListView1.Items(i - 1).Focused = True
            Form1.ListView1.Items(i - 1).Selected = True
            Form1.ListView1.Items(i - 1).EnsureVisible()
            'Form1.ListView1.Select()
        End If

    End Sub
End Class
   M S B A M L   `   `   `  ���� B  >SPI-AOI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:SPI_AOI.Views.ModelManagementSPI_AOI.Views.ModelManagement  4    .SPI_AOI.Views.ModelManagement.FOVCaptureWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     =local+clr-namespace:SPI_AOI.Views.ModelManagement     -     � Title$  FOV Settings��$	��700��$
��1118�����	#FFEEEEEE � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=�.���  � ColumnDefinitions �� $��1* ��� $	��350 �.+�� $��0��.+��� $
��View��$��10q�.���� -   }�. ���	#FF111111._��� -    ��Name$ imbView�� J� RenderTransformOrigin$	 0,0&� � LayoutTransform V� .~��� T� � $��1��.+��� $��Settings��$��
0,10,10,10q�.���  � RowDefinitions �� $��60 ��� $	��100 ��� .+�� $��0��.+���  �� Orientation$ 
Horizontal=�$��0, 10, 0, 5q�.+��� $��Found:��$��20(��� -   	 ��Name$	 	lbNumFovs��$��0��$��20(�$
��Bold"��� $
��FOVs��$��20(�� $��1��.+��� $ 
Horizontal=�$��0, 10, 0, 5q�.+��� $��
0, 0, 5, 0q�$��Start capturing corner:��$��Center=�$��Center=��� $��90��
 �� CornerRadius$
 5��$��50��
�������$��
40,10,0,10q�._��� .+�	� -    	�Name$ 	rbTopLeft��
 �� Top$ 2�� �� Left$ 2�� � ToolTip$ Top Left��	� -   $ 
rbTopRight��$ 2�� �� Right$ 2��$ 	Top Right��	� -   $ 	rbBotLeft�� �� Bottom$ 2��$ 2��$ Bottom Left��	� -   $ 
rbBitRight��$ 2��$ 2��$ Bottom Right��� $��2��.+��� $��FOV Details��$��5q�.��]� .+�&   System.Windows.Controls.DataGrid  -	     Name$ 
dgwAnchors��   
IsReadOnly .    AutoGenerateColumns .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 1*    Header$ NO��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *NO+  $ 2* $ X�� �� *
Anchor.X+  $ 2* $ Y�� �� *
Anchor.Y+
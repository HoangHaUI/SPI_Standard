   M S B A M L   `   `   `  ���� B  >SPI-AOI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:SPI_AOI.Views.ModelManagementSPI_AOI.Views.ModelManagement  -clr-namespace:SPI_AOI.ViewsSPI_AOI.Views  [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms T PEmgu.CV.UI, Version=4.1.1.3497, Culture=neutral, PublicKeyToken=7281126722ab4438;,clr-namespace:Emgu.CV.UI;assembly=Emgu.CV.UI
Emgu.CV.UI /    )SPI_AOI.Views.ModelManagement.GerberTools   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     =local+clr-namespace:SPI_AOI.Views.ModelManagement     +cvtclr-namespace:SPI_AOI.Views     Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     =emgu,clr-namespace:Emgu.CV.UI;assembly=Emgu.CV.UI     X TXceed.Wpf.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     � Title$  Gerber Tools��$	��900��$
��1600�����	#FFEEEEEE � WindowState$ 	Maximized=� � WindowStartupLocation$ CenterScreen=� � 	Resources  �� MergedDictionaries!   SPI_AOI.Views.BrushColorCvt   
BrushColor   SPI_AOI.Views.BrushPadCvt  BrushPad��  %   &	        &	          .���  � RowDefinitions �� $��80 ��� .+�� $��0��.+��� .+�o� $��35��.@�m� $��_File��$��5q�.@�m� $��_Open Gerber file��m� $��_Open CAD file��m� $��_��m� $��_Tools��$��5q�.@�m� $��_Open Gerber file��m� $��_Open CAD file��m� $��_��m� $��_View��$��5q�.@�m� -    m�Name$ chbHighlightPadLinked��$��_Highlight Pad Linked�� m� IsCheckable . m� -   $ chbShowLinkLine��$��_Show Link Line�� . m� -   $ chbShowComponentCenter��$��_Show Component Center�� . m� -   $ chbShowComponentName��$��_Show Component Name�� . m� $��_Short Keys��$��5q�.@�m� $��_Open Gerber file��m� $��_Open CAD file��m� $��_��m� $��_Help��$��5q�.@�m� $��_Open Gerber file��m� $��_Open CAD file��m� $��_��c� -    c�Name$ toolBarMain��
������ $��0,5,0,0q�.@��� -   	 ��Name$	 btImportGerber��
 � ToolTip$
 Import gerber file��$��35��$��35��$��5,0,5,0q�.���� $+��%../../Resources/Icons/gerber_icon.png���� -   $	 btImportCad��$
 Import Cad file��$��35��$��35��$��5,0,5,0q�.���� $,��&../../Resources/Icons/file_cad-512.png���� -	   $	 btSetROI��$
 ROI - Crop (Ctrl + R)��$��35��$��35��$��5,0,5,0q�.���� $(��"../../Resources/Icons/crop-512.png���� -
   $	 
btRotation��$��35��$��35��$��5,0,5,0q�$
 Manual adjust R - XY��.���� $(��"../../Resources/Icons/rotation.png���� -   $	 btSelectPad��$��35��$��35��$��5,0,5,0q�$
 Select Items (Ctrl + B)��.���� $#��../../Resources/Icons/pad.png���� -   $	 btAdjustsadxXY��$��35��$��35��$��5,0,5,0q�$
 Auto Adjsut XY��.���� $$��../../Resources/Icons/copy.png���� -   $	 	btLinkPad��$��35��$��35��$��5,0,5,0q�$
 Auto Link Pad��.���� $$��../../Resources/Icons/link.png���� -   $	 btSetLinkPad��$��35��$��35��$��5,0,5,0q�$
 Set Pad Name��.���� $#��../../Resources/Icons/D10.png���� -   $	 btDeleteLinkPad��$��35��$��35��$��5,0,5,0q�$
 Remove Pad Name��.���� $%��../../Resources/Icons/-icon.png���� -   $	 btPadSettings��$��35��$��35��$��5,0,5,0q�$
 Pad settings��.���� $(��"../../Resources/Icons/settings.png���� -   $	 btMark1��$��35��$��35��$��5,0,5,0q�$
 
Set Mark 1��.���� $%��../../Resources/Icons/mark1.png���� -   $	 btMark2��$��35��$��35��$��5,0,5,0q�$
 
Set Mark 1��.���� $%��../../Resources/Icons/mark2.png��� $��1�� � ColumnDefinitions �� $
��157* ��� $	��300 �.+�� $��0��.+��� $��5q�$
��View��.��7  1System.Windows.Forms.Integration.WindowsFormsHost  $��	0,10,10,0q�   
Background �	#FF111111   Child.   Emgu.CV.UI.ImageBox  -   	imBox��   FunctionalMode$ RightClickMenu=�� $��1��.+��  �� $
��251* ��� $
��538* �.+�� $��0��.+��� $��5q�$��Files�� ��%    .���� -    ��Name$ listImportedFile��$��10q�
������  �� SelectionMode$ Single=����� .R�]� .+���  �� Orientation$ 
Horizontal=�$��5,0,0,0q�.+��� -   $��	10,5,10,0q� d� 	IsChecked �� *	Visible+�� -   $��1q�
���   �$��15��$��15������  �� Path$ Color� �� 	Converter# [  �� $��10,0,0,0q����� *
FileName+ � ContextMenu �� .@�m� $ menuCtxCopy��-   $��_Copy��m� $ menuCtxDelete��-   $��_Delete��� $��1��.+��� $��5q�$
��Pads��.��]� .+�&   System.Windows.Controls.DataGrid  -     Name$ dgwPads��$��0,10,0,0q�$��0q�   
IsReadOnly .    AutoGenerateColumns .     Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Status��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 50    CellTemplate �� .R��� $��1q�
���   �$��15��$��15������ $ CadItemIndex�# [ 0	  *System.Windows.Controls.DataGridTextColumn	  $ 1* $ X��1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding �� *
Center.X+	  $ 1* $ Y�� �� *
Center.Y+
   M S B A M L   `   `   `  ���� E  APickImages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:PickImages
PickImages      PickImages.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     *localclr-namespace:PickImages     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$!  顿の根据长图找原图��$	��820��$	��450��$��Consolas)�$��14(�$��0q�.���  � ColumnDefinitions �� $	��600 ��� $	��200 � � RowDefinitions �� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $	��220 �.+��� -    ��Name$ 
OCRButton1��$��1��$	��190��$��40��$��5q�$
��Left=�$��选择文件并找原图���� -   $ DetectCSButton��$��2��$��1��$	��190��$��40��$��5q�$
��Left=�$!��选择文件夹并找原图���� -   $ DetectZXButton��$��3��$��1��$	��190��$��40��$��5q�$
��Left=�$!��选择文件夹并找原图���� -    ��Name$ OCRFile1��$
��Auto��$��20��$��10q� �� 
IsReadOnly$
 True��$.��(======选择站点名_日期XXX.csv=====���� -   $ DetectCSFolder��$��2��$
��Auto��$��20��$��10q�$
 True��$/��)======选择车身误检测文件夹=====���� -   $ DetectZXFolder��$��3��$
��Auto��$��20��$��10q�$
 True��$0��*======选择走行误检测文件夹======���� -   $ 	myConsole��$��4��$��0��$��2��$	��750��$	��190��$��5q� �� AcceptsReturn$
 True�� �� 
AcceptsTab$
 True��
�������$��Consolas)�$��13(�
���   �$ �� HorizontalScrollBarVisibility$
 Auto=�$
 True��	 �� TextWrapping$	 NoWrap=�"
 �� VerticalScrollBarVisibility$

 Auto=�
   M S B A M L   `   `   `  ����K  GSegmentationTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SegmentationTestSegmentationTest  5      !    SegmentationTest.MainWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:SegmentationTest     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -   5          �Name$  dundun��5       � Title$ 顿の切割测试��5	      $
��1200��5      $	��500��5      .��5      � $��5q�6    � DataContext 6   ��  �� ElementName$ dundun�� � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��1��6   $��0��6"    �� Orientation$ Vertical=�62   $��0,0,0,10q�6I   $��3��6[   .+�5      �� -   5         ��Name$ selectResult��5      $��16(�5      $��0,0,0,10q�5   y   ��6   �� *SelectedItem+$ stations�� �� StringFormat$ 目前站点是{0}��	 �� TargetNullValue$	 未选择任何站点��6y   �� 5       -   5        
 ��Name$
 stations��5!      $��0,0,0,10q�5$   3   ��5"      �� *StationList+ �� SelectedItem 5#      Z� 5$   3   �� 5%      -   5         ��Name$ gatherLongButton��5&      $��1��5'      $	��150��5(      $��60��5)      $
��Left=�5*      $	��Top=�5+      $��汇总车身左长图��5-      $
{�True��5.      $��0,0,0,10q�6"   �� 5/      -   5        $ gatherLocomotiveButton��50      $��1��51      $	��150��52      $��60��53      $
��Left=�54      $	��Top=�55      $��汇总车头长图��57      $
{�True��58      $��0,0,0,10q�6"   �� 59      -   5        $ pickShortButton��5:      $��1��5;      $	��150��5<      $��60��5=      $
��Left=�5>      $	��Top=�5?      $'��!保留切错长图并获取短图��5A      $
{�True��5B      $��0,0,0,10q�6"   �� 5C      -   5        $ ftpUploadButton��5D      $��1��5E      $	��150��5F      $��60��5G      $
��Left=�5H      $	��Top=�5I      $!��FTP上传软件错误短图��5K      $
{�True��5L      5M      �� 5O   
   -   5         ��Name$ 	myConsole��5P      $��5��5Q      $��1��5R       �� AcceptsReturn$
 True��5S       �� 
AcceptsTab$
 True��5T      
�������$��Consolas)�5V      $��14(�5W      
���   �$ �� HorizontalScrollBarVisibility$
 Auto=�5Y       �� 
IsReadOnly$
 True��5Z       �� TextWrapping$ NoWrap=�5[      " �� VerticalScrollBarVisibility$
 Auto=�5\      5_      5a      
   M S B A M L   `   `   `  ����  Processes_Manage,    &Processes_Manage.SystemCpuUsageLineDlg   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Y UDynamicDataDisplay, Version=0.3.0.0, Culture=neutral, PublicKeyToken=5b7d744a7263923fBlinedll4http://research.microsoft.com/DynamicDataDisplay/1.0  5      -   5          � Title$  系统CPU使用率曲线��5       � WindowStyle$
 None=�6    � WindowStartupLocation$ CenterOwner=�5   	    � WindowState$ 	Maximized=�5   	   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $ ��系统 CPU 使用曲线图��6   $��	20,10,0,0q�6,   $��15(�5      $
��Bold"�6&   �� 5      -   5         ��Name$ cpuUsageText��5      $��	10,10,0,0q�6.   $��15(�5      5      8  2Microsoft.Research.DynamicDataDisplay.ChartPlotter  5   
   -   5          Name$ plotter��5      $��10,10,20,10q�60   $��1��6E      VerticalAxis 5      K  EMicrosoft.Research.DynamicDataDisplay.Charts.Axes.VerticalIntegerAxis  5      	   HorizontalAxis	 5      M  GMicrosoft.Research.DynamicDataDisplay.Charts.Axes.HorizontalIntegerAxis  5      3  -Microsoft.Research.DynamicDataDisplay.Plotter
   Children.
 5      2  ,Microsoft.Research.DynamicDataDisplay.Header  $��CPU Performance History��6   =  7Microsoft.Research.DynamicDataDisplay.VerticalAxisTitle  5      $��
Percentage��6(   5      5       5!      
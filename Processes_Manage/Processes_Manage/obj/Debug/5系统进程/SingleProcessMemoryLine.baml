   M S B A M L   `   `   `  ����  Processes_Manage.    (Processes_Manage.SingleProcessMemoryLine   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Y UDynamicDataDisplay, Version=0.3.0.0, Culture=neutral, PublicKeyToken=5b7d744a7263923f=d34http://research.microsoft.com/DynamicDataDisplay/1.0  5      -   5          � WindowStartupLocation$  CenterScreen=�5   	    � WindowStyle$
 None=�6.    � Title$ -内存曲线-��5       � WindowState$ 	Maximized=�6+    � ShowInTaskbar .  .��5      �  � RowDefinitions 5      �� 5	   
   $
��Auto �6   �� 5
   
   $��* �6   .+�5      �� $��0��6    �� Orientation$ 
Horizontal=�6   .+�5      �� -   5        $��processName��5      $��15(�6+   $��	20,10,0,0q�69   �� 5      $��内存曲线:��6   $��	10,10,0,0q�6%   $��15(�5      $
��Bold"�6&   �� 5      -   5         ��Name$ 
memoryText��5      $��	10,10,0,0q�6,   $��15(�5      �� 5   
   $��	20,10,0,0q�6   $��15(�6'   $
��Bold"�65   	进程ID:6N   �� 5   
   -   5        $��	processId��5      $��15(�6%   $��	10,10,0,0q�63   5      8  2Microsoft.Research.DynamicDataDisplay.ChartPlotter  5      -   5          Name$ plotter��5      $��10,10,20,10q�6'   $��1��6<   	   VerticalAxis	 5   
   K  EMicrosoft.Research.DynamicDataDisplay.Charts.Axes.VerticalIntegerAxis  5      
   HorizontalAxis
 5   
   M  GMicrosoft.Research.DynamicDataDisplay.Charts.Axes.HorizontalIntegerAxis  5      3  -Microsoft.Research.DynamicDataDisplay.Plotter   Children. 5   
   2  ,Microsoft.Research.DynamicDataDisplay.Header  $��内 存 曲 线��6   =  7Microsoft.Research.DynamicDataDisplay.VerticalAxisTitle  5   
   $	��值��6   5       5!      5"      
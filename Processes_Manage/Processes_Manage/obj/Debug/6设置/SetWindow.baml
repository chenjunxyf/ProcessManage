   M S B A M L   `   `   `  ����K  GProcesses_Manage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Processes_ManageProcesses_Manage  5           Processes_Manage.SetWindow   5   	   W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      .localclr-namespace:Processes_Manage    5        � Title$  -常用设置-��5      $	��250��6   $	��400��6!    � 
ResizeMode$ NoResize=�6-    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$ 
ToolWindow=�6.    � 	Resources 5         Processes_Manage.MyKeys   AllKeys��%   &	          5   
   6   5	      .��5
      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��5* �6   .+�5   
   �� $��0��6     
shinyBrush#��[ 6   $��1,1,1,1q�6G   
�������$��1q�6l   ._�5      �� 
�������$��18(�6+   $��Center=�69   $��Center=�6V   唤起热键设置6y   5      �� 5   
   $��1��6   ._�5      �� $��3,1,3,1q�6   $��选择设置��6(   $��15(�66   .��5      ��  �� Orientation$ Vertical=�6   $��3,3,3,3q�64   $��Center=�6E   .+�5      �� $ 
Horizontal=�6!   $��Center=�6:   .+�5      �� $	��150��6%   $��30��61   $��0,0,10,0q�6=   	主键：6T   �� 5      $	��150��6%   $��30��61   $��10,0,0,0q�6=   	副键：6T   5      �� 5      $ 
Horizontal=�6!   $��Center=�6:   .+�5      �� -   5        $��hotKey1_ComboBox��5   N   $	��150��6#   $��30��6/   $��	0,0,10,20q�6;   @�5      �� 5   "   $	��ALT��6/   �� 5   "   $
��CTRL��6/   �� 5   "   $��SHIFT��6/   �� 5   "   $	��WIN��6/   5!      �� 5"      -   5        $��hotKey2_ComboBox��5"   N   $	��150��6#   $��30��6/   $��	10,0,0,20q�6;   #��[  5#   #   5$      �� 5%      -   5        $��	ok_Button��5%   H   $��0,15,0,0q�6   $	��150��60   $��33��6<   .��6r   	确 定6w   5&      5'      5(      5)      5*      
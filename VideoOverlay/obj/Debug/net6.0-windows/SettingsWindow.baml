   M S B A M L   `   `   `  ����G  CVideoOverlay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:VideoOverlayVideoOverlay  5   	   !    VideoOverlay.SettingsWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:VideoOverlay     5   	     � Title$  Settings��5   	   $	��300��6   $	��400��6'    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�5
   	   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6!   $��Toggle Overlay Hotkey:��61   $��Center=�6O   $��0,0,10,0q�6j   � 5   
   $��0��6   $��1��6   $	��0,5q�6,    5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5       �� -   5         ��Name$ HotkeyTextBox��5    &   $��0��6    �� 
IsReadOnly$
 True��6=   $��Center=�6O   �� 5!      -   5        $��1��5!      $��Change��6%   $��5,0,0,0q�66   $
��10,0q�6G   6V   5"      �� 5%   
   $��1��6   $��0��6!   $��Always on Top:��61   $��Center=�6G   $��0,0,10,0q�6b   �� 5&   
   -   5         ��Name$ AlwaysOnTopCheckBox��5&   0   $��1��6   $��1��6    $��Center=�6M   $	��0,5q�6h   �� 5)   
   $��2��6   $��0��6!   $��Start Minimized:��61   $��Center=�6I   $��0,0,10,0q�6d   �� 5*   
   -   5        $ StartMinimizedCheckBox��5*   0   $��2��6   $��1��6    $��Center=�6P   $	��0,5q�6k   �� 5-   
   -   5         ��Name$ HotkeyInputDialog��5-   B   $��4��6   $��0��6   $��2��6.   ���#F0F0F0���#CCCCCC$��1q�5.   <   	 �� Padding$	 10q�6P   $y�	Collapsed=�5/      $��Center=�6(   $��Center=�6C   ._�50      �� .+�51      �� $0��*Press the key combination you want to use:��6   $��0,0,0,10q�6N   �� 52      -   5        $ NewHotkeyTextBox��52      $
 True��64   $��0,0,0,10q�6F   $��Center=�6X   $	��200��6u   �� 53      
 �� Orientation$
 
Horizontal=�6   $��Center=�66   .+�54      �� -   5        $��OK��54      $��80��6*   $��0,0,10,0q�65   6G   �� 55      -   5        $��Cancel��55      $��80��6.   69   56      57      58      �� 5;   
   $��5��6   $��0��6"   $��2��62   $
 
Horizontal=�6F   $��Right=�6_   $��0,10,0,0q�6{   .+�5<      �� -	   5        $��OK��5<      $��80��6"   $��0,0,10,0q�6-   6?   �� 5=      -
   5        $��Cancel��5=      $��80��6&   61   5>      5?      5@      
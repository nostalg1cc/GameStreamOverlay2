   M S B A M L   `   `   `  ����G  CVideoOverlay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:VideoOverlayVideoOverlay  5   	       VideoOverlay.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:VideoOverlay     5   	   -   5          � Title$  Video Overlay��5   	   $	��600��5	   	   $
��1000��6   $	��300��5
   	   $	��400��6    � WindowStartupLocation$ CenterScreen=�5   	   $y�Hidden=�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6   ���#222222 � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   .+�5      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $
��10,5q�6B   .+�5      �� -   5         ��Name$ 
UrlTextBox��5      $	��300��6.   $��0,0,10,0q�6:   6L   �� 5      -   5        $
��Load��5      $
��10,0q�6A   �� 5      -   5        $��	Pin Video��5      $��10,0,0,0q�6E   $
��10,0q�6W   5      �� 5      $��1��6   $ 
Horizontal=�6)   $
��10,5q�6B   .+�5      �� -   5        $��Settings��5      $
��10,0q�6I   $��0,0,10,0q�6X   �� 5       -   5        $��Minimize��5       $
��10,0q�6I   5!      5"      � 5$   
   -   5         �Name$ ContentGrid��5$      $��1��6   5&      5'      5(      
   M S B A M L   `   `   `  ����e  aMicrosoft.Web.WebView2.Wpf, Version=1.0.1343.22, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf  5      G CVideoOverlay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:VideoOverlayVideoOverlay 5      &    VideoOverlay.VideoOverlayControl   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      \wv2Lclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf     5      ,localclr-namespace:VideoOverlay     5      -   5          � 	Resources  5         TransparentButton �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  ResizeGripStyle	 �� Triggers��%�  &	        &	 q    �� 5   
   " ��6*   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 �����6+   �� 5       � ��
 ���� 6,   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��5      #��z��5      $ 3��5      ._�5      �� $��Center=�6/   $��Center=�6L   5       5      P� 5       �@J�IsMouseOver$
 True��6=   . �5   "   ��  � �� �	#22FFFFFF6?   5      P� 5        � ��$
 True��6;   . �5!   "   ��  � �� �	#44FFFFFF6?   5"      5$      5&      5'      �� 5)   
   " n�6(   .�5*      ��  � �� 5+      �� 5,      " n�6&   .R�5-      �� ���	#22FFFFFF$��10��65   $��10��6@   ��5.      �� 5/   "   " ��6(   	 50   &   P� 51   *    �@J�IsMouseOver$
 True��6I   . �52   .   ��  � �� �	#66FFFFFF6E   53   +   55   #   57      58      5:      5;      5<      .��5>      � 
 � RowDefinitions
 5?   
   �� 5@      $
��Auto �6   �� 5A      $��* �6   �� 5B      $
��Auto �6   .+�5F   
   �� -   5         ��Name$ 
DragHandle��5F      $��0��6%   $��10��62   
������  � Cursor$ SizeAll��5H      ._�5I      �� $��2��6   ���	#44FFFFFF$��Center=�64   $
��10,0q�6O   5J      �� 5M   
   -   5        $ WebViewContainer��5M   1   $��1��6   
���   �$
}�True��6K   5O      �� 5R   
   -   5        $ 
ControlBar��5R   5   $��2��6   ���	#AA000000$y�	Collapsed=�6I   ._�5S      �  � ColumnDefinitions 5T      �� 5U      $
��Auto �6'   �� 5V      $��* �6'   �� 5W      $
��Auto �6'   .+�5Z      �� $��0��6    �� Orientation$ 
Horizontal=�6-   $��5q�6F   .+�5[      �� -   5        $	��⨯��5[      #��[  6C    � ToolTip$ Close��5\      $
��Bold"�6-   $��24��6?   $��24��6J   5]      �� 5_      $��1��6   $ 
Horizontal=�6-   $��Center=�6F   $��Center=�6a   .+�5`      �� $��Volume:��6   
�������$��Center=�6A   $��0,0,5,0q�6\   �� 5a      -   5         ��Name$ VolumeSlider��5a       � Minimum$ 0Q�63    � Maximum$ 1Q�6?    � Value$ 1Q�6K   $	��100��5b      $��Center=�6)   5c      5d      �� 5f      $��2��6   $ 
Horizontal=�6-   $��5q�6F   .+�5g      �� -   5        $	��−��5g      #��[  6D   $ Shrink��5h      $��24��6.   $��24��69   $
��Bold"�6E   �� 5i      -   5        $��+��5i      #��[  6B   $
 Grow��5j      $��24��6,   $��24��67   $
��Bold"�6C   5k      5l      5m      n� 5p   
   -	   5         n�Name$ BottomRightResize��5p      $��2��6+   $��16��68   $��16��6C   $��Right=�5q      $��Bottom=�6,   $��0,0,0,0q�5r      $ SizeNWSE��6!   #��[ 5s       J� Opacity$	 0.5Q�69   5t      5u      
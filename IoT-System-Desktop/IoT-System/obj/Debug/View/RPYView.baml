   M S B A M L   `   `   `  ����F  BMultiWPFApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:MultiWPFApp.ViewMultiWPFApp.View  5      =#clr-namespace:MultiWPFApp.ViewModelMultiWPFApp.ViewModel  5          MultiWPFApp.View.RPYView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      R NOxyPlot.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      *localclr-namespace:MultiWPFApp.View  5      ,vm#clr-namespace:MultiWPFApp.ViewModel  5        � DataContext  5      (   "MultiWPFApp.ViewModel.RPYViewModel  5   
   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��381* �6   �� 5      $	��40* �6   .+�5   
     OxyPlot.Wpf.PlotView  -   5          Name$ DataPlotView��5      $��0��5      $��0��6$   $��0,0,10,0q�5      $��2��5   7      Model 6   �� *DataPlotModel+67   � 5    
   $��1��6   $��0��6   $��2��6,    5!      �� 5"      $
��Auto �6#   �� 5#      $
��Auto �6#   �� 5$      $��1* �6#   �� 5%      $
��Auto �6#   �� 5&      $
��Auto �6#   5(      � 5*   
   $��2��6   $��0��6   $��2��6,    5+      �� 5,      $��* �6#   �� 5-      $
��Auto �6#   �� 5.      $
��Auto �6#   �� 5/      $��* �6#   .+�51      �� $��1��52      $��30��53      $	��100��54      $��0,10,200,10q�55      
����� �$��START��58      $��Right=�59      ��57      �� *StartButton+.��59   -   START5;      �� 5<      $��3��5=      $��30��5>      $	��100��5?      $��
0,10,15,10q�5@      
����� ���5B      �� *
StopButton+.��60   STOP5D      5E      5F      5G      